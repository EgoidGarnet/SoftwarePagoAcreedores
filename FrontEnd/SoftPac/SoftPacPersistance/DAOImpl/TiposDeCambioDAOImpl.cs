using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class TiposDeCambioDAOImpl : TiposDeCambioDAO
    {
        private BindingList<TiposDeCambioDTO> tiposDeCambio = new BindingList<TiposDeCambioDTO>();

        public TiposDeCambioDAOImpl()
        {
            tiposDeCambio.Add(new TiposDeCambioDTO {
                TipoCambioId = 1,
                MonedaOrigen = new MonedasDTO {
                    MonedaId = 1,
                    Nombre = "Dólar"
                },
                MonedaDestino = new MonedasDTO
                {
                    MonedaId = 2,
                    Nombre = "Peso Mexicano"
                },
                TasaDeCambio = 540.25m
            });
            tiposDeCambio.Add(new TiposDeCambioDTO {
                TipoCambioId = 2,
                MonedaOrigen = new MonedasDTO
                {
                    MonedaId = 2,
                    Nombre = "Peso Mexicano"
                },
                MonedaDestino = new MonedasDTO
                {
                    MonedaId = 1,
                    Nombre = "Dólar"
                },
                TasaDeCambio = 0.185m
            });
        }

        public int Insertar(TiposDeCambioDTO tipoDeCambio)
        {
            if (tipoDeCambio.TipoCambioId == null || tiposDeCambio.Any(t => t.TipoCambioId == tipoDeCambio.TipoCambioId))
                return 0;
            tiposDeCambio.Add(tipoDeCambio);
            return 1;
        }
        public int Modificar(TiposDeCambioDTO tipoDeCambio)
        {
            var existing = tiposDeCambio.FirstOrDefault(t => t.TipoCambioId == tipoDeCambio.TipoCambioId);
            if (existing == null)
                return 0;
            int idx = tiposDeCambio.IndexOf(existing);
            tiposDeCambio[idx] = tipoDeCambio;
            return 1;
        }
        public int Eliminar(TiposDeCambioDTO tipoDeCambio)
        {
            var existing = tiposDeCambio.FirstOrDefault(t => t.TipoCambioId == tipoDeCambio.TipoCambioId);
            if (existing == null)
                return 0;
            tiposDeCambio.Remove(existing);
            return 1;
        }
        public TiposDeCambioDTO ObtenerPorId(int tipoDeCambioId)
        {
            return tiposDeCambio.FirstOrDefault(t => t.TipoCambioId == tipoDeCambioId);
        }
        public IList<TiposDeCambioDTO> ListarTodos()
        {
            return tiposDeCambio.ToList();
        }
    }
}
