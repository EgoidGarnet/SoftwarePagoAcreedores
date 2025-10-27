using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class CrearPropuestaPaso3 : System.Web.UI.Page
    {
        private CuentasPropiasBO cuentasPropiasBO = new CuentasPropiasBO();

        // Clase auxiliar para mostrar los saldos requeridos
        [Serializable]
        public class SaldoRequerido
        {
            public MonedasDTO Moneda { get; set; }
            public decimal MontoRequerido { get; set; }
        }

        // Clase auxiliar para agrupar cuentas por moneda
        [Serializable]
        public class CuentasPorMoneda
        {
            public MonedasDTO Moneda { get; set; }
            public int CantidadCuentas { get; set; }
            public List<CuentasPropiasDTO> Cuentas { get; set; }
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
            return Session["PropuestaPago_DetallesParciales"] is PropuestasPagoDTO propuesta
                   && propuesta.DetallesPropuesta?.Count > 0;
        }

        private void CargarResumenFiltros()
        {
            try
            {
                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as PropuestasPagoDTO;

                if (propuestaParcial == null)
                {
                    MostrarMensaje("Error: No se encontraron los datos de la propuesta", "danger");
                    return;
                }

                lblPais.Text = propuestaParcial.EntidadBancaria?.Pais?.Nombre ?? "-";
                lblBanco.Text = propuestaParcial.EntidadBancaria?.Nombre ?? "-";
                lblCantidadFacturas.Text = propuestaParcial.DetallesPropuesta?.Count.ToString() ?? "0";
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
                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as PropuestasPagoDTO;

                if (propuestaParcial?.DetallesPropuesta == null)
                {
                    MostrarMensaje("Error: No se encontraron detalles de la propuesta", "danger");
                    return;
                }

                var saldosRequeridos = propuestaParcial.DetallesPropuesta
                    .Where(d => d.Factura?.Moneda != null)
                    .GroupBy(d => d.Factura.Moneda)
                    .Select(g => new SaldoRequerido
                    {
                        Moneda = g.Key,
                        MontoRequerido = g.Sum(d => d.MontoPago) * 1.01m // margen 1%
                    })
                    .OrderBy(s => s.Moneda.Nombre)
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
                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as PropuestasPagoDTO;
                var saldosRequeridos = ViewState["SaldosRequeridos"] as List<SaldoRequerido>;

                if (propuestaParcial == null || saldosRequeridos == null || saldosRequeridos.Count == 0)
                {
                    pnlNoCuentas.Visible = true;
                    return;
                }

                int bancoId = propuestaParcial.EntidadBancaria?.EntidadBancariaId ?? 0;
                if (bancoId == 0)
                {
                    MostrarMensaje("Error: No se pudo identificar la entidad bancaria", "danger");
                    pnlNoCuentas.Visible = true;
                    return;
                }

                var todasCuentas = cuentasPropiasBO.ListarPorEntidadBancaria(bancoId)
                    .Where(c => c.Activa && c.Moneda != null)
                    .ToList();

                if (todasCuentas.Count == 0)
                {
                    pnlNoCuentas.Visible = true;
                    MostrarMensaje("No hay cuentas activas disponibles para este banco", "warning");
                    return;
                }

                var monedasIdsRequeridas = saldosRequeridos
                    .Where(s => s.Moneda != null)
                    .Select(s => s.Moneda.MonedaId)
                    .ToHashSet();

                var cuentasPorMoneda = todasCuentas
                    .Where(c => monedasIdsRequeridas.Contains(c.Moneda.MonedaId))
                    .GroupBy(c => c.Moneda)
                    .Select(g => new CuentasPorMoneda
                    {
                        Moneda = g.Key,
                        CantidadCuentas = g.Count(),
                        Cuentas = g.OrderByDescending(c => c.SaldoDisponible).ToList()
                    })
                    .OrderBy(c => c.Moneda.Nombre)
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

                var propuestaParcial = Session["PropuestaPago_DetallesParciales"] as PropuestasPagoDTO;

                if (propuestaParcial == null)
                {
                    MostrarMensaje("Error: No se encontraron los datos de la propuesta", "danger");
                    return;
                }

                // Generar detalles con asignación de cuentas
                var propuestasBO = new PropuestasPagoBO();
                PropuestasPagoDTO propuestaCompleta = propuestasBO.GenerarDetallesPropuesta(
                    propuestaParcial,
                    cuentasSeleccionadasIds
                );

                if (propuestaCompleta.DetallesPropuesta == null || propuestaCompleta.DetallesPropuesta.Count == 0)
                {
                    MostrarMensaje("Error al generar los detalles de la propuesta", "danger");
                    return;
                }

                // Verificar si existen pagos sin cuenta asignada
                var pagosSinCuenta = propuestaCompleta.DetallesPropuesta
                    .Where(d => d.CuentaPropia == null)
                    .ToList();

                if (pagosSinCuenta.Any())
                {
                    string mensaje = "No se pudo asignar una cuenta a los siguientes pagos:<br/><ul>";
                    foreach (var pago in pagosSinCuenta)
                    {
                        mensaje += $"<li>Factura: <strong>{pago.Factura?.NumeroFactura ?? "(sin número)"}</strong> — Monto: {pago.MontoPago:N2} {pago.Factura?.Moneda?.CodigoIso ?? ""}</li>";
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
