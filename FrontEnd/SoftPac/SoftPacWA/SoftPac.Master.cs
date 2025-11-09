using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class SoftPac : System.Web.UI.MasterPage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            // Obtenemos el nombre de la página actual
            string currentPage = Path.GetFileName(Request.Url.AbsolutePath);

            // Si la página actual no es Login.aspx Y no hay sesión, redirigimos
            if (currentPage.ToLower() != "login.aspx" && Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("~/Login.aspx");
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
                usuariosDTO usuario = (usuariosDTO)Session["UsuarioLogueado"];
                lblUsuario.Text = usuario.nombre_de_usuario;
                lblRol.Text = usuario.superusuario ? "Superusuario" : "Usuario";
            }
        }
        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}