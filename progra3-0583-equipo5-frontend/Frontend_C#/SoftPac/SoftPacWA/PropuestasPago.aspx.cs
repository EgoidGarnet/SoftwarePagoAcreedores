using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class PropuestasPago : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();
        private PaisesBO paisesBO = new PaisesBO();
        private EntidadesBancariasBO bancosBO = new EntidadesBancariasBO();
        private UsuariosBO usuariosBO = new UsuariosBO();

        private UsuariosDTO UsuarioLogueado
        {
            get
            {
                return (UsuariosDTO)Session["UsuarioLogueado"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!ValidarSesion())
                    return;

                MostrarMensajeConfirmacion();
                CargarFiltros();
                CargarPropuestas();
            }
        }

        private bool ValidarSesion()
        {
            if (UsuarioLogueado == null)
            {
                Response.Redirect("~/Login.aspx");
                return false;
            }
            return true;
        }

        private void MostrarMensajeConfirmacion()
        {
            if (Request.QueryString["confirm"] == "ok")
            {
                MostrarMensaje("Propuesta guardada exitosamente", "success");
            }
        }

        private void CargarFiltros()
        {
            try
            {
                ddlFiltroPais.DataSource = UsuarioLogueado.UsuarioPais.Where(up => up.Acceso == true).Select(up => up.Pais);
                ddlFiltroPais.DataTextField = "Nombre";
                ddlFiltroPais.DataValueField = "PaisId";
                ddlFiltroPais.DataBind();
                ddlFiltroPais.Items.Insert(0, new ListItem("-- Todos --", ""));

                ddlFiltroBanco.Items.Clear();
                ddlFiltroBanco.Items.Insert(0, new ListItem("-- Todos --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar filtros: {ex.Message}", "danger");
            }
        }

        private void CargarPropuestas()
        {
            try
            {
                int? paisId = string.IsNullOrEmpty(ddlFiltroPais.SelectedValue) ? (int?)null : int.Parse(ddlFiltroPais.SelectedValue);
                int? bancoId = string.IsNullOrEmpty(ddlFiltroBanco.SelectedValue) ? (int?)null : int.Parse(ddlFiltroBanco.SelectedValue);
                string estado = ddlFiltroEstado.SelectedValue;

                var propuestas = propuestasBO.ListarConFiltros(paisId, bancoId, estado)
                    .OrderByDescending(p => p.FechaHoraCreacion)
                    .ToList();

                if (propuestas.Count > 0)
                {
                    gvPropuestas.DataSource = propuestas;
                    gvPropuestas.DataBind();
                    gvPropuestas.Visible = true;
                    pnlEmptyState.Visible = false;
                    lblTotalRegistros.Text = $"Mostrando {propuestas.Count} propuesta(s)";
                }
                else
                {
                    gvPropuestas.Visible = false;
                    pnlEmptyState.Visible = true;
                    lblTotalRegistros.Text = "0 propuestas";
                }

                upPropuestas.Update();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar propuestas: {ex.Message}", "danger");
                gvPropuestas.Visible = false;
                pnlEmptyState.Visible = true;
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            string url = "ReportePropuestas.aspx?";

            if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                url += $"pais={ddlFiltroPais.SelectedValue}&";

            if (!string.IsNullOrEmpty(ddlFiltroBanco.SelectedValue))
                url += $"banco={ddlFiltroBanco.SelectedValue}&";

            if (!string.IsNullOrEmpty(ddlFiltroEstado.SelectedValue))
                url += $"estado={ddlFiltroEstado.SelectedValue}&";

            Response.Redirect(url.TrimEnd('&'));
        }

        protected void btnCrearPropuesta_Click(object sender, EventArgs e)
        {
            LimpiarDatosCreacion();
            Response.Redirect("~/CrearPropuestaPaso1.aspx");
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvPropuestas.PageIndex = 0;
            CargarPropuestas();
        }

        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            ddlFiltroPais.SelectedIndex = 0;
            ddlFiltroBanco.SelectedIndex = 0;
            ddlFiltroEstado.SelectedIndex = 0;
            gvPropuestas.PageIndex = 0;
            CargarPropuestas();
        }

        protected void gvPropuestas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPropuestas.PageIndex = e.NewPageIndex;
            CargarPropuestas();
        }

        protected void ddlFiltroPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlFiltroBanco.Items.Clear();

                if (string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    ddlFiltroBanco.Items.Insert(0, new ListItem("-- Todos --", ""));
                    return;
                }

                int paisId = int.Parse(ddlFiltroPais.SelectedValue);

                var bancos = bancosBO.ListarTodos()
                    .Where(b => b.Pais.PaisId == paisId)
                    .OrderBy(b => b.Nombre)
                    .ToList();

                ddlFiltroBanco.DataSource = bancos;
                ddlFiltroBanco.DataTextField = "Nombre";
                ddlFiltroBanco.DataValueField = "EntidadBancariaId";
                ddlFiltroBanco.DataBind();

                ddlFiltroBanco.Items.Insert(0, new ListItem("-- Todos --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar bancos: {ex.Message}", "danger");
            }
        }

        private void LimpiarDatosCreacion()
        {
            Session.Remove("PropuestaPago_PaisId");
            Session.Remove("PropuestaPago_BancoId");
            Session.Remove("PropuestaPago_PlazoVencimiento");
            Session.Remove("PropuestaPago_DetallesParciales");
            Session.Remove("PropuestaPago_CuentasSeleccionadas");
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }

        protected string GetEstadoClass(string estado)
        {
            switch (estado?.ToUpper())
            {
                case "PENDIENTE": return "bg-warning text-dark";
                case "ENVIADA": return "bg-success";
                case "ELIMINADA": return "bg-danger";
                default: return "bg-light text-dark";
            }
        }
    }
}