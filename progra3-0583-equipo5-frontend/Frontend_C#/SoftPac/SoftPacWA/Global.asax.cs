using System;
using System.Web;
using System.Web.UI;

namespace SoftPacWA
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Configurar jQuery para validadores UnobtrusiveValidation
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery-3.7.1.min.js",
                    DebugPath = "~/Scripts/jquery-3.7.1.js",
                    CdnPath = "https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js",
                    CdnDebugPath = "https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.js",
                    CdnSupportsSecureConnection = true,
                    LoadSuccessExpression = "window.jQuery"
                });
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Código cuando inicia una sesión
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Código antes de cada request
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Código de autenticación
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Manejo de errores global
            Exception ex = Server.GetLastError();
            // TODO: Registrar el error en un log
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Código cuando termina una sesión
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Código cuando termina la aplicación
        }
    }
}