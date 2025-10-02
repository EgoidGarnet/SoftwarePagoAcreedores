using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;
using System.ComponentModel;

namespace SoftPac.Persistance.DAO
{
    public class MonedasDAOImpl : MonedasDAO
    {
        private BindingList<MonedasDTO> monedas = new BindingList<MonedasDTO>();

        public MonedasDAOImpl()
        {
            monedas.Add(new MonedasDTO
            {
                MonedaId = 1,
                Nombre = "Dólar",
                CodigoIso = "USD",
                Simbolo = "$"
            });
            monedas.Add(new MonedasDTO {
                MonedaId = 2,
                Nombre = "Peso Mexicano",
                CodigoIso = "MXN",
                Simbolo = "$"
            });
        }

        public int Insertar(MonedasDTO moneda)
        {
            if (moneda.MonedaId == null || monedas.Any(m => m.MonedaId == moneda.MonedaId))
                return 0;
            monedas.Add(moneda);
            return 1;
        }
        public int Modificar(MonedasDTO moneda)
        {
            var existing = monedas.FirstOrDefault(m => m.MonedaId == moneda.MonedaId);
            if (existing == null)
                return 0;
            int idx = monedas.IndexOf(existing);
            monedas[idx] = moneda;
            return 1;
        }
        public int eliminar(MonedasDTO moneda)
        {
            var existing = monedas.FirstOrDefault(m => m.MonedaId == moneda.MonedaId);
            if (existing == null)
                return 0;
            monedas.Remove(existing);
            return 1;
        }
        public MonedasDTO ObtenerPorId(int monedaId)
        {
            return monedas.FirstOrDefault(m => m.MonedaId == monedaId);
        }
        public IList<MonedasDTO> ListarTodos()
        {
            return monedas.ToList();
        }
    }
}
