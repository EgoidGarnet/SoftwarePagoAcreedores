using System;
using System.Collections.Generic;
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
            if (!IsPostBack)
            {
                CargarInformacionUsuario();
            }
        }

        private void CargarInformacionUsuario()
        {
            // Verificar si hay sesión activa
            if (true)
            {
                // Aquí puedes obtener el usuario de la sesión
                // UsuariosDTO usuario = (UsuariosDTO)Session["Usuario"];
                // lblUsuario.Text = usuario.Nombre + " " + usuario.Apellidos;
                // lblRol.Text = usuario.Superusuario ? "Superusuario" : "Usuario";

                // Por ahora valores por defecto

            }
            else
            {
                // Redirigir al login si no hay sesión
                Response.Redirect("~/Login.aspx");
            }
        }
    }
}