using SoftPac.Business;
using SoftPacBusiness.CuentasPropiasWS;
using SoftPacBusiness.PropuestaPagoWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using monedasDTO = SoftPacBusiness.CuentasPropiasWS.monedasDTO;
using cuentasPropiasDTO = SoftPacBusiness.CuentasPropiasWS.cuentasPropiasDTO;

namespace SoftPacWA
{
    public partial class CrearPropuestaPaso3 : System.Web.UI.Page
    {
        private CuentasPropiasBO cuentasPropiasBO = new CuentasPropiasBO();

        // Clase auxiliar para mostrar los saldos requeridos
        [Serializable]
        public class SaldoRequerido
        {
            public monedasDTO Moneda { get; set; }
            public decimal MontoRequerido { get; set; }
        }

        // Clase auxiliar para agrupar cuentas por moneda
        [Serializable]
        public class CuentasPorMoneda
        {
            public monedasDTO Moneda { get; set; }
            public int CantidadCuentas { get; set; }
            public List<cuentasPropiasDTO> Cuentas { get; set; }
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

                CargarResumenFiltros();
                CargarSaldosRequeridos();
                CargarCuentasPorMoneda();
            }
        }

        private bool ValidarSesionPasoAnterior()
        {
            return Session["PropuestaPago_DetallesParciales"] is propuestasPagoDTO propuesta
                   && propuesta.detalles_propuesta?.Length > 0;
        }

        private void CargarResumenFiltros()
        {
            try
            {
                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as propuestasPagoDTO;

                if (propuestaParcial == null)
                {
                    MostrarMensaje("Error: No se encontraron los datos de la propuesta", "danger");
                    return;
                }

                lblPais.Text = propuestaParcial.entidad_bancaria?.pais?.nombre ?? "-";
                lblBanco.Text = propuestaParcial.entidad_bancaria?.nombre ?? "-";
                lblCantidadFacturas.Text = propuestaParcial.detalles_propuesta?.Length.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar resumen: {ex.Message}", "danger");
            }
        }

        private void CargarSaldosRequeridos()
        {
            try
            {
                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as propuestasPagoDTO;

                if (propuestaParcial?.detalles_propuesta == null)
                {
                    MostrarMensaje("Error: No se encontraron detalles de la propuesta", "danger");
                    return;
                }

                var saldosRequeridos = propuestaParcial.detalles_propuesta
                    .Where(d => d.factura?.moneda != null)
                    .GroupBy(d => d.factura.moneda.moneda_id)
                    .Select(g => new SaldoRequerido
                    {
                        Moneda = DTOConverter.Convertir<SoftPacBusiness.PropuestaPagoWS.monedasDTO, monedasDTO>(g.First(d => true).factura.moneda),
                        MontoRequerido = g.Sum(d => d.monto_pago) * 1.01m // margen 1%
                    })
                    .OrderBy(s => s.Moneda.nombre)
                    .ToList();

                if (saldosRequeridos.Count == 0)
                {
                    MostrarMensaje("No se pudieron calcular los saldos requeridos", "warning");
                    return;
                }

                rptSaldosRequeridos.DataSource = saldosRequeridos;
                rptSaldosRequeridos.DataBind();
                ViewState["SaldosRequeridos"] = saldosRequeridos;
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al calcular saldos requeridos: {ex.Message}", "danger");
            }
        }

        private void CargarCuentasPorMoneda()
        {
            try
            {
                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as propuestasPagoDTO;
                var saldosRequeridos = ViewState["SaldosRequeridos"] as List<SaldoRequerido>;

                if (propuestaParcial == null || saldosRequeridos == null || saldosRequeridos.Count == 0)
                {
                    pnlNoCuentas.Visible = true;
                    return;
                }

                int bancoId = propuestaParcial.entidad_bancaria?.entidad_bancaria_id ?? 0;
                if (bancoId == 0)
                {
                    MostrarMensaje("Error: No se pudo identificar la entidad bancaria", "danger");
                    pnlNoCuentas.Visible = true;
                    return;
                }

                var todasCuentas = cuentasPropiasBO.ListarPorEntidadBancaria(bancoId)
                    .Where(c => c.activa && c.moneda != null)
                    .ToList();

                if (todasCuentas.Count == 0)
                {
                    pnlNoCuentas.Visible = true;
                    MostrarMensaje("No hay cuentas activas disponibles para este banco", "warning");
                    return;
                }

                var monedasIdsRequeridas = saldosRequeridos
                    .Where(s => s.Moneda != null)
                    .Select(s => s.Moneda.moneda_id)
                    .ToHashSet();

                var cuentasPorMoneda = todasCuentas
                    .Where(c => monedasIdsRequeridas.Contains(c.moneda.moneda_id))
                    .GroupBy(c => c.moneda)
                    .Select(g => new CuentasPorMoneda
                    {
                        Moneda = g.Key,
                        CantidadCuentas = g.Count(),
                        Cuentas = g.OrderByDescending(c => c.saldo_disponible).ToList()
                    })
                    .OrderBy(c => c.Moneda.nombre)
                    .ToList();

                if (cuentasPorMoneda.Count == 0)
                {
                    pnlNoCuentas.Visible = true;
                    MostrarMensaje("No hay cuentas disponibles en las monedas requeridas", "warning");
                    return;
                }

                rptCuentasPorMoneda.DataSource = cuentasPorMoneda;
                rptCuentasPorMoneda.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar cuentas: {ex.Message}", "danger");
                pnlNoCuentas.Visible = true;
            }
        }

        protected void rptCuentasPorMoneda_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cuentasPorMoneda = e.Item.DataItem as CuentasPorMoneda;
                if (cuentasPorMoneda != null)
                {
                    var rptCuentas = e.Item.FindControl("rptCuentas") as Repeater;
                    if (rptCuentas != null)
                    {
                        rptCuentas.DataSource = cuentasPorMoneda.Cuentas;
                        rptCuentas.DataBind();
                    }
                }
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/CrearPropuestaPaso2.aspx");
        }

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener los checkboxes seleccionados
                var checkboxesSeleccionados = Request.Form.GetValues("cuentaSeleccionada");

                if (checkboxesSeleccionados == null || checkboxesSeleccionados.Length == 0)
                {
                    MostrarMensaje("Debe seleccionar al menos una cuenta", "warning");
                    return;
                }

                // Construir lista de IDs de cuentas seleccionadas
                var cuentasSeleccionadasIds = checkboxesSeleccionados
                    .Select(idStr => int.TryParse(idStr, out int id) ? id : 0)
                    .Where(id => id > 0)
                    .ToList();

                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as propuestasPagoDTO;

                if (propuestaParcial == null)
                {
                    MostrarMensaje("Error: No se encontraron los datos de la propuesta", "danger");
                    return;
                }

                // Generar detalles con asignación de cuentas
                var propuestasBO = new PropuestasPagoBO();
                propuestasPagoDTO propuestaCompleta = propuestasBO.GenerarDetallesPropuesta(
                    propuestaParcial,
                    cuentasSeleccionadasIds
                );

                if (propuestaCompleta.detalles_propuesta == null || propuestaCompleta.detalles_propuesta.Length == 0)
                {
                    MostrarMensaje("Error al generar los detalles de la propuesta", "danger");
                    return;
                }

                // Verificar si existen pagos sin cuenta asignada
                var pagosSinCuenta = propuestaCompleta.detalles_propuesta
                    .Where(d => d.cuenta_propia == null)
                    .ToList();

                if (pagosSinCuenta.Any())
                {
                    string mensaje = "No se pudo asignar una cuenta a los siguientes pagos:<br/><ul>";
                    foreach (var pago in pagosSinCuenta)
                    {
                        mensaje += $"<li>Factura: <strong>{pago.factura?.numero_factura ?? "(sin número)"}</strong> — Monto: {pago.monto_pago:N2} {pago.factura?.moneda?.codigo_iso ?? ""}</li>";
                    }
                    mensaje += "</ul><br/>Verifique que las cuentas seleccionadas tengan saldo suficiente en las monedas requeridas.";

                    MostrarMensaje(mensaje, "danger");
                    return;
                }

                // Todo correcto → guardar propuesta y continuar
                Session["PropuestaPago_CuentasSeleccionadas"] = cuentasSeleccionadasIds;
                Session["PropuestaPago_PropuestaCompleta"] = propuestaCompleta;

                Response.Redirect("~/ResumenPropuesta.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al procesar las cuentas seleccionadas: {ex.Message}", "danger");
            }
        }


        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
            ScriptManager.RegisterStartupScript(this, GetType(), "scrollTop", "window.scrollTo(0,0);", true);
        }
    }
}