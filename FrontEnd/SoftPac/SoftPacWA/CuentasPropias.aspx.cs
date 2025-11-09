using SoftPac.Business;
using SoftPacBusiness.CuentasPropiasWS;
using SoftPacBusiness.EntidadesBancariasWS;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ListaCompletaCuentas = cuentasBO.ListarTodos().ToList();
                ListaFiltradaCuentas = ListaCompletaCuentas;

                CargarFiltros();
                CargarCatalogosModal();
                CargarGrid();
            }
        }

        #region Carga de Datos
        private void CargarGrid()
        {
            gvCuentasPropias.DataSource = ListaFiltradaCuentas;
            gvCuentasPropias.DataBind();
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
            else if (e.CommandName == "Eliminar")
            {
                var usuario = (SoftPacBusiness.CuentasPropiasWS.usuariosDTO)Session["UsuarioLogueado"];
                cuentasPropiasDTO cuentaEliminada = new cuentasPropiasDTO();
                cuentaEliminada.cuenta_bancaria_id = cuentaId;
                int resultado = cuentasBO.Eliminar(cuentaEliminada, usuario);

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
        }
        #endregion

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
            txtTipoCuenta.Text = cuenta.tipo_cuenta;
            txtNumeroCuenta.Text = cuenta.numero_cuenta;
            txtCci.Text = cuenta.cci;
            txtSaldoDisponible.Text = cuenta.saldo_disponible.ToString("F2");
            chkActiva.Checked = cuenta.activa;
        }

        private void LimpiarModal()
        {
            litModalTitulo.Text = "Nueva Cuenta Propia";
            hfCuentaId.Value = "0";
            ddlEntidadBancaria.SelectedIndex = 0;
            ddlMoneda.SelectedIndex = 0;
            txtTipoCuenta.Text = string.Empty;
            txtNumeroCuenta.Text = string.Empty;
            txtCci.Text = string.Empty;
            txtSaldoDisponible.Text = string.Empty;
            chkActiva.Checked = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                cuentasPropiasDTO cuenta = new cuentasPropiasDTO
                {
                    cuenta_bancaria_id = Convert.ToInt32(hfCuentaId.Value),
                    entidad_bancaria = new SoftPacBusiness.CuentasPropiasWS.entidadesBancariasDTO { entidad_bancaria_id = Convert.ToInt32(ddlEntidadBancaria.SelectedValue), entidad_bancaria_idSpecified = true },
                    moneda = new monedasDTO { moneda_id = Convert.ToInt32(ddlMoneda.SelectedValue), moneda_idSpecified = true },
                    tipo_cuenta = txtTipoCuenta.Text,
                    numero_cuenta = txtNumeroCuenta.Text,
                    cci = txtCci.Text,
                    saldo_disponible = Convert.ToDecimal(txtSaldoDisponible.Text),
                    activa = chkActiva.Checked,
                    cuenta_bancaria_idSpecified = true,
                    
                    activaSpecified = true,
                    saldo_disponibleSpecified = true
                };

                int resultado;
                if (cuenta.cuenta_bancaria_id == 0)
                {
                    resultado = cuentasBO.Insertar(cuenta);
                }
                else
                {
                    resultado = cuentasBO.Modificar(cuenta);
                }

                if (resultado > 0)
                {
                    MostrarMensaje("Cuenta guardada correctamente.", "success");
                    ListaCompletaCuentas = cuentasBO.ListarTodos().ToList();
                    btnFiltrar_Click(null, null);
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