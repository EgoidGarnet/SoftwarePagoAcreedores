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
            return this.facturaClienteSOAP.eliminarFactura(factura, usuario);
        }

        public int InsertarDetalle(detallesFacturaDTO detalle)
        {
            return this.facturaClienteSOAP.insertarDetalleFactura(detalle);
        }

        public int ModificarDetalle(detallesFacturaDTO detalle)
        {
            return this.facturaClienteSOAP.modificarDetalleFactura(detalle);
        }

        public int EliminarDetalle(detallesFacturaDTO detalle, usuariosDTO usuario)
        {
            return this.facturaClienteSOAP.eliminarDetalleFactura(detalle, usuario);
        }

        public facturasDTO ObtenerPorId(int facturaId)
        {
            return this.facturaClienteSOAP.obtenerFactura(facturaId);
        }

        public int Modificar(facturasDTO factura)
        {
            return this.facturaClienteSOAP.modificarFactura(factura);   
        }

        public int Insertar(facturasDTO factura)
        {
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
