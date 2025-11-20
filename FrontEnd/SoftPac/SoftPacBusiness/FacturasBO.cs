using SoftPacBusiness;
using SoftPacBusiness.FacturasWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

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
            if (factura.detalles_Factura != null)
            {
                foreach (detallesFacturaDTO detalle in factura.detalles_Factura)
                {
                    detalle.factura = null;
                }
            }

            ConfigurarEspecificadoresFactura(factura);
            return this.facturaClienteSOAP.modificarFactura(factura);
        }

        public int Insertar(facturasDTO factura)
        {
            ConfigurarEspecificadoresFactura(factura);
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

        #region Métodos para Carga Masiva

        /// <summary>
        /// Procesa un archivo XML de facturas según el país especificado
        /// </summary>
        public List<ResultadoCargaDTO> ProcesarXmlFacturas(string xmlContent, int paisId, string paisNombre, AcreedoresBO acreedoresBO, MonedasBO monedasBO)
        {
            switch (paisNombre)
            {
                case "Perú":
                    return ProcesarFacturasPerú(xmlContent, paisId, acreedoresBO, monedasBO);
                case "México":
                    return ProcesarFacturasMéxico(xmlContent, paisId, acreedoresBO, monedasBO);
                case "Colombia":
                    return ProcesarFacturasColombia(xmlContent, paisId, acreedoresBO, monedasBO);
                default:
                    throw new ArgumentException($"País no soportado: {paisNombre}");
            }
        }

        /// <summary>
        /// Procesa facturas de Perú (formato UBL 2.1)
        /// </summary>
        private List<ResultadoCargaDTO> ProcesarFacturasPerú(string xmlContent, int paisId, AcreedoresBO acreedoresBO, MonedasBO monedasBO)
        {
            List<ResultadoCargaDTO> resultados = new List<ResultadoCargaDTO>();

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                XNamespace xmlns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";

                var invoices = doc.Descendants(xmlns + "Invoice");

                foreach (var invoice in invoices)
                {
                    ResultadoCargaDTO resultado = new ResultadoCargaDTO();

                    try
                    {
                        // Extraer datos del XML
                        var datosFactura = ExtraerDatosFacturaPerú(invoice, cac, cbc);

                        resultado.NumeroFactura = datosFactura.NumeroFactura;
                        resultado.RazonSocial = datosFactura.RazonSocial;
                        resultado.MontoTotal = datosFactura.MontoTotal;

                        // Crear factura
                        facturasDTO factura = CrearFacturaDesdeXML(
                            datosFactura, paisId, acreedoresBO, monedasBO);

                        // Insertar en BD
                        int resultadoId = Insertar(factura);
                        AsignarEstadoResultado(resultado, resultadoId);
                    }
                    catch (Exception ex)
                    {
                        resultado.Estado = "Fallida";
                        resultado.Mensaje = $"Error: {ex.Message}";
                    }

                    resultados.Add(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al parsear XML de Perú: {ex.Message}", ex);
            }

            return resultados;
        }

        /// <summary>
        /// Procesa facturas de México (formato CFDI 3.3)
        /// </summary>
        private List<ResultadoCargaDTO> ProcesarFacturasMéxico(string xmlContent, int paisId, AcreedoresBO acreedoresBO, MonedasBO monedasBO)
        {
            List<ResultadoCargaDTO> resultados = new List<ResultadoCargaDTO>();

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";

                var comprobantes = doc.Descendants(cfdi + "Comprobante");

                foreach (var comprobante in comprobantes)
                {
                    ResultadoCargaDTO resultado = new ResultadoCargaDTO();

                    try
                    {
                        // Extraer datos del XML
                        var datosFactura = ExtraerDatosFacturaMéxico(comprobante, cfdi);

                        resultado.NumeroFactura = datosFactura.NumeroFactura;
                        resultado.RazonSocial = datosFactura.RazonSocial;
                        resultado.MontoTotal = datosFactura.MontoTotal;

                        // Crear factura
                        facturasDTO factura = CrearFacturaDesdeXML(
                            datosFactura, paisId, acreedoresBO, monedasBO);

                        // Insertar en BD
                        int resultadoId = Insertar(factura);
                        AsignarEstadoResultado(resultado, resultadoId);
                    }
                    catch (Exception ex)
                    {
                        resultado.Estado = "Fallida";
                        resultado.Mensaje = $"Error: {ex.Message}";
                    }

                    resultados.Add(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al parsear XML de México: {ex.Message}", ex);
            }

            return resultados;
        }

        /// <summary>
        /// Procesa facturas de Colombia (formato UBL 2.1)
        /// </summary>
        private List<ResultadoCargaDTO> ProcesarFacturasColombia(string xmlContent, int paisId, AcreedoresBO acreedoresBO, MonedasBO monedasBO)
        {
            List<ResultadoCargaDTO> resultados = new List<ResultadoCargaDTO>();

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace xmlns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

                var invoices = doc.Descendants(xmlns + "Invoice");

                foreach (var invoice in invoices)
                {
                    ResultadoCargaDTO resultado = new ResultadoCargaDTO();

                    try
                    {
                        // Extraer datos del XML
                        var datosFactura = ExtraerDatosFacturaColombia(invoice, cac, cbc);

                        resultado.NumeroFactura = datosFactura.NumeroFactura;
                        resultado.RazonSocial = datosFactura.RazonSocial;
                        resultado.MontoTotal = datosFactura.MontoTotal;

                        // Crear factura
                        facturasDTO factura = CrearFacturaDesdeXML(
                            datosFactura, paisId, acreedoresBO, monedasBO);

                        // Insertar en BD
                        int resultadoId = Insertar(factura);
                        AsignarEstadoResultado(resultado, resultadoId);
                    }
                    catch (Exception ex)
                    {
                        resultado.Estado = "Fallida";
                        resultado.Mensaje = $"Error: {ex.Message}";
                    }

                    resultados.Add(resultado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al parsear XML de Colombia: {ex.Message}", ex);
            }

            return resultados;
        }

        #endregion

        #region Métodos Auxiliares Privados

        private DatosFacturaXML ExtraerDatosFacturaPerú(XElement invoice, XNamespace cac, XNamespace cbc)
        {
            var customerParty = invoice.Descendants(cac + "AccountingCustomerParty").FirstOrDefault();

            string numeroFactura = invoice.Element(cbc + "ID")?.Value;
            DateTime fechaEmision = DateTime.Parse(invoice.Element(cbc + "IssueDate")?.Value);
            string fechaVenc = invoice.Element(cbc + "DueDate")?.Value;
            DateTime fechaVencimiento = string.IsNullOrEmpty(fechaVenc)
                ? fechaEmision.AddDays(30)
                : DateTime.Parse(fechaVenc);

            return new DatosFacturaXML
            {
                NumeroFactura = numeroFactura,
                FechaEmision = fechaEmision,
                FechaVencimiento = fechaVencimiento,
                RucNitRfc = customerParty?.Descendants(cbc + "ID").FirstOrDefault()?.Value,
                RazonSocial = customerParty?.Descendants(cbc + "RegistrationName").FirstOrDefault()?.Value,
                MontoTotal = decimal.Parse(invoice.Descendants(cbc + "PayableAmount").FirstOrDefault()?.Value ?? "0"),
                MontoImpuesto = decimal.Parse(invoice.Descendants(cbc + "TaxAmount").FirstOrDefault()?.Value ?? "0"),
                CodigoMoneda = invoice.Descendants(cbc + "DocumentCurrencyCode").FirstOrDefault()?.Value ?? "PEN"
            };
        }

        private DatosFacturaXML ExtraerDatosFacturaMéxico(XElement comprobante, XNamespace cfdi)
        {
            var receptor = comprobante.Element(cfdi + "Receptor");

            string serie = comprobante.Attribute("Serie")?.Value ?? "";
            string folio = comprobante.Attribute("Folio")?.Value ?? "";
            string numeroFactura = $"{serie}{folio}";

            DateTime fechaEmision = DateTime.Parse(comprobante.Attribute("Fecha")?.Value);
            DateTime fechaVencimiento = fechaEmision.AddDays(30);

            decimal montoIVA = decimal.Parse(comprobante.Descendants(cfdi + "Traslado")
                .Where(t => t.Attribute("Impuesto")?.Value == "002")
                .FirstOrDefault()?.Attribute("Importe")?.Value ?? "0");

            return new DatosFacturaXML
            {
                NumeroFactura = numeroFactura,
                FechaEmision = fechaEmision,
                FechaVencimiento = fechaVencimiento,
                RucNitRfc = receptor?.Attribute("Rfc")?.Value,
                RazonSocial = receptor?.Attribute("Nombre")?.Value,
                MontoTotal = decimal.Parse(comprobante.Attribute("Total")?.Value ?? "0"),
                MontoImpuesto = montoIVA,
                CodigoMoneda = comprobante.Attribute("Moneda")?.Value ?? "MXN"
            };
        }

        private DatosFacturaXML ExtraerDatosFacturaColombia(XElement invoice, XNamespace cac, XNamespace cbc)
        {
            var customerParty = invoice.Descendants(cac + "AccountingCustomerParty").FirstOrDefault();

            string numeroFactura = invoice.Element(cbc + "ID")?.Value;
            DateTime fechaEmision = DateTime.Parse(invoice.Element(cbc + "IssueDate")?.Value);
            string fechaVenc = invoice.Element(cbc + "DueDate")?.Value;
            DateTime fechaVencimiento = string.IsNullOrEmpty(fechaVenc)
                ? fechaEmision.AddDays(30)
                : DateTime.Parse(fechaVenc);

            return new DatosFacturaXML
            {
                NumeroFactura = numeroFactura,
                FechaEmision = fechaEmision,
                FechaVencimiento = fechaVencimiento,
                RucNitRfc = customerParty?.Descendants(cbc + "ID").FirstOrDefault()?.Value,
                RazonSocial = customerParty?.Descendants(cbc + "RegistrationName").FirstOrDefault()?.Value,
                MontoTotal = decimal.Parse(invoice.Descendants(cbc + "PayableAmount").FirstOrDefault()?.Value ?? "0"),
                MontoImpuesto = decimal.Parse(invoice.Descendants(cbc + "TaxAmount").FirstOrDefault()?.Value ?? "0"),
                CodigoMoneda = invoice.Descendants(cbc + "DocumentCurrencyCode").FirstOrDefault()?.Value ?? "COP"
            };
        }

        private facturasDTO CrearFacturaDesdeXML(
            DatosFacturaXML datos, int paisId, AcreedoresBO acreedoresBO, MonedasBO monedasBO)
        {
            // Buscar o crear acreedor
            var acreedores = acreedoresBO.ListarTodos();
            var acreedor = acreedores.FirstOrDefault(a =>
                a.ruc == datos.RucNitRfc && a.pais.pais_id == paisId);

            if (acreedor == null)
            {
                acreedor = CrearNuevoAcreedor(datos, paisId, acreedoresBO);
            }

            // Buscar moneda
            var moneda = monedasBO.ListarTodos().FirstOrDefault(m => m.codigo_iso == datos.CodigoMoneda);
            if (moneda == null)
            {
                string codigoDefault = paisId == 1 ? "PEN" : (paisId == 2 ? "COP" : "MXN");
                moneda = monedasBO.ListarTodos().FirstOrDefault(m => m.codigo_iso == codigoDefault);
            }

            // Crear factura
            facturasDTO factura = new facturasDTO
            {
                numero_factura = datos.NumeroFactura,
                fecha_emision = datos.FechaEmision,
                fecha_recepcion = DateTime.Now,
                fecha_limite_pago = datos.FechaVencimiento,
                estado = "Pendiente",
                monto_total = datos.MontoTotal,
                monto_igv = datos.MontoImpuesto,
                monto_restante = datos.MontoTotal,
                regimen_fiscal = "General",
                tasa_iva = 0,
                otros_tributos = 0,
                acreedor = new SoftPacBusiness.FacturasWS.acreedoresDTO
                {
                    acreedor_id = acreedor.acreedor_id
                },
                moneda = new SoftPacBusiness.FacturasWS.monedasDTO
                {
                    moneda_id = moneda.moneda_id
                }
            };

            return factura;
        }

        private SoftPacBusiness.AcreedoresWS.acreedoresDTO CrearNuevoAcreedor(
            DatosFacturaXML datos, int paisId, AcreedoresBO acreedoresBO)
        {
            var nuevoAcreedor = new SoftPacBusiness.AcreedoresWS.acreedoresDTO
            {
                razon_social = datos.RazonSocial,
                ruc = datos.RucNitRfc,
                direccion_fiscal = "Sin especificar",
                condicion = "Contado",
                plazo_de_pago = 30,
                activo = true,
                pais = new SoftPacBusiness.AcreedoresWS.paisesDTO { pais_id = paisId }
            };

            int acreedorId = acreedoresBO.insertar(nuevoAcreedor.razon_social, nuevoAcreedor.ruc, nuevoAcreedor.direccion_fiscal,
                                                    nuevoAcreedor.condicion, nuevoAcreedor.plazo_de_pago, nuevoAcreedor.activo ? "S" : "N", nuevoAcreedor.pais.pais_id);
            nuevoAcreedor.acreedor_id = acreedorId;

            return nuevoAcreedor;
        }

        private void ConfigurarEspecificadoresFactura(facturasDTO factura)
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
        }

        private void AsignarEstadoResultado(ResultadoCargaDTO resultado, int resultadoId)
        {
            if (resultadoId > 0)
            {
                resultado.Estado = "Exitosa";
                resultado.Mensaje = "Factura creada correctamente";
            }
            else if (resultadoId == 0)
            {
                resultado.Estado = "Duplicada";
                resultado.Mensaje = "La factura ya existe";
            }
            else
            {
                resultado.Estado = "Fallida";
                resultado.Mensaje = "Error al insertar en base de datos";
            }
        }

        #endregion
    }

    #region Clases Auxiliares

      public class ResultadoCargaDTO
    {
        public string Estado { get; set; }
        public string NumeroFactura { get; set; }
        public string RazonSocial { get; set; }
        public decimal MontoTotal { get; set; }
        public string Mensaje { get; set; }
        public string Archivo { get; set; }  
    }


    public class DatosFacturaXML
    {
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string RucNitRfc { get; set; }
        public string RazonSocial { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoImpuesto { get; set; }
        public string CodigoMoneda { get; set; }
    }

    #endregion
}
