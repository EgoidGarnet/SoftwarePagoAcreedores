using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class PropuestasPagoDAOImpl : PropuestasPagoDAO
    {
        private BindingList<PropuestasPagoDTO> propuestasPago = new BindingList<PropuestasPagoDTO>();

        public PropuestasPagoDAOImpl()
        {
            propuestasPago.Add(new PropuestasPagoDTO {
                PropuestaId = 1,
                DetallesPropuesta = new BindingList<DetallesPropuestaDTO> {
                    new DetallesPropuestaDTO(1,100.0m,'T',null,null,null,null),
                    new DetallesPropuestaDTO(2,120.0m,'T',null,null,null,null),
                },
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
            propuestasPago.Add(new PropuestasPagoDTO {
                PropuestaId = 2,
                DetallesPropuesta = new BindingList<DetallesPropuestaDTO> {
                    new DetallesPropuestaDTO(1,100.0m,'T',null,null,null,null),
                    new DetallesPropuestaDTO(2,120.0m,'T',null,null,null,null),
                },
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
        }

        public int Insertar(PropuestasPagoDTO propuestaPago)
        {
            if (propuestaPago.PropuestaId == null || propuestasPago.Any(p => p.PropuestaId == propuestaPago.PropuestaId))
                return 0;
            propuestasPago.Add(propuestaPago);
            return 1;
        }
        public int Modificar(PropuestasPagoDTO propuestaPago)
        {
            var existing = propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPago.PropuestaId);
            if (existing == null)
                return 0;
            int idx = propuestasPago.IndexOf(existing);
            propuestasPago[idx] = propuestaPago;
            return 1;
        }
        public int Eliminar(PropuestasPagoDTO propuestaPago)
        {
            var existing = propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPago.PropuestaId);
            if (existing == null)
                return 0;
            propuestasPago.Remove(existing);
            return 1;
        }
        public int EliminarLogico(PropuestasPagoDTO propuestaPago)
        {
            var existing = propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPago.PropuestaId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = propuestaPago.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public PropuestasPagoDTO ObtenerPorId(int propuestaPagoId)
        {
            return propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPagoId);
        }
        public IList<PropuestasPagoDTO> ListarTodos()
        {
            return propuestasPago.ToList();
        }
    }
}
