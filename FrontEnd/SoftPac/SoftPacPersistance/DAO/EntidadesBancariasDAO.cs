using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface EntidadesBancariasDAO
    {
        int Insertar(EntidadesBancariasDTO entidadBancaria);
        int Modificar(EntidadesBancariasDTO entidadBancaria);
        int Eliminar(EntidadesBancariasDTO entidadBancaria);
        EntidadesBancariasDTO ObtenerPorId(int entidadBancariaId);
        IList<EntidadesBancariasDTO> ListarTodos();
    }
}
