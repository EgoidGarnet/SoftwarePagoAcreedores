using SoftPac.Business;
using SoftPac.Model;
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

        private List<CuentasPropiasDTO> ListaCompletaCuentas
        {
            get { return Session["ListaCompletaCuentasPropias"] as List<CuentasPropiasDTO>; }
            set { Session["ListaCompletaCuentasPropias"] = value; }
        }

        private List<CuentasPropiasDTO> ListaFiltradaCuentas
        {
            get { return Session["ListaFiltradaCuentasPropias"] as List<CuentasPropiasDTO>; }
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
            ddlFiltroEntidad.DataTextField = "Nombre";
            ddlFiltroEntidad.DataValueField = "EntidadBancariaId";
            ddlFiltroEntidad.DataBind();
            ddlFiltroEntidad.Items.Insert(0, new ListItem("Todas", ""));

            // Cargar monedas
            ddlFiltroMoneda.DataSource = monedasBO.ListarTodos();
            ddlFiltroMoneda.DataTextField = "Nombre";
            ddlFiltroMoneda.DataValueField = "MonedaId";
            ddlFiltroMoneda.DataBind();
            ddlFiltroMoneda.Items.Insert(0, new ListItem("Todas", ""));
        }

        private void CargarCatalogosModal()
        {
            ddlEntidadBancaria.DataSource = entidadesBO.ListarTodos();
            ddlEntidadBancaria.DataTextField = "Nombre";
            ddlEntidadBancaria.DataValueField = "EntidadBancariaId";
            ddlEntidadBancaria.DataBind();
            ddlEntidadBancaria.Items.Insert(0, new ListItem("-- Seleccione --", "0"));

            ddlMoneda.DataSource = monedasBO.ListarTodos();
            ddlMoneda.DataTextField = "Nombre";
            ddlMoneda.DataValueField = "MonedaId";
            ddlMoneda.DataBind();
            ddlMoneda.Items.Insert(0, new ListItem("-- Seleccione --", "0"));
        }
        #endregion

        #region Filtros
        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            IEnumerable<CuentasPropiasDTO> filtrados = ListaCompletaCuentas;

            // Filtro por entidad bancaria
            if (!string.IsNullOrEmpty(ddlFiltroEntidad.SelectedValue))
            {
                int entidadId = Convert.ToInt32(ddlFiltroEntidad.SelectedValue);
                filtrados = filtrados.Where(c => c.EntidadBancaria.EntidadBancariaId == entidadId);
            }

            // Filtro por moneda
            if (!string.IsNullOrEmpty(ddlFiltroMoneda.SelectedValue))
            {
                int monedaId = Convert.ToInt32(ddlFiltroMoneda.SelectedValue);
                filtrados = filtrados.Where(c => c.Moneda.MonedaId == monedaId);
            }

            // Filtro por saldo
            if (!string.IsNullOrWhiteSpace(txtFiltroSaldo.Text))
            {
                decimal saldo = Convert.ToDecimal(txtFiltroSaldo.Text);
                filtrados = filtrados.Where(c => c.SaldoDisponible >= saldo);
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
                var usuario = (UsuariosDTO)Session["UsuarioLogueado"];
                int resultado = cuentasBO.Eliminar(cuentaId, usuario.UsuarioId.Value);

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

        private void CargarDatosEnModal(CuentasPropiasDTO cuenta)
        {
            litModalTitulo.Text = "Modificar Cuenta Propia";
            hfCuentaId.Value = cuenta.CuentaBancariaId.ToString();

            ddlEntidadBancaria.SelectedValue = cuenta.EntidadBancaria.EntidadBancariaId.ToString();
            ddlMoneda.SelectedValue = cuenta.Moneda.MonedaId.ToString();
            txtTipoCuenta.Text = cuenta.TipoCuenta;
            txtNumeroCuenta.Text = cuenta.NumeroCuenta;
            txtCci.Text = cuenta.Cci;
            txtSaldoDisponible.Text = cuenta.SaldoDisponible.ToString("F2");
            chkActiva.Checked = cuenta.Activa;
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
                CuentasPropiasDTO cuenta = new CuentasPropiasDTO
                {
                    CuentaBancariaId = Convert.ToInt32(hfCuentaId.Value),
                    EntidadBancaria = new EntidadesBancariasDTO { EntidadBancariaId = Convert.ToInt32(ddlEntidadBancaria.SelectedValue) },
                    Moneda = new MonedasDTO { MonedaId = Convert.ToInt32(ddlMoneda.SelectedValue) },
                    TipoCuenta = txtTipoCuenta.Text,
                    NumeroCuenta = txtNumeroCuenta.Text,
                    Cci = txtCci.Text,
                    SaldoDisponible = Convert.ToDecimal(txtSaldoDisponible.Text),
                    Activa = chkActiva.Checked
                };

                int resultado;
                if (cuenta.CuentaBancariaId == 0)
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