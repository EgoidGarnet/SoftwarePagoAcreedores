using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface PaisesDAO
    {
        int Insertar(PaisesDTO pais);
        int Modificar(PaisesDTO pais);
        int Eliminar(PaisesDTO pais);
        PaisesDTO ObtenerPorId(int paisId);
        IList<PaisesDTO> ListarTodos();
    }
}
