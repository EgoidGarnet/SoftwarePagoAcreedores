using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class FacturasDAOImpl : FacturasDAO
    {
        private BindingList<FacturasDTO> facturas = new BindingList<FacturasDTO>();

        public FacturasDAOImpl()
        {
            facturas.Add(new FacturasDTO {
                FacturaId = 1,
                Acreedor = new AcreedoresDTO {
                    AcreedorId = 1,
                    RazonSocial = "Proveedor Uno",
                    DireccionFiscal = "Calle Falsa 123",
                },
                DetallesFactura = new BindingList<DetallesFacturaDTO> {
                    new DetallesFacturaDTO {
                        DetalleFacturaId = 1,
                        Descripcion = "Producto A",
                        Subtotal = 100.0m
                    }
                },
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
            facturas.Add(new FacturasDTO {
                FacturaId = 2,
                Acreedor = new AcreedoresDTO {
                    AcreedorId = 2,
                    RazonSocial = "Proveedor Dos",
                    DireccionFiscal = "Calle Falsa 123",
                },
                DetallesFactura = new BindingList<DetallesFacturaDTO> {
                    new DetallesFacturaDTO {
                        DetalleFacturaId = 1,
                        Descripcion = "Producto B",
                        Subtotal = 100.0m
                    }
                },
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
        }

        public int Insertar(FacturasDTO factura)
        {
            if (factura.FacturaId == null || facturas.Any(f => f.FacturaId == factura.FacturaId))
                return 0;
            facturas.Add(factura);
            return 1;
        }
        public int Modificar(FacturasDTO factura)
        {
            var existing = facturas.FirstOrDefault(f => f.FacturaId == factura.FacturaId);
            if (existing == null)
                return 0;
            int idx = facturas.IndexOf(existing);
            facturas[idx] = factura;
            return 1;
        }
        public int eliminar(FacturasDTO factura)
        {
            var existing = facturas.FirstOrDefault(f => f.FacturaId == factura.FacturaId);
            if (existing == null)
                return 0;
            facturas.Remove(existing);
            return 1;
        }
        public int eliminarLogico(FacturasDTO factura)
        {
            var existing = facturas.FirstOrDefault(f => f.FacturaId == factura.FacturaId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = factura.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public FacturasDTO ObtenerPorId(int facturaId)
        {
            return facturas.FirstOrDefault(f => f.FacturaId == facturaId);
        }
        public IList<FacturasDTO> ListarTodos()
        {
            return facturas;
        }
    }
}
