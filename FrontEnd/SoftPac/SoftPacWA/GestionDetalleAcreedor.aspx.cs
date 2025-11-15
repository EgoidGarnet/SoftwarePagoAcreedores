using System;
using System.Web.UI;
using System.Web.UI.WebControls;
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

        // NUEVOS BO PARA CATÁLOGOS
        private readonly EntidadesBancariasBO entidadesBancariasBO = new EntidadesBancariasBO();
        private readonly MonedasBO monedasBO = new MonedasBO();

        private usuariosDTO UsuarioLogueado { get { return (usuariosDTO)Session["UsuarioLogueado"]; } }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.QueryString["popup"] == "1")
            {
                this.MasterPageFile = "~/Emergente.Master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");
            if (!IsPostBack)
            {
                int acreedorId;
                if (!int.TryParse(Request.QueryString["acreedorId"], out acreedorId)) { Response.Redirect("Acreedores.aspx"); return; }
                hfAcreedorId.Value = acreedorId.ToString();
                //lnkVolver.NavigateUrl = "DetalleAcreedor.aspx?id=" + hfAcreedorId.Value;
                //btnCancelar.NavigateUrl = lnkVolver.NavigateUrl;

                // CARGAR COMBOS SIEMPRE ANTES DE CARGAR CUENTA
                CargarCatalogos();

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

        private void CargarCatalogos()
        {
            // ENTIDADES BANCARIAS
            txtEntidad.Items.Clear();
            txtEntidad.DataSource = entidadesBancariasBO.ListarTodos();
            txtEntidad.DataTextField = "nombre";
            txtEntidad.DataValueField = "nombre"; // Mantenemos la lógica: guardamos por nombre
            txtEntidad.DataBind();
            txtEntidad.Items.Insert(0, new ListItem("--Seleccione--", ""));

            // MONEDAS
            txtDivisa.Items.Clear();
            txtDivisa.DataSource = monedasBO.ListarTodos(); // según tu requerimiento
            txtDivisa.DataTextField = "codigo_iso";
            txtDivisa.DataValueField = "codigo_iso"; // seguimos guardando por código ISO
            txtDivisa.DataBind();
            txtDivisa.Items.Insert(0, new ListItem("--Seleccione--", ""));
        }

        private void CargarCuenta(int cuentaId)
        {
            cuentasAcreedorDTO c = cuentasBO.ObtenerPorId(cuentaId);
            //if (c == null) { Response.Redirect(lnkVolver.NavigateUrl); return; }

            // ENTIDAD: seleccionamos por nombre
            if (c.entidad_bancaria != null && !string.IsNullOrWhiteSpace(c.entidad_bancaria.nombre))
            {
                var item = txtEntidad.Items.FindByValue(c.entidad_bancaria.nombre);
                if (item != null) txtEntidad.SelectedValue = c.entidad_bancaria.nombre;
            }

            txtNumero.Text = c.numero_cuenta ?? string.Empty;
            txtCCI.Text = c.cci ?? string.Empty;
            txtTipo.Text = c.tipo_cuenta ?? string.Empty;

            // DIVISA: seleccionamos por código ISO
            if (c.moneda != null && !string.IsNullOrWhiteSpace(c.moneda.codigo_iso))
            {
                var item = txtDivisa.Items.FindByValue(c.moneda.codigo_iso);
                if (item != null) txtDivisa.SelectedValue = c.moneda.codigo_iso;
            }
        }

        // ---------- VALIDACIÓN ----------
        private bool ValidarCampos()
        {
            bool esValido = true;

            // Limpiar mensajes
            lblEntidadError.Text = string.Empty;
            lblNumeroError.Text = string.Empty;
            lblCCIError.Text = string.Empty;
            lblTipoError.Text = string.Empty;
            lblDivisaError.Text = string.Empty;

            string entidad = txtEntidad.SelectedValue; // ahora combo
            string numero = (txtNumero.Text ?? string.Empty).Trim();
            string cci = (txtCCI.Text ?? string.Empty).Trim();
            string tipo = (txtTipo.Text ?? string.Empty).Trim();
            string divisa = txtDivisa.SelectedValue; // ahora combo

            if (string.IsNullOrEmpty(entidad))
            {
                lblEntidadError.Text = "Seleccione una entidad bancaria.";
                esValido = false;
            }

            if (string.IsNullOrEmpty(numero))
            {
                lblNumeroError.Text = "Ingrese el número de cuenta.";
                esValido = false;
            }

            if (string.IsNullOrEmpty(cci))
            {
                lblCCIError.Text = "Ingrese el CCI / IBAN.";
                esValido = false;
            }

            if (string.IsNullOrEmpty(tipo))
            {
                lblTipoError.Text = "Ingrese el tipo de cuenta.";
                esValido = false;
            }

            if (string.IsNullOrEmpty(divisa))
            {
                lblDivisaError.Text = "Seleccione una divisa.";
                esValido = false;
            }

            return esValido;
        }
        // -------------------------------

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            int acreedorId = int.Parse(hfAcreedorId.Value);

            cuentasAcreedorDTO dto = new cuentasAcreedorDTO();

            if (!string.IsNullOrEmpty(hfCuentaId.Value))
            {
                dto.cuenta_bancaria_id = int.Parse(hfCuentaId.Value);
                dto.cuenta_bancaria_idSpecified = true;
            }
            else
            {
                dto.cuenta_bancaria_idSpecified = false;
            }

            dto.acreedor = new SoftPacBusiness.CuentasAcreedorWS.acreedoresDTO
            {
                acreedor_id = acreedorId,
                acreedor_idSpecified = true
            };
            var entidadOrigen = entidadesBancariasBO.ObtenerPorNombre((txtEntidad.Text ?? "").Trim());
            dto.entidad_bancaria = new SoftPacBusiness.CuentasAcreedorWS.entidadesBancariasDTO
            {
                entidad_bancaria_id = entidadOrigen.entidad_bancaria_id,
                entidad_bancaria_idSpecified = true,
                nombre = entidadOrigen.nombre
            };
            var monedaOrigen = monedasBO.ObtenerPorDivisa((txtDivisa.Text ?? "").Trim());
            dto.moneda = new SoftPacBusiness.CuentasAcreedorWS.monedasDTO
            {
                moneda_id = monedaOrigen.moneda_id,
                moneda_idSpecified = true,
                codigo_iso = monedaOrigen.codigo_iso
            };

            dto.numero_cuenta = (txtNumero.Text ?? "").Trim();
            dto.cci = (txtCCI.Text ?? "").Trim();
            dto.tipo_cuenta = (txtTipo.Text ?? "").Trim();

            dto.activa = true;
            dto.activaSpecified = true;
            dto.fecha_eliminacionSpecified = false;
            dto.usuario_eliminacion = null;

            int ok;
            if (dto.cuenta_bancaria_idSpecified)
                ok = cuentasBO.modificar(dto);
            else
                ok = cuentasBO.insertar(dto);

            string url = "DetalleAcreedor.aspx?id=" + acreedorId;
            string script = $@"
                        if (window.top !== window.self) {{
                            window.top.location.href = '{url}';
                        }} else {{
                            window.location.href = '{url}';
                        }}";
            ScriptManager.RegisterStartupScript(this, GetType(), "redirDetalleAcreedor", script, true);
        }

    }
}