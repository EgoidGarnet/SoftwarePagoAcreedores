using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    [Serializable]
    public class DetallesFacturaDTO : EliminableDTOBase
    {
        private int? detalleFacturaId;
        private decimal subtotal;
        private String descripcion;
        private FacturasDTO factura;

        public int? DetalleFacturaId { get => detalleFacturaId; set => detalleFacturaId = value; }
        public decimal Subtotal { get => subtotal; set => subtotal = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public FacturasDTO Factura { get => factura; set => factura = value; }

        public DetallesFacturaDTO() : base()
        {
            DetalleFacturaId = null;
            Subtotal = 0.0m;
            Descripcion = null;
            Factura = new FacturasDTO();
        }


        public DetallesFacturaDTO(int detalle_factura_id, decimal subtotal, string descripcion, FacturasDTO factura) : base()
        {
            this.DetalleFacturaId = detalle_factura_id;
            this.Subtotal = subtotal;
            this.Descripcion = descripcion;
            this.Factura = factura;
        }

        public DetallesFacturaDTO(DetallesFacturaDTO other) : base(other)
        {
            this.DetalleFacturaId = other.DetalleFacturaId;
            this.Subtotal = other.Subtotal;
            this.Descripcion = other.Descripcion;
            this.Factura = new FacturasDTO(other.Factura);
        }
    }
}
