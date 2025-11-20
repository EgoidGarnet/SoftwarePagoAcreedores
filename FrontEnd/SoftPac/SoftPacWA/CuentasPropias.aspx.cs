using SoftPac.Business;
using SoftPacBusiness.CuentasPropiasWS;
using SoftPacBusiness.EntidadesBancariasWS;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace SoftPacWA
{
    public partial class CuentasPropias : System.Web.UI.Page
    {
        private CuentasPropiasBO cuentasBO = new CuentasPropiasBO();
        private EntidadesBancariasBO entidadesBO = new EntidadesBancariasBO();
        private MonedasBO monedasBO = new MonedasBO();

        private List<cuentasPropiasDTO> ListaCompletaCuentas
        {
            get { return Session["ListaCompletaCuentasPropias"] as List<cuentasPropiasDTO>; }
            set { Session["ListaCompletaCuentasPropias"] = value; }
        }

        private List<cuentasPropiasDTO> ListaFiltradaCuentas
        {
            get { return Session["ListaFiltradaCuentasPropias"] as List<cuentasPropiasDTO>; }
            set { Session["ListaFiltradaCuentasPropias"] = value; }
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
                if ((string)Session["MensajeExito"] != null)
                {
                    MostrarMensaje((string)Session["MensajeExito"], "success");
                    Session.Remove("MensajeExito");
                }
                ListaCompletaCuentas = cuentasBO.ListarTodos().ToList();
                ListaFiltradaCuentas = ListaCompletaCuentas;

                CargarFiltros();
                CargarCatalogosModal();
                CargarCuentasParaAutocomplete();
                CargarGrid();
            }
        }

        #region Autocomplete
        private void CargarCuentasParaAutocomplete()
        {
            try
            {
                List<cuentasPropiasDTO> listaCuentas = cuentasBO.ListarTodos().ToList();

                var cuentasSimplificadas = listaCuentas.Select(c => new
                {
                    numero_cuenta = c.numero_cuenta ?? "",
                    entidad_bancaria = c.entidad_bancaria?.nombre ?? "",
                    cci = c.cci ?? "",
                    moneda = c.moneda?.codigo_iso ?? ""
                }).ToList();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(cuentasSimplificadas);
                hfCuentasJson.Value = json;
            }
            catch (Exception ex)
            {
                hfCuentasJson.Value = "[]";
            }
        }
        #endregion

        #region Carga de Datos
        private void CargarGrid()
        {
            gvCuentasPropias.DataSource = ListaFiltradaCuentas;
            gvCuentasPropias.DataBind();
            lblRegistros.Text = $"Mostrando {ListaFiltradaCuentas.Count} cuenta(s)";
        }

        private void CargarFiltros()
        {
            // Cargar entidades bancarias
            ddlFiltroEntidad.DataSource = entidadesBO.ListarTodos();
            ddlFiltroEntidad.DataTextField = "nombre";
            ddlFiltroEntidad.DataValueField = "entidad_bancaria_id";
            ddlFiltroEntidad.DataBind();
            ddlFiltroEntidad.Items.Insert(0, new ListItem("Todas", ""));

            // Cargar monedas
            ddlFiltroMoneda.DataSource = monedasBO.ListarTodos();
            ddlFiltroMoneda.DataTextField = "nombre";
            ddlFiltroMoneda.DataValueField = "moneda_id";
            ddlFiltroMoneda.DataBind();
            ddlFiltroMoneda.Items.Insert(0, new ListItem("Todas", ""));
        }

        private void CargarCatalogosModal()
        {
            ddlEntidadBancaria.DataSource = entidadesBO.ListarTodos();
            ddlEntidadBancaria.DataTextField = "nombre";
            ddlEntidadBancaria.DataValueField = "entidad_bancaria_id";
            ddlEntidadBancaria.DataBind();
            ddlEntidadBancaria.Items.Insert(0, new ListItem("-- Seleccione --", "0"));

            ddlMoneda.DataSource = monedasBO.ListarTodos();
            ddlMoneda.DataTextField = "nombre";
            ddlMoneda.DataValueField = "moneda_id";
            ddlMoneda.DataBind();
            ddlMoneda.Items.Insert(0, new ListItem("-- Seleccione --", "0"));
        }
        #endregion

        #region Filtros
        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            IEnumerable<cuentasPropiasDTO> filtrados = ListaCompletaCuentas;

            // Filtro por número de cuenta (búsqueda)
            string numeroCuentaBusqueda = (txtBuscarCuenta.Text ?? string.Empty).Trim().ToLower();
            if (!string.IsNullOrEmpty(numeroCuentaBusqueda))
            {
                filtrados = filtrados.Where(c =>
                    (c.numero_cuenta ?? string.Empty).ToLower().Contains(numeroCuentaBusqueda) ||
                    (c.cci ?? string.Empty).ToLower().Contains(numeroCuentaBusqueda) ||
                    (c.entidad_bancaria?.nombre ?? string.Empty).ToLower().Contains(numeroCuentaBusqueda)
                );
            }

            // Filtro por entidad bancaria
            if (!string.IsNullOrEmpty(ddlFiltroEntidad.SelectedValue))
            {
                int entidadId = Convert.ToInt32(ddlFiltroEntidad.SelectedValue);
                filtrados = filtrados.Where(c => c.entidad_bancaria.entidad_bancaria_id == entidadId);
            }

            // Filtro por moneda
            if (!string.IsNullOrEmpty(ddlFiltroMoneda.SelectedValue))
            {
                int monedaId = Convert.ToInt32(ddlFiltroMoneda.SelectedValue);
                filtrados = filtrados.Where(c => c.moneda.moneda_id == monedaId);
            }

            // Filtro por saldo
            if (!string.IsNullOrWhiteSpace(txtFiltroSaldo.Text))
            {
                decimal saldo = Convert.ToDecimal(txtFiltroSaldo.Text);
                filtrados = filtrados.Where(c => c.saldo_disponible >= saldo);
            }

            ListaFiltradaCuentas = filtrados.ToList();
            gvCuentasPropias.PageIndex = 0;
            CargarGrid();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarCuenta.Text = string.Empty;
            ddlFiltroEntidad.SelectedIndex = 0;
            ddlFiltroMoneda.SelectedIndex = 0;
            txtFiltroSaldo.Text = string.Empty;
            ListaFiltradaCuentas = ListaCompletaCuentas;
            gvCuentasPropias.PageIndex = 0;
            CargarGrid();
        }
        #endregion

        #region Eventos del Grid
        protected void gvCuentasPropias_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCuentasPropias.PageIndex = e.NewPageIndex;
            CargarGrid();
        }

        protected void gvCuentasPropias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int cuentaId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Modificar")
            {
                var cuenta = cuentasBO.ObtenerPorId(cuentaId);
                if (cuenta != null)
                {
                    CargarDatosEnModal(cuenta);
                    ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
                }
            }
            else if (e.CommandName == "MostrarModalEliminar") // CAMBIO AQUÍ
            {
                // Guardar ID de la cuenta en ViewState
                ViewState["CuentaEliminar"] = cuentaId;

                // Abrir modal de confirmación
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(),
                    "abrirModalEliminar",
                    "var m = new bootstrap.Modal(document.getElementById('modalConfirmarEliminar')); m.show();",
                    true
                );
            }
            else if (e.CommandName == "VerDetalle")
            {
                Response.Redirect("DetalleCuentaPropia.aspx?id="+cuentaId);
            }
        }
        #endregion

        protected void btnConfirmarEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int cuentaId = (int)ViewState["CuentaEliminar"];
                var usuario = UsuarioLogueado;

                cuentasPropiasDTO cuentaEliminada = new cuentasPropiasDTO();
                cuentaEliminada.cuenta_bancaria_id = cuentaId;

                int resultado = cuentasBO.Eliminar(cuentaEliminada,
                    DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO, SoftPacBusiness.CuentasPropiasWS.usuariosDTO>(usuario));

                if (resultado > 0)
                {
                    MostrarMensaje("Cuenta eliminada correctamente.", "success");
                    ListaCompletaCuentas = cuentasBO.ListarTodos().ToList();
                    btnFiltrar_Click(null, null);
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

        #region Lógica del Modal
        protected void btnAbrirModalNuevo_Click(object sender, EventArgs e)
        {
            LimpiarModal();
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
        }

        private void CargarDatosEnModal(cuentasPropiasDTO cuenta)
        {
            litModalTitulo.Text = "Modificar Cuenta Propia";
            hfCuentaId.Value = cuenta.cuenta_bancaria_id.ToString();

            ddlEntidadBancaria.SelectedValue = cuenta.entidad_bancaria.entidad_bancaria_id.ToString();
            ddlMoneda.SelectedValue = cuenta.moneda.moneda_id.ToString();

            if (!string.IsNullOrWhiteSpace(cuenta.tipo_cuenta))
            {
                ListItem item = ddlTipoCuenta.Items.FindByValue(cuenta.tipo_cuenta);
                if (item != null)
                {
                    ddlTipoCuenta.SelectedValue = cuenta.tipo_cuenta;
                }
                else
                {
                    ddlTipoCuenta.Items.Add(new ListItem(cuenta.tipo_cuenta, cuenta.tipo_cuenta));
                    ddlTipoCuenta.SelectedValue = cuenta.tipo_cuenta;
                }
            }

            txtNumeroCuenta.Text = cuenta.numero_cuenta;
            txtCci.Text = cuenta.cci;
            txtSaldoDisponible.Text = cuenta.saldo_disponible.ToString("0.00");

            chkActiva.Checked = cuenta.activa;

            chkActiva.Disabled = false;

            txtNumeroCuenta.ReadOnly = true;
        }

        private void LimpiarModal()
        {
            litModalTitulo.Text = "Nueva Cuenta Propia";
            hfCuentaId.Value = "0";
            ddlEntidadBancaria.SelectedIndex = 0;
            ddlMoneda.SelectedIndex = 0;
            ddlTipoCuenta.SelectedIndex = 0;
            txtNumeroCuenta.Text = string.Empty;
            txtCci.Text = string.Empty;
            txtSaldoDisponible.Text = string.Empty;

            // NUEVO: Marcar como activo y deshabilitar en INSERT
            chkActiva.Checked = true;
            chkActiva.Disabled = true;

            txtNumeroCuenta.ReadOnly = false;
            LimpiarErrores();
        }

        #region Validación
        private bool ValidarCampos()
        {
            bool esValido = true;
            LimpiarErrores();

            // Validar Entidad Bancaria
            if (ddlEntidadBancaria.SelectedValue == "0")
            {
                lblEntidadError.Text = "Debe seleccionar una entidad bancaria.";
                ddlEntidadBancaria.CssClass = "form-select is-invalid";
                esValido = false;
            }

            // Validar Moneda
            if (ddlMoneda.SelectedValue == "0")
            {
                lblMonedaError.Text = "Debe seleccionar una moneda.";
                ddlMoneda.CssClass = "form-select is-invalid";
                esValido = false;
            }

            // Validar Tipo de Cuenta
            if (string.IsNullOrEmpty(ddlTipoCuenta.SelectedValue))
            {
                lblTipoCuentaError.Text = "Debe seleccionar un tipo de cuenta.";
                ddlTipoCuenta.CssClass = "form-select is-invalid";
                esValido = false;
            }

            // Validar Número de Cuenta
            string numeroCuenta = (txtNumeroCuenta.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(numeroCuenta))
            {
                lblNumeroCuentaError.Text = "El número de cuenta es obligatorio.";
                txtNumeroCuenta.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (numeroCuenta.Length < 10)
            {
                lblNumeroCuentaError.Text = "El número de cuenta debe tener al menos 10 caracteres.";
                txtNumeroCuenta.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (numeroCuenta.Length > 20)
            {
                lblNumeroCuentaError.Text = "El número de cuenta no puede exceder 20 caracteres.";
                txtNumeroCuenta.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(numeroCuenta, @"^\d+$"))
            {
                lblNumeroCuentaError.Text = "El número de cuenta debe contener solo dígitos.";
                txtNumeroCuenta.CssClass = "form-control is-invalid";
                esValido = false;
            }

            // Validar CCI
            string cci = (txtCci.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(cci))
            {
                lblCciError.Text = "El CCI es obligatorio.";
                txtCci.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (cci.Length < 20)
            {
                lblCciError.Text = "El CCI debe tener al menos 20 caracteres.";
                txtCci.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (cci.Length > 28)
            {
                lblCciError.Text = "El CCI no puede exceder 28 caracteres.";
                txtCci.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(cci, @"^[0-9-]+$"))
            {
                lblCciError.Text = "El CCI debe contener solo números y guiones.";
                txtCci.CssClass = "form-control is-invalid";
                esValido = false;
            }

            // Validar Saldo Disponible
            string saldoText = (txtSaldoDisponible.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(saldoText))
            {
                lblSaldoError.Text = "El saldo disponible es obligatorio.";
                txtSaldoDisponible.CssClass = "form-control is-invalid";
                esValido = false;
            }
            else
            {
                decimal saldo;
                if (!decimal.TryParse(saldoText, out saldo))
                {
                    lblSaldoError.Text = "El saldo debe ser un número válido.";
                    txtSaldoDisponible.CssClass = "form-control is-invalid";
                    esValido = false;
                }
                else if (saldo < 0)
                {
                    lblSaldoError.Text = "El saldo no puede ser negativo.";
                    txtSaldoDisponible.CssClass = "form-control is-invalid";
                    esValido = false;
                }
            }

            return esValido;
        }

        private void LimpiarErrores()
        {
            // Limpiar mensajes de error
            lblEntidadError.Text = string.Empty;
            lblMonedaError.Text = string.Empty;
            lblTipoCuentaError.Text = string.Empty;
            lblNumeroCuentaError.Text = string.Empty;
            lblCciError.Text = string.Empty;
            lblSaldoError.Text = string.Empty;

            // Restaurar clases CSS normales
            ddlEntidadBancaria.CssClass = "form-select";
            ddlMoneda.CssClass = "form-select";
            ddlTipoCuenta.CssClass = "form-select";
            txtNumeroCuenta.CssClass = "form-control";
            txtCci.CssClass = "form-control";
            txtSaldoDisponible.CssClass = "form-control";
        }
        #endregion

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                cuentasPropiasDTO cuenta = new cuentasPropiasDTO
                {
                    cuenta_bancaria_id = Convert.ToInt32(hfCuentaId.Value),
                    entidad_bancaria = new SoftPacBusiness.CuentasPropiasWS.entidadesBancariasDTO { entidad_bancaria_id = Convert.ToInt32(ddlEntidadBancaria.SelectedValue) },
                    moneda = new monedasDTO { moneda_id = Convert.ToInt32(ddlMoneda.SelectedValue) },
                    tipo_cuenta = ddlTipoCuenta.SelectedValue,
                    numero_cuenta = txtNumeroCuenta.Text.Trim(),
                    cci = txtCci.Text.Trim(),
                    saldo_disponible = Convert.ToDecimal(txtSaldoDisponible.Text),
                    activa = chkActiva.Checked,
                };

                int resultado;
                if (cuenta.cuenta_bancaria_id == 0)
                {
                    resultado = cuentasBO.Insertar(cuenta);
                }
                else
                {
                    resultado = cuentasBO.Modificar(cuenta,UsuarioLogueado.usuario_id);
                }

                if (resultado > 0)
                {
                    ListaCompletaCuentas = cuentasBO.ListarTodos().ToList();
                    btnFiltrar_Click(null, null);
                    MostrarMensaje("Cuenta guardada correctamente.", "success");

                    ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal",
                        "cerrarModal();", true);
                }
                else
                {
                    MostrarMensaje("Error al guardar la cuenta.", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error: {ex.Message}", "danger");
            }
        }
        #endregion

        private void MostrarMensaje(string mensaje, string tipo)
        {
            divMensaje.Visible = true;
            divMensaje.InnerHtml = $"<div class='alert alert-{tipo} alert-dismissible fade show'>{mensaje}<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";
        }
    }
}