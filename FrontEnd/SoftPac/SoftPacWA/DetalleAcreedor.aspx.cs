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
            lnkNuevaCuenta.NavigateUrl = "GestionDetalleAcreedor.aspx?acreedorId=" + id;

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
            lblPlazo.Text = (a.plazo_de_pagoSpecified ? a.plazo_de_pago:0).ToString();
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
    }
}
