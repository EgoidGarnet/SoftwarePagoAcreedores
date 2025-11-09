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
        private static BindingList<FacturasDTO> facturas = new BindingList<FacturasDTO>();
        private static int IdFac = 3;
        private static int IdDet = 5;
        static FacturasDAOImpl()
        {
            facturas.Add(new FacturasDTO {
                FacturaId = 1,
                NumeroFactura = "3542524131",
                Estado = "Pendiente",
                MontoTotal = 118.0m,
                MontoIgv = 18.0m,
                MontoRestante = 118.0m,
                RegimenFiscal = "General",
                FechaEmision = DateTime.Now.AddDays(-10),
                FechaRecepcion = DateTime.Now.AddDays(-10),
                FechaLimitePago = DateTime.Now.AddDays(20),
                Acreedor = new AcreedoresDTO {
                    AcreedorId = 1,
                    RazonSocial = "Acreedor Uno",
                    Ruc = "123456789",
                    DireccionFiscal = "Calle 1",
                    Condicion = "Contado",
                    PlazoDePago = 30,
                    Activo = true,
                    Pais = new PaisesDTO(1, "Mexico", "MX", "+56")
                },
                
                Moneda = new MonedasDTO
                {
                    MonedaId = 1,
                    Nombre = "Dólar Estadounidense",
                    CodigoIso = "USD",
                    Simbolo = "$"
                },
                UsuarioEliminacion = null,
                FechaEliminacion = null,
            });
            BindingList<DetallesFacturaDTO> DetallesFactura = new BindingList<DetallesFacturaDTO> {
                    new DetallesFacturaDTO {
                        DetalleFacturaId = 1,
                        Descripcion = "Producto A",
                        Subtotal = 100.0m,
                        UsuarioEliminacion = null,
                        FechaEliminacion = null
                    },
                    new DetallesFacturaDTO {
                        DetalleFacturaId = 2,
                        Descripcion = "Cargos adicionales",
                        Subtotal = 18.0m,
                        UsuarioEliminacion = null,
                        FechaEliminacion = null
                    }
            };
            facturas[0].DetallesFactura = new BindingList<DetallesFacturaDTO>();
            foreach(var detalle in DetallesFactura)
            {
                detalle.Factura = facturas[0];
                facturas[0].DetallesFactura.Add(detalle);
            }
            facturas.Add(new FacturasDTO
            {
                FacturaId = 2,
                NumeroFactura = "3542524132",
                Estado = "Pagada",
                MontoTotal = 1250.0m,
                MontoIgv = 225.0m,
                MontoRestante = 0.0m,
                RegimenFiscal = "General",
                FechaEmision = DateTime.Now.AddDays(-25),
                FechaRecepcion = DateTime.Now.AddDays(-24),
                FechaLimitePago = DateTime.Now.AddDays(5),
                Acreedor = new AcreedoresDTO
                {
                    AcreedorId = 2,
                    RazonSocial = "Acreedor Dos",
                    Ruc = "123456789",
                    DireccionFiscal = "Calle 2",
                    Condicion = "Contado",
                    PlazoDePago = 45,
                    Activo = true,
                    Pais = new PaisesDTO(2, "Colombia", "CO", "+50")
                },
                DetallesFactura = new BindingList<DetallesFacturaDTO> {
                    new DetallesFacturaDTO {
                        DetalleFacturaId = 3,
                        Descripcion = "Producto B",
                        Subtotal = 750.0m
                    },
                    new DetallesFacturaDTO {
                        DetalleFacturaId = 4,
                        Descripcion = "Servicios de entrega",
                        Subtotal = 500.0m
                    }
                },
                Moneda = new MonedasDTO
                {
                    MonedaId = 4,
                    Nombre = "Peso Colombiano",
                    CodigoIso = "COP",
                    Simbolo = "$"
                },
                UsuarioEliminacion = null,
                FechaEliminacion = null
            });
            facturas[1].DetallesFactura = new BindingList<DetallesFacturaDTO>();
            DetallesFactura = new BindingList<DetallesFacturaDTO> {
                new DetallesFacturaDTO {
                    DetalleFacturaId = 3,
                    Descripcion = "Producto B",
                    Subtotal = 750.0m,
                    UsuarioEliminacion = null,
                    FechaEliminacion = null
                },
                new DetallesFacturaDTO {
                    DetalleFacturaId = 4,
                    Descripcion = "Servicios de entrega",
                    Subtotal = 500.0m,
                    UsuarioEliminacion = null,
                    FechaEliminacion = null
                }
            };
            foreach (var detalle in DetallesFactura)
            {
                detalle.Factura = facturas[1];
                facturas[1].DetallesFactura.Add(detalle);
            }
        }

        public int Insertar(FacturasDTO factura)
        {
            factura.FacturaId = IdFac++;
            facturas.Add(factura);
            return 1;
        }
        public int Modificar(FacturasDTO factura)
        {
            var existing = facturas.FirstOrDefault(f => f.FacturaId == factura.FacturaId);
            if (existing == null)
                return 0;
            foreach(DetallesFacturaDTO detalle in existing.DetallesFactura)
            {
                if (detalle.DetalleFacturaId == null) detalle.DetalleFacturaId = IdDet++;
            }
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
            existing.FechaEliminacion = factura.FechaEliminacion;
            return 1;
        }
        public FacturasDTO ObtenerPorId(int facturaId)
        {
            FacturasDTO factura = facturas.FirstOrDefault(f => f.FacturaId == facturaId);
            if(factura.DetallesFactura!=null)
                factura.DetallesFactura = new BindingList<DetallesFacturaDTO>(factura.DetallesFactura.Where(n => n.UsuarioEliminacion == null).ToList());
            else factura.DetallesFactura = new BindingList<DetallesFacturaDTO>();
            return factura;
        }
        public IList<FacturasDTO> ListarTodos()
        {
            return new BindingList<FacturasDTO>(facturas.Where(n => n.FechaEliminacion == null).ToList());
        }
    }
}
