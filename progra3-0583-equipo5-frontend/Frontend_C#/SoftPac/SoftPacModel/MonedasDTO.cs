using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class MonedasDTO
    {
        private int? monedaId;
        private String nombre;
        private String codigoIso;
        private String simbolo;
        private BindingList<TiposDeCambioDTO> tiposCambio;

       
        public MonedasDTO()
        {
            this.TiposCambio = new BindingList<TiposDeCambioDTO>();
            MonedaId = null;
            Nombre = null;
            CodigoIso = null;
            Simbolo = null;
        }
        public MonedasDTO(int moneda_id, string nombre, string codigo_iso, string simbolo)
        {
            this.MonedaId = moneda_id;
            this.Nombre = nombre;
            this.CodigoIso = codigo_iso;
            this.Simbolo = simbolo;
        }

        public MonedasDTO(int moneda_id, string nombre, string codigo_iso, string simbolo, BindingList<TiposDeCambioDTO> tipos_cambio)
        {
            this.MonedaId = moneda_id;
            this.Nombre = nombre;
            this.CodigoIso = codigo_iso;
            this.Simbolo = simbolo;
            this.TiposCambio = tipos_cambio;
        }

        public MonedasDTO(MonedasDTO other)
        {
            this.MonedaId = other.MonedaId;
            this.Nombre = other.Nombre;
            this.CodigoIso = other.CodigoIso;
            this.Simbolo = other.Simbolo;
            this.TiposCambio = other.TiposCambio;
        }

       
        public void AddTipoDeCambio(TiposDeCambioDTO tipo)
        {
            if (TiposCambio == null)
            {
                TiposCambio = new BindingList<TiposDeCambioDTO>();
            }
            TiposCambio.Add(tipo);
        }

        public int? MonedaId { get => monedaId; set => monedaId = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string CodigoIso { get => codigoIso; set => codigoIso = value; }
        public string Simbolo { get => simbolo; set => simbolo = value; }
        public BindingList<TiposDeCambioDTO> TiposCambio { get => tiposCambio; set => tiposCambio = value; }


    }

}
