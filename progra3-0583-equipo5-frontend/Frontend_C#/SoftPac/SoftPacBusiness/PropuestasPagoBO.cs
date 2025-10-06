using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class PropuestasPagoBO
    {
        private PropuestasPagoDAO propuestasDAO;

        public PropuestasPagoBO()
        {
            this.propuestasDAO = new PropuestasPagoDAOImpl();
        }

        public IList<PropuestasPagoDTO> ListarUltimasPorUsuario(int usuarioId, int cantidad)
        {
            var todas = propuestasDAO.ListarTodos();
            return todas.Where(p => p.UsuarioCreacion != null && p.UsuarioCreacion.UsuarioId == usuarioId)
                        .OrderByDescending(p => p.FechaHoraCreacion)
                        .Take(cantidad)
                        .ToList();
        }

        // --- MÉTODO NUEVO ---
        public IList<PropuestasPagoDTO> ListarTodasPorUsuario(int usuarioId)
        {
            var todas = propuestasDAO.ListarTodos();
            return todas.Where(p => p.UsuarioCreacion != null && p.UsuarioCreacion.UsuarioId == usuarioId).ToList();
        }

        // --- MÉTODO NUEVO ---
        public IList<PropuestasPagoDTO> ListarActividadPorUsuario(int usuarioId)
        {
            // Llama al nuevo método del DAO que no filtra por borrado lógico
            return propuestasDAO.ListarTodaActividad()
                .Where(p =>
                    (p.UsuarioCreacion?.UsuarioId == usuarioId) ||
                    (p.UsuarioModificacion?.UsuarioId == usuarioId) ||
                    (p.UsuarioEliminacion?.UsuarioId == usuarioId)
                ).ToList();
        }
    }
}
