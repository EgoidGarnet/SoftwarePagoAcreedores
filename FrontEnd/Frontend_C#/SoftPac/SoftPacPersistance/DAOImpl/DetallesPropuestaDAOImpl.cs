using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class DetallesPropuestaDAOImpl : DetallesPropuestaDAO
    {
        private static BindingList<DetallesPropuestaDTO> detallesPropuesta = new BindingList<DetallesPropuestaDTO>();

        static DetallesPropuestaDAOImpl()
        {
            if (detallesPropuesta.Any()) return;

            // Instanciamos los DAOs necesarios para obtener datos reales
            var facturasDAO = new FacturasDAOImpl();
            var propuestasDAO = new PropuestasPagoDAOImpl();

            // Obtenemos facturas y propuestas existentes para asociar
            FacturasDTO factura1 = facturasDAO.ObtenerPorId(1);
            FacturasDTO factura2 = facturasDAO.ObtenerPorId(2);
            PropuestasPagoDTO propuesta1 = propuestasDAO.ObtenerPorId(1);

            // Creamos los detalles asociando los objetos completos
            detallesPropuesta.Add(new DetallesPropuestaDTO(1, 100.0m, 'T', null, propuesta1, null, factura1));
            detallesPropuesta.Add(new DetallesPropuestaDTO(2, 120.0m, 'T', null, propuesta1, null, factura2));
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
            if (existing == null) return 0;
            int idx = detallesPropuesta.IndexOf(existing);
            detallesPropuesta[idx] = detallePropuesta;
            return 1;
        }

        public int eliminarPorPropuesta(int propuestaId)
        {
            var items = detallesPropuesta.Where(d => d.PropuestaPago != null && d.PropuestaPago.PropuestaId == propuestaId).ToList();
            if (!items.Any()) return 0;
            foreach (var item in items)
                detallesPropuesta.Remove(item);
            return items.Count;
        }

        public int eliminarLogico(DetallesPropuestaDTO detallePropuesta)
        {
            var existing = detallesPropuesta.FirstOrDefault(d => d.DetallePropuestaId == detallePropuesta.DetallePropuestaId);
            if (existing == null) return 0;
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