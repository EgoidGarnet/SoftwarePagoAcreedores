using System;
using System.Web.UI;
using SoftPac.Business;
using SoftPacBusiness.AcreedoresWS;
using SoftPacBusiness.CuentasAcreedorWS;
using SoftPacBusiness.EntidadesBancariasWS;
using SoftPacBusiness.UsuariosWS;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;

namespace SoftPacWA
{
    public partial class GestionDetalleAcreedor : Page
    {
        private readonly CuentasAcreedorBO cuentasBO = new CuentasAcreedorBO();
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();
        private usuariosDTO UsuarioLogueado { get { return (usuariosDTO)Session["UsuarioLogueado"]; } }

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
            cuentasAcreedorDTO c = cuentasBO.ObtenerPorId(cuentaId);
            if (c == null) { Response.Redirect(lnkVolver.NavigateUrl); return; }

            txtEntidad.Text = c.entidad_bancaria != null ? c.entidad_bancaria.nombre : string.Empty;
            txtNumero.Text = c.numero_cuenta ?? string.Empty;
            txtCCI.Text = c.cci ?? string.Empty;
            txtTipo.Text = c.tipo_cuenta ?? string.Empty;
            txtDivisa.Text = c.moneda != null ? c.moneda.codigo_iso : string.Empty;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int acreedorId = int.Parse(hfAcreedorId.Value);

            cuentasAcreedorDTO dto = new cuentasAcreedorDTO();
            if (!string.IsNullOrEmpty(hfCuentaId.Value)) dto.cuenta_bancaria_id = int.Parse(hfCuentaId.Value);
            dto.acreedor = new SoftPacBusiness.CuentasAcreedorWS.acreedoresDTO { acreedor_id = acreedorId };
            dto.entidad_bancaria = new SoftPacBusiness.CuentasAcreedorWS.entidadesBancariasDTO { nombre = (txtEntidad.Text ?? "").Trim() };
            dto.numero_cuenta = (txtNumero.Text ?? "").Trim();
            dto.cci = (txtCCI.Text ?? "").Trim();
            dto.tipo_cuenta = (txtTipo.Text ?? "").Trim();
            dto.moneda = new monedasDTO { codigo_iso = (txtDivisa.Text ?? "").Trim() };
            dto.activa = true;

            int ok;
            if (dto.cuenta_bancaria_idSpecified) ok = cuentasBO.modificar(dto);
            else ok = cuentasBO.insertar(dto);

            Response.Redirect("DetalleAcreedor.aspx?id=" + acreedorId);
        }
    }
}
