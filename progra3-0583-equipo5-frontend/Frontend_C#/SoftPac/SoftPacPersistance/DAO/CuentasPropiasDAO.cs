using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface CuentasPropiasDAO
    {
        int Insertar(CuentasPropiasDTO cuentaPropia);
        int Modificar(CuentasPropiasDTO cuentaPropia);
        int eliminar(CuentasPropiasDTO cuentaPropia);
        int eliminarLogico(CuentasPropiasDTO cuentaPropia);
        CuentasPropiasDTO ObtenerPorId(int cuentaPropiaId);
        IList<CuentasPropiasDTO> ListarTodos();

    }
}
