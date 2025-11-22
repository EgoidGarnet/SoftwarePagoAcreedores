using SoftPacBusiness.PropuestaPagoWS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

// ============================================================================
// CLASES DTO PARA EXPORTACIÓN XML - SOLO DATOS BANCARIOS
// ============================================================================
namespace SoftPac.Business
{
    [XmlRoot("PropuestaPago")]
    public class PropuestaExportXML
    {
        public int PropuestaId { get; set; }
        public EntidadBancariaExportXML EntidadBancaria { get; set; }

        [XmlArray("Detalles")]
        [XmlArrayItem("Detalle")]
        public List<DetalleExportXML> Detalles { get; set; }
    }

    public class EntidadBancariaExportXML
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string SWIFT { get; set; }
        public PaisExportXML Pais { get; set; }
    }

    public class PaisExportXML
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
    }

    public class DetalleExportXML
    {
        public int DetalleId { get; set; }
        public decimal MontoPago { get; set; }
        public string FormaPago { get; set; }
        public FacturaExportXML Factura { get; set; }
        public CuentaExportXML CuentaOrigen { get; set; }
        public CuentaExportXML CuentaDestino { get; set; }
    }

    public class FacturaExportXML
    {
        public int FacturaId { get; set; }
        public string NumeroFactura { get; set; }
        public string FechaEmision { get; set; }
        public string FechaLimitePago { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoIGV { get; set; }
        public string RegimenFiscal { get; set; }
        public decimal TasaIVA { get; set; }
        public decimal OtrosTributos { get; set; }
        public AcreedorExportXML Acreedor { get; set; }
        public MonedaExportXML Moneda { get; set; }
    }

    public class AcreedorExportXML
    {
        public int AcreedorId { get; set; }
        public string RazonSocial { get; set; }
    }

    public class MonedaExportXML
    {
        public int MonedaId { get; set; }
        public string CodigoISO { get; set; }
    }

    public class CuentaExportXML
    {
        public int CuentaId { get; set; }
        public string TipoCuenta { get; set; }
        public string NumeroCuenta { get; set; }
        public string CCI { get; set; }
        public EntidadBancariaExportXML EntidadBancaria { get; set; }
        public MonedaExportXML Moneda { get; set; }
    }

    // ============================================================================
    // MÉTODOS DE EXPORTACIÓN - SOLO DATOS BANCARIOS
    // ============================================================================

    public class ExportacionPropuestasUtil
    {
        private readonly Action<string, string, string> _descargarArchivoCallback;

        public ExportacionPropuestasUtil(Action<string, string, string> descargarArchivoCallback)
        {
            _descargarArchivoCallback = descargarArchivoCallback ?? throw new ArgumentNullException(nameof(descargarArchivoCallback));
        }

        public void ExportarCSV(propuestasPagoDTO propuesta)
        {
            var sb = new StringBuilder();

            // Encabezado con solo datos bancarios
            sb.AppendLine("DetalleID,FacturaID,NumeroFactura,FechaEmision,FechaLimitePago," +
                         "AcreedorID,Proveedor,Moneda,MontoPago,MontoTotal,MontoIGV,RegimenFiscal,TasaIVA,OtrosTributos," +
                         "CuentaOrigenID,TipoCuentaOrigen,NumeroCuentaOrigen,CCIOrigen,BancoOrigen,SWIFTOrigen,PaisOrigen," +
                         "CuentaDestinoID,TipoCuentaDestino,NumeroCuentaDestino,CCIDestino,BancoDestino,SWIFTDestino,PaisDestino," +
                         "FormaPago");

            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false);

            foreach (var detalle in detallesActivos)
            {
                // Factura
                string detalleId = detalle.detalle_propuesta_idSpecified ? detalle.detalle_propuesta_id.ToString() : "";
                string facturaId = detalle.factura?.factura_idSpecified == true ? detalle.factura.factura_id.ToString() : "";
                string numeroFactura = EscaparCSV(detalle.factura?.numero_factura ?? "");
                string fechaEmision = detalle.factura?.fecha_emisionSpecified == true
                    ? detalle.factura.fecha_emision.ToString("yyyy-MM-dd") : "";
                string fechaLimitePago = detalle.factura?.fecha_limite_pagoSpecified == true
                    ? detalle.factura.fecha_limite_pago.ToString("yyyy-MM-dd") : "";

                // Acreedor
                string acreedorId = detalle.factura?.acreedor?.acreedor_idSpecified == true
                    ? detalle.factura.acreedor.acreedor_id.ToString() : "";
                string proveedor = EscaparCSV(RemoverTildes(detalle.factura?.acreedor?.razon_social ?? ""));

                // Moneda y montos
                string moneda = detalle.factura?.moneda?.codigo_iso ?? "";
                string montoPago = detalle.monto_pagoSpecified ? detalle.monto_pago.ToString("F2", CultureInfo.InvariantCulture) : "";
                string montoTotal = detalle.factura?.monto_totalSpecified == true
                    ? detalle.factura.monto_total.ToString("F2", CultureInfo.InvariantCulture) : "";
                string montoIGV = detalle.factura?.monto_igvSpecified == true
                    ? detalle.factura.monto_igv.ToString("F2", CultureInfo.InvariantCulture) : "";

                // Datos fiscales
                string regimenFiscal = EscaparCSV(RemoverTildes(detalle.factura?.regimen_fiscal ?? ""));
                string tasaIVA = detalle.factura?.tasa_ivaSpecified == true
                    ? detalle.factura.tasa_iva.ToString("F2", CultureInfo.InvariantCulture) : "";
                string otrosTributos = detalle.factura?.otros_tributosSpecified == true
                    ? detalle.factura.otros_tributos.ToString("F2", CultureInfo.InvariantCulture) : "";

                // Cuenta Origen
                string cuentaOrigenId = detalle.cuenta_propia?.cuenta_bancaria_idSpecified == true
                    ? detalle.cuenta_propia.cuenta_bancaria_id.ToString() : "";
                string tipoCuentaOrigen = detalle.cuenta_propia?.tipo_cuenta ?? "";
                string numeroCuentaOrigen = detalle.cuenta_propia?.numero_cuenta ?? "";
                string cciOrigen = detalle.cuenta_propia?.cci ?? "";
                string bancoOrigen = EscaparCSV(RemoverTildes(detalle.cuenta_propia?.entidad_bancaria?.nombre ?? ""));
                string swiftOrigen = detalle.cuenta_propia?.entidad_bancaria?.codigo_swift ?? "";
                string paisOrigen = EscaparCSV(RemoverTildes(detalle.cuenta_propia?.entidad_bancaria?.pais?.nombre ?? ""));

                // Cuenta Destino
                string cuentaDestinoId = detalle.cuenta_acreedor?.cuenta_bancaria_idSpecified == true
                    ? detalle.cuenta_acreedor.cuenta_bancaria_id.ToString() : "";
                string tipoCuentaDestino = detalle.cuenta_acreedor?.tipo_cuenta ?? "";
                string numeroCuentaDestino = detalle.cuenta_acreedor?.numero_cuenta ?? "";
                string cciDestino = detalle.cuenta_acreedor?.cci ?? "";
                string bancoDestino = EscaparCSV(RemoverTildes(detalle.cuenta_acreedor?.entidad_bancaria?.nombre ?? ""));
                string swiftDestino = detalle.cuenta_acreedor?.entidad_bancaria?.codigo_swift ?? "";
                string paisDestino = EscaparCSV(RemoverTildes(detalle.cuenta_acreedor?.entidad_bancaria?.pais?.nombre ?? ""));

                // Forma de pago
                string formaPago = EscaparCSV(MapearFormaPago((char?)detalle.forma_pago));

                sb.AppendLine($"{detalleId},{facturaId},{numeroFactura},{fechaEmision},{fechaLimitePago}," +
                             $"{acreedorId},{proveedor},{moneda},{montoPago},{montoTotal},{montoIGV},{regimenFiscal},{tasaIVA},{otrosTributos}," +
                             $"{cuentaOrigenId},{tipoCuentaOrigen},{numeroCuentaOrigen},{cciOrigen},{bancoOrigen},{swiftOrigen},{paisOrigen}," +
                             $"{cuentaDestinoId},{tipoCuentaDestino},{numeroCuentaDestino},{cciDestino},{bancoDestino},{swiftDestino},{paisDestino}," +
                             $"{formaPago}");
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _descargarArchivoCallback(sb.ToString(), $"PropuestaPago_{propuesta.propuesta_id}_{timestamp}.csv", "text/csv");
        }

        public void ExportarXML(propuestasPagoDTO propuesta)
        {
            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToList();

            var propuestaXML = new PropuestaExportXML
            {
                PropuestaId = propuesta.propuesta_idSpecified ? propuesta.propuesta_id : 0,

                EntidadBancaria = propuesta.entidad_bancaria != null ? new EntidadBancariaExportXML
                {
                    ID = propuesta.entidad_bancaria.entidad_bancaria_idSpecified
                        ? propuesta.entidad_bancaria.entidad_bancaria_id : 0,
                    Nombre = propuesta.entidad_bancaria.nombre ?? "",
                    SWIFT = propuesta.entidad_bancaria.codigo_swift ?? "",
                    Pais = propuesta.entidad_bancaria.pais != null ? new PaisExportXML
                    {
                        ID = propuesta.entidad_bancaria.pais.pais_idSpecified
                            ? propuesta.entidad_bancaria.pais.pais_id : 0,
                        Nombre = propuesta.entidad_bancaria.pais.nombre ?? ""
                    } : null
                } : null,

                Detalles = detallesActivos.Select(d => {
                    // Extraer la moneda de la factura para reutilizarla
                    MonedaExportXML monedaComun = null;
                    if (d.factura?.moneda != null)
                    {
                        monedaComun = new MonedaExportXML
                        {
                            MonedaId = d.factura.moneda.moneda_idSpecified ? d.factura.moneda.moneda_id : 0,
                            CodigoISO = d.factura.moneda.codigo_iso ?? ""
                        };
                    }

                    return new DetalleExportXML
                    {
                        DetalleId = d.detalle_propuesta_idSpecified ? d.detalle_propuesta_id : 0,
                        MontoPago = d.monto_pagoSpecified ? d.monto_pago : 0,
                        FormaPago = MapearFormaPago((char?)d.forma_pago),

                        Factura = d.factura != null ? new FacturaExportXML
                        {
                            FacturaId = d.factura.factura_idSpecified ? d.factura.factura_id : 0,
                            NumeroFactura = d.factura.numero_factura ?? "",
                            FechaEmision = d.factura.fecha_emisionSpecified
                                ? d.factura.fecha_emision.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            FechaLimitePago = d.factura.fecha_limite_pagoSpecified
                                ? d.factura.fecha_limite_pago.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            MontoTotal = d.factura.monto_totalSpecified ? d.factura.monto_total : 0,
                            MontoIGV = d.factura.monto_igvSpecified ? d.factura.monto_igv : 0,
                            RegimenFiscal = d.factura.regimen_fiscal ?? "",
                            TasaIVA = d.factura.tasa_ivaSpecified ? d.factura.tasa_iva : 0,
                            OtrosTributos = d.factura.otros_tributosSpecified ? d.factura.otros_tributos : 0,

                            Acreedor = d.factura.acreedor != null ? new AcreedorExportXML
                            {
                                AcreedorId = d.factura.acreedor.acreedor_idSpecified ? d.factura.acreedor.acreedor_id : 0,
                                RazonSocial = d.factura.acreedor.razon_social ?? ""
                            } : null,

                            Moneda = monedaComun
                        } : null,

                        CuentaOrigen = d.cuenta_propia != null ? new CuentaExportXML
                        {
                            CuentaId = d.cuenta_propia.cuenta_bancaria_idSpecified ? d.cuenta_propia.cuenta_bancaria_id : 0,
                            TipoCuenta = d.cuenta_propia.tipo_cuenta ?? "",
                            NumeroCuenta = d.cuenta_propia.numero_cuenta ?? "",
                            CCI = d.cuenta_propia.cci ?? "",

                            EntidadBancaria = d.cuenta_propia.entidad_bancaria != null ? new EntidadBancariaExportXML
                            {
                                ID = d.cuenta_propia.entidad_bancaria.entidad_bancaria_idSpecified
                                    ? d.cuenta_propia.entidad_bancaria.entidad_bancaria_id : 0,
                                Nombre = d.cuenta_propia.entidad_bancaria.nombre ?? "",
                                SWIFT = d.cuenta_propia.entidad_bancaria.codigo_swift ?? "",
                                Pais = d.cuenta_propia.entidad_bancaria.pais != null ? new PaisExportXML
                                {
                                    ID = d.cuenta_propia.entidad_bancaria.pais.pais_idSpecified
                                        ? d.cuenta_propia.entidad_bancaria.pais.pais_id : 0,
                                    Nombre = d.cuenta_propia.entidad_bancaria.pais.nombre ?? ""
                                } : null
                            } : null,

                            Moneda = monedaComun
                        } : null,

                        CuentaDestino = d.cuenta_acreedor != null ? new CuentaExportXML
                        {
                            CuentaId = d.cuenta_acreedor.cuenta_bancaria_idSpecified ? d.cuenta_acreedor.cuenta_bancaria_id : 0,
                            TipoCuenta = d.cuenta_acreedor.tipo_cuenta ?? "",
                            NumeroCuenta = d.cuenta_acreedor.numero_cuenta ?? "",
                            CCI = d.cuenta_acreedor.cci ?? "",

                            EntidadBancaria = d.cuenta_acreedor.entidad_bancaria != null ? new EntidadBancariaExportXML
                            {
                                ID = d.cuenta_acreedor.entidad_bancaria.entidad_bancaria_idSpecified
                                    ? d.cuenta_acreedor.entidad_bancaria.entidad_bancaria_id : 0,
                                Nombre = d.cuenta_acreedor.entidad_bancaria.nombre ?? "",
                                SWIFT = d.cuenta_acreedor.entidad_bancaria.codigo_swift ?? "",
                                Pais = d.cuenta_acreedor.entidad_bancaria.pais != null ? new PaisExportXML
                                {
                                    ID = d.cuenta_acreedor.entidad_bancaria.pais.pais_idSpecified
                                        ? d.cuenta_acreedor.entidad_bancaria.pais.pais_id : 0,
                                    Nombre = d.cuenta_acreedor.entidad_bancaria.pais.nombre ?? ""
                                } : null
                            } : null,

                            Moneda = monedaComun
                        } : null
                    };
                }).ToList()
            };

            var serializer = new XmlSerializer(typeof(PropuestaExportXML));
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                {
                    serializer.Serialize(xmlWriter, propuestaXML);
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    _descargarArchivoCallback(stringWriter.ToString(), $"PropuestaPago_{propuesta.propuesta_id}_{timestamp}.xml", "application/xml");
                }
            }
        }

        public void ExportarTXT(propuestasPagoDTO propuesta)
        {
            var sb = new StringBuilder();
            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToArray();

            // Encabezado simplificado
            sb.AppendLine("═══════════════════════════════════════════════════════════════════");
            sb.AppendLine($"PROPUESTA DE PAGO #{propuesta.propuesta_id}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════");

            if (propuesta.entidad_bancaria != null)
            {
                string nombreBanco = propuesta.entidad_bancaria.nombre ?? "N/A";
                string swiftBanco = propuesta.entidad_bancaria.codigo_swift ?? "N/A";
                string paisBanco = propuesta.entidad_bancaria.pais?.nombre ?? "N/A";
                sb.AppendLine($"Entidad Bancaria: {nombreBanco} ({swiftBanco}) - {paisBanco}");
            }

            sb.AppendLine();
            sb.AppendLine("-------------------------------------------------------------------");
            sb.AppendLine($"DETALLE DE PAGOS ({detallesActivos.Length} pago{(detallesActivos.Length != 1 ? "s" : "")})");
            sb.AppendLine("-------------------------------------------------------------------");
            sb.AppendLine();

            int contador = 1;
            foreach (var detalle in detallesActivos)
            {
                sb.AppendLine($"[{contador}] DETALLE #{detalle.detalle_propuesta_id}");

                // Información de la factura
                if (detalle.factura != null)
                {
                    sb.AppendLine($"    Factura: {detalle.factura.numero_factura ?? "N/A"} (ID: {detalle.factura.factura_id})");

                    if (detalle.factura.fecha_emisionSpecified)
                    {
                        sb.AppendLine($"    - Fecha Emisión: {detalle.factura.fecha_emision:yyyy-MM-dd}");
                    }

                    if (detalle.factura.fecha_limite_pagoSpecified)
                    {
                        sb.AppendLine($"    - Fecha Límite Pago: {detalle.factura.fecha_limite_pago:yyyy-MM-dd}");
                    }

                    string moneda = detalle.factura.moneda?.codigo_iso ?? "N/A";

                    if (detalle.factura.monto_totalSpecified)
                    {
                        string montoTotal = detalle.factura.monto_total.ToString("N2", CultureInfo.InvariantCulture);
                        string montoIGV = detalle.factura.monto_igvSpecified
                            ? detalle.factura.monto_igv.ToString("N2", CultureInfo.InvariantCulture)
                            : "0.00";
                        sb.AppendLine($"    - Monto Total: {montoTotal} {moneda} (IGV: {montoIGV})");
                    }

                    if (!string.IsNullOrEmpty(detalle.factura.regimen_fiscal))
                    {
                        sb.AppendLine($"    - Régimen Fiscal: {detalle.factura.regimen_fiscal}");
                    }

                    if (detalle.factura.tasa_ivaSpecified)
                    {
                        sb.AppendLine($"    - Tasa IVA: {detalle.factura.tasa_iva:F2}%");
                    }

                    if (detalle.factura.otros_tributosSpecified)
                    {
                        sb.AppendLine($"    - Otros Tributos: {detalle.factura.otros_tributos:N2}");
                    }
                }

                sb.AppendLine();

                // Información del acreedor
                if (detalle.factura?.acreedor != null)
                {
                    sb.AppendLine($"    Acreedor: {detalle.factura.acreedor.razon_social ?? "N/A"} (ID: {detalle.factura.acreedor.acreedor_id})");
                    sb.AppendLine();
                }

                // Cuenta origen
                if (detalle.cuenta_propia != null)
                {
                    string tipoCuenta = detalle.cuenta_propia.tipo_cuenta ?? "N/A";
                    sb.AppendLine($"    Cuenta Origen ({tipoCuenta}):");
                    sb.AppendLine($"    - ID: {detalle.cuenta_propia.cuenta_bancaria_id}");
                    sb.AppendLine($"    - Número: {detalle.cuenta_propia.numero_cuenta ?? "N/A"}");

                    if (!string.IsNullOrEmpty(detalle.cuenta_propia.cci))
                    {
                        sb.AppendLine($"    - CCI: {detalle.cuenta_propia.cci}");
                    }

                    if (detalle.cuenta_propia.entidad_bancaria != null)
                    {
                        string nombreBanco = detalle.cuenta_propia.entidad_bancaria.nombre ?? "N/A";
                        string swiftBanco = detalle.cuenta_propia.entidad_bancaria.codigo_swift ?? "N/A";
                        string paisBanco = detalle.cuenta_propia.entidad_bancaria.pais?.nombre ?? "N/A";
                        sb.AppendLine($"    - Banco: {nombreBanco} ({swiftBanco}) - {paisBanco}");
                    }

                    string moneda = detalle.cuenta_propia.moneda?.codigo_iso ?? "";
                    if (!string.IsNullOrEmpty(moneda))
                    {
                        sb.AppendLine($"    - Moneda: {moneda}");
                    }

                    sb.AppendLine();
                }

                // Cuenta destino
                if (detalle.cuenta_acreedor != null)
                {
                    string tipoCuenta = detalle.cuenta_acreedor.tipo_cuenta ?? "N/A";
                    sb.AppendLine($"    Cuenta Destino ({tipoCuenta}):");
                    sb.AppendLine($"    - ID: {detalle.cuenta_acreedor.cuenta_bancaria_id}");
                    sb.AppendLine($"    - Número: {detalle.cuenta_acreedor.numero_cuenta ?? "N/A"}");

                    if (!string.IsNullOrEmpty(detalle.cuenta_acreedor.cci))
                    {
                        sb.AppendLine($"    - CCI: {detalle.cuenta_acreedor.cci}");
                    }

                    if (detalle.cuenta_acreedor.entidad_bancaria != null)
                    {
                        string nombreBanco = detalle.cuenta_acreedor.entidad_bancaria.nombre ?? "N/A";
                        string swiftBanco = detalle.cuenta_acreedor.entidad_bancaria.codigo_swift ?? "N/A";
                        string paisBanco = detalle.cuenta_acreedor.entidad_bancaria.pais?.nombre ?? "N/A";
                        sb.AppendLine($"    - Banco: {nombreBanco} ({swiftBanco}) - {paisBanco}");
                    }

                    string moneda = detalle.cuenta_acreedor.moneda?.codigo_iso ?? "";
                    if (!string.IsNullOrEmpty(moneda))
                    {
                        sb.AppendLine($"    - Moneda: {moneda}");
                    }

                    sb.AppendLine();
                }

                // Información del pago
                if (detalle.monto_pagoSpecified)
                {
                    string moneda = detalle.factura?.moneda?.codigo_iso ?? "";
                    string monto = detalle.monto_pago.ToString("N2", CultureInfo.InvariantCulture);
                    sb.AppendLine($"    Pago: {monto} {moneda}");
                }

                string formaPago = MapearFormaPago((char?)detalle.forma_pago);
                sb.AppendLine($"    Forma de Pago: {formaPago}");

                sb.AppendLine();
                sb.AppendLine("-------------------------------------------------------------------");
                sb.AppendLine();

                contador++;
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _descargarArchivoCallback(sb.ToString(), $"PropuestaPago_{propuesta.propuesta_id}_{timestamp}.txt", "text/plain; charset=utf-8");
        }

        public void ExportarMT101(propuestasPagoDTO propuesta)
        {
            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToArray();

            if (detallesActivos.Length == 0)
            {
                throw new InvalidOperationException("No hay detalles activos para exportar en formato MT101");
            }

            var sb = new StringBuilder();

            // Obtener SWIFT del primer detalle (cuenta propia origen)
            string swiftOrigen = detallesActivos[0].cuenta_propia?.entidad_bancaria?.codigo_swift ?? "BANKXXXXAXXX";
            string nombreBancoOrigen = detallesActivos[0].cuenta_propia?.entidad_bancaria?.nombre ?? "BANCO ORIGEN";
            string paisOrigen = detallesActivos[0].cuenta_propia?.entidad_bancaria?.pais?.nombre ?? "";
            string cuentaOrdenante = detallesActivos[0].cuenta_propia?.numero_cuenta ?? "";
            string cciOrdenante = detallesActivos[0].cuenta_propia?.cci ?? "";

            // Encabezado MT101
            sb.AppendLine($"{{1:F01{PadRight(swiftOrigen, 12)}0000000000}}");
            sb.AppendLine($"{{2:I101{PadRight(swiftOrigen, 12)}N}}");
            sb.AppendLine("{4:");

            // Campo :20: Referencia de la transacción
            sb.AppendLine($":20:PROP{propuesta.propuesta_id:D16}");

            // Campo :23E: Tipo de instrucción
            sb.AppendLine(":23E:CRED");

            // Campo :28D: Número de mensaje / Total de mensajes
            sb.AppendLine($":28D:1/{detallesActivos.Length}");

            // Campo :50H: Ordenante (con CCI o cuenta)
            string cuentaOrdenanteField = !string.IsNullOrEmpty(cciOrdenante) ? cciOrdenante : cuentaOrdenante;
            if (!string.IsNullOrEmpty(cuentaOrdenanteField))
            {
                sb.AppendLine($":50H:/{cuentaOrdenanteField}");
            }
            else
            {
                sb.AppendLine(":50H:");
            }
            sb.AppendLine(TruncarTexto(RemoverTildes(nombreBancoOrigen), 35));
            if (!string.IsNullOrEmpty(paisOrigen))
            {
                sb.AppendLine(TruncarTexto(RemoverTildes(paisOrigen), 35));
            }

            // Campo :30: Fecha de ejecución deseada
            DateTime fechaEjecucion = DateTime.Now;
            if (detallesActivos[0].factura?.fecha_limite_pagoSpecified == true)
            {
                fechaEjecucion = detallesActivos[0].factura.fecha_limite_pago;
            }
            sb.AppendLine($":30:{fechaEjecucion:yyyyMMdd}");

            // Campo :25: Cuenta del ordenante
            if (!string.IsNullOrEmpty(cuentaOrdenante))
            {
                sb.AppendLine($":25:{cuentaOrdenante}");
            }

            // Detalles individuales
            int secuencia = 1;
            foreach (var detalle in detallesActivos)
            {
                // Campo :21: Referencia de la transacción individual
                string referenciaDetalle = detalle.detalle_propuesta_idSpecified
                ? $"DET{detalle.detalle_propuesta_id:D13}"
                : $"SEQ{secuencia:D13}";
                sb.AppendLine($":21:{referenciaDetalle.Substring(0, Math.Min(referenciaDetalle.Length, 16))}");

                // Campo :32B: Moneda y monto
                string moneda = detalle.factura?.moneda?.codigo_iso ?? "USD";
                decimal monto = detalle.monto_pagoSpecified ? detalle.monto_pago : 0;
                string montoFormateado = monto.ToString("F2", CultureInfo.InvariantCulture).Replace(".", ",");
                sb.AppendLine($":32B:{moneda}{montoFormateado}");

                // Campo :50K: Ordenante (cuenta origen)
                string cciOrigen = detalle.cuenta_propia?.cci ?? "";
                string cuentaOrigen = detalle.cuenta_propia?.numero_cuenta ?? "";
                string nombreBancoOrigenDetalle = RemoverTildes(detalle.cuenta_propia?.entidad_bancaria?.nombre ?? "");

                string cuentaOrigenField = !string.IsNullOrEmpty(cciOrigen) ? cciOrigen : cuentaOrigen;
                if (!string.IsNullOrEmpty(cuentaOrigenField))
                {
                    sb.AppendLine($":50K:/{cuentaOrigenField}");
                }
                else
                {
                    sb.AppendLine(":50K:");
                }
                sb.AppendLine(TruncarTexto(nombreBancoOrigenDetalle, 35));

                // Campo :59: Beneficiario (cuenta destino)
                string cciDestino = detalle.cuenta_acreedor?.cci ?? "";
                string cuentaDestino = detalle.cuenta_acreedor?.numero_cuenta ?? "";
                string razonSocial = RemoverTildes(detalle.factura?.acreedor?.razon_social ?? "");
                string nombreBancoDestino = RemoverTildes(detalle.cuenta_acreedor?.entidad_bancaria?.nombre ?? "");
                string swiftDestino = detalle.cuenta_acreedor?.entidad_bancaria?.codigo_swift ?? "";

                string cuentaDestinoField = !string.IsNullOrEmpty(cciDestino) ? cciDestino : cuentaDestino;
                if (!string.IsNullOrEmpty(cuentaDestinoField))
                {
                    sb.AppendLine($":59:/{cuentaDestinoField}");
                }
                else
                {
                    sb.AppendLine(":59:");
                }
                sb.AppendLine(TruncarTexto(razonSocial, 35));

                if (!string.IsNullOrEmpty(nombreBancoDestino))
                {
                    string lineaBanco = !string.IsNullOrEmpty(swiftDestino)
                        ? $"{nombreBancoDestino} - {swiftDestino}"
                        : nombreBancoDestino;
                    sb.AppendLine(TruncarTexto(lineaBanco, 35));
                }

                // Campo :70: Detalles del pago / Referencia
                string numeroFactura = detalle.factura?.numero_factura ?? "";
                if (!string.IsNullOrEmpty(numeroFactura))
                {
                    sb.AppendLine($":70:Factura {TruncarTexto(numeroFactura, 27)}");
                }

                if (detalle.detalle_propuesta_idSpecified)
                {
                    sb.AppendLine($":70:Detalle ID: {detalle.detalle_propuesta_id}");
                }

                // Campo :71A: Cargos
                sb.AppendLine(":71A:OUR");

                secuencia++;
            }

            // Fin del mensaje
            sb.AppendLine("-}");

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _descargarArchivoCallback(sb.ToString(), $"PropuestaPago_{propuesta.propuesta_id}_{timestamp}.mt101", "text/plain; charset=utf-8");
        }

        // ============================================================================
        // MÉTODOS AUXILIARES
        // ============================================================================

        private string RemoverTildes(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            string normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private string TruncarTexto(string texto, int maxLength)
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            return texto.Length <= maxLength ? texto : texto.Substring(0, maxLength);
        }

        private string PadRight(string texto, int length)
        {
            if (string.IsNullOrEmpty(texto))
                return new string(' ', length);

            return texto.Length >= length ? texto.Substring(0, length) : texto.PadRight(length);
        }

        private string EscaparCSV(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            if (texto.Contains(",") || texto.Contains("\"") || texto.Contains("\n") || texto.Contains("\r"))
            {
                return "\"" + texto.Replace("\"", "\"\"") + "\"";
            }

            return texto;
        }

        private string MapearFormaPago(char? formaPago)
        {
            if (!formaPago.HasValue)
                return "No especificado";

            switch (formaPago.Value)
            {
                case 'T':
                    return "Transferencia";
                case 'C':
                    return "Cheque";
                case 'E':
                    return "Efectivo";
                case 'D':
                    return "Depósito";
                case 'L':
                    return "Letra de cambio";
                default:
                    return $"Otro ({formaPago.Value})";
            }
        }
    }
}