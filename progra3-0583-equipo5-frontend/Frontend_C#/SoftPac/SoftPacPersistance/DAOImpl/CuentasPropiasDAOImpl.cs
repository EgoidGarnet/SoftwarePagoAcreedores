using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class CuentasPropiasDAOImpl : CuentasPropiasDAO
    {
        private BindingList<CuentasPropiasDTO> cuentasPropias = new BindingList<CuentasPropiasDTO>();

        public CuentasPropiasDAOImpl()
        {
            cuentasPropias.Add(new CuentasPropiasDTO(
                1, "Corriente", "CCI001", true, "111222333",
                new MonedasDTO(1, "Dólar","USD","$"), // Example Moneda
                new EntidadesBancariasDTO(1, "Banco Uno", "PDF", "BANCOCR1", new PaisesDTO(1, "Mexico", "MX", "+56")),
                10000.50M
            ));
            cuentasPropias.Add(new CuentasPropiasDTO(
                1, "Corriente", "CCI002", true, "111222333",
                new MonedasDTO(1, "Dólar", "USD", "$"), // Example Moneda
                new EntidadesBancariasDTO(1, "Banco Uno", "PDF", "BANCOCR1", new PaisesDTO(2, "Colombia", "CO", "+50")),
                10000.50M
            ));
        }

        public int Insertar(CuentasPropiasDTO cuentaPropia)
        {
            if (cuentaPropia.CuentaBancariaId == null || cuentasPropias.Any(c => c.CuentaBancariaId == cuentaPropia.CuentaBancariaId))
                return 0;
            cuentasPropias.Add(cuentaPropia);
            return 1;
        }
        public int Modificar(CuentasPropiasDTO cuentaPropia)
        {
            var existing = cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropia.CuentaBancariaId);
            if (existing == null)
                return 0;
            int idx = cuentasPropias.IndexOf(existing);
            cuentasPropias[idx] = cuentaPropia;
            return 1;
        }
        public int eliminar(CuentasPropiasDTO cuentaPropia)
        {
            var existing = cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropia.CuentaBancariaId);
            if (existing == null)
                return 0;
            cuentasPropias.Remove(existing);
            return 1;
        }
        public int eliminarLogico(CuentasPropiasDTO cuentaPropia)
        {
            var existing = cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropia.CuentaBancariaId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = cuentaPropia.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public CuentasPropiasDTO ObtenerPorId(int cuentaPropiaId)
        {
            return cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropiaId);
        }
        public IList<CuentasPropiasDTO> ListarTodos()
        {
            return cuentasPropias.ToList();
        }
    }
}
