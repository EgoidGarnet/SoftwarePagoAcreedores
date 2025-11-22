using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPac.Business;
using SoftPacBusiness.AcreedoresWS;
using SoftPacBusiness.CuentasAcreedorWS;
using SoftPacBusiness.UsuariosWS;
using acreedoresDTO = SoftPacBusiness.AcreedoresWS.acreedoresDTO;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;

namespace SoftPacWA
{
    public partial class DetalleAcreedor : Page
    {
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();
        private readonly CuentasAcreedorBO cuentasBO = new CuentasAcreedorBO();

        private usuariosDTO UsuarioLogueado
        {
            get { return (usuariosDTO)Session["UsuarioLogueado"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");
            if (IsPostBack) return;

            int id;
            if (!int.TryParse(Request.QueryString["id"], out id))
            {
                Response.Redirect("Acreedores.aspx");
                return;
            }

            hfAcreedorId.Value = id.ToString();
            lnkNuevaCuenta.NavigateUrl = "GestionDetalleAcreedor.aspx?acreedorId=" + id + "&popup=1";

            acreedoresDTO a = acreedoresBO.obtenerPorId(id);
            if (a == null)
            {
                Response.Redirect("Acreedores.aspx");
                return;
            }

            lblRazon.Text = a.razon_social ?? string.Empty;
            lblRuc.Text = a.ruc ?? string.Empty;
            lblPais.Text = a.pais != null ? a.pais.nombre : string.Empty;
            lblDir.Text = a.direccion_fiscal ?? string.Empty;
            lblPlazo.Text = (a.plazo_de_pagoSpecified ? a.plazo_de_pago : 0).ToString();
            litEstado.Text = a.activo
                ? "<span class='badge-estado badge-activo'>Activo</span>"
                : "<span class='badge-estado badge-inactivo'>Inactivo</span>";

            CargarCuentas();
        }

        private void CargarCuentas()
        {
            int acreedorId = int.Parse(hfAcreedorId.Value);

            IList<cuentasAcreedorDTO> cuentas = cuentasBO.ObtenerPorAcreedor(acreedorId);

            gvCuentas.DataSource = (cuentas ?? new List<cuentasAcreedorDTO>()).Select(c => new
            {
                CuentaBancariaId = c.cuenta_bancaria_id,
                AcreedorId = acreedorId,
                Entidad = c.entidad_bancaria != null ? c.entidad_bancaria.nombre : string.Empty,
                NumeroCuenta = c.numero_cuenta,
                CCI = c.cci,
                TipoCuenta = c.tipo_cuenta,
                Divisa = c.moneda != null ? c.moneda.codigo_iso : string.Empty,
                Estado = c.activa ? "Activa" : "Inactiva"
            }).ToList();

            gvCuentas.DataBind();

            int total = cuentas != null ? cuentas.Count : 0;
            lblTotalCuentas.Text = "Total de cuentas: " + total;
        }

        protected void gvCuentas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Toggle") return;
            if (string.IsNullOrEmpty(e.CommandArgument as string)) return;

            int cuentaId;
            if (!int.TryParse((string)e.CommandArgument, out cuentaId)) return;

            SoftPacBusiness.CuentasAcreedorWS.usuariosDTO usuarioLog = new SoftPacBusiness.CuentasAcreedorWS.usuariosDTO();
            usuarioLog.usuario_id = UsuarioLogueado.usuario_id;
            int result = cuentasBO.eliminarLogico(cuentaId, usuarioLog);
            if (result == 1) CargarCuentas();
        }

        protected void gvCuentas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            string estado = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Estado"));
            LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggleCuenta");

            if (btnToggle != null)
            {
                if (string.Equals(estado, "Activa", StringComparison.OrdinalIgnoreCase))
                {
                    btnToggle.CssClass = "btn btn-sm btn-danger btn-icon";
                    btnToggle.ToolTip = "Inactivar";
                    btnToggle.Text = "<i class='fas fa-trash'></i>";
                }
                else
                {
                    btnToggle.CssClass = "btn btn-sm btn-success btn-icon";
                    btnToggle.ToolTip = "Activar";
                    btnToggle.Text = "<i class='fas fa-rotate-left'></i>";
                }
            }
        }

        protected string GetCuentaEstadoClass(object estadoObj)
        {
            string e = Convert.ToString(estadoObj);
            if (string.Equals(e, "Activa", StringComparison.OrdinalIgnoreCase)) return "badge-activo";
            if (string.Equals(e, "Inactiva", StringComparison.OrdinalIgnoreCase)) return "badge-inactivo";
            return "badge-inactivo";
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            int acreedorId = int.Parse(hfAcreedorId.Value);
            Response.Redirect("GestionAcreedor.aspx?accion=editar&id=" + acreedorId);
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            int acreedorId = int.Parse(hfAcreedorId.Value);

            SoftPacBusiness.AcreedoresWS.usuariosDTO usuarioLog = new SoftPacBusiness.AcreedoresWS.usuariosDTO();
            usuarioLog.usuario_id = UsuarioLogueado.usuario_id;

            int result = acreedoresBO.Eliminar(acreedorId, usuarioLog);

            if (result == 1)
            {
                Session["MensajeExito"] = "El acreedor se eliminó correctamente";
                Response.Redirect("Acreedores.aspx");
            }
            else
            {
                MostrarMensaje("No se pudo eliminar al acreedor", "danger");
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
                    setTimeout(function() {{ $('.alert').fadeOut(); }}, 5000);
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje", script, true);
        }


        private void ConfigurarBotonesSegunEstado(bool acreedorActivo)
        {
            if (!acreedorActivo)
            {
                // Deshabilitar botón de editar acreedor
                btnEditar.Enabled = false;
                btnEditar.CssClass = "btn btn-warning btn-sm disabled";
                btnEditar.ToolTip = "No se puede editar un acreedor inactivo";

                // Deshabilitar botón de nueva cuenta
                lnkNuevaCuenta.Enabled = false;
                lnkNuevaCuenta.CssClass = "btn btn-primary btn-sm disabled";
                lnkNuevaCuenta.ToolTip = "No se puede agregar cuentas a un acreedor inactivo";

                // Remover atributo data-modal-cuenta para que no se abra
                lnkNuevaCuenta.Attributes.Remove("data-modal-cuenta");

                // Solo dejar habilitado el botón de eliminar
                btnEliminar.Enabled = true;
            }
            else
            {
                // Habilitar todos los botones
                btnEditar.Enabled = true;
                btnEditar.CssClass = "btn btn-warning btn-sm";

                lnkNuevaCuenta.Enabled = true;
                lnkNuevaCuenta.CssClass = "btn btn-primary btn-sm";
                lnkNuevaCuenta.Attributes["data-modal-cuenta"] = "true";

                btnEliminar.Enabled = true;
            }
        }
        protected string ObtenerInicialTipoCuenta(object tipoCuenta)
        {
            if (tipoCuenta == null) return "";

            string tipo = tipoCuenta.ToString().ToUpper();

            if (tipo.Contains("AHORRO")) return "AHO";
            if (tipo.Contains("CORRIENTE")) return "CTE";
            if (tipo.Contains("VISTA")) return "VTA";
            if (tipo.Contains("PLAZO")) return "PLZ";

            // Si no coincide, tomar las primeras 3 letras
            return tipo.Length >= 3 ? tipo.Substring(0, 3) : tipo;
        }
    }
}