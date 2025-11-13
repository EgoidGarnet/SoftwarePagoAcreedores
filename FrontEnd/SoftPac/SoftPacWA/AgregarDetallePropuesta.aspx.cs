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
                var items = facturas.Select(f => new
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
                string sel = Request.Form["cuentaSeleccionada"]; // devuelve el value del radio (id de cuenta)
                if (string.IsNullOrEmpty(sel) || !int.TryParse(sel, out int cuentaSeleccionadaId))
                {
                    MostrarMensaje("Seleccione una cuenta propia.", "warning");
                    return;
                }

                if (cuentaSeleccionadaId==0)
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

                // Guardar en la propuesta que tenemos en Session (o recargar si no existe)
                var propuesta = Session["PropuestaToAdd"] as propuestasPagoDTO ?? propuestasBO.ObtenerPorId(PropuestaId);
                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta para agregar el detalle.", "danger");
                    return;
                }

                detalleGenerado.detalle_propuesta_id = 0;
                detalleGenerado.detalle_propuesta_idSpecified = false;
                detalleGenerado.usuario_eliminacion = null;
                
                // Agregar el detalle al array de detalles de la propuesta
                var lista = new List<detallesPropuestaDTO>();
                lista.Add(detalleGenerado);
                propuesta.detalles_propuesta = lista.ToArray();
                
                // Auditoría básica
                propuesta.usuario_modificacion = DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO, SoftPacBusiness.PropuestaPagoWS.usuariosDTO>((SoftPacBusiness.UsuariosWS.usuariosDTO)Session["UsuarioLogueado"]);
                propuesta.fecha_hora_modificacion = DateTime.Now;

                // Guardar en persistencia
                bool ok = propuestasBO.Modificar(propuesta) != 0;
                if (ok)
                {
                    // Redirigir a EditarPropuesta con el id
                    Response.Redirect($"~/EditarPropuesta.aspx?id={PropuestaId}");
                    MostrarMensaje("El detalle se agregó correctamente", "success");
                }
                else
                {
                    MostrarMensaje("Error al guardar el detalle en la propuesta.", "danger");
                }
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