using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface FacturasDAO
    {
        int Insertar(FacturasDTO factura);
        int Modificar(FacturasDTO factura);
        int eliminar(FacturasDTO factura);
        int eliminarLogico(FacturasDTO factura);
        FacturasDTO ObtenerPorId(int facturaId);
        IList<FacturasDTO> ListarTodos();
    }
}
