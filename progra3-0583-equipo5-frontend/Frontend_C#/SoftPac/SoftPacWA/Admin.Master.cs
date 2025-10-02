using SoftPac.Model;
using System;
using System.Web.UI;

namespace SoftPacWA
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificamos si hay un usuario logueado y si es Superusuario
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                UsuariosDTO usuario = (UsuariosDTO)Session["UsuarioLogueado"];
                if (!usuario.Superusuario)
                {
                    // Si no es superusuario, lo mandamos a la página por defecto de usuarios normales
                    Response.Redirect("~/Default.aspx");
                }
            }

            if (!IsPostBack)
            {
                CargarInformacionUsuario();
            }
        }

        private void CargarInformacionUsuario()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                UsuariosDTO usuario = (UsuariosDTO)Session["UsuarioLogueado"];
                lblUsuario.Text = usuario.Nombre;
                lblRol.Text = "Superusuario";
            }
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}