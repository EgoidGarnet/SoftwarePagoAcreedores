using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class TiposDeCambioDTO
    {
        private int? tipoCambioId;
        private DateTime? fecha;
        private decimal? tasaDeCambio;
        private MonedasDTO monedaOrigen;
        private MonedasDTO monedaDestino;

        public TiposDeCambioDTO()
        {
            this.MonedaOrigen = null;
            this.MonedaDestino = null;
            this.TipoCambioId = null;
            this.Fecha = null;
            this.TasaDeCambio = null;
        }

        public TiposDeCambioDTO(int? tipo_cambio_id, DateTime? fecha, decimal? tasa_de_cambio)
        {
            this.TipoCambioId = tipo_cambio_id;
            this.Fecha = fecha;
            this.TasaDeCambio = tasa_de_cambio;
            this.MonedaOrigen = null;
            this.MonedaDestino = null;
        }
        public TiposDeCambioDTO(int? tipo_cambio_id, DateTime? fecha, decimal? tasa_de_cambio, MonedasDTO moneda_origen, MonedasDTO moneda_destino)
        {
            this.TipoCambioId = tipo_cambio_id;
            this.Fecha = fecha;
            this.TasaDeCambio = tasa_de_cambio;
            this.MonedaOrigen = moneda_origen;
            this.MonedaDestino = moneda_destino;
        }

        public TiposDeCambioDTO(TiposDeCambioDTO other)
        {
            this.TipoCambioId = other.TipoCambioId;
            this.Fecha = other.Fecha;
            this.TasaDeCambio = other.TasaDeCambio;
            this.MonedaOrigen = new MonedasDTO(other.MonedaOrigen);
            this.MonedaDestino = new MonedasDTO(other.MonedaDestino);
        }


        public int? TipoCambioId { get => tipoCambioId; set => tipoCambioId = value; }
        public DateTime? Fecha { get => fecha; set => fecha = value; }
        public decimal? TasaDeCambio { get => tasaDeCambio; set => tasaDeCambio = value; }
        public MonedasDTO MonedaOrigen { get => monedaOrigen; set => monedaOrigen = value; }
        public MonedasDTO MonedaDestino { get => monedaDestino; set => monedaDestino = value; }
    }
}
