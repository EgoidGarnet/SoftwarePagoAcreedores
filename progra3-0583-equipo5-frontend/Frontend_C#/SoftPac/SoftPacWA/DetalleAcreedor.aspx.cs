using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPac.Business;
using SoftPac.Model;

namespace SoftPacWA
{
    public partial class DetalleAcreedor : Page
    {
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();
        private readonly CuentasAcreedorBO cuentasBO = new CuentasAcreedorBO();

        private UsuariosDTO UsuarioLogueado
        {
            get { return (UsuariosDTO)Session["UsuarioLogueado"]; }
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

            AcreedoresDTO a = acreedoresBO.obtenerPorId(id);
            if (a == null)
            {
                Response.Redirect("Acreedores.aspx");
                return;
            }

            lblRazon.Text = a.RazonSocial ?? string.Empty;
            lblRuc.Text = a.Ruc ?? string.Empty;
            lblPais.Text = a.Pais != null ? a.Pais.Nombre : string.Empty;
            lblDir.Text = a.DireccionFiscal ?? string.Empty;
            lblPlazo.Text = (a.PlazoDePago ?? 0).ToString();
            litEstado.Text = a.Activo
                ? "<span class='badge-estado badge-activo'>Activo</span>"
                : "<span class='badge-estado badge-inactivo'>Inactivo</span>";

            CargarCuentas();
        }

        private void CargarCuentas()
        {
            int acreedorId = int.Parse(hfAcreedorId.Value);

            IList<CuentasAcreedorDTO> cuentas = cuentasBO.ObtenerPorAcreedor(acreedorId);

            gvCuentas.DataSource = (cuentas ?? new List<CuentasAcreedorDTO>()).Select(c => new
            {
                CuentaBancariaId = c.CuentaBancariaId,
                AcreedorId = acreedorId,
                Entidad = c.EntidadBancaria != null ? c.EntidadBancaria.Nombre : string.Empty,
                NumeroCuenta = c.NumeroCuenta,
                CCI = c.Cci,
                TipoCuenta = c.TipoCuenta,
                Divisa = c.Moneda != null ? c.Moneda.CodigoIso : string.Empty,
                Estado = c.Activa ? "Activa" : "Inactiva"
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

            int result = cuentasBO.eliminarLogico(cuentaId, UsuarioLogueado);
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
