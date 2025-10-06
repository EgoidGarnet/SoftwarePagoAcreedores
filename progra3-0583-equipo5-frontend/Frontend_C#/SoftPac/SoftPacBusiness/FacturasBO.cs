using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class FacturasBO
    {
        FacturasDAO facturaDAO;

        public FacturasBO()
        {
            this.facturaDAO = new FacturasDAOImpl();
        }
        public BindingList<FacturasDTO> ListarTodos()
        {
            BindingList<FacturasDTO> facturas = (BindingList<FacturasDTO>)this.facturaDAO.ListarTodos();

            foreach (var factura in facturas)
            {
                System.Diagnostics.Debug.WriteLine("Factura encontrada: " + factura.NumeroFactura);
            }

            return facturas;
        }

        public int Eliminar(FacturasDTO factura)
        {
            return this.facturaDAO.eliminarLogico(factura);
        }

        public int Eliminar(int facturaId, UsuariosDTO usuario)
        {
            FacturasDTO factura = new FacturasDTO();
            factura.FacturaId = facturaId;
            factura.UsuarioEliminacion= usuario;
            factura.FechaEliminacion = DateTime.Now;
            return Eliminar(factura);
        }

        public int InsertarDetalle(DetallesFacturaDTO detalle)
        {
            return this.facturaDAO.Modificar(detalle.Factura);
        }

        public int ModificarDetalle(DetallesFacturaDTO detalle)
        {
            return this.facturaDAO.Modificar(detalle.Factura);
        }

        public int EliminarDetalle(DetallesFacturaDTO detalle,UsuariosDTO usuario)
        {
            detalle.UsuarioEliminacion = usuario;
            detalle.FechaEliminacion = DateTime.Now;
            return this.facturaDAO.Modificar(detalle.Factura);
        }

        public FacturasDTO ObtenerPorId(int facturaId)
        {
            return facturaDAO.ObtenerPorId(facturaId);
        }

        public int Modificar(FacturasDTO factura)
        {
            return facturaDAO.Modificar(factura);
        }

        public int Insertar(FacturasDTO factura)
        {
            return facturaDAO.Insertar(factura);
        }

    }
}
