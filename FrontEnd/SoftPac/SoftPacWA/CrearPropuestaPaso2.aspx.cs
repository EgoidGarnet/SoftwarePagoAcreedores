using SoftPac.Business;
using SoftPacBusiness.FacturasWS;
using SoftPacBusiness.PropuestaPagoWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class CrearPropuestaPaso2 : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();
        private PaisesBO paisesBO = new PaisesBO();
        private EntidadesBancariasBO bancosBO = new EntidadesBancariasBO();
        private PropuestasPagoBO propuestasPagoBO = new PropuestasPagoBO();
        private List<SoftPacBusiness.FacturasWS.facturasDTO> FacturasDisponibles { get { return Session["FacturasDisponibles"] as List<SoftPacBusiness.FacturasWS.facturasDTO>; } set { Session["FacturasDisponibles"] = (List<SoftPacBusiness.FacturasWS.facturasDTO>)value; } }
        private int PaisId
        {
            get => Convert.ToInt32(Session["PropuestaPago_PaisId"]);
        }

        private int BancoId
        {
            get => Convert.ToInt32(Session["PropuestaPago_BancoId"]);

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
                CargarFacturas();
            }
        }

        private bool ValidarSesionPasoAnterior()
        {
            return Session["PropuestaPago_PaisId"] != null &&
                   Session["PropuestaPago_BancoId"] != null &&
                   Session["PropuestaPago_FechaLimiteVencimiento"] != null;
        }

        private void CargarResumenFiltros()
        {
            try
            {
                int paisId = Convert.ToInt32(Session["PropuestaPago_PaisId"]);
                int bancoId = Convert.ToInt32(Session["PropuestaPago_BancoId"]);
                int diasPlazo = Convert.ToInt32(Session["PropuestaPago_PlazoVencimiento"]);

                var pais = paisesBO.ObtenerPorId(PaisId);
                var banco = bancosBO.ObtenerPorId(BancoId);

                lblPais.Text = pais?.nombre ?? "-";
                lblBanco.Text = banco?.nombre ?? "-";
                lblPlazo.Text = $"Próximos {diasPlazo} días";
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar resumen: {ex.Message}", "danger");
            }
        }

        private void CargarFacturas()
        {
            try
            {
                int paisId = Convert.ToInt32(Session["PropuestaPago_PaisId"]);
                int bancoId = Convert.ToInt32(Session["PropuestaPago_BancoId"]);
                DateTime fechaLimite = Convert.ToDateTime(Session["PropuestaPago_FechaLimiteVencimiento"]);

                // Llamar al BO para obtener facturas pendientes filtradas
                var facturas = facturasBO.ListarPendientesPorCriterios(paisId, fechaLimite)
                    .OrderBy(f => f.fecha_limite_pago)
                    .ToList();

                gvFacturas.DataSource = facturas;
                gvFacturas.DataBind();

                lblTotalRegistros.Text = $"{facturas.Count} factura(s) disponible(s)";

                // Guardar lista en Session para uso posterior
                FacturasDisponibles = facturas;

                if (facturas.Count == 0)
                {
                    MostrarMensaje("No hay facturas pendientes que cumplan con los criterios seleccionados", "info");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar facturas: {ex.Message}", "danger");
                gvFacturas.DataSource = null;
                gvFacturas.DataBind();
                lblTotalRegistros.Text = "0 facturas disponibles";
            }
        }

        protected void gvFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFacturas.PageIndex = e.NewPageIndex;
            var facturas = FacturasDisponibles as List<SoftPacBusiness.FacturasWS.facturasDTO>;
            gvFacturas.DataSource = facturas;
            gvFacturas.DataBind();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            // Volver al Paso 1 sin borrar datos
            Response.Redirect("~/CrearPropuestaPaso1.aspx");
        }

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener facturas seleccionadas
                var facturasSeleccionadas = new List<int>();

                foreach (GridViewRow row in gvFacturas.Rows)
                {
                    var chk = row.FindControl("chkSeleccionar") as CheckBox;
                    if (chk != null && chk.Checked)
                    {
                        // Obtener el ID de la factura de la fila
                        var facturas = FacturasDisponibles;
                        if (facturas != null && row.RowIndex < facturas.Count)
                        {
                            var factura = facturas[row.RowIndex];
                            facturasSeleccionadas.Add(factura.factura_id);
                        }
                    }
                }

                // Validar que se haya seleccionado al menos una factura
                if (facturasSeleccionadas.Count == 0)
                {
                    MostrarMensaje("Debe seleccionar al menos una factura para continuar", "warning");
                    return;
                }

                // Llamar al BO para generar detalles parciales (Factura + Cuenta Acreedor)
                propuestasPagoDTO propuestaParcial = propuestasPagoBO.GenerarDetallesParciales(facturasSeleccionadas, BancoId);

                if (propuestaParcial.detalles_propuesta == null || propuestaParcial.detalles_propuesta.Length == 0)
                {
                    MostrarMensaje("Error al procesar las facturas seleccionadas", "danger");
                    return;
                }
                List<int> facturasNoIncluidas = new List<int>();
                foreach (int fac in facturasSeleccionadas)
                {
                    if (propuestaParcial.detalles_propuesta.All(d => d.factura.factura_id != fac))
                    {
                        facturasNoIncluidas.Add(fac);
                    }
                }
                if (facturasNoIncluidas.Count > 0)
                {
                    string mensaje = "Las siguientes facturas no se pudieron incluir en la propuesta debido a la falta de cuentas bancarias correspondientes:<br/>";
                    mensaje += string.Join(", ", facturasNoIncluidas);
                    MostrarMensaje(mensaje, "warning");
                }

                // Guardar detalles parciales en sesión
                Session["PropuestaPago_DetallesParciales"] = propuestaParcial;

                // Redirigir al Paso 3
                Session.Remove("FacturasDisponibles");
                Response.Redirect("~/CrearPropuestaPaso3.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al procesar las facturas: {ex.Message}", "danger");
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