using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class DetallePropuesta : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();

        // Clase auxiliar para el GridView
        [Serializable]
        public class DetalleViewModel
        {
            public string NumeroFactura { get; set; }
            public string RazonSocialAcreedor { get; set; }
            public string CodigoMoneda { get; set; }
            public decimal Monto { get; set; }
            public string CuentaOrigen { get; set; }
            public string BancoOrigen { get; set; }
            public string CuentaDestino { get; set; }
            public string BancoDestino { get; set; }
            public string FormaPago { get; set; }
        }

        private int PropuestaId
        {
            get
            {
                int.TryParse(Request.QueryString["id"], out var id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PropuestaId <= 0)
                {
                    MostrarMensaje("ID de propuesta inválido", "danger");
                    btnEditar.Visible = false;
                    btnAnular.Visible = false;
                    return;
                }

                CargarDetalle();
            }
        }

        private void CargarDetalle()
        {
            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta solicitada", "danger");
                    btnEditar.Visible = false;
                    btnAnular.Visible = false;
                    return;
                }

                // Cargar información general
                lblPropuestaId.Text = propuesta.PropuestaId?.ToString() ?? "-";
                lblEstado.Text = propuesta.Estado ?? "-";
                lblEstado.CssClass = $"badge-estado estado-{propuesta.Estado?.ToLower()}";
                lblFechaCreacion.Text = propuesta.FechaHoraCreacion?.ToString("dd/MM/yyyy HH:mm") ?? "-";
                lblUsuarioCreador.Text = propuesta.UsuarioCreacion != null
                    ? $"{propuesta.UsuarioCreacion.Nombre} {propuesta.UsuarioCreacion.Apellidos}"
                    : "-";
                lblPais.Text = propuesta.EntidadBancaria?.Pais?.Nombre ?? "-";
                lblBanco.Text = propuesta.EntidadBancaria?.Nombre ?? "-";
                lblTotalPagos.Text = propuesta.DetallesPropuesta?.Count.ToString() ?? "0";

                // Controlar visibilidad de botones según estado
                ConfigurarBotonesPorEstado(propuesta.Estado);

                // Cargar totales por moneda
                CargarTotalesPorMoneda(propuesta);

                // Cargar detalles
                CargarDetallesPagos(propuesta);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar el detalle de la propuesta: {ex.Message}", "danger");
            }
        }

        private void ConfigurarBotonesPorEstado(string estado)
        {
            // Solo se puede editar y anular propuestas en estado "Pendiente"
            bool esPendiente = estado == "Pendiente";
            btnEditar.Visible = esPendiente;
            btnAnular.Visible = esPendiente;

            if (esPendiente)
            {
                btnAnular.OnClientClick = "return mostrarModalAnular();";
            }
        }

        private void CargarTotalesPorMoneda(PropuestasPagoDTO propuesta)
        {
            if (propuesta.DetallesPropuesta == null || propuesta.DetallesPropuesta.Count == 0)
                return;

            var totales = propuesta.DetallesPropuesta
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

        private void CargarDetallesPagos(PropuestasPagoDTO propuesta)
        {
            if (propuesta.DetallesPropuesta == null || propuesta.DetallesPropuesta.Count == 0)
            {
                lblCantidadPagos.Text = "0 pago(s)";
                return;
            }

            var detallesVM = propuesta.DetallesPropuesta
                .Select(d => new DetalleViewModel
                {
                    NumeroFactura = d.Factura?.NumeroFactura ?? "-",
                    RazonSocialAcreedor = d.Factura?.Acreedor?.RazonSocial ?? "-",
                    CodigoMoneda = d.Factura?.Moneda?.CodigoIso ?? "-",
                    Monto = d.MontoPago,
                    CuentaOrigen = d.CuentaPropia?.NumeroCuenta ?? "-",
                    BancoOrigen = d.CuentaPropia?.EntidadBancaria?.Nombre ?? "-",
                    CuentaDestino = d.CuentaAcreedor?.NumeroCuenta ?? "-",
                    BancoDestino = d.CuentaAcreedor?.EntidadBancaria?.Nombre ?? "-",
                    FormaPago = MapearFormaPago(d.FormaPago)
                })
                .ToList();

            gvDetalles.DataSource = detallesVM;
            gvDetalles.DataBind();

            lblCantidadPagos.Text = $"{detallesVM.Count} pago(s)";

            // Guardar en ViewState para paginación
            ViewState["DetallesViewModel"] = detallesVM;
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

            var detallesVM = ViewState["DetallesViewModel"] as List<DetalleViewModel>;
            if (detallesVM != null)
            {
                gvDetalles.DataSource = detallesVM;
                gvDetalles.DataBind();
            }

            upDetalles.Update();
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/EditarPropuesta.aspx?id={PropuestaId}");
        }

        protected void btnAnular_Click(object sender, EventArgs e)
        {
            // Este botón abre el modal mediante JavaScript
            // La anulación real se hace en btnConfirmarAnulacion_Click
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            // Lógica para exportar a Excel (no implementada en este snippet)
        }

        protected void btnConfirmarAnulacion_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                if (propuesta.Estado != "Pendiente")
                {
                    MostrarMensaje("Solo se pueden anular propuestas en estado Pendiente", "warning");
                    return;
                }

                // Actualizar estado y motivo
                propuesta.Estado = "Anulada";
                // Aquí deberías tener un campo en el DTO para guardar el motivo de anulación
                // propuesta.MotivoAnulacion = txtMotivoAnulacion.Text;

                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                propuesta.UsuarioModificacion = new UsuariosDTO { UsuarioId = usuarioId };
                propuesta.FechaHoraModificacion = DateTime.Now;

                bool resultado = propuestasBO.Modificar(propuesta)==1;

                if (resultado)
                {
                    // Cerrar modal y recargar
                    ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal",
                        "$('#modalAnular').modal('hide');", true);

                    MostrarMensaje("Propuesta anulada exitosamente", "success");
                    CargarDetalle();
                }
                else
                {
                    MostrarMensaje("Error al anular la propuesta", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al anular la propuesta: {ex.Message}", "danger");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }
    }
}