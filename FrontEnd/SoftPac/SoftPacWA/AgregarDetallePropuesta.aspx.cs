using SoftPac.Business;
using SoftPacBusiness.PropuestaPagoWS;
using SoftPacBusiness.UsuariosWS;
using SoftPacBusiness.FacturasWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class AgregarDetallePropuesta : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();
        private FacturasBO facturasBO = new FacturasBO();
        private CuentasPropiasBO cuentasPropiasBO = new CuentasPropiasBO();

        private int PropuestaId
        {
            get
            {
                int.TryParse(Request.QueryString["id"], out var id);
                return id;
            }
        }

        private List<int> FacturasIds
        {
            get
            {
                return (List<int>)Session["FacturasIds"];
            }
        }

        private List<detallesPropuestaDTO> DetallesNuevos
        {
            get
            {
                if (Session["DetallesNuevos"] == null)
                    Session["DetallesNuevos"] = new List<detallesPropuestaDTO>();
                return (List<detallesPropuestaDTO>)Session["DetallesNuevos"];
            }
            set
            {
                Session["DetallesNuevos"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PropuestaId <= 0)
                {
                    MostrarMensaje("ID de propuesta inválido", "danger");
                    pnlDetalleGenerado.Visible = false;
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
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                // Mostrar datos básicos
                lblPropuestaId.Text = propuesta.propuesta_id.ToString();
                lblBanco.Text = propuesta.entidad_bancaria?.nombre ?? "-";
                lblPais.Text = propuesta.entidad_bancaria?.pais?.nombre ?? "-";

                // Guardar en Session para seguir el flujo (el usuario confirmó que este flujo está OK)
                Session["PropuestaToAdd"] = propuesta;
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar la propuesta: {ex.Message}", "danger");
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/EditarPropuesta.aspx?id={PropuestaId}");
        }

        protected void btnBuscarFacturas_Click(object sender, EventArgs e)
        {
            try
            {
                var propuesta = Session["PropuestaToAdd"] as propuestasPagoDTO ?? propuestasBO.ObtenerPorId(PropuestaId);
                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                int paisId = propuesta.entidad_bancaria?.pais?.pais_id ?? 0;
                if (paisId == 0)
                {
                    MostrarMensaje("No se pudo determinar el país de la propuesta", "warning");
                    return;
                }

                int dias;
                if (!int.TryParse(ddlIntervaloDias.SelectedValue, out dias))
                    dias = 15;

                DateTime fechaLimite = DateTime.Now.AddDays(dias);

                var facturas = facturasBO.ListarPendientesPorCriterios(paisId, fechaLimite) ?? new List<SoftPacBusiness.FacturasWS.facturasDTO>();

                // Preparar datasource sin fecha_vencimiento como indicaste
                var items = facturas
                    .Where(f=>!FacturasIds.Contains(f.factura_id))
                    .Select(f => new
                    {
                        Id = f.factura_id,
                        Text = $"{f.numero_factura} - {f.acreedor?.razon_social} - {f.moneda?.codigo_iso}"
                    }).ToList();

                ddlFacturas.DataSource = items;
                ddlFacturas.DataTextField = "Text";
                ddlFacturas.DataValueField = "Id";
                
                ddlFacturas.DataBind();

                if (ddlFacturas.Items.Count > 0)
                {
                    ddlFacturas.Items.Insert(0, new ListItem("-- Seleccione facturas --", ""));
                    ddlFacturas.SelectedIndex = 0;
                }

                if (!items.Any())
                {
                    ddlFacturas.Items.Insert(0, new ListItem("-- No hay facturas pendientes --",""));
                    MostrarMensaje("No se encontraron facturas pendientes en el intervalo seleccionado.", "info");
                }
                else
                {
                    MostrarMensaje($"{items.Count} factura(s) encontradas.", "success");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al listar facturas: {ex.Message}", "danger");
            }
        }

        protected void btnGenerarDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlFacturas.SelectedValue) || ddlFacturas.SelectedIndex == 0)
                {
                    MostrarMensaje("Seleccione una factura válida.", "warning");
                    return;
                }

                var propuesta = Session["PropuestaToAdd"] as propuestasPagoDTO ?? propuestasBO.ObtenerPorId(PropuestaId);
                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                int facturaId = int.Parse(ddlFacturas.SelectedValue);
                int bancoId = propuesta.entidad_bancaria?.entidad_bancaria_id ?? 0;
                if (bancoId == 0)
                {
                    MostrarMensaje("No se pudo determinar la entidad bancaria de la propuesta", "warning");
                    return;
                }

                // Llamar a GenerarDetallesParciales con la lista que contiene solo esta factura
                var resultadoPropuesta = propuestasBO.GenerarDetallesParciales(new List<int> { facturaId }, bancoId);
                if (resultadoPropuesta == null || resultadoPropuesta.detalles_propuesta == null || resultadoPropuesta.detalles_propuesta.Length == 0)
                {
                    MostrarMensaje("No se pudo generar el detalle para la factura seleccionada.", "danger");
                    return;
                }

                var detalleGenerado = resultadoPropuesta.detalles_propuesta[0];

                // Guardar el detalle generado en ViewState para confirmación
                ViewState["DetalleGenerado"] = detalleGenerado;

                // Mostrar previsualización
                lblFacturaGenerada.Text = detalleGenerado.factura?.numero_factura ?? "-";
                lblAcreedorFactura.Text = detalleGenerado.factura?.acreedor?.razon_social ?? "-";
                lblMontoGenerado.Text = detalleGenerado.monto_pago.ToString("N2");
                lblMonedaGenerada.Text = detalleGenerado.factura?.moneda?.codigo_iso ?? "-";

                // Cargar cuentas propias por entidad bancaria
                var cuentas = cuentasPropiasBO.ListarPorEntidadBancaria(bancoId) ?? new BindingList<SoftPacBusiness.CuentasPropiasWS.cuentasPropiasDTO>();
                if(cuentas==null || cuentas.Count() == 0)
                {
                    rptCuentas.DataSource = null;
                    rptCuentas.DataBind();
                    MostrarMensaje("No existen cuentas disponibles para el pago de esta factura.", "warning");
                    return;
                }
                // Añadir moneda codigo para mostrar si existe
                var cuentasSource = DTOConverter.ConvertirLista<SoftPacBusiness.CuentasPropiasWS.cuentasPropiasDTO, SoftPacBusiness.PropuestaPagoWS.cuentasPropiasDTO>(cuentas)
                    .Where(c => c.moneda.moneda_id == detalleGenerado.factura.moneda.moneda_id)
                    .Select(c => new
                    {
                        c.cuenta_bancaria_id,
                        c.numero_cuenta,
                        c.tipo_cuenta,
                        entidad_bancaria = c.entidad_bancaria,
                        saldo_disponible = c.saldo_disponible,
                        moneda = c.moneda,
                        cci = c.cci
                    }).ToList();

                if (cuentasSource == null || cuentasSource.Count() == 0)
                {
                    rptCuentas.DataSource = null;
                    rptCuentas.DataBind();
                    MostrarMensaje("No existen cuentas disponibles para el pago de esta factura.", "warning");
                    return;
                }
                rptCuentas.DataSource = cuentasSource;
                rptCuentas.DataBind();

                pnlDetalleGenerado.Visible = true;
                MostrarMensaje("Factura seleccionada. Seleccione una cuenta propia y forma de pago, luego confirme.", "info");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al generar detalle: {ex.Message}", "danger");
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                var detalleGenerado = ViewState["DetalleGenerado"] as detallesPropuestaDTO;
                if (detalleGenerado == null)
                {
                    MostrarMensaje("No hay un detalle generado para confirmar.", "warning");
                    return;
                }

                // Obtener cuenta seleccionada del Repeater
                string sel = Request.Form["cuentaSeleccionada"];
                if (string.IsNullOrEmpty(sel) || !int.TryParse(sel, out int cuentaSeleccionadaId))
                {
                    MostrarMensaje("Seleccione una cuenta propia.", "warning");
                    return;
                }

                if (cuentaSeleccionadaId == 0)
                {
                    MostrarMensaje("Seleccione una cuenta propia.", "warning");
                    return;
                }

                // Obtener la cuenta completa
                var cuentaDTO = cuentasPropiasBO.ObtenerPorId(cuentaSeleccionadaId);
                if (cuentaDTO == null)
                {
                    MostrarMensaje("No se encontró la cuenta seleccionada.", "danger");
                    return;
                }

                // Asignar cuenta y forma de pago al detalle generado
                detalleGenerado.cuenta_propia = DTOConverter.Convertir<SoftPacBusiness.CuentasPropiasWS.cuentasPropiasDTO, SoftPacBusiness.PropuestaPagoWS.cuentasPropiasDTO>(cuentaDTO);
                if (!string.IsNullOrWhiteSpace(ddlFormaPago.SelectedValue))
                    detalleGenerado.forma_pago = ddlFormaPago.SelectedValue[0];

                // Preparar el detalle como nuevo (sin ID)
                detalleGenerado.detalle_propuesta_id = 0;
                detalleGenerado.usuario_eliminacion = null;

                // ⭐ CAMBIO: Guardar en lista de detalles nuevos en Session
                var detallesNuevos = DetallesNuevos;
                detallesNuevos.Add(detalleGenerado);
                DetallesNuevos = detallesNuevos;

                // Redirigir de vuelta a EditarPropuesta
                Response.Redirect($"~/EditarPropuesta.aspx?id={PropuestaId}");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al confirmar el detalle: {ex.Message}", "danger");
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/EditarPropuesta.aspx?id={PropuestaId}");
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }
    }
}