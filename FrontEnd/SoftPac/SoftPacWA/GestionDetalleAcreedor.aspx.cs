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
        private readonly EntidadesBancariasBO entidadesBancariasBO = new EntidadesBancariasBO();
        private readonly MonedasBO monedasBO = new MonedasBO();

        private usuariosDTO UsuarioLogueado
        {
            get { return (usuariosDTO)Session["UsuarioLogueado"]; }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.QueryString["popup"] == "1")
            {
                this.MasterPageFile = "~/Emergente.Master";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null)
                Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                int acreedorId;
                if (!int.TryParse(Request.QueryString["acreedorId"], out acreedorId))
                {
                    Response.Redirect("Acreedores.aspx");
                    return;
                }

                hfAcreedorId.Value = acreedorId.ToString();

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
            ddlEntidad.Items.Clear();
            ddlEntidad.DataSource = entidadesBancariasBO.ListarTodos();
            ddlEntidad.DataTextField = "nombre";
            ddlEntidad.DataValueField = "nombre";
            ddlEntidad.DataBind();
            ddlEntidad.Items.Insert(0, new ListItem("--Seleccione--", ""));

            // MONEDAS
            ddlDivisa.Items.Clear();
            ddlDivisa.DataSource = monedasBO.ListarTodos();
            ddlDivisa.DataTextField = "codigo_iso";
            ddlDivisa.DataValueField = "codigo_iso";
            ddlDivisa.DataBind();
            ddlDivisa.Items.Insert(0, new ListItem("--Seleccione--", ""));
        }

        private void CargarCuenta(int cuentaId)
        {
            cuentasAcreedorDTO c = cuentasBO.ObtenerPorId(cuentaId);
            if (c == null) return;

            // ENTIDAD: seleccionamos por nombre
            if (c.entidad_bancaria != null && !string.IsNullOrWhiteSpace(c.entidad_bancaria.nombre))
            {
                var item = ddlEntidad.Items.FindByValue(c.entidad_bancaria.nombre);
                if (item != null) ddlEntidad.SelectedValue = c.entidad_bancaria.nombre;
            }

            txtNumero.Text = c.numero_cuenta ?? string.Empty;
            txtCCI.Text = c.cci ?? string.Empty;

            // TIPO: seleccionamos el tipo
            if (!string.IsNullOrWhiteSpace(c.tipo_cuenta))
            {
                var item = ddlTipo.Items.FindByValue(c.tipo_cuenta);
                if (item != null)
                    ddlTipo.SelectedValue = c.tipo_cuenta;
            }

            // DIVISA: seleccionamos por código ISO
            if (c.moneda != null && !string.IsNullOrWhiteSpace(c.moneda.codigo_iso))
            {
                var item = ddlDivisa.Items.FindByValue(c.moneda.codigo_iso);
                if (item != null) ddlDivisa.SelectedValue = c.moneda.codigo_iso;
            }
        }

        // ==================== VALIDACIÓN ====================
        private bool ValidarCampos()
        {
            bool esValido = true;

            // LIMPIAR TODOS LOS MENSAJES DE ERROR Y CLASES
            LimpiarErrores();

            string entidad = ddlEntidad.SelectedValue;
            string numero = (txtNumero.Text ?? string.Empty).Trim();
            string cci = (txtCCI.Text ?? string.Empty).Trim();
            string tipo = ddlTipo.SelectedValue;
            string divisa = ddlDivisa.SelectedValue;

            // VALIDAR ENTIDAD BANCARIA
            if (string.IsNullOrEmpty(entidad))
            {
                lblEntidadError.Text = "Debe seleccionar una entidad bancaria.";
                ddlEntidad.CssClass = "form-select is-invalid";
                esValido = false;
            }

            // VALIDAR NÚMERO DE CUENTA (longitud entre 10 y 20 dígitos)
            if (string.IsNullOrEmpty(numero))
            {
                lblNumeroError.Text = "El número de cuenta es obligatorio.";
                txtNumero.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (numero.Length < 10)
            {
                lblNumeroError.Text = "El número de cuenta debe tener al menos 10 caracteres.";
                txtNumero.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (numero.Length > 20)
            {
                lblNumeroError.Text = "El número de cuenta no puede exceder 20 caracteres.";
                txtNumero.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(numero, @"^\d+$"))
            {
                lblNumeroError.Text = "El número de cuenta debe contener solo dígitos.";
                txtNumero.CssClass = "form-control is-invalid";
                esValido = false;
            }

            if (string.IsNullOrEmpty(cci))
            {
                lblCCIError.Text = "El CCI/IBAN es obligatorio.";
                txtCCI.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (cci.Length < 20)
            {
                lblCCIError.Text = "El CCI/IBAN debe tener al menos 20 caracteres.";
                txtCCI.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (cci.Length > 28)
            {
                lblCCIError.Text = "El CCI/IBAN no puede exceder 28 caracteres.";
                txtCCI.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(cci, @"^[a-zA-Z0-9]+$"))
            {
                lblCCIError.Text = "El CCI/IBAN debe contener solo letras y números.";
                txtCCI.CssClass = "form-control is-invalid";
                esValido = false;
            }

            // VALIDAR TIPO DE CUENTA
            if (string.IsNullOrEmpty(tipo))
            {
                lblTipoError.Text = "Debe seleccionar un tipo de cuenta.";
                ddlTipo.CssClass = "form-select is-invalid";
                esValido = false;
            }

            // VALIDAR DIVISA
            if (string.IsNullOrEmpty(divisa))
            {
                lblDivisaError.Text = "Debe seleccionar una divisa.";
                ddlDivisa.CssClass = "form-select is-invalid";
                esValido = false;
            }

            return esValido;
        }

        private void LimpiarErrores()
        {
            // Limpiar mensajes de error
            lblEntidadError.Text = string.Empty;
            lblNumeroError.Text = string.Empty;
            lblCCIError.Text = string.Empty;
            lblTipoError.Text = string.Empty;
            lblDivisaError.Text = string.Empty;

            // Restaurar clases CSS normales
            ddlEntidad.CssClass = "form-select";
            txtNumero.CssClass = "form-control";
            txtCCI.CssClass = "form-control";
            ddlTipo.CssClass = "form-select";
            ddlDivisa.CssClass = "form-select";
        }
        // =====================================================

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
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

                var entidadOrigen = entidadesBancariasBO.ObtenerPorNombre(ddlEntidad.SelectedValue);
                dto.entidad_bancaria = new SoftPacBusiness.CuentasAcreedorWS.entidadesBancariasDTO
                {
                    entidad_bancaria_id = entidadOrigen.entidad_bancaria_id,
                    entidad_bancaria_idSpecified = true,
                    nombre = entidadOrigen.nombre
                };

                var monedaOrigen = monedasBO.ObtenerPorDivisa(ddlDivisa.SelectedValue);
                dto.moneda = new SoftPacBusiness.CuentasAcreedorWS.monedasDTO
                {
                    moneda_id = monedaOrigen.moneda_id,
                    moneda_idSpecified = true,
                    codigo_iso = monedaOrigen.codigo_iso
                };

                dto.numero_cuenta = txtNumero.Text.Trim();
                dto.cci = txtCCI.Text.Trim();
                dto.tipo_cuenta = ddlTipo.SelectedValue;

                dto.activa = true;
                dto.activaSpecified = true;
                dto.fecha_eliminacionSpecified = false;
                dto.usuario_eliminacion = null;

                int resultado;
                if (dto.cuenta_bancaria_idSpecified)
                    resultado = cuentasBO.modificar(dto);
                else
                    resultado = cuentasBO.insertar(dto);

                string url = "DetalleAcreedor.aspx?id=" + acreedorId;
                string script = $@"
                    if (window.top !== window.self) {{
                        window.top.location.href = '{url}';
                    }} else {{
                        window.location.href = '{url}';
                    }}";
                ScriptManager.RegisterStartupScript(this, GetType(), "redirDetalleAcreedor", script, true);
            }
            catch (Exception ex)
            {
                // En caso de error, mostrar mensaje
                string errorScript = $"alert('Error al guardar la cuenta: {ex.Message}');";
                ScriptManager.RegisterStartupScript(this, GetType(), "errorGuardar", errorScript, true);
            }
        }
    }
}