using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface DetallesPropuestaDAO
    {
        int Insertar(DetallesPropuestaDTO detallePropuesta);
        int Modificar(DetallesPropuestaDTO detallePropuesta);
        int eliminarPorPropuesta(int propuestaId);
        int eliminarLogico(DetallesPropuestaDTO detallePropuesta);
        DetallesPropuestaDTO ObtenerPorId(int detallePropuestaId);
    }
}
