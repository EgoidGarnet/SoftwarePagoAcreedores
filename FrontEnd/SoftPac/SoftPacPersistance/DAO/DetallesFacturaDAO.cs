using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface DetallesFacturaDAO
    {
        int Insertar(DetallesFacturaDTO detalleFactura);
        int Modificar(DetallesFacturaDTO detalleFactura);
        int Eliminar(DetallesFacturaDTO detalleFactura);
        int EliminarLogico(DetallesFacturaDTO detalleFactura);
        DetallesFacturaDTO ObtenerPorId(int detalleFacturaId);
        IList<DetallesFacturaDTO> ListarTodos();
    }
}
