using SoftPacBusiness.UsuariosWS;
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
                usuariosDTO usuario = (usuariosDTO)Session["UsuarioLogueado"];
                litNombreUsuario.Text = Server.HtmlEncode(usuario.nombre);
            }
            else
            {
                // Si por alguna razón llega aquí sin sesión, lo regresamos al login.
                Response.Redirect("~/Login.aspx");
            }
        }
    }
}