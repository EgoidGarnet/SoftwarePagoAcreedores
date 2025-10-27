using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class ResumenPropuesta : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();

        // Clase auxiliar para mostrar en el GridView
        [Serializable]
        public class PagoViewModel
        {
            public int DetalleId { get; set; }
            public string NumeroFactura { get; set; }
            public string RazonSocialAcreedor { get; set; }
            public string CodigoMoneda { get; set; }
            public decimal Monto { get; set; }
            public string CuentaOrigen { get; set; }
            public string CuentaDestino { get; set; }
            public string FormaPago { get; set; }
        }
        private UsuariosDTO UsuarioLogueado
        {
            get
            {
                return (UsuariosDTO)Session["UsuarioLogueado"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!ValidarSesionPasoAnterior())
                {
                    Response.Redirect("~/CrearPropuestaPaso1.aspx");
                    return;
                }

                CargarResumen();
            }
        }

        private bool ValidarSesionPasoAnterior()
        {
            return Session["PropuestaPago_DetallesParciales"] != null &&
                   Session["PropuestaPago_CuentasSeleccionadas"] != null;
        }

        private void CargarResumen()
        {
            try
            {
                var propuestaCompleta = Session["PropuestaPago_PropuestaCompleta"] as PropuestasPagoDTO;

                // Guardar en sesión para uso posterior
                Session["PropuestaPago_PropuestaCompleta"] = propuestaCompleta;

                // Cargar información en pantalla
                CargarInformacionResumen(propuestaCompleta);
                CargarTotalesPorMoneda(propuestaCompleta);
                CargarDetallePagos(propuestaCompleta);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar resumen: {ex.Message}", "danger");
            }
        }

        private void CargarInformacionResumen(PropuestasPagoDTO propuesta)
        {
            lblPais.Text = propuesta.EntidadBancaria?.Pais?.Nombre ?? "-";
            lblBanco.Text = propuesta.EntidadBancaria?.Nombre ?? "-";
            lblTotalFacturas.Text = propuesta.DetallesPropuesta?.Select(d => d.Factura?.FacturaId).Distinct().Count().ToString() ?? "0";
            lblTotalPagos.Text = propuesta.DetallesPropuesta?.Count.ToString() ?? "0";
        }

        private void CargarTotalesPorMoneda(PropuestasPagoDTO propuesta)
        {
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

        private void CargarDetallePagos(PropuestasPagoDTO propuesta)
        {
            var pagosVM = propuesta.DetallesPropuesta
                .Select(d => new PagoViewModel
                {
                    DetalleId = d.DetallePropuestaId ?? 0,
                    NumeroFactura = d.Factura?.NumeroFactura ?? "-",
                    RazonSocialAcreedor = d.Factura?.Acreedor?.RazonSocial ?? "-",
                    CodigoMoneda = d.Factura?.Moneda?.CodigoIso ?? "-",
                    Monto = d.MontoPago,
                    CuentaOrigen = d.CuentaPropia?.NumeroCuenta ?? "-",
                    CuentaDestino = d.CuentaAcreedor?.NumeroCuenta ?? "-",
                    FormaPago = d.FormaPago.ToString() ?? "T"
                })
                .ToList();

            gvPagos.DataSource = pagosVM;
            gvPagos.DataBind();

            // Guardar en ViewState para paginación
            ViewState["PagosViewModel"] = pagosVM;
        }

        protected void gvPagos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPagos.PageIndex = e.NewPageIndex;

            var pagosVM = ViewState["PagosViewModel"] as List<PagoViewModel>;
            if (pagosVM != null)
            {
                gvPagos.DataSource = pagosVM;
                gvPagos.DataBind();
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            // Volver al Paso 3 sin borrar datos
            Response.Redirect("~/CrearPropuestaPaso3.aspx");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Limpiar todas las sesiones del flujo
            LimpiarSesionesPropuesta();
            Response.Redirect("~/PropuestasPago.aspx");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var propuestaCompleta = Session["PropuestaPago_PropuestaCompleta"] as PropuestasPagoDTO;

                if (propuestaCompleta == null)
                {
                    MostrarMensaje("Error: No se encontró la propuesta a guardar", "danger");
                    return;
                }

                // Actualizar formas de pago según lo seleccionado en el GridView
                ActualizarFormasDePago(propuestaCompleta);

                // Configurar datos de auditoría

                propuestaCompleta.UsuarioCreacion = UsuarioLogueado;
                propuestaCompleta.FechaHoraCreacion = DateTime.Now;
                propuestaCompleta.Estado = "Pendiente";

                // Guardar en base de datos
                bool resultado = propuestasBO.Insertar(propuestaCompleta)==1;

                if (resultado && propuestaCompleta.PropuestaId.HasValue)
                {
                    // Guardar el ID para mostrarlo en el modal
                    lblPropuestaId.Text = propuestaCompleta.PropuestaId.Value.ToString();

                    // Limpiar sesiones
                    LimpiarSesionesPropuesta();

                    // Mostrar modal de confirmación usando JavaScript
                    ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal",
                             "var modal = new bootstrap.Modal(document.getElementById('modalConfirmacion')); modal.show();", true);
                }
                else
                {
                    MostrarMensaje("Error al guardar la propuesta. Intente nuevamente.", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al guardar la propuesta: {ex.Message}", "danger");
            }
        }

        private void ActualizarFormasDePago(PropuestasPagoDTO propuesta)
        {
            foreach (GridViewRow row in gvPagos.Rows)
            {
                var ddlFormaPago = row.FindControl("ddlFormaPago") as DropDownList;
                if (ddlFormaPago != null && row.RowIndex < propuesta.DetallesPropuesta.Count)
                {
                    var detalle = propuesta.DetallesPropuesta[row.RowIndex];
                    detalle.FormaPago = ddlFormaPago.SelectedValue[0]; // Tomar el primer carácter (T, C, E)
                }
            }
        }

        protected void btnIrADetalle_Click(object sender, EventArgs e)
        {
            int propuestaId = int.Parse(lblPropuestaId.Text);
            Response.Redirect($"~/DetallePropuesta.aspx?id={propuestaId}");
        }

        protected void btnIrAListado_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PropuestasPago.aspx");
        }

        private void LimpiarSesionesPropuesta()
        {
            Session.Remove("PropuestaPago_PaisId");
            Session.Remove("PropuestaPago_BancoId");
            Session.Remove("PropuestaPago_PlazoVencimiento");
            Session.Remove("PropuestaPago_FechaLimiteVencimiento");
            Session.Remove("PropuestaPago_DetallesParciales");
            Session.Remove("PropuestaPago_CuentasSeleccionadas");
            Session.Remove("PropuestaPago_PropuestaCompleta");
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }
    }
}