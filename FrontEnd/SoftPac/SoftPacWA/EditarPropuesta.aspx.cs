using SoftPac.Business;
using SoftPacBusiness.PropuestaPagoWS;
using SoftPacBusiness.UsuariosWS;
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

        private SoftPacBusiness.UsuariosWS.usuariosDTO UsuarioLogueado
        {
            get
            {
                return (SoftPacBusiness.UsuariosWS.usuariosDTO)Session["UsuarioLogueado"];
            }
        }

        private HashSet<int> DetallesEliminados
        {
            get
            {
                if (Session["DetallesEliminados"] == null)
                    Session["DetallesEliminados"] = new HashSet<int>();
                return (HashSet<int>)Session["DetallesEliminados"];
            }
            set
            {
                Session["DetallesEliminados"] = value;
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

                if (propuesta.estado != "Pendiente")
                {
                    MostrarMensaje("Solo se pueden editar propuestas en estado Pendiente", "warning");
                    Response.Redirect($"~/DetallePropuesta.aspx?id={PropuestaId}");
                    return;
                }

                // Cargar información general
                lblPropuestaId.Text = propuesta.propuesta_id.ToString() ?? "-";
                lblFechaCreacion.Text = propuesta.fecha_hora_creacion.ToString("dd/MM/yyyy HH:mm") ?? "-";
                lblUsuarioCreador.Text = propuesta.usuario_creacion != null
                    ? $"{propuesta.usuario_creacion.nombre} {propuesta.usuario_creacion.apellidos}"
                    : "-";
                lblPais.Text = propuesta.entidad_bancaria?.pais?.nombre ?? "-";
                lblBanco.Text = propuesta.entidad_bancaria?.nombre ?? "-";

                // Guardar propuesta en Session
                Session["PropuestaOriginal"] = propuesta;

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
            var propuesta = Session["PropuestaOriginal"] as propuestasPagoDTO;
            if (propuesta == null)
                return;
            var detallesActivos = new List<detallesPropuestaDTO>();
            if (!(propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0))
            {

                var detallesEliminados = DetallesEliminados;

                detallesActivos = propuesta.detalles_propuesta
                    .Where(d => d.detalle_propuesta_idSpecified && !detallesEliminados.Contains(d.detalle_propuesta_id))
                    .ToList();
            }
            lblTotalPagos.Text = detallesActivos.Count.ToString();

            // Cargar totales por moneda
            CargarTotalesPorMoneda(detallesActivos);

            // Cargar detalles
            CargarDetallesPagos(detallesActivos);
        }

        private void CargarTotalesPorMoneda(List<detallesPropuestaDTO> detalles)
        {
            if (detalles == null || detalles.Count == 0)
            {
                rptTotales.DataSource = null;
                rptTotales.DataBind();
                PnlTotalesMoneda.Visible = false;
                return;
            }

            var totales = detalles
                .GroupBy(d => d.factura?.moneda?.codigo_iso ?? "-")
                .Select(g => new
                {
                    CodigoMoneda = g.Key,
                    Total = g.Sum(d => d.monto_pago)
                })
                .OrderBy(t => t.CodigoMoneda)
                .ToList();

            rptTotales.DataSource = totales;
            rptTotales.DataBind();
        }

        private void CargarDetallesPagos(List<detallesPropuestaDTO> detalles)
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
                    DetalleId = d.detalle_propuesta_idSpecified ? d.detalle_propuesta_id : 0,
                    NumeroFactura = d.factura?.numero_factura ?? "-",
                    RazonSocialAcreedor = d.factura?.acreedor?.razon_social ?? "-",
                    CodigoMoneda = d.factura?.moneda?.codigo_iso ?? "-",
                    Monto = d.monto_pago,
                    CuentaOrigen = d.cuenta_propia?.numero_cuenta ?? "-",
                    CuentaDestino = d.cuenta_acreedor?.numero_cuenta ?? "-",
                    FormaPago = MapearFormaPago((char?)d.forma_pago),
                    FormaPagoChar = ((char?)d.forma_pago).ToString() ?? "T"
                })
                .ToList();

            gvDetalles.DataSource = detallesVM;
            gvDetalles.DataBind();

            lblCantidadPagos.Text = $"{detallesVM.Count} pago(s)";

            // Guardar en Session para paginación
            Session["DetallesViewModel"] = detallesVM;

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
                var propuesta = Session["PropuestaOriginal"] as propuestasPagoDTO;
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
                    foreach (var detalle in propuesta.detalles_propuesta)
                    {
                        if (detallesEliminados.Contains(detalle.detalle_propuesta_id))
                        {
                            detalle.fecha_eliminacion = DateTime.Now;
                            detalle.usuario_eliminacion = DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO, SoftPacBusiness.PropuestaPagoWS.usuariosDTO>(UsuarioLogueado);
                        }
                    }
                }

                // Validar que quede al menos un detalle
                if (propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0)
                {
                    MostrarMensaje("La propuesta debe tener al menos un pago", "warning");
                    return;
                }

                // Configurar datos de auditoría

                propuesta.usuario_modificacion = DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO, SoftPacBusiness.PropuestaPagoWS.usuariosDTO>(UsuarioLogueado);
                propuesta.fecha_hora_modificacion = DateTime.Now;

                // Guardar en base de datos
                bool resultado = propuestasBO.Modificar(propuesta) == 1;

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

        private void ActualizarFormasDePago(propuestasPagoDTO propuesta)
        {
            foreach (GridViewRow row in gvDetalles.Rows)
            {
                var ddlFormaPago = row.FindControl("ddlFormaPago") as DropDownList;
                if (ddlFormaPago != null)
                {
                    // Obtener el DetalleId de la fila
                    var detallesVM = Session["DetallesViewModel"] as List<DetalleViewModel>;
                    if (detallesVM != null && row.RowIndex < detallesVM.Count)
                    {
                        int detalleId = detallesVM[row.RowIndex].DetalleId;

                        // Buscar el detalle correspondiente en la propuesta
                        var detalle = propuesta.detalles_propuesta
                            .FirstOrDefault(d => d.detalle_propuesta_id == detalleId);

                        if (detalle != null)
                        {
                            detalle.forma_pago = ddlFormaPago.SelectedValue[0];
                        }
                    }
                }
            }
        }
        protected void btnAgregarDetalle_Click(object sender, EventArgs e)
        {
            // Redirige a la nueva página para agregar un detalle, pasando el id de la propuesta
            Response.Redirect($"~/AgregarDetallePropuesta.aspx?id={PropuestaId}");
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