using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class CuentasAcreedorDAOImpl : CuentasAcreedorDAO
    {
        private static BindingList<CuentasAcreedorDTO> cuentas = new BindingList<CuentasAcreedorDTO>();
        private static int IdCuenta = 3;

        static CuentasAcreedorDAOImpl()
        {
            if (cuentas.Count > 0) return;

            cuentas.Add(new CuentasAcreedorDTO(
                1, "Corriente", "CCI002", true, "1234567890",
                new MonedasDTO(1, "Dólar", "USD", "$"),
                new EntidadesBancariasDTO(1, "Banco Uno", "PDF", "BANCOCR1", new PaisesDTO(1, "Costa Rica", "CR", "+506")),
                new AcreedoresDTO(1, "Proveedor Uno", "123456789", "Calle 1", "Contado", 30, true, new PaisesDTO(1, "Mexico", "MX", "+56"))
            ));

            cuentas.Add(new CuentasAcreedorDTO(
                2, "Corriente", "CCI003", true, "1234567890",
                new MonedasDTO(1, "Dólar", "USD", "$"),
                new EntidadesBancariasDTO(1, "Banco Uno", "PDF", "BANCOCR1", new PaisesDTO(1, "Costa Rica", "CR", "+506")),
                new AcreedoresDTO(1, "Proveedor Uno", "123456789", "Calle 1", "Contado", 30, true, new PaisesDTO(2, "Colombia", "CO", "+50"))
            ));
        }

        public int Insertar(CuentasAcreedorDTO cuenta)
        {
            if (cuenta == null) return 0;
            cuenta.CuentaBancariaId = IdCuenta++;
            cuentas.Add(cuenta);
            return 1;
        }

        public int Modificar(CuentasAcreedorDTO cuenta)
        {
            var existing = cuentas.FirstOrDefault(c => c.CuentaBancariaId == cuenta.CuentaBancariaId);
            if (existing == null) return 0;
            int idx = cuentas.IndexOf(existing);
            cuentas[idx] = cuenta;
            return 1;
        }

        public int eliminar(CuentasAcreedorDTO cuenta)
        {
            var existing = cuentas.FirstOrDefault(c => c.CuentaBancariaId == cuenta.CuentaBancariaId);
            if (existing == null) return 0;
            cuentas.Remove(existing);
            return 1;
        }

        public int eliminarLogico(CuentasAcreedorDTO cuenta)
        {
            var existing = cuentas.FirstOrDefault(c => c.CuentaBancariaId == cuenta.CuentaBancariaId);
            if (existing == null) return 0;
            if (existing.Activa)
            {
                existing.Activa = false;
                existing.UsuarioEliminacion = cuenta.UsuarioEliminacion;
                existing.FechaEliminacion = DateTime.Now;
            }
            else
            {
                existing.Activa = true;
                existing.UsuarioEliminacion = null;
                existing.FechaEliminacion = null;
            }
            return 1;
        }

        public CuentasAcreedorDTO ObtenerPorId(int cuentaAcreedorId)
        {
            return cuentas.FirstOrDefault(c => c.CuentaBancariaId == cuentaAcreedorId);
        }

        public IList<CuentasAcreedorDTO> ObtenerPorAcreedor(int acreedorId)
        {
            return cuentas
                .Where(c => c.Acreedor != null && c.Acreedor.AcreedorId == acreedorId)
                .OrderBy(c => c.CuentaBancariaId)
                .ToList();
        }

        public IList<CuentasAcreedorDTO> ListarTodos()
        {
            return cuentas.Where(c => c.UsuarioEliminacion == null).ToList();
        }
    }
}
