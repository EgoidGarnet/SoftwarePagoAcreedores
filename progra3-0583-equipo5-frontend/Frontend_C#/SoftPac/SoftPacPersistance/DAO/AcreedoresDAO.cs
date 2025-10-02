using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface AcreedoresDAO
    {
        int insertar(AcreedoresDTO acreedor);
        int modificar(AcreedoresDTO acreedor);
        int eliminar(AcreedoresDTO acreedor);
        int eliminarLogico(AcreedoresDTO acreedor);
        AcreedoresDTO obtenerPorId(int acreedorId);
        IList<AcreedoresDTO> ListarTodos();

    }
}
