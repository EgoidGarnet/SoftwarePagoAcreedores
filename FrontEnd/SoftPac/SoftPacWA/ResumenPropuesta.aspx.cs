using SoftPac.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPacBusiness.UsuariosWS;

using SoftPacBusiness.PropuestaPagoWS;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;


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
        private usuariosDTO UsuarioLogueado
        {
            get
            {
                return (usuariosDTO)Session["UsuarioLogueado"];
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
                var propuestaCompleta = Session["PropuestaPago_PropuestaCompleta"] as propuestasPagoDTO;

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

        private void CargarInformacionResumen(propuestasPagoDTO propuesta)
        {
            lblPais.Text = propuesta.entidad_bancaria?.pais?.nombre ?? "-";
            lblBanco.Text = propuesta.entidad_bancaria?.nombre ?? "-";
            lblTotalFacturas.Text = propuesta.detalles_propuesta?.Select(d => d.factura?.factura_id).Distinct().Count().ToString() ?? "0";
            lblTotalPagos.Text = propuesta.detalles_propuesta?.Length.ToString() ?? "0";
        }

        private void CargarTotalesPorMoneda(propuestasPagoDTO propuesta)
        {
            var totales = propuesta.detalles_propuesta
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

        private void CargarDetallePagos(propuestasPagoDTO propuesta)
        {
            var pagosVM = propuesta.detalles_propuesta
                .Select(d => new PagoViewModel
                {
                    DetalleId = d.detalle_propuesta_idSpecified ? d.detalle_propuesta_id : 0,
                    NumeroFactura = d.factura?.numero_factura ?? "-",
                    RazonSocialAcreedor = d.factura?.acreedor?.razon_social ?? "-",
                    CodigoMoneda = d.factura?.moneda?.codigo_iso ?? "-",
                    Monto = d.monto_pago,
                    CuentaOrigen = d.cuenta_propia?.numero_cuenta ?? "-",
                    CuentaDestino = d.cuenta_acreedor?.numero_cuenta ?? "-",
                    FormaPago = d.forma_pago == 0 ? "T" : ((char)d.forma_pago).ToString()
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
                var propuestaCompleta = Session["PropuestaPago_PropuestaCompleta"] as propuestasPagoDTO;

                if (propuestaCompleta == null)
                {
                    MostrarMensaje("Error: No se encontró la propuesta a guardar", "danger");
                    return;
                }

                // Actualizar formas de pago según lo seleccionado en el GridView
                ActualizarFormasDePago(propuestaCompleta);

                // Configurar datos de auditoría

                SoftPacBusiness.PropuestaPagoWS.usuariosDTO user = new SoftPacBusiness.PropuestaPagoWS.usuariosDTO();
                user.usuario_id = UsuarioLogueado.usuario_id;
               

                propuestaCompleta.usuario_creacion = user;
                propuestaCompleta.usuario_modificacion = user;
                propuestaCompleta.fecha_hora_creacion = DateTime.Now;
                propuestaCompleta.fecha_hora_modificacion = propuestaCompleta.fecha_hora_creacion;
                propuestaCompleta.estado = "Pendiente";

                // Guardar en base de datos
                int resultado = propuestasBO.Insertar(propuestaCompleta);
                propuestaCompleta.propuesta_id = resultado;
                if (resultado>0)
                {
                    // Guardar el ID para mostrarlo en el modal
                    lblPropuestaId.Text = propuestaCompleta.propuesta_id.ToString();

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

        private void ActualizarFormasDePago(propuestasPagoDTO propuesta)
        {
            foreach (GridViewRow row in gvPagos.Rows)
            {
                var ddlFormaPago = row.FindControl("ddlFormaPago") as DropDownList;
                if (ddlFormaPago != null && row.RowIndex < propuesta.detalles_propuesta.Length)
                {
                    var detalle = propuesta.detalles_propuesta[row.RowIndex];
                    detalle.forma_pago = ddlFormaPago.SelectedValue[0]; // Tomar el primer carácter (T, C, E)
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