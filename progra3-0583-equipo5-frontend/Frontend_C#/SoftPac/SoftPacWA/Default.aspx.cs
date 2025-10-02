using SoftPac.Model;
using System;
using System.Web.UI;

namespace SoftPacWA
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosUsuario();
            }
        }

        private void CargarDatosUsuario()
        {
            if (Session["UsuarioLogueado"] != null)
            {
                UsuariosDTO usuario = (UsuariosDTO)Session["UsuarioLogueado"];
                litNombreUsuario.Text = Server.HtmlEncode(usuario.Nombre);
            }
            else
            {
                // Si por alguna razón llega aquí sin sesión, lo regresamos al login.
                Response.Redirect("~/Login.aspx");
            }
        }
    }
}