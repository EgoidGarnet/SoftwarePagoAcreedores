using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class PaisesDTO
    {
        private int? paisId;
        private String nombre;
        private String codigoIso;
        private String codigoTelefonico;

        public PaisesDTO()
        {
            PaisId= null;
            Nombre= null;
            CodigoIso= null;
            CodigoTelefonico= null;
        }

        public PaisesDTO(int pais_id, string nombre, string codigo_iso, string codigo_telefonico)
        {
            this.PaisId = pais_id;
            this.Nombre = nombre;
            this.CodigoIso = codigo_iso;
            this.CodigoTelefonico = codigo_telefonico;
        }

        public PaisesDTO(PaisesDTO other)
        {
            this.PaisId = other.PaisId;
            this.Nombre = other.Nombre;
            this.CodigoIso = other.CodigoIso;
            this.CodigoTelefonico = other.CodigoTelefonico;
        }

        public int? PaisId { get => paisId; set => paisId = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string CodigoIso { get => codigoIso; set => codigoIso = value; }
        public string CodigoTelefonico { get => codigoTelefonico; set => codigoTelefonico = value; }
    }
}
