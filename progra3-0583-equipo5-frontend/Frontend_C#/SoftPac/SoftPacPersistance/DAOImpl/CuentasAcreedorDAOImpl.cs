using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class CuentasAcreedorDAOImpl : CuentasAcreedorDAO
    {
        private BindingList<CuentasAcreedorDTO> cuentasAcreedor = new BindingList<CuentasAcreedorDTO>();

        public CuentasAcreedorDAOImpl()
        {
            cuentasAcreedor.Add(new CuentasAcreedorDTO(
                1, "Corriente", "CCI002", true, "1234567890",
                new MonedasDTO(1, "Dólar","USD","$"),
                new EntidadesBancariasDTO(1, "Banco Uno", "PDF", "BANCOCR1", new PaisesDTO(1, "Costa Rica", "CR", "+506")),
                new AcreedoresDTO(1, "Proveedor Uno", "123456789", "Calle 1", "Contado", 30, true, new PaisesDTO(1, "Mexico", "MX", "+56"))
            ));
            cuentasAcreedor.Add(new CuentasAcreedorDTO(1, "Corriente", "CCI003", true, "1234567890",
                new MonedasDTO(1, "Dólar", "USD", "$"),
                new EntidadesBancariasDTO(1, "Banco Uno", "PDF", "BANCOCR1", new PaisesDTO(1, "Costa Rica", "CR", "+506")),
                new AcreedoresDTO(1, "Proveedor Uno", "123456789", "Calle 1", "Contado", 30, true, new PaisesDTO(2, "Colombia", "CO", "+50"))
            ));
        }

        public int Insertar(CuentasAcreedorDTO cuentaAcreedor)
        {
            if (cuentaAcreedor.CuentaBancariaId == null || cuentasAcreedor.Any(c => c.CuentaBancariaId == cuentaAcreedor.CuentaBancariaId))
                return 0;
            cuentasAcreedor.Add(cuentaAcreedor);
            return 1;
        }
        public int Modificar(CuentasAcreedorDTO cuentaAcreedor)
        {
            var existing = cuentasAcreedor.FirstOrDefault(c => c.CuentaBancariaId == cuentaAcreedor.CuentaBancariaId);
            if (existing == null)
                return 0;
            int idx = cuentasAcreedor.IndexOf(existing);
            cuentasAcreedor[idx] = cuentaAcreedor;
            return 1;
        }
        public int eliminar(CuentasAcreedorDTO cuentaAcreedor)
        {
            var existing = cuentasAcreedor.FirstOrDefault(c => c.CuentaBancariaId == cuentaAcreedor.CuentaBancariaId);
            if (existing == null)
                return 0;
            cuentasAcreedor.Remove(existing);
            return 1;
        }
        public int eliminarLogico(CuentasAcreedorDTO cuentaAcreedor)
        {
            var existing = cuentasAcreedor.FirstOrDefault(c => c.CuentaBancariaId == cuentaAcreedor.CuentaBancariaId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = cuentaAcreedor.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public CuentasAcreedorDTO ObtenerPorId(int cuentaAcreedorId)
        {
            return cuentasAcreedor.FirstOrDefault(c => c.CuentaBancariaId == cuentaAcreedorId);
        }
        public IList<CuentasAcreedorDTO> ListarTodos()
        {
            return cuentasAcreedor.ToList();
        }
    }
}
