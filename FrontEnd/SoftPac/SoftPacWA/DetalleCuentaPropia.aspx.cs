using SoftPac.Business;
using SoftPacBusiness;
using SoftPacBusiness.CuentasPropiasWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class DetalleCuentaPropia : System.Web.UI.Page
    {
        private CuentasPropiasBO cuentasBO = new CuentasPropiasBO();
        private int CuentaId
        {
            get
            {
                return ViewState["CuentaId"] != null ? (int)ViewState["CuentaId"] : 0;
            }
            set
            {
                ViewState["CuentaId"] = value;
            }
        }

        private cuentasPropiasDTO CuentaActual
        {
            get
            {
                if (ViewState["CuentaActual"] == null && CuentaId > 0)
                {
                    ViewState["CuentaActual"] = cuentasBO.ObtenerPorId(CuentaId);
                }
                return (cuentasPropiasDTO)ViewState["CuentaActual"];
            }
            set
            {
                ViewState["CuentaActual"] = value;
            }
        }

        private SoftPacBusiness.UsuariosWS.usuariosDTO UsuarioLogueado
        {
            get
            {
                return (SoftPacBusiness.UsuariosWS.usuariosDTO)Session["UsuarioLogueado"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int cuentaId))
                {
                    CuentaId = cuentaId;
                    hfCuentaId.Value = cuentaId.ToString();
                    CargarDatosCuenta();
                    CargarKardex();
                }
                else
                {
                    Response.Redirect("CuentasPropias.aspx");
                }
            }
        }

        private void CargarDatosCuenta()
        {
            try
            {
                cuentasPropiasDTO cuenta = CuentaActual;

                if (cuenta != null)
                {
                    // Datos básicos de la cuenta
                    lblEntidad.Text = cuenta.entidad_bancaria?.nombre ?? "N/A";
                    lblNumeroCuenta.Text = cuenta.numero_cuenta ?? "N/A";
                    lblTipoCuenta.Text = cuenta.tipo_cuenta ?? "N/A";
                    lblCCI.Text = cuenta.cci ?? "N/A";
                    lblMoneda.Text = $"{cuenta.moneda?.nombre} ({cuenta.moneda?.codigo_iso})";

                    // Estado
                    string estadoTexto = cuenta.activa == true ? "Activa" : "Inactiva";
                    string estadoClass = cuenta.activa == true ? "badge-activo" : "badge-inactivo";
                    litEstado.Text = $"<span class='badge-estado {estadoClass}'>{estadoTexto}</span>";
                }
                else
                {
                    MostrarMensaje("No se encontró la cuenta.", "danger");
                    Response.Redirect("CuentasPropias.aspx");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar los datos: {ex.Message}", "danger");
            }
        }

        private void CargarKardex()
        {
            try
            {
                var kardexList = cuentasBO.ObtenerKardexCuentaPropiaPorCuenta(CuentaId);

                if (kardexList != null && kardexList.Count > 0)
                {
                    // Ordenar por fecha descendente (más recientes primero)
                    var listaOrdenada = kardexList.OrderByDescending(k => k.fecha_modificacion).ToList();

                    gvKardex.DataSource = listaOrdenada;
                    gvKardex.DataBind();
                    gvKardex.Visible = true;
                    pnlSinDatos.Visible = false;

                    lblTotalMovimientos.Text = $"Total de movimientos: {kardexList.Count}";

                    // Calcular resumen
                    CalcularResumenMovimientos(listaOrdenada);
                }
                else
                {
                    gvKardex.DataSource = null;
                    gvKardex.DataBind();
                    gvKardex.Visible = false;
                    pnlSinDatos.Visible = true;

                    lblTotalMovimientos.Text = "Total de movimientos: 0";
                    LimpiarResumenMovimientos();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar el historial: {ex.Message}", "danger");
            }
        }

        protected string FormatearMonto(decimal monto)
        {
            string codigoISO = CuentaActual?.moneda?.codigo_iso ?? "";
            return $"{codigoISO} {monto:N2}";
        }

        private void CalcularResumenMovimientos(List<kardexCuentasPropiasDTO> kardexList)
        {
            try
            {
                if (kardexList.Count == 0)
                {
                    LimpiarResumenMovimientos();
                    return;
                }
                string codigoISO = CuentaActual?.moneda?.codigo_iso ?? "";

                decimal totalIngresos = kardexList.Where(k => k.saldo_modificacion > 0).Sum(k => k.saldo_modificacion);
                decimal totalEgresos = Math.Abs(kardexList.Where(k => k.saldo_modificacion < 0).Sum(k => k.saldo_modificacion));
                decimal diferencia = totalIngresos - totalEgresos;
                decimal saldoTotal = CuentaActual?.saldo_disponible ?? 0;

                lblTotalIngresos.Text = $"{codigoISO} {totalIngresos:N2}";
                lblTotalEgresos.Text = $"{codigoISO} {totalEgresos:N2}";
                lblDiferencia.Text = $"{codigoISO} {diferencia:N2}";
                lblSaldoTotal.Text = $"{codigoISO} {saldoTotal:N2}";
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al calcular resumen: {ex.Message}", "danger");
            }
        }

        private void LimpiarResumenMovimientos()
        {
            string codigoISO = CuentaActual?.moneda?.codigo_iso ?? "";
            decimal saldoTotal = CuentaActual?.saldo_disponible ?? 0;

            lblTotalIngresos.Text = $"{codigoISO} 0.00";
            lblTotalEgresos.Text = $"{codigoISO} 0.00";
            lblDiferencia.Text = $"{codigoISO} 0.00";
            lblSaldoTotal.Text = $"{codigoISO} {saldoTotal:N2}";
        }

        protected void gvKardex_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                kardexCuentasPropiasDTO kardex = (kardexCuentasPropiasDTO)e.Row.DataItem;

                if (kardex != null)
                {
                    Label lblMontoModificado = (Label)e.Row.FindControl("lblMontoModificado");

                    if (lblMontoModificado != null)
                    {
                        decimal monto = kardex.saldo_modificacion;
                        string cssClass = monto < 0 ? "monto-negativo" : "monto-positivo";
                        lblMontoModificado.Text = FormatearMonto(monto);
                        lblMontoModificado.CssClass = cssClass;
                    }
                }
            }
        }

        protected void gvKardex_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvKardex.PageIndex = e.NewPageIndex;
            CargarKardex();
        }

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = UsuarioLogueado;

                if (usuario == null)
                {
                    MostrarMensaje("Sesión expirada. Por favor, inicie sesión nuevamente.", "danger");
                    return;
                }

                cuentasPropiasDTO cuentaEliminada = new cuentasPropiasDTO();
                cuentaEliminada.cuenta_bancaria_id = CuentaId;

                int resultado = cuentasBO.Eliminar(cuentaEliminada,
                    DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO, SoftPacBusiness.CuentasPropiasWS.usuariosDTO>(usuario));

                if (resultado > 0)
                {
                    Session["MensajeExito"] = "Cuenta eliminada correctamente.";
                    Response.Redirect("CuentasPropias.aspx");
                }
                else
                {
                    MostrarMensaje("Error al eliminar la cuenta.", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al eliminar la cuenta: {ex.Message}", "danger");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string mensajeEscapado = mensaje.Replace("'", "\\'").Replace("\"", "\\\"");
            string script = $@"
                $(document).ready(function() {{
                    var alertHtml = '<div class=""alert alert-{tipo} alert-dismissible fade show"" role=""alert"">' +
                                    '{mensajeEscapado}' +
                                    '<button type=""button"" class=""btn-close"" data-bs-dismiss=""alert""></button>' +
                                    '</div>';
                    $('.content-area').prepend(alertHtml);
                    setTimeout(function() {{
                        $('.alert').fadeOut();
                    }}, 5000);
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje", script, true);
        }
    }
}