using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftPac.Model;

namespace SoftPac.Persistance.DAO
{
    public class PropuestasPagoDAOImpl : PropuestasPagoDAO
    {

        private static BindingList<PropuestasPagoDTO> propuestasPago = new BindingList<PropuestasPagoDTO>();

        static PropuestasPagoDAOImpl()
        {
            // Obtener usuarios para simular la creación
            var usuariosDAO = new UsuariosDAOImpl();
            var usuario1 = usuariosDAO.ObtenerPorId(1);
            var usuario2 = usuariosDAO.ObtenerPorId(2);
            // ========================
            // Propuesta 1 – BCP (Pendiente)
            // ========================
            propuestasPago.Add(new PropuestasPagoDTO
            {
                PropuestaId = 1,
                UsuarioCreacion = usuario2, // Creado por Ana Gomez
                FechaHoraCreacion = DateTime.Now.AddDays(-2),
                Estado = "Pendiente",
                EntidadBancaria = new EntidadesBancariasDTO
                {
                    EntidadBancariaId = 1,
                    Nombre = "Banco de Crédito del Perú",
                    CodigoSwift = "BCPLPEPL",
                    Pais = new PaisesDTO
                    {
                        PaisId = 3,
                        Nombre = "Perú",
                        CodigoIso = "PE",
                        CodigoTelefonico = "+51"
                    }
                },
                DetallesPropuesta = new BindingList<DetallesPropuestaDTO> {
                new DetallesPropuestaDTO
                {
                    DetallePropuestaId = 1,
                    MontoPago = 100.00m,
                    FormaPago = 'T',
                    Factura = new FacturasDTO
                    {
                        FacturaId = 101,
                        NumeroFactura = "F001-000101",
                        Moneda = new MonedasDTO(3, "Sol Peruano", "PEN", "S/.")
                    },
                    CuentaAcreedor = new CuentasAcreedorDTO
                    {
                        CuentaBancariaId = 1,
                        NumeroCuenta = "192-100001-01",
                        EntidadBancaria = new EntidadesBancariasDTO
                        {
                            EntidadBancariaId = 4,
                            Nombre = "Scotiabank Perú",
                            CodigoSwift = "BSUDPEPL",
                            Pais = new PaisesDTO { PaisId = 3, Nombre = "Perú", CodigoIso = "PE", CodigoTelefonico = "+51" }
                        }
                    },
                    CuentaPropia = new CuentasPropiasDTO
                    {
                        CuentaBancariaId = 1,
                        TipoCuenta = "Ahorros Soles",
                        NumeroCuenta = "002193000000000001",
                        Cci = "193-12345678-1-01",
                        Activa = true,
                        SaldoDisponible = 50000.75m,
                        Moneda = new MonedasDTO(3, "Sol Peruano", "PEN", "S/."),
                        EntidadBancaria = new EntidadesBancariasDTO
                        {
                            EntidadBancariaId = 1,
                            Nombre = "Banco de Crédito del Perú",
                            CodigoSwift = "BCPLPEPL"
                        }
                    }
                },
                new DetallesPropuestaDTO
                {
                    DetallePropuestaId = 2,
                    MontoPago = 120.00m,
                    FormaPago = 'T',
                    Factura = new FacturasDTO
                    {
                        FacturaId = 102,
                        NumeroFactura = "F001-000102",
                        Moneda = new MonedasDTO(2, "Dólar Americano", "USD","$")
                    },
                    CuentaAcreedor = new CuentasAcreedorDTO
                    {
                        CuentaBancariaId = 2,
                        NumeroCuenta = "194-100002-01",
                        EntidadBancaria = new EntidadesBancariasDTO
                        {
                            EntidadBancariaId = 5,
                            Nombre = "BBVA Perú",
                            CodigoSwift = "BBVAPERL",
                            Pais = new PaisesDTO { PaisId = 3, Nombre = "Perú", CodigoIso = "PE", CodigoTelefonico = "+51" }
                        }
                    },
                    CuentaPropia = new CuentasPropiasDTO
                    {
                        CuentaBancariaId = 2,
                        TipoCuenta = "Corriente Dólares",
                        NumeroCuenta = "003194000000000002",
                        Cci = "194-87654321-1-02",
                        Activa = true,
                        SaldoDisponible = 12500.00m,
                        Moneda = new MonedasDTO(2, "Dólar Americano", "USD","$"),
                        EntidadBancaria = new EntidadesBancariasDTO
                        {
                            EntidadBancariaId = 1,
                            Nombre = "Banco de Crédito del Perú",
                            CodigoSwift = "BCPLPEPL"
                        }
                    }
                }
            }
            });
            // ========================
            // Propuesta 2 – Interbank (Enviada)
            // ========================
            propuestasPago.Add(new PropuestasPagoDTO
            {
                PropuestaId = 2,
                UsuarioCreacion = usuario2,
                FechaHoraCreacion = DateTime.Now.AddDays(-5),
                Estado = "Enviada",
                EntidadBancaria = new EntidadesBancariasDTO
                {
                    EntidadBancariaId = 2,
                    Nombre = "Interbank",
                    CodigoSwift = "BINPPEPL",
                    Pais = new PaisesDTO
                    {
                        PaisId = 3,
                        Nombre = "Perú",
                        CodigoIso = "PE",
                        CodigoTelefonico = "+51"
                    }
                },
                DetallesPropuesta = new BindingList<DetallesPropuestaDTO>
                {
                    new DetallesPropuestaDTO
                    {
                        DetallePropuestaId = 3,
                        MontoPago = 250.00m,
                        FormaPago = 'T',
                        Factura = new FacturasDTO
                        {
                            FacturaId = 103,
                            NumeroFactura = "F001-000103",
                            Moneda = new MonedasDTO(3, "Sol Peruano", "PEN", "S/.")
                        },
                        CuentaAcreedor = new CuentasAcreedorDTO
                        {
                            CuentaBancariaId = 3,
                            NumeroCuenta = "191-300003-01",
                            EntidadBancaria = new EntidadesBancariasDTO
                            {
                                EntidadBancariaId = 2,
                                Nombre = "Interbank",
                                CodigoSwift = "BINPPEPL",
                                Pais = new PaisesDTO { PaisId = 3, Nombre = "Perú", CodigoIso = "PE", CodigoTelefonico = "+51" }
                            }
                        },
                        CuentaPropia = new CuentasPropiasDTO
                        {
                            CuentaBancariaId = 3,
                            TipoCuenta = "Corriente Soles",
                            NumeroCuenta = "004191000000000003",
                            Cci = "191-11223344-1-03",
                            Activa = true,
                            SaldoDisponible = 120300.00m,
                            Moneda = new MonedasDTO(3, "Sol Peruano", "PEN", "S/."),
                            EntidadBancaria = new EntidadesBancariasDTO
                            {
                                EntidadBancariaId = 2,
                                Nombre = "Interbank",
                                CodigoSwift = "BINPPEPL"
                            }
                        }
                    }
                }
            });

        }

        public int Insertar(PropuestasPagoDTO propuestaPago)
        {
            if (propuestaPago.PropuestaId != null && propuestasPago.Any(p => p.PropuestaId == propuestaPago.PropuestaId))
                return 0;
            propuestaPago.PropuestaId = propuestasPago.Count > 0 ? propuestasPago.Max(p => p.PropuestaId) + 1 : 1;
            propuestasPago.Add(propuestaPago);
            return 1;
        }
        public int Modificar(PropuestasPagoDTO propuestaPago)
        {
            var existing = propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPago.PropuestaId);
            if (existing == null)
                return 0;
            int idx = propuestasPago.IndexOf(existing);
            propuestasPago[idx] = propuestaPago;
            return 1;
        }
        public int Eliminar(PropuestasPagoDTO propuestaPago)
        {
            var existing = propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPago.PropuestaId);
            if (existing == null)
                return 0;
            propuestasPago.Remove(existing);
            return 1;
        }
        public int EliminarLogico(PropuestasPagoDTO propuestaPago)
        {
            var existing = propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPago.PropuestaId);
            if (existing == null)
                return 0;
            existing.UsuarioEliminacion = propuestaPago.UsuarioEliminacion;
            existing.FechaEliminacion = DateTime.Now;
            return 1;
        }
        public PropuestasPagoDTO ObtenerPorId(int propuestaPagoId)
        {
            return propuestasPago.FirstOrDefault(p => p.PropuestaId == propuestaPagoId);
        }
        public IList<PropuestasPagoDTO> ListarTodos()
        {
            return propuestasPago.ToList();
        }

        // --- MÉTODO NUEVO IMPLEMENTADO ---
        public IList<PropuestasPagoDTO> ListarTodaActividad()
        {
            // Este método devuelve TODOS, incluyendo los eliminados
            return propuestasPago.ToList();
        }
    }
}
