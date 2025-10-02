using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    public class AcreedoresDTO : EliminableDTOBase
    {
        private int? acreedorId;
        private String razonSocial;
        private String ruc;
        private String direccionFiscal;
        private String condicion;
        private int? plazoDePago;
        private Boolean activo;
        private PaisesDTO pais;

        public AcreedoresDTO() : base()
        {
            this.Pais = new PaisesDTO();
            AcreedorId = null;
            RazonSocial = null;
            Ruc = null;
            DireccionFiscal = null;
            Condicion = null;
            PlazoDePago = null;
            Activo = false;
        }

        public AcreedoresDTO(int acreedor_id, string razon_social, string ruc, string direccion_fiscal, string condicion, int plazo_de_pago, bool activo, PaisesDTO pais) : base()
        {
            this.AcreedorId = acreedor_id;
            this.RazonSocial = razon_social;
            this.Ruc = ruc;
            this.DireccionFiscal = direccion_fiscal;
            this.Condicion = condicion;
            this.PlazoDePago = plazo_de_pago;
            this.Activo = activo;
            this.Pais = pais;
        }

        public AcreedoresDTO(AcreedoresDTO other) : base(other)
        {
            this.AcreedorId = other.AcreedorId;
            this.RazonSocial = other.RazonSocial;
            this.Ruc = other.Ruc;
            this.DireccionFiscal = other.DireccionFiscal;
            this.Condicion = other.Condicion;
            this.PlazoDePago = other.PlazoDePago;
            this.Activo = other.Activo;
            this.Pais = new PaisesDTO(other.Pais);
        }

        public int? AcreedorId { get => acreedorId; set => acreedorId = value; }
        public string RazonSocial { get => razonSocial; set => razonSocial = value; }
        public string Ruc { get => ruc; set => ruc = value; }
        public string DireccionFiscal { get => direccionFiscal; set => direccionFiscal = value; }
        public string Condicion { get => condicion; set => condicion = value; }
        public int? PlazoDePago { get => plazoDePago; set => plazoDePago = value; }
        public bool Activo { get => activo; set => activo = value; }
        public PaisesDTO Pais { get => pais; set => pais = value; }
    }
}
