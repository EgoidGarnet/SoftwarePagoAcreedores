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
                CargarFiltroEntidades(); // Cargar todas las entidades inicialmente
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
                ddlFiltroPais.DataTextField = "nombre";
                ddlFiltroPais.DataValueField = "pais_id";
                ddlFiltroPais.DataBind();
                ddlFiltroPais.Items.Insert(0, new ListItem("Todos los países", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar filtros: " + ex.Message, "danger");
            }
        }

        private void CargarFiltroEntidades()
        {
            try
            {
                // Obtener países del usuario
                var paisesUsuario = UsuarioLogueado.usuario_pais
                    .Where(up => up.acceso == true)
                    .Select(up => up.pais.pais_id)
                    .ToList();

                // Obtener todas las entidades según los países del usuario
                var entidades = entidadesBO.ListarTodos()
                    .Where(e => e.pais != null && paisesUsuario.Contains(e.pais.pais_id))
                    .OrderBy(e => e.nombre)
                    .ToList();

                // Si hay un país seleccionado, filtrar solo entidades de ese país
                if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    int paisId = int.Parse(ddlFiltroPais.SelectedValue);
                    entidades = entidades.Where(e => e.pais != null && e.pais.pais_id == paisId).ToList();
                }

                // Limpiar el dropdown antes de cargar
                ddlFiltroEntidad.Items.Clear();

                ddlFiltroEntidad.DataSource = entidades;
                ddlFiltroEntidad.DataTextField = "nombre";
                ddlFiltroEntidad.DataValueField = "entidad_bancaria_id";
                ddlFiltroEntidad.DataBind();

                // Agregar la opción por defecto DESPUÉS de hacer DataBind
                ddlFiltroEntidad.Items.Insert(0, new ListItem("Todas las entidades", ""));

                // IMPORTANTE: Asegurar que esté seleccionada la primera opción
                ddlFiltroEntidad.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar entidades: " + ex.Message, "danger");
            }
        }

        protected void ddlFiltroPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Recargar el dropdown de entidades según el país seleccionado
            CargarFiltroEntidades();
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

                // Iniciar con todas las entidades del usuario
                ListaEntidades = entidadesBO.ListarTodos()
                    .Where(e => e.pais != null && paisesUsuario.Contains(e.pais.pais_id)).ToList();

                // Debug: Ver cuántas entidades tenemos antes de filtrar
                System.Diagnostics.Debug.WriteLine($"Total entidades antes de filtrar: {ListaEntidades.Count}");
                System.Diagnostics.Debug.WriteLine($"País seleccionado: '{ddlFiltroPais.SelectedValue}'");
                System.Diagnostics.Debug.WriteLine($"Entidad seleccionada: '{ddlFiltroEntidad.SelectedValue}'");

                // Aplicar filtro de país si está seleccionado
                if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    int paisId = int.Parse(ddlFiltroPais.SelectedValue);
                    ListaEntidades = ListaEntidades.Where(e => e.pais != null && e.pais.pais_id == paisId).ToList();
                    System.Diagnostics.Debug.WriteLine($"Después de filtrar por país: {ListaEntidades.Count}");
                }

                // Aplicar filtro de entidad bancaria si está seleccionado
                if (!string.IsNullOrEmpty(ddlFiltroEntidad.SelectedValue))
                {
                    int entidadId = int.Parse(ddlFiltroEntidad.SelectedValue);
                    ListaEntidades = ListaEntidades.Where(e => e.entidad_bancaria_id == entidadId).ToList();
                    System.Diagnostics.Debug.WriteLine($"Después de filtrar por entidad: {ListaEntidades.Count}");
                }

                // Ordenar por nombre
                ListaEntidades = ListaEntidades.OrderBy(e => e.nombre).ToList();

                gvEntidades.DataSource = ListaEntidades;
                gvEntidades.DataBind();

                // Actualizar contador de registros
                lblRegistros.Text = $"Mostrando {ListaEntidades.Count} banco(s)";
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar entidades bancarias: " + ex.Message, "danger");
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            gvEntidades.PageIndex = 0; // Resetear a la primera página
            CargarEntidades();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Resetear ambos dropdowns al índice 0 (primera opción)
            ddlFiltroPais.SelectedIndex = 0;
            ddlFiltroEntidad.Items.Clear(); // Limpiar el dropdown de entidades

            // Recargar todas las entidades disponibles
            CargarFiltroEntidades();

            // Asegurar que el dropdown de entidades esté en "Todas las entidades"
            ddlFiltroEntidad.SelectedIndex = 0;

            // Resetear la paginación y recargar datos
            gvEntidades.PageIndex = 0;
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