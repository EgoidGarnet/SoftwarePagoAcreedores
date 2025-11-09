using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPac.Business;
using SoftPacBusiness.EntidadesBancariasWS;
using SoftPacBusiness.UsuariosWS;

namespace SoftPacWA
{
    public partial class EntidadesBancarias : System.Web.UI.Page
    {
        private EntidadesBancariasBO entidadesBO = new EntidadesBancariasBO();
        private PaisesBO paisesBO = new PaisesBO();

        private List<entidadesBancariasDTO> ListaEntidades;

        private usuariosDTO UsuarioLogueado
        {
            get
            {
                return (usuariosDTO)Session["UsuarioLogueado"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
            {
                CargarFiltros();
                CargarEntidades();
            }
        }

        private void CargarFiltros()
        {
            try
            {
                // Cargar países disponibles del usuario
                var paisesUsuario = UsuarioLogueado.usuario_pais
                    .Where(up => up.acceso == true)
                    .Select(up => up.pais)
                    .ToList();

                ddlFiltroPais.DataSource = paisesUsuario;
                ddlFiltroPais.DataTextField = "Nombre";
                ddlFiltroPais.DataValueField = "PaisId";
                ddlFiltroPais.DataBind();
                ddlFiltroPais.Items.Insert(0, new ListItem("Todos los países", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar filtros: " + ex.Message, "danger");
            }
        }

        private void CargarEntidades()
        {
            try
            {
                // Obtener países del usuario
                var paisesUsuario = UsuarioLogueado.usuario_pais
                    .Where(up => up.acceso == true)
                    .Select(up => up.pais.pais_id)
                    .ToList();

                ListaEntidades = entidadesBO.ListarTodos()
                    .Where(e => paisesUsuario.Contains(e.pais.pais_id)).ToList();

                // Aplicar filtro de país si está seleccionado
                if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    int paisId = int.Parse(ddlFiltroPais.SelectedValue);
                    ListaEntidades = ListaEntidades.Where(e => e.pais?.pais_id == paisId).ToList();
                }

                // Ordenar por nombre
                ListaEntidades = ListaEntidades.OrderBy(e => e.nombre).ToList();

                gvEntidades.DataSource = ListaEntidades;
                gvEntidades.DataBind();

                // Actualizar contador de registros
                lblRegistros.Text = $"Mostrando {ListaEntidades.Count} registro(s)";
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar entidades bancarias: " + ex.Message, "danger");
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarEntidades();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            ddlFiltroPais.SelectedIndex = 0;
            CargarEntidades();
        }

        protected void gvEntidades_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEntidades.PageIndex = e.NewPageIndex;
            CargarEntidades();
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string mensajeEscapado = mensaje.Replace("'", "\\'").Replace("\"", "\\\"");
            string script = $@"
                $(document).ready(function() {{
                    var alertHtml = '<div class=""alert alert-{tipo} alert-dismissible fade show"" role=""alert"">' +
                                    '{mensajeEscapado}' +
                                    '<button type=""button"" class=""btn-close"" data-bs-dismiss=""alert""></button>' +
                                    '</div>';
                    $('.content-area').prepend(alertHtml);
                    setTimeout(function() {{
                        $('.alert').fadeOut();
                    }}, 5000);
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje", script, true);
        }
    }
}