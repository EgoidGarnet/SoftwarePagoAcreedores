using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class DetallesFacturaDAOImpl : DetallesFacturaDAO
    {
        private BindingList<DetallesFacturaDTO> detallesFactura = new BindingList<DetallesFacturaDTO>();

        public DetallesFacturaDAOImpl()
        {
            detallesFactura.Add(new DetallesFacturaDTO {
                DetalleFacturaId = 1,
                Descripcion = "Producto A",
                Subtotal = 100.0m,
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
            detallesFactura.Add(new DetallesFacturaDTO {
                DetalleFacturaId = 2,
                Descripcion = "Producto B",
                Subtotal = 200.0m,
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
        }

        public int Insertar(DetallesFacturaDTO detalleFactura)
        {
            if (detalleFactura.DetalleFacturaId == null || detallesFactura.Any(d => d.DetalleFacturaId == detalleFactura.DetalleFacturaId))
                return 0;
            detallesFactura.Add(detalleFactura);
            return 1;
        }
        public int Modificar(DetallesFacturaDTO detalleFactura)
        {
            var existing = detallesFactura.FirstOrDefault(d => d.DetalleFacturaId == detalleFactura.DetalleFacturaId);
            if (existing == null)
                return 0;
            int idx = detallesFactura.IndexOf(existing);
            detallesFactura[idx] = detalleFactura;
            return 1;
        }
        public int Eliminar(DetallesFacturaDTO detalleFactura)
        {
            var existing = detallesFactura.FirstOrDefault(d => d.DetalleFacturaId == detalleFactura.DetalleFacturaId);
            if (existing == null)
                return 0;
            detallesFactura.Remove(existing);
            return 1;
        }
        public int EliminarLogico(DetallesFacturaDTO detalleFactura)
        {
            var existing = detallesFactura.FirstOrDefault(d => d.DetalleFacturaId == detalleFactura.DetalleFacturaId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = detalleFactura.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public DetallesFacturaDTO ObtenerPorId(int detalleFacturaId)
        {
            return detallesFactura.FirstOrDefault(d => d.DetalleFacturaId == detalleFacturaId);
        }
        public IList<DetallesFacturaDTO> ListarTodos()
        {
            return detallesFactura.ToList();
        }
    }
}
