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

            // Debug en la ventana de salida de Visual Studio
            foreach (var factura in facturas)
            {
                System.Diagnostics.Debug.WriteLine("Factura encontrada: " + factura.NumeroFactura);
            }

            return facturas;
        }

        public FacturasDTO ObtenerPorId(int facturaId)
        {
            return this.facturaDAO.ObtenerPorId(facturaId);
        }

        public int Insertar(FacturasDTO factura)
        {
            return this.facturaDAO.Insertar(factura);
        }

        public int Modificar(FacturasDTO factura)
        {
            return this.facturaDAO.Modificar(factura);
        }

        public int Eliminar(FacturasDTO factura)
        {
            return this.facturaDAO.eliminarLogico(factura);
        }

        public int Eliminar(int facturaId)
        {
            FacturasDTO factura = new FacturasDTO();
            factura.FacturaId = facturaId;
            return Eliminar(factura);
        }
    }
}
