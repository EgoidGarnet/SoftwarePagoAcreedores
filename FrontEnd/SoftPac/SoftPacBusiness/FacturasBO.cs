using SoftPacBusiness;
using SoftPacBusiness.FacturasWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPac.Business
{
    public class FacturasBO
    {
        private FacturasWSClient facturaClienteSOAP;

        public FacturasBO()
        {
            this.facturaClienteSOAP = new FacturasWSClient();
        }
        public BindingList<facturasDTO> ListarTodos()
        {
            facturasDTO[] facturas = this.facturaClienteSOAP.listarFacturas();

            if (facturas == null)
                facturas = Array.Empty<facturasDTO>();

            return new BindingList<facturasDTO>(facturas.ToList());

        }

        public int Eliminar(facturasDTO factura)
        {
            return this.facturaClienteSOAP.eliminarFactura(factura, factura.usuario_eliminacion);
        }

        public int Eliminar(int facturaId, usuariosDTO usuario)
        {
            facturasDTO factura = new facturasDTO();
            factura.factura_id = facturaId;
            factura.factura_idSpecified = true;
            usuario.usuario_idSpecified = true;
            return this.facturaClienteSOAP.eliminarFactura(factura, usuario);
        }

        public int InsertarDetalle(detallesFacturaDTO detalle)
        {
            detalle.factura.detalles_Factura = null;
            detalle.subtotalSpecified = true;
            detalle.factura.factura_idSpecified = true;
            return this.facturaClienteSOAP.insertarDetalleFactura(detalle);
        }

        public int ModificarDetalle(detallesFacturaDTO detalle)
        {
            detalle.factura.detalles_Factura = null;
            detalle.subtotalSpecified = true;
            detalle.subtotalSpecified = true;
            return this.facturaClienteSOAP.modificarDetalleFactura(detalle);
        }

        public int EliminarDetalle(detallesFacturaDTO detalle, usuariosDTO usuario)
        {
            usuario.usuario_idSpecified = true;
            detalle.factura.detalles_Factura = null;
            return this.facturaClienteSOAP.eliminarDetalleFactura(detalle, usuario);
        }

        public facturasDTO ObtenerPorId(int facturaId)
        {
            facturasDTO factura = this.facturaClienteSOAP.obtenerFactura(facturaId);
            if (factura.detalles_Factura != null)
            {
                foreach (var detalle in factura.detalles_Factura)
                {
                    detalle.factura = factura;
                }
            }
            return factura;
        }

        public int Modificar(facturasDTO factura)
        {
            return this.facturaClienteSOAP.modificarFactura(factura);
        }

        public int Insertar(facturasDTO factura)
        {
            factura.fecha_emisionSpecified = true;
            factura.fecha_recepcionSpecified = true;
            factura.fecha_limite_pagoSpecified = true;
            factura.moneda.moneda_idSpecified = true;
            factura.acreedor.acreedor_idSpecified = true;
            factura.monto_totalSpecified = true;
            factura.monto_restanteSpecified = true;
            factura.monto_igvSpecified = true;
            factura.tasa_ivaSpecified = true;
            factura.otros_tributosSpecified = true;
            return this.facturaClienteSOAP.insertarFactura(factura);
        }

        public BindingList<facturasDTO> ListarPendientes()
        {
            facturasDTO[] facturasPend = facturaClienteSOAP.listarPendientes();

            if (facturasPend == null)
                facturasPend = Array.Empty<facturasDTO>();

            return new BindingList<facturasDTO>(facturasPend.ToList());

        }

        public IList<facturasDTO> ListarPendientesPorCriterios(int paisId, DateTime fechaLimite)
        {

            return this.facturaClienteSOAP.listarPendientesPorCriterios(paisId, fechaLimite);

        }
    }
}