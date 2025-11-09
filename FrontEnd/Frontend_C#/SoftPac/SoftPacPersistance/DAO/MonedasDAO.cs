using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface MonedasDAO
    {
        int Insertar(MonedasDTO moneda);
        int Modificar(MonedasDTO moneda);
        int eliminar(MonedasDTO moneda);
        MonedasDTO ObtenerPorId(int monedaId);
        IList<MonedasDTO> ListarTodos();
    }
}
