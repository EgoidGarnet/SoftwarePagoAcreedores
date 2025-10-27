using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Model
{
    [Serializable]
    public class DetallesPropuestaDTO : EliminableDTOBase
    {
        private int? detallePropuestaId;
        private decimal montoPago;
        private char formaPago;
        private PropuestasPagoDTO propuestaPago;
        private FacturasDTO factura;
        private CuentasAcreedorDTO cuentaAcreedor;
        private CuentasPropiasDTO cuentaPropia;

        public DetallesPropuestaDTO() : base()
        {
            DetallePropuestaId = null;
            MontoPago = 0.0m;
            FormaPago = 'T';
            PropuestaPago = null;
            Factura = null;
            CuentaAcreedor = null;
            CuentaPropia = null;
        }

        public DetallesPropuestaDTO(int? detallePropuestaId, decimal montoPago, char formaPago,
                                    CuentasAcreedorDTO cuentaAcreedor, PropuestasPagoDTO propuestaPago,
                                    CuentasPropiasDTO cuentaPropia, FacturasDTO factura)
        {
            DetallePropuestaId = detallePropuestaId;
            MontoPago = montoPago;
            FormaPago = formaPago;
            PropuestaPago = propuestaPago;
            Factura = factura;
            CuentaAcreedor = cuentaAcreedor;
            CuentaPropia = cuentaPropia;
        }

        public DetallesPropuestaDTO(DetallesPropuestaDTO detallePropuestaPago) : base(detallePropuestaPago)
        {
            DetallePropuestaId = detallePropuestaPago.DetallePropuestaId;
            MontoPago = detallePropuestaPago.MontoPago;
            FormaPago = detallePropuestaPago.FormaPago;
            PropuestaPago = detallePropuestaPago.PropuestaPago;
            CuentaAcreedor = detallePropuestaPago.CuentaAcreedor;
            CuentaPropia = detallePropuestaPago.CuentaPropia;
            Factura = detallePropuestaPago.Factura;
        }

        public int? DetallePropuestaId { get => detallePropuestaId; set => detallePropuestaId = value; }
        public decimal MontoPago { get => montoPago; set => montoPago = value; }
        public char FormaPago { get => formaPago; set => formaPago = value; }
        public PropuestasPagoDTO PropuestaPago { get => propuestaPago; set => propuestaPago = value; }
        public FacturasDTO Factura { get => factura; set => factura = value; }
        public CuentasAcreedorDTO CuentaAcreedor { get => cuentaAcreedor; set => cuentaAcreedor = value; }
        public CuentasPropiasDTO CuentaPropia { get => cuentaPropia; set => cuentaPropia = value; }


    }
}
