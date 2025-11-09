using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface TiposDeCambioDAO
    {
        int Insertar(TiposDeCambioDTO tipoDeCambio);
        int Modificar(TiposDeCambioDTO tipoDeCambio);
        int Eliminar(TiposDeCambioDTO tipoDeCambio);
        TiposDeCambioDTO ObtenerPorId(int tipoDeCambioId);
        IList<TiposDeCambioDTO> ListarTodos();
    }
}
