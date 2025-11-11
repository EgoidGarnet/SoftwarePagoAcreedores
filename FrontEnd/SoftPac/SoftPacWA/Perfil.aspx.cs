using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
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
        private usuariosDTO usuarioLogueado;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            usuarioLogueado = (usuariosDTO)Session["UsuarioLogueado"];

            if (!IsPostBack)
            {
                CargarDatosPerfil();
            }
        }

        private void CargarDatosPerfil()
        {
            litNombreCompleto.Text = Server.HtmlEncode(usuarioLogueado.nombre + " " + usuarioLogueado.apellidos);
            litCorreo.Text = Server.HtmlEncode(usuarioLogueado.correo_electronico);

            var paisesConAcceso = usuarioLogueado.usuario_pais.Where(up => up.acceso).Select(up => up.pais).ToList();
            rptPaisesAcceso.DataSource = paisesConAcceso;
            rptPaisesAcceso.DataBind();

            CargarActividadReciente();
        }

        private void CargarActividadReciente()
        {
            // Obtenemos todas las propuestas donde el usuario participó (creó, modificó o eliminó)
            var propuestasDelUsuario = propuestasBO.ListarActividadPorUsuario(usuarioLogueado.usuario_id);

            // 1. Calcular estadísticas (lógica sin cambios)
            int propuestasEsteMes = propuestasDelUsuario.Count(p => p.fecha_hora_creacionSpecified && p.fecha_hora_creacion.Month == DateTime.Now.Month && p.fecha_hora_creacion.Year == DateTime.Now.Year);
            litPropuestasMes.Text = propuestasEsteMes.ToString();

            var ultimaPropuesta = propuestasDelUsuario.OrderByDescending(p => p.fecha_hora_creacion).FirstOrDefault();
            if (ultimaPropuesta != null && ultimaPropuesta.entidad_bancaria!=null && ultimaPropuesta.entidad_bancaria.pais!=null)
            {
                litUltimoPais.Text = ultimaPropuesta.entidad_bancaria.pais.nombre;
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
                if (propuesta.usuario_creacion?.usuario_id == usuarioLogueado.usuario_id && propuesta.fecha_hora_creacionSpecified)
                {
                    logDeAcciones.Add(new AccionUsuario
                    {
                        PropuestaId = propuesta.propuesta_id,
                        FechaAccion = propuesta.fecha_hora_creacion,
                        TipoAccion = "Creación",
                        Estado = propuesta.estado,
                        NumFacturas = propuesta.detalles_propuesta.Length
                    });
                }
                // Acción de Modificación
                if (propuesta.usuario_modificacion?.usuario_id == usuarioLogueado.usuario_id && propuesta.fecha_hora_modificacionSpecified)
                {
                    logDeAcciones.Add(new AccionUsuario
                    {
                        PropuestaId = propuesta.propuesta_id,
                        FechaAccion = propuesta.fecha_hora_modificacion,
                        TipoAccion = "Modificación",
                        Estado = propuesta.estado,
                        NumFacturas = propuesta.detalles_propuesta.Length
                    });
                }
                // Acción de Eliminación
                if (propuesta.usuario_eliminacion?.usuario_id == usuarioLogueado.usuario_id && propuesta.fecha_eliminacionSpecified)
                {
                    logDeAcciones.Add(new AccionUsuario
                    {
                        PropuestaId = propuesta.propuesta_id,
                        FechaAccion = propuesta.fecha_eliminacion,
                        TipoAccion = "Eliminación",
                        Estado = "Eliminada",
                        NumFacturas = propuesta.detalles_propuesta.Length
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
                case "ANULADA": return "bg-danger";
                default: return "bg-light text-dark";
            }
        }
    }
}