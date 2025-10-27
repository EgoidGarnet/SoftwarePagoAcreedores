using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace SoftPacWA
{
    // Clase auxiliar para el nuevo formato de la tabla de actividad
    public class AccionUsuario
    {
        public int PropuestaId { get; set; }
        public DateTime FechaAccion { get; set; }
        public string TipoAccion { get; set; }
        public string Estado { get; set; }
        public int NumFacturas { get; set; }
    }

    public partial class Perfil : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();
        private UsuariosDTO usuarioLogueado;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            usuarioLogueado = (UsuariosDTO)Session["UsuarioLogueado"];

            if (!IsPostBack)
            {
                CargarDatosPerfil();
            }
        }

        private void CargarDatosPerfil()
        {
            litNombreCompleto.Text = Server.HtmlEncode(usuarioLogueado.Nombre + " " + usuarioLogueado.Apellidos);
            litCorreo.Text = Server.HtmlEncode(usuarioLogueado.CorreoElectronico);

            var paisesConAcceso = usuarioLogueado.UsuarioPais.Where(up => up.Acceso).Select(up => up.Pais).ToList();
            rptPaisesAcceso.DataSource = paisesConAcceso;
            rptPaisesAcceso.DataBind();

            CargarActividadReciente();
        }

        private void CargarActividadReciente()
        {
            // Obtenemos todas las propuestas donde el usuario participó (creó, modificó o eliminó)
            var propuestasDelUsuario = propuestasBO.ListarActividadPorUsuario(usuarioLogueado.UsuarioId.Value);

            // 1. Calcular estadísticas (lógica sin cambios)
            int propuestasEsteMes = propuestasDelUsuario.Count(p => p.FechaHoraCreacion.HasValue && p.FechaHoraCreacion.Value.Month == DateTime.Now.Month && p.FechaHoraCreacion.Value.Year == DateTime.Now.Year);
            litPropuestasMes.Text = propuestasEsteMes.ToString();

            var ultimaPropuesta = propuestasDelUsuario.OrderByDescending(p => p.FechaHoraCreacion).FirstOrDefault();
            if (ultimaPropuesta != null && ultimaPropuesta.EntidadBancaria!=null && ultimaPropuesta.EntidadBancaria.Pais!=null)
            {
                litUltimoPais.Text = ultimaPropuesta.EntidadBancaria.Pais.Nombre;
            }
            else
            {
                litUltimoPais.Text = "-";
            }

            // 2. Generar la lista de acciones para la nueva tabla
            var logDeAcciones = new List<AccionUsuario>();

            foreach (var propuesta in propuestasDelUsuario)
            {
                // Acción de Creación
                if (propuesta.UsuarioCreacion?.UsuarioId == usuarioLogueado.UsuarioId && propuesta.FechaHoraCreacion.HasValue)
                {
                    logDeAcciones.Add(new AccionUsuario
                    {
                        PropuestaId = propuesta.PropuestaId.Value,
                        FechaAccion = propuesta.FechaHoraCreacion.Value,
                        TipoAccion = "Creación",
                        Estado = propuesta.Estado,
                        NumFacturas = propuesta.DetallesPropuesta.Count
                    });
                }
                // Acción de Modificación
                if (propuesta.UsuarioModificacion?.UsuarioId == usuarioLogueado.UsuarioId && propuesta.FechaHoraModificacion.HasValue)
                {
                    logDeAcciones.Add(new AccionUsuario
                    {
                        PropuestaId = propuesta.PropuestaId.Value,
                        FechaAccion = propuesta.FechaHoraModificacion.Value,
                        TipoAccion = "Modificación",
                        Estado = propuesta.Estado,
                        NumFacturas = propuesta.DetallesPropuesta.Count
                    });
                }
                // Acción de Eliminación
                if (propuesta.UsuarioEliminacion?.UsuarioId == usuarioLogueado.UsuarioId && propuesta.FechaEliminacion.HasValue)
                {
                    logDeAcciones.Add(new AccionUsuario
                    {
                        PropuestaId = propuesta.PropuestaId.Value,
                        FechaAccion = propuesta.FechaEliminacion.Value,
                        TipoAccion = "Eliminación",
                        Estado = "Eliminada",
                        NumFacturas = propuesta.DetallesPropuesta.Count
                    });
                }
            }

            // Ordenar por la fecha más reciente y mostrar en la tabla
            gvUltimasAcciones.DataSource = logDeAcciones.OrderByDescending(a => a.FechaAccion).Take(10).ToList();
            gvUltimasAcciones.DataBind();
        }

        // Helpers para los badges de colores
        protected string GetActionClass(string accion)
        {
            switch (accion?.ToUpper())
            {
                case "CREACIÓN": return "bg-success";
                case "MODIFICACIÓN": return "bg-info text-dark"; // Azul para modificación
                case "ELIMINACIÓN": return "bg-danger";
                default: return "bg-secondary";
            }
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