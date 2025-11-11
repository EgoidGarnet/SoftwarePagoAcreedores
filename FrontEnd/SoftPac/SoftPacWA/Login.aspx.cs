using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Web.UI;

namespace SoftPacWA
{
    public partial class Login : System.Web.UI.Page
    {
        private UsuariosBO usuariosBO = new UsuariosBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Si el usuario ya está logueado, lo redirigimos a la página principal
                if (Session["UsuarioLogueado"] != null)
                {
                    Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(password))
            {
                MostrarError("Por favor, ingrese usuario y contraseña.");
                return;
            }

            try
            {
                usuariosDTO usuario = usuariosBO.AutenticarUsuario(nombreUsuario, password);

                if (usuario != null && usuario.usuario_id != 0)
                {
                    Session["UsuarioLogueado"] = usuario;

                    // --- LÓGICA DE REDIRECCIÓN MODIFICADA ---
                    if (usuario.superusuario)
                    {
                        Response.Redirect("~/GestionUsuarios.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/Default.aspx");
                    }
                }
                else
                {
                    MostrarError("Usuario o contraseña no válidos.");
                }
            }
            catch (Exception ex)
            {
                // Manejo de cualquier otro error
                MostrarError("Ocurrió un error inesperado. Por favor, intente más tarde.");

            }
        }

        private void MostrarError(string mensaje)
        {
            lblMensajeError.Text = mensaje;
            lblMensajeError.Visible = true;
        }
    }
}