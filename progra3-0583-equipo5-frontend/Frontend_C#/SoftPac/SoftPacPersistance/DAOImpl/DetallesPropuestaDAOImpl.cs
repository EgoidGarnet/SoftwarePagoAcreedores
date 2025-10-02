using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class DetallesPropuestaDAOImpl : DetallesPropuestaDAO
    {
        private BindingList<DetallesPropuestaDTO> detallesPropuesta = new BindingList<DetallesPropuestaDTO>();

        public DetallesPropuestaDAOImpl()
        {
            detallesPropuesta.Add(new DetallesPropuestaDTO(1,100.0m,'T',null,new PropuestasPagoDTO(1,null,DateTime.Now,"P",null,null,null),null,null));
            detallesPropuesta.Add(new DetallesPropuestaDTO(2, 100.0m, 'T', null, new PropuestasPagoDTO(1, null, DateTime.Now, "P", null, null, null), null, null));
        }

        public int Insertar(DetallesPropuestaDTO detallePropuesta)
        {
            if (detallePropuesta.DetallePropuestaId == null || detallesPropuesta.Any(d => d.DetallePropuestaId == detallePropuesta.DetallePropuestaId))
                return 0;
            detallesPropuesta.Add(detallePropuesta);
            return 1;
        }
        public int Modificar(DetallesPropuestaDTO detallePropuesta)
        {
            var existing = detallesPropuesta.FirstOrDefault(d => d.DetallePropuestaId == detallePropuesta.DetallePropuestaId);
            if (existing == null)
                return 0;
            int idx = detallesPropuesta.IndexOf(existing);
            detallesPropuesta[idx] = detallePropuesta;
            return 1;
        }
        public int eliminarPorPropuesta(int propuestaId)
        {
            var items = detallesPropuesta.Where(d => d.PropuestaPago.PropuestaId == propuestaId).ToList();
            if (!items.Any())
                return 0;
            foreach (var item in items)
                detallesPropuesta.Remove(item);
            return items.Count;
        }
        public int eliminarLogico(DetallesPropuestaDTO detallePropuesta)
        {
            var existing = detallesPropuesta.FirstOrDefault(d => d.DetallePropuestaId == detallePropuesta.DetallePropuestaId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = detallePropuesta.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public DetallesPropuestaDTO ObtenerPorId(int detallePropuestaId)
        {
            return detallesPropuesta.FirstOrDefault(d => d.DetallePropuestaId == detallePropuestaId);
        }
    }
}
