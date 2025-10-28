using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class CuentasPropiasDAOImpl : CuentasPropiasDAO
    {
        private static BindingList<CuentasPropiasDTO> cuentasPropias = new BindingList<CuentasPropiasDTO>();

        static CuentasPropiasDAOImpl()
        {
            if (cuentasPropias.Any()) return; // Inicializar solo una vez

            var entidadesDAO = new EntidadesBancariasDAOImpl();
            var monedasDAO = new MonedasDAOImpl();

            var bcp = entidadesDAO.ObtenerPorId(1); // Banco de Crédito del Perú
            var interbank = entidadesDAO.ObtenerPorId(2); // Interbank
            var bbva = entidadesDAO.ObtenerPorId(3); // BBVA Perú

            var soles = monedasDAO.ObtenerPorId(3); // Sol Peruano
            var dolares = monedasDAO.ObtenerPorId(1); // Dólar
            var pesosMexicanos = monedasDAO.ObtenerPorId(2); // Peso Mexicano
            var pesosColombianos = monedasDAO.ObtenerPorId(4); // Peso Colombiano

            var bancolombia = entidadesDAO.ObtenerPorId(3); // Bancolombia
            var bancoBogota = entidadesDAO.ObtenerPorId(4); // Banco de Bogotá
            var davivienda = entidadesDAO.ObtenerPorId(5); // Davivienda
            var bbvaColombia = entidadesDAO.ObtenerPorId(6); // BBVA Colombia

            var bbvaMexico = entidadesDAO.ObtenerPorId(7); // BBVA México
            var santanderMexico = entidadesDAO.ObtenerPorId(8); // Santander México
            var banorte = entidadesDAO.ObtenerPorId(9); // Banorte
            var citibanamex = entidadesDAO.ObtenerPorId(10); // Citibanamex

            cuentasPropias.Add(new CuentasPropiasDTO(1, "Ahorros Soles", "002193000000000001", true, "193-12345678-1-01", soles, bcp, 50000.75M));
            cuentasPropias.Add(new CuentasPropiasDTO(2, "Corriente Dólares", "003194000000000002", true, "194-87654321-1-02", dolares, bcp, 12500.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(3, "Corriente Soles", "004191000000000003", true, "191-11223344-1-03", soles, interbank, 120300.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(4, "Maestra Dólares", "005192000000000004", false, "192-44556677-1-04", dolares, interbank, 7800.50M));
            cuentasPropias.Add(new CuentasPropiasDTO(5, "Ahorros Soles", "006111000000000005", true, "111-99887766-1-05", soles, bbva, 9500.20M));
            cuentasPropias.Add(new CuentasPropiasDTO(6, "Corriente Dólares", "007222000000000006", true, "222-12312312-1-06", dolares, bbva, 33250.00M));

            cuentasPropias.Add(new CuentasPropiasDTO(7, "Corriente Pesos COP", "008310000000000007", true, "310-55555555-1-07", pesosColombianos, bancolombia, 98500000.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(8, "Ahorros Pesos COP", "009311000000000008", true, "311-22223333-1-08", pesosColombianos, bancolombia, 40200000.50M));
            cuentasPropias.Add(new CuentasPropiasDTO(9, "Empresarial Pesos COP", "010312000000000009", true, "312-11114444-1-09", pesosColombianos, bancoBogota, 17500000.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(10, "Maestra COP", "011313000000000010", false, "313-99990000-1-10", pesosColombianos, davivienda, 6850000.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(11, "Corporativa USD", "012314000000000011", true, "314-44448888-1-11", dolares, bbvaColombia, 22500000.00M));
            
            cuentasPropias.Add(new CuentasPropiasDTO(12, "Ahorros MXN", "013410000000000012", true, "410-12345678-1-12", dolares, bbvaMexico, 1500000.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(13, "Corriente MXN", "014411000000000013", true, "411-87654321-1-13", pesosMexicanos, santanderMexico, 875000.25M));
            cuentasPropias.Add(new CuentasPropiasDTO(14, "Empresarial MXN", "015412000000000014", true, "412-55554444-1-14", pesosMexicanos, banorte, 1275000.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(15, "Dólares USA", "016413000000000015", true, "413-22221111-1-15", dolares, banorte, 52000.00M));
            cuentasPropias.Add(new CuentasPropiasDTO(16, "Cuenta General MXN", "017414000000000016", true, "414-66667777-1-16", pesosMexicanos, citibanamex, 930000.80M));

        }

        public int Insertar(CuentasPropiasDTO cuentaPropia)
        {
            int nuevoId = (cuentasPropias.Any() ? cuentasPropias.Max(c => c.CuentaBancariaId.Value) : 0) + 1;
            cuentaPropia.CuentaBancariaId = nuevoId;
            cuentasPropias.Add(cuentaPropia);
            return 1;
        }

        public int Modificar(CuentasPropiasDTO cuentaPropia)
        {
            var existing = cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropia.CuentaBancariaId);
            if (existing == null) return 0;

            var entidad = new EntidadesBancariasDAOImpl().ObtenerPorId(cuentaPropia.EntidadBancaria.EntidadBancariaId.Value);
            var moneda = new MonedasDAOImpl().ObtenerPorId(cuentaPropia.Moneda.MonedaId.Value);

            existing.EntidadBancaria = entidad;
            existing.Moneda = moneda;
            existing.TipoCuenta = cuentaPropia.TipoCuenta;
            existing.NumeroCuenta = cuentaPropia.NumeroCuenta;
            existing.Cci = cuentaPropia.Cci;
            existing.SaldoDisponible = cuentaPropia.SaldoDisponible;
            existing.Activa = cuentaPropia.Activa;
            return 1;
        }

        public int eliminarLogico(CuentasPropiasDTO cuentaPropia)
        {
            var existing = cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropia.CuentaBancariaId);
            if (existing == null) return 0;

            existing.FechaEliminacion = DateTime.Now;
            existing.UsuarioEliminacion = cuentaPropia.UsuarioEliminacion;
            return 1;
        }

        public IList<CuentasPropiasDTO> ListarTodos()
        {
            return cuentasPropias.Where(c => c.FechaEliminacion == null).ToList();
        }

        public CuentasPropiasDTO ObtenerPorId(int cuentaPropiaId)
        {
            return cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropiaId);
        }

        public int eliminar(CuentasPropiasDTO cuentaPropia)
        {
            // Implementación para cumplir con la interfaz, aunque usamos borrado lógico.
            var itemToRemove = cuentasPropias.FirstOrDefault(c => c.CuentaBancariaId == cuentaPropia.CuentaBancariaId);
            if (itemToRemove != null)
            {
                cuentasPropias.Remove(itemToRemove);
                return 1;
            }
            return 0;
        }
    }
}
