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
