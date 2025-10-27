using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    [Serializable]
    public class FacturasDTO : EliminableDTOBase
    {
        private int? facturaId;
        private String numeroFactura;
        private DateTime? fechaEmision;
        private DateTime? fechaRecepcion;
        private DateTime? fechaLimitePago;
        private String estado;
        private decimal montoTotal;
        private decimal montoIgv;
        private decimal montoRestante;
        private String regimenFiscal;
        private decimal tasaIva;
        private decimal otrosTributos;
        private AcreedoresDTO acreedor;
        private MonedasDTO moneda;
        private BindingList<DetallesFacturaDTO> detallesFactura;



        public FacturasDTO() : base()
        {
            FacturaId = null;
            NumeroFactura = null;
            FechaEmision = null;
            FechaRecepcion = null;
            FechaLimitePago = null;
            Estado = "I";
            MontoTotal = 0.0m;
            MontoIgv = 0.0m;
            MontoRestante = 0.0m;
            RegimenFiscal = null;
            TasaIva = 0.0m;
            OtrosTributos = 0.0m;
        }

        public FacturasDTO(int facturaId, String numeroFactura, DateTime fechaEmision,
                       DateTime fechaRecepcion, DateTime fechaLimitePago, String estado,
                       decimal montoTotal, decimal montoIgv, decimal montoRestante,
                       String regimenFiscal, decimal tasaIva, decimal otrosTributos,
                       AcreedoresDTO acreedor, MonedasDTO moneda)
        {
            this.FacturaId = facturaId;
            this.NumeroFactura = numeroFactura;
            this.FechaEmision = fechaEmision;
            this.FechaRecepcion = FechaRecepcion;
            this.FechaLimitePago = FechaLimitePago;
            this.Estado = estado;
            this.MontoTotal = montoTotal;
            this.MontoIgv = montoIgv;
            this.MontoRestante = montoRestante;
            this.RegimenFiscal = regimenFiscal;
            this.TasaIva = tasaIva;
            this.OtrosTributos = otrosTributos;
            this.Acreedor = acreedor;
            this.Moneda = moneda;
            this.DetallesFactura = new BindingList<DetallesFacturaDTO>();
        }

        public FacturasDTO(int factura_id, String numero_factura, DateTime fecha_emision,
                       DateTime fecha_recepcion, DateTime fecha_limite_pago, String estado,
                       decimal monto_total, decimal monto_igv, decimal monto_restante,
                       String regimen_fiscal, decimal tasa_iva, decimal otros_tributos,
                       AcreedoresDTO acreedor, MonedasDTO moneda, BindingList<DetallesFacturaDTO> detalles_factura)
        {
            this.FacturaId = factura_id;
            this.NumeroFactura = numero_factura;
            this.FechaEmision = fecha_emision;
            this.FechaRecepcion = fecha_recepcion;
            this.FechaLimitePago = fecha_limite_pago;
            this.Estado = estado;
            this.MontoTotal = monto_total;
            this.MontoIgv = monto_igv;
            this.MontoRestante = monto_restante;
            this.RegimenFiscal = regimen_fiscal;
            this.TasaIva = tasa_iva;
            this.OtrosTributos = otros_tributos;
            this.Acreedor = acreedor;
            this.Moneda = moneda;
            this.DetallesFactura = detalles_factura;
        }

        public FacturasDTO(FacturasDTO other) : base(other)
        {
            this.FacturaId = other.FacturaId;
            this.NumeroFactura = other.NumeroFactura;
            this.FechaEmision = other.FechaEmision;
            this.FechaRecepcion = other.FechaRecepcion;
            this.FechaLimitePago = other.FechaLimitePago;
            this.Estado = other.Estado;
            this.MontoTotal = other.MontoTotal;
            this.MontoIgv = other.MontoIgv;
            this.MontoRestante = other.MontoRestante;
            this.RegimenFiscal = other.RegimenFiscal;
            this.TasaIva = other.TasaIva;
            this.OtrosTributos = other.OtrosTributos;
            this.Acreedor = other.Acreedor;
            this.Moneda = other.Moneda;
            this.DetallesFactura = other.DetallesFactura;
        }


        public int? FacturaId { get => facturaId; set => facturaId = value; }
        public string NumeroFactura { get => numeroFactura; set => numeroFactura = value; }
        public DateTime? FechaEmision { get => fechaEmision; set => fechaEmision = value; }
        public DateTime? FechaRecepcion { get => fechaRecepcion; set => fechaRecepcion = value; }
        public DateTime? FechaLimitePago { get => fechaLimitePago; set => fechaLimitePago = value; }
        public string Estado { get => estado; set => estado = value; }
        public decimal MontoTotal { get => montoTotal; set => montoTotal = value; }
        public decimal MontoIgv { get => montoIgv; set => montoIgv = value; }
        public decimal MontoRestante { get => montoRestante; set => montoRestante = value; }
        public string RegimenFiscal { get => regimenFiscal; set => regimenFiscal = value; }
        public decimal TasaIva { get => tasaIva; set => tasaIva = value; }
        public decimal OtrosTributos { get => otrosTributos; set => otrosTributos = value; }
        public AcreedoresDTO Acreedor { get => acreedor; set => acreedor = value; }
        public MonedasDTO Moneda { get => moneda; set => moneda = value; }
        public BindingList<DetallesFacturaDTO> DetallesFactura { get => detallesFactura; set => detallesFactura = value; }


        public void AddDetalleFactura(DetallesFacturaDTO detalle)
        {
            if (DetallesFactura == null)
            {
                DetallesFactura = new BindingList<DetallesFacturaDTO>();
            }
            DetallesFactura.Add(detalle);
        }

    }
}
