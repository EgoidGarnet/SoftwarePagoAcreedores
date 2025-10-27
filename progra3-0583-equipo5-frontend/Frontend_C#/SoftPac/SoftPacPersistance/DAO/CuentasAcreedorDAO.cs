using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public interface CuentasAcreedorDAO
    {
        int Insertar(CuentasAcreedorDTO cuentaAcreedor);
        int Modificar(CuentasAcreedorDTO cuentaAcreedor);
        int eliminar(CuentasAcreedorDTO cuentaAcreedor);
        int eliminarLogico(CuentasAcreedorDTO cuentaAcreedor);
        CuentasAcreedorDTO ObtenerPorId(int cuentaAcreedorId);
        IList<CuentasAcreedorDTO> ObtenerPorAcreedor(int acreedorId);
        IList<CuentasAcreedorDTO> ListarTodos();
    }
}
