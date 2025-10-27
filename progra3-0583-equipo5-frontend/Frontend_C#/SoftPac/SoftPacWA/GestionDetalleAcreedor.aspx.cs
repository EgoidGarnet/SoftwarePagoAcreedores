using System;
using System.Web.UI;
using SoftPac.Business;
using SoftPac.Model;

namespace SoftPacWA
{
    public partial class GestionDetalleAcreedor : Page
    {
        private readonly CuentasAcreedorBO cuentasBO = new CuentasAcreedorBO();
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();
        private UsuariosDTO UsuarioLogueado { get { return (UsuariosDTO)Session["UsuarioLogueado"]; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");
            if (!IsPostBack)
            {
                int acreedorId;
                if (!int.TryParse(Request.QueryString["acreedorId"], out acreedorId)) { Response.Redirect("Acreedores.aspx"); return; }
                hfAcreedorId.Value = acreedorId.ToString();
                lnkVolver.NavigateUrl = "DetalleAcreedor.aspx?id=" + hfAcreedorId.Value;
                btnCancelar.NavigateUrl = lnkVolver.NavigateUrl;

                int cuentaId;
                if (int.TryParse(Request.QueryString["cuentaId"], out cuentaId))
                {
                    hfCuentaId.Value = cuentaId.ToString();
                    lblTitulo.Text = "Editar cuenta";
                    CargarCuenta(cuentaId);
                }
                else
                {
                    lblTitulo.Text = "Nueva cuenta";
                }
            }
        }

        private void CargarCuenta(int cuentaId)
        {
            CuentasAcreedorDTO c = cuentasBO.ObtenerPorId(cuentaId);
            if (c == null) { Response.Redirect(lnkVolver.NavigateUrl); return; }

            txtEntidad.Text = c.EntidadBancaria != null ? c.EntidadBancaria.Nombre : string.Empty;
            txtNumero.Text = c.NumeroCuenta ?? string.Empty;
            txtCCI.Text = c.Cci ?? string.Empty;
            txtTipo.Text = c.TipoCuenta ?? string.Empty;
            txtDivisa.Text = c.Moneda != null ? c.Moneda.CodigoIso : string.Empty;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int acreedorId = int.Parse(hfAcreedorId.Value);

            CuentasAcreedorDTO dto = new CuentasAcreedorDTO();
            if (!string.IsNullOrEmpty(hfCuentaId.Value)) dto.CuentaBancariaId = int.Parse(hfCuentaId.Value);
            dto.Acreedor = new AcreedoresDTO { AcreedorId = acreedorId };
            dto.EntidadBancaria = new EntidadesBancariasDTO { Nombre = (txtEntidad.Text ?? "").Trim() };
            dto.NumeroCuenta = (txtNumero.Text ?? "").Trim();
            dto.Cci = (txtCCI.Text ?? "").Trim();
            dto.TipoCuenta = (txtTipo.Text ?? "").Trim();
            dto.Moneda = new MonedasDTO { CodigoIso = (txtDivisa.Text ?? "").Trim() };
            dto.Activa = true;

            int ok;
            if (dto.CuentaBancariaId.HasValue) ok = cuentasBO.modificar(dto);
            else ok = cuentasBO.insertar(dto);

            Response.Redirect("DetalleAcreedor.aspx?id=" + acreedorId);
        }
    }
}
