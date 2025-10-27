using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class EditarPropuesta : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();

        // Clase auxiliar para el GridView
        [Serializable]
        public class DetalleViewModel
        {
            public int DetalleId { get; set; }
            public string NumeroFactura { get; set; }
            public string RazonSocialAcreedor { get; set; }
            public string CodigoMoneda { get; set; }
            public decimal Monto { get; set; }
            public string CuentaOrigen { get; set; }
            public string CuentaDestino { get; set; }
            public string FormaPago { get; set; }
            public string FormaPagoChar { get; set; }
        }

        private int PropuestaId
        {
            get
            {
                int.TryParse(Request.QueryString["id"], out var id);
                return id;
            }
        }

        private HashSet<int> DetallesEliminados
        {
            get
            {
                if (ViewState["DetallesEliminados"] == null)
                    ViewState["DetallesEliminados"] = new HashSet<int>();
                return (HashSet<int>)ViewState["DetallesEliminados"];
            }
            set
            {
                ViewState["DetallesEliminados"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PropuestaId <= 0)
                {
                    MostrarMensaje("ID de propuesta inválido", "danger");
                    btnGuardar.Enabled = false;
                    return;
                }

                CargarPropuesta();
            }
        }

        private void CargarPropuesta()
        {
            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta solicitada", "danger");
                    btnGuardar.Enabled = false;
                    return;
                }

                if (propuesta.Estado != "Pendiente")
                {
                    MostrarMensaje("Solo se pueden editar propuestas en estado Pendiente", "warning");
                    Response.Redirect($"~/DetallePropuesta.aspx?id={PropuestaId}");
                    return;
                }

                // Cargar información general
                lblPropuestaId.Text = propuesta.PropuestaId?.ToString() ?? "-";
                lblFechaCreacion.Text = propuesta.FechaHoraCreacion?.ToString("dd/MM/yyyy HH:mm") ?? "-";
                lblUsuarioCreador.Text = propuesta.UsuarioCreacion != null
                    ? $"{propuesta.UsuarioCreacion.Nombre} {propuesta.UsuarioCreacion.Apellidos}"
                    : "-";
                lblPais.Text = propuesta.EntidadBancaria?.Pais?.Nombre ?? "-";
                lblBanco.Text = propuesta.EntidadBancaria?.Nombre ?? "-";

                // Guardar propuesta en ViewState
                ViewState["PropuestaOriginal"] = propuesta;

                // Cargar detalles
                ActualizarVista();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar la propuesta: {ex.Message}", "danger");
            }
        }

        private void ActualizarVista()
        {
            var propuesta = ViewState["PropuestaOriginal"] as PropuestasPagoDTO;
            if (propuesta == null)
                return;

            var detallesEliminados = DetallesEliminados;

            // Filtrar detalles eliminados
            var detallesActivos = propuesta.DetallesPropuesta
                .Where(d => d.DetallePropuestaId.HasValue && !detallesEliminados.Contains(d.DetallePropuestaId.Value))
                .ToList();

            lblTotalPagos.Text = detallesActivos.Count.ToString();

            // Cargar totales por moneda
            CargarTotalesPorMoneda(detallesActivos);

            // Cargar detalles
            CargarDetallesPagos(detallesActivos);
        }

        private void CargarTotalesPorMoneda(List<DetallesPropuestaDTO> detalles)
        {
            if (detalles == null || detalles.Count == 0)
            {
                rptTotales.DataSource = null;
                rptTotales.DataBind();
                return;
            }

            var totales = detalles
                .GroupBy(d => d.Factura?.Moneda?.CodigoIso ?? "-")
                .Select(g => new
                {
                    CodigoMoneda = g.Key,
                    Total = g.Sum(d => d.MontoPago)
                })
                .OrderBy(t => t.CodigoMoneda)
                .ToList();

            rptTotales.DataSource = totales;
            rptTotales.DataBind();
        }

        private void CargarDetallesPagos(List<DetallesPropuestaDTO> detalles)
        {
            if (detalles == null || detalles.Count == 0)
            {
                lblCantidadPagos.Text = "0 pago(s)";
                gvDetalles.DataSource = null;
                gvDetalles.DataBind();
                return;
            }

            var detallesVM = detalles
                .Select(d => new DetalleViewModel
                {
                    DetalleId = d.DetallePropuestaId ?? 0,
                    NumeroFactura = d.Factura?.NumeroFactura ?? "-",
                    RazonSocialAcreedor = d.Factura?.Acreedor?.RazonSocial ?? "-",
                    CodigoMoneda = d.Factura?.Moneda?.CodigoIso ?? "-",
                    Monto = d.MontoPago,
                    CuentaOrigen = d.CuentaPropia?.NumeroCuenta ?? "-",
                    CuentaDestino = d.CuentaAcreedor?.NumeroCuenta ?? "-",
                    FormaPago = MapearFormaPago(d.FormaPago),
                    FormaPagoChar = d.FormaPago.ToString() ?? "T"
                })
                .ToList();

            gvDetalles.DataSource = detallesVM;
            gvDetalles.DataBind();

            lblCantidadPagos.Text = $"{detallesVM.Count} pago(s)";

            // Guardar en ViewState para paginación
            ViewState["DetallesViewModel"] = detallesVM;

            // Configurar evento onclick para los botones de eliminar
            foreach (GridViewRow row in gvDetalles.Rows)
            {
                var btnEliminar = row.FindControl("btnEliminar") as LinkButton;
                if (btnEliminar != null)
                {
                    btnEliminar.OnClientClick = $"return mostrarModalEliminar('{btnEliminar.CommandArgument}');";
                }
            }
        }

        private string MapearFormaPago(char? formaPago)
        {
            if (!formaPago.HasValue)
                return "-";

            switch (char.ToUpper(formaPago.Value))
            {
                case 'T':
                    return "Transferencia";
                case 'C':
                    return "Cheque";
                case 'E':
                    return "Efectivo";
                default:
                    return formaPago.ToString();
            }
        }

        protected void gvDetalles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDetalles.PageIndex = e.NewPageIndex;
            ActualizarVista();
            upDetalles.Update();
        }

        protected void gvDetalles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Los comandos se manejan mediante el modal
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int detalleId = int.Parse(hdnDetalleIdEliminar.Value);

                // Agregar a la lista de eliminados
                var eliminados = DetallesEliminados;
                eliminados.Add(detalleId);
                DetallesEliminados = eliminados;

                // Actualizar vista
                ActualizarVista();

                // Cerrar modal
                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal",
                    "$('#modalEliminar').modal('hide');", true);

                MostrarMensaje("Pago marcado para eliminación. Presione 'Guardar Cambios' para confirmar.", "info");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al eliminar el detalle: {ex.Message}", "danger");
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var propuesta = ViewState["PropuestaOriginal"] as PropuestasPagoDTO;
                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta a guardar", "danger");
                    return;
                }

                // Actualizar formas de pago desde el GridView
                ActualizarFormasDePago(propuesta);

                // Eliminar detalles marcados
                var detallesEliminados = DetallesEliminados;
                if (detallesEliminados.Count > 0)
                {
                    var detallesActivos = propuesta.DetallesPropuesta
                        .Where(d => !d.DetallePropuestaId.HasValue || !detallesEliminados.Contains(d.DetallePropuestaId.Value))
                        .ToList();

                    propuesta.DetallesPropuesta.Clear();
                    foreach (var detalle in detallesActivos)
                    {
                        propuesta.DetallesPropuesta.Add(detalle);
                    }
                }

                // Validar que quede al menos un detalle
                if (propuesta.DetallesPropuesta.Count == 0)
                {
                    MostrarMensaje("La propuesta debe tener al menos un pago", "warning");
                    return;
                }

                // Configurar datos de auditoría
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                propuesta.UsuarioModificacion = new UsuariosDTO { UsuarioId = usuarioId };
                propuesta.FechaHoraModificacion = DateTime.Now;

                // Guardar en base de datos
                bool resultado = propuestasBO.Modificar(propuesta)==1;

                if (resultado)
                {
                    MostrarMensaje("Propuesta actualizada exitosamente", "success");

                    // Redirigir al detalle después de un breve momento
                    ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                        $"setTimeout(function(){{ window.location.href='DetallePropuesta.aspx?id={PropuestaId}'; }}, 2000);", true);
                }
                else
                {
                    MostrarMensaje("Error al actualizar la propuesta", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al guardar los cambios: {ex.Message}", "danger");
            }
        }

        private void ActualizarFormasDePago(PropuestasPagoDTO propuesta)
        {
            foreach (GridViewRow row in gvDetalles.Rows)
            {
                var ddlFormaPago = row.FindControl("ddlFormaPago") as DropDownList;
                if (ddlFormaPago != null)
                {
                    // Obtener el DetalleId de la fila
                    var detallesVM = ViewState["DetallesViewModel"] as List<DetalleViewModel>;
                    if (detallesVM != null && row.RowIndex < detallesVM.Count)
                    {
                        int detalleId = detallesVM[row.RowIndex].DetalleId;

                        // Buscar el detalle correspondiente en la propuesta
                        var detalle = propuesta.DetallesPropuesta
                            .FirstOrDefault(d => d.DetallePropuestaId == detalleId);

                        if (detalle != null)
                        {
                            detalle.FormaPago = ddlFormaPago.SelectedValue[0];
                        }
                    }
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/DetallePropuesta.aspx?id={PropuestaId}");
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }
    }
}