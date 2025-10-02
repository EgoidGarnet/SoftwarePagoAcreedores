using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class EntidadesBancariasDTO
    {
        private int? entidadBancariaId;
        private String nombre;
        private String formatoAceptado;
        private String codigoSwift;
        private PaisesDTO pais;

        public int? EntidadBancariaId { get => entidadBancariaId; set => entidadBancariaId = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string FormatoAceptado { get => formatoAceptado; set => formatoAceptado = value; }
        public string CodigoSwift { get => codigoSwift; set => codigoSwift = value; }
        public PaisesDTO Pais { get => pais; set => pais = value; }

        public EntidadesBancariasDTO()
        {
            Pais = null;
            EntidadBancariaId = null;
            Nombre = null;
            FormatoAceptado = null;
            CodigoSwift = null;
        }

        public EntidadesBancariasDTO(int entidadBancariaId, string nombre, string formatoAceptado, string codigoSwift, PaisesDTO pais)
        {
            this.EntidadBancariaId = entidadBancariaId;
            this.Nombre = nombre;
            this.FormatoAceptado = formatoAceptado;
            this.CodigoSwift = codigoSwift;
            this.Pais = new PaisesDTO(pais);
        }

        public EntidadesBancariasDTO(EntidadesBancariasDTO other)
        {
            this.EntidadBancariaId = other.EntidadBancariaId;
            this.Nombre = other.Nombre;
            this.FormatoAceptado = other.FormatoAceptado;
            this.CodigoSwift = other.CodigoSwift;
            this.Pais = new PaisesDTO(other.Pais);
        }


    }
}
