using SoftPac.Business;
using SoftPacBusiness.AcreedoresWS;
using SoftPacBusiness.FacturasWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;
using acreedoresDTO = SoftPacBusiness.FacturasWS.acreedoresDTO;

namespace SoftPacWA
{
    public partial class CargaFacturas : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();
        private AcreedoresBO acreedoresBO = new AcreedoresBO();
        private MonedasBO monedasBO = new MonedasBO();

        private usuariosDTO UsuarioLogueado
        {
            get { return (usuariosDTO)Session["UsuarioLogueado"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarPaises();
            }
        }

        private void CargarPaises()
        {
            ddlPais.DataSource = UsuarioLogueado.usuario_pais
                .Where(up => up.acceso == true)
                .Select(up => up.pais)
                .ToList();
            ddlPais.DataTextField = "nombre";
            ddlPais.DataValueField = "pais_id";
            ddlPais.DataBind();
            ddlPais.Items.Insert(0, new ListItem("-- Seleccione un País --", ""));
        }

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Limpiar acreedor
            ddlAcreedor.Items.Clear();
            ddlAcreedor.Items.Insert(0, new ListItem("-- Seleccione un Acreedor --", ""));
            pnlAcreedorInfo.Visible = false;

            if (!string.IsNullOrEmpty(ddlPais.SelectedValue))
            {
                // Cargar acreedores del país seleccionado
                int paisId = int.Parse(ddlPais.SelectedValue);
                var acreedores = acreedoresBO.ListarTodos()
                    .Where(a => a.pais.pais_id == paisId && a.activo)
                    .OrderBy(a => a.razon_social)
                    .ToList();

                ddlAcreedor.DataSource = acreedores;
                ddlAcreedor.DataTextField = "razon_social";
                ddlAcreedor.DataValueField = "acreedor_id";
                ddlAcreedor.DataBind();
                ddlAcreedor.Items.Insert(0, new ListItem("-- Seleccione un Acreedor --", ""));
            }
        }

        protected void ddlAcreedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlAcreedor.SelectedValue))
            {
                int acreedorId = int.Parse(ddlAcreedor.SelectedValue);
                var acreedor = acreedoresBO.obtenerPorId(acreedorId);

                if (acreedor != null)
                {
                    lblAcreedorRuc.Text = acreedor.ruc;
                    lblAcreedorRazon.Text = acreedor.razon_social;
                    pnlAcreedorInfo.Visible = true;
                }
            }
            else
            {
                pnlAcreedorInfo.Visible = false;
            }
        }

        protected void btnProcesar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrEmpty(ddlPais.SelectedValue))
                {
                    MostrarMensaje("Por favor seleccione un país", "warning");
                    return;
                }

                if (string.IsNullOrEmpty(ddlAcreedor.SelectedValue))
                {
                    MostrarMensaje("Por favor seleccione un acreedor", "warning");
                    return;
                }

                if (!fuXmlFile.HasFiles)
                {
                    MostrarMensaje("Por favor seleccione al menos un archivo XML", "warning");
                    return;
                }

                // Validar que todos sean archivos XML
                foreach (HttpPostedFile archivo in fuXmlFile.PostedFiles)
                {
                    if (!archivo.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        MostrarMensaje($"El archivo '{archivo.FileName}' no es un archivo XML válido", "danger");
                        return;
                    }
                }

                // Mostrar progreso
                pnlProgress.Visible = true;
                pnlResultados.Visible = false;

                // Obtener datos
                string paisNombre = ddlPais.SelectedItem.Text;
                int paisId = int.Parse(ddlPais.SelectedValue);
                int acreedorId = int.Parse(ddlAcreedor.SelectedValue);
                var acreedor = DTOConverter.Convertir<SoftPacBusiness.AcreedoresWS.acreedoresDTO,SoftPacBusiness.FacturasWS.acreedoresDTO>(acreedoresBO.obtenerPorId(acreedorId));

                // Procesar todos los archivos
                List<ResultadoCarga> resultadosTotales = new List<ResultadoCarga>();
                int archivosProcesados = 0;
                int archivosConError = 0;

                foreach (HttpPostedFile archivo in fuXmlFile.PostedFiles)
                {
                    try
                    {
                        // Leer archivo
                        string xmlContent = "";
                        using (StreamReader reader = new StreamReader(archivo.InputStream))
                        {
                            xmlContent = reader.ReadToEnd();
                        }

                        // Procesar facturas del archivo
                        List<ResultadoCarga> resultados = ProcesarFacturas(
                            xmlContent, paisNombre, paisId, acreedor);

                        // Agregar nombre del archivo a los resultados
                        foreach (var resultado in resultados)
                        {
                            resultado.Archivo = archivo.FileName;
                        }

                        resultadosTotales.AddRange(resultados);
                        archivosProcesados++;
                    }
                    catch (Exception ex)
                    {
                        archivosConError++;
                        resultadosTotales.Add(new ResultadoCarga
                        {
                            Estado = "Fallida",
                            NumeroFactura = "N/A",
                            RazonSocial = acreedor.razon_social,
                            MontoTotal = 0,
                            Mensaje = $"Error al procesar archivo '{archivo.FileName}': {ex.Message}",
                            Archivo = archivo.FileName
                        });
                    }
                }

                // Ocultar progreso y mostrar resultados
                pnlProgress.Visible = false;
                MostrarResultados(resultadosTotales, archivosProcesados, archivosConError);
            }
            catch (Exception ex)
            {
                pnlProgress.Visible = false;
                MostrarMensaje($"Error general al procesar archivos: {ex.Message}", "danger");
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        private List<ResultadoCarga> ProcesarFacturas(
            string xmlContent, string paisNombre, int paisId, acreedoresDTO acreedor)
        {
            List<ResultadoCarga> resultados = new List<ResultadoCarga>();

            try
            {
                switch (paisNombre)
                {
                    case "Perú":
                        resultados = ProcesarFacturasPerú(xmlContent, paisId, acreedor);
                        break;
                    case "México":
                        resultados = ProcesarFacturasMéxico(xmlContent, paisId, acreedor);
                        break;
                    case "Colombia":
                        resultados = ProcesarFacturasColombia(xmlContent, paisId, acreedor);
                        break;
                    default:
                        throw new Exception("País no soportado");
                }
            }
            catch (Exception ex)
            {
                ResultadoCarga error = new ResultadoCarga
                {
                    Estado = "Fallida",
                    NumeroFactura = "N/A",
                    RazonSocial = acreedor.razon_social,
                    MontoTotal = 0,
                    Mensaje = $"Error al parsear XML: {ex.Message}"
                };
                resultados.Add(error);
            }

            return resultados;
        }

        #region Procesadores por País

        private List<ResultadoCarga> ProcesarFacturasPerú(
            string xmlContent, int paisId, acreedoresDTO acreedor)
        {
            List<ResultadoCarga> resultados = new List<ResultadoCarga>();

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
                XNamespace xmlns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";

                var invoices = doc.Descendants(xmlns + "Invoice");

                foreach (var invoice in invoices)
                {
                    ResultadoCarga resultado = new ResultadoCarga();

                    try
                    {
                        // Extraer datos del XML
                        string numeroFactura = invoice.Element(cbc + "ID")?.Value;
                        DateTime fechaEmision = DateTime.Parse(invoice.Element(cbc + "IssueDate")?.Value);
                        DateTime fechaVencimiento = DateTime.Parse(
                            invoice.Element(cbc + "DueDate")?.Value ??
                            fechaEmision.AddDays(30).ToString("yyyy-MM-dd"));

                        var customerParty = invoice.Descendants(cac + "AccountingCustomerParty").FirstOrDefault();
                        string rucXml = customerParty?.Descendants(cbc + "ID").FirstOrDefault()?.Value;
                        string razonSocialXml = customerParty?.Descendants(cbc + "RegistrationName").FirstOrDefault()?.Value;

                        decimal montoTotal = decimal.Parse(
                            invoice.Descendants(cbc + "PayableAmount").FirstOrDefault()?.Value ?? "0");
                        decimal montoIGV = decimal.Parse(
                            invoice.Descendants(cbc + "TaxAmount").FirstOrDefault()?.Value ?? "0");
                        string codigoMoneda = invoice.Descendants(cbc + "DocumentCurrencyCode").FirstOrDefault()?.Value ?? "PEN";

                        resultado.NumeroFactura = numeroFactura;
                        resultado.RazonSocial = acreedor.razon_social;
                        resultado.MontoTotal = montoTotal;

                        // VALIDACIÓN: Verificar que el RUC coincida
                        if ((rucXml+="  ") != acreedor.ruc)
                        {
                            resultado.Estado = "Fallida";
                            resultado.Mensaje = $"RUC no coincide. XML: {rucXml}, Esperado: {acreedor.ruc}";
                            resultados.Add(resultado);
                            continue;
                        }

                        // Crear factura
                        facturasDTO factura = CrearFacturaDesdeXML(
                            numeroFactura, fechaEmision, fechaVencimiento,
                            acreedor, montoTotal, montoIGV, codigoMoneda);

                        // Insertar en BD
                        int resultadoId = facturasBO.Insertar(factura);

                        if (resultadoId > 0)
                        {
                            resultado.Estado = "Exitosa";
                            resultado.Mensaje = "Factura creada correctamente";
                        }
                        else if (resultadoId == 0)
                        {
                            resultado.Estado = "Duplicada";
                            resultado.Mensaje = "La factura ya existe en el sistema";
                        }
                        else
                        {
                            resultado.Estado = "Fallida";
                            resultado.Mensaje = "Error al insertar en base de datos";
                        }
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

        private List<ResultadoCarga> ProcesarFacturasMéxico(
            string xmlContent, int paisId, acreedoresDTO acreedor)
        {
            List<ResultadoCarga> resultados = new List<ResultadoCarga>();

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace cfdi = "http://www.sat.gob.mx/cfd/3";

                var comprobantes = doc.Descendants(cfdi + "Comprobante");

                foreach (var comprobante in comprobantes)
                {
                    ResultadoCarga resultado = new ResultadoCarga();

                    try
                    {
                        string serie = comprobante.Attribute("Serie")?.Value ?? "";
                        string folio = comprobante.Attribute("Folio")?.Value ?? "";
                        string numeroFactura = $"{serie}{folio}";

                        DateTime fechaEmision = DateTime.Parse(comprobante.Attribute("Fecha")?.Value);
                        DateTime fechaVencimiento = fechaEmision.AddDays(30);

                        var receptor = comprobante.Element(cfdi + "Receptor");
                        string rfcXml = receptor?.Attribute("Rfc")?.Value;
                        string razonSocialXml = receptor?.Attribute("Nombre")?.Value;

                        decimal montoTotal = decimal.Parse(comprobante.Attribute("Total")?.Value ?? "0");
                        decimal montoIVA = decimal.Parse(
                            comprobante.Descendants(cfdi + "Traslado")
                            .Where(t => t.Attribute("Impuesto")?.Value == "002")
                            .FirstOrDefault()?.Attribute("Importe")?.Value ?? "0");
                        string codigoMoneda = comprobante.Attribute("Moneda")?.Value ?? "MXN";

                        resultado.NumeroFactura = numeroFactura;
                        resultado.RazonSocial = acreedor.razon_social;
                        resultado.MontoTotal = montoTotal;

                        if (rfcXml.Length == 12) rfcXml += " ";

                        // VALIDACIÓN: Verificar que el RFC coincida
                        if ((rfcXml) != acreedor.ruc)
                        {
                            resultado.Estado = "Fallida";
                            resultado.Mensaje = $"RFC no coincide. XML: {rfcXml}, Esperado: {acreedor.ruc}";
                            resultados.Add(resultado);
                            continue;
                        }

                        // Crear factura
                        facturasDTO factura = CrearFacturaDesdeXML(
                            numeroFactura, fechaEmision, fechaVencimiento,
                            acreedor, montoTotal, montoIVA, codigoMoneda);

                        // Insertar en BD
                        int resultadoId = facturasBO.Insertar(factura);

                        if (resultadoId > 0)
                        {
                            resultado.Estado = "Exitosa";
                            resultado.Mensaje = "Factura creada correctamente";
                        }
                        else if (resultadoId == 0)
                        {
                            resultado.Estado = "Duplicada";
                            resultado.Mensaje = "La factura ya existe en el sistema";
                        }
                        else
                        {
                            resultado.Estado = "Fallida";
                            resultado.Mensaje = "Error al insertar en base de datos";
                        }
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

        private List<ResultadoCarga> ProcesarFacturasColombia(
            string xmlContent, int paisId, acreedoresDTO acreedor)
        {
            List<ResultadoCarga> resultados = new List<ResultadoCarga>();

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace xmlns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
                XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
                XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

                var invoices = doc.Descendants(xmlns + "Invoice");

                foreach (var invoice in invoices)
                {
                    ResultadoCarga resultado = new ResultadoCarga();

                    try
                    {
                        string numeroFactura = invoice.Element(cbc + "ID")?.Value;
                        DateTime fechaEmision = DateTime.Parse(invoice.Element(cbc + "IssueDate")?.Value);
                        DateTime fechaVencimiento = DateTime.Parse(
                            invoice.Element(cbc + "DueDate")?.Value ??
                            fechaEmision.AddDays(30).ToString("yyyy-MM-dd"));

                        var customerParty = invoice.Descendants(cac + "AccountingCustomerParty").FirstOrDefault();
                        string nitXml = customerParty?.Descendants(cbc + "ID").FirstOrDefault()?.Value;
                        string razonSocialXml = customerParty?.Descendants(cbc + "RegistrationName").FirstOrDefault()?.Value;

                        decimal montoTotal = decimal.Parse(
                            invoice.Descendants(cbc + "PayableAmount").FirstOrDefault()?.Value ?? "0");
                        decimal montoIVA = decimal.Parse(
                            invoice.Descendants(cbc + "TaxAmount").FirstOrDefault()?.Value ?? "0");
                        string codigoMoneda = invoice.Descendants(cbc + "DocumentCurrencyCode").FirstOrDefault()?.Value ?? "COP";

                        resultado.NumeroFactura = numeroFactura;
                        resultado.RazonSocial = acreedor.razon_social;
                        resultado.MontoTotal = montoTotal;

                        // VALIDACIÓN: Verificar que el NIT coincida
                        if ((nitXml+="  ") != acreedor.ruc)
                        {
                            resultado.Estado = "Fallida";
                            resultado.Mensaje = $"NIT no coincide. XML: {nitXml}, Esperado: {acreedor.ruc}";
                            resultados.Add(resultado);
                            continue;
                        }

                        // Crear factura
                        facturasDTO factura = CrearFacturaDesdeXML(
                            numeroFactura, fechaEmision, fechaVencimiento,
                            acreedor, montoTotal, montoIVA, codigoMoneda);

                        // Insertar en BD
                        int resultadoId = facturasBO.Insertar(factura);

                        if (resultadoId > 0)
                        {
                            resultado.Estado = "Exitosa";
                            resultado.Mensaje = "Factura creada correctamente";
                        }
                        else if (resultadoId == 0)
                        {
                            resultado.Estado = "Duplicada";
                            resultado.Mensaje = "La factura ya existe en el sistema";
                        }
                        else
                        {
                            resultado.Estado = "Fallida";
                            resultado.Mensaje = "Error al insertar en base de datos";
                        }
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

        private facturasDTO CrearFacturaDesdeXML(
            string numeroFactura, DateTime fechaEmision, DateTime fechaVencimiento,
            acreedoresDTO acreedor, decimal montoTotal, decimal montoImpuesto, string codigoMoneda)
        {
            // Buscar moneda
            var moneda = monedasBO.ListarTodos().FirstOrDefault(m => m.codigo_iso == codigoMoneda);
            if (moneda == null)
            {
                // Por defecto usar la moneda del país
                string codigoDefault = acreedor.pais.pais_id == 1 ? "PEN" :
                    (acreedor.pais.pais_id == 2 ? "MXN" : "COP");
                moneda = monedasBO.ListarTodos().FirstOrDefault(m => m.codigo_iso == codigoDefault);
            }

            // Crear factura
            facturasDTO factura = new facturasDTO();
            factura.numero_factura = numeroFactura;
            factura.fecha_emision = fechaEmision;
            factura.fecha_recepcion = DateTime.Now;
            factura.fecha_limite_pago = fechaVencimiento;
            factura.estado = "Pendiente";
            factura.monto_total = montoTotal;
            factura.monto_igv = montoImpuesto;
            factura.monto_restante = montoTotal;
            factura.regimen_fiscal = "General";
            factura.tasa_iva = 0;
            factura.otros_tributos = 0;

            SoftPacBusiness.FacturasWS.acreedoresDTO acreedorFactura = new SoftPacBusiness.FacturasWS.acreedoresDTO();
            acreedorFactura.acreedor_id = acreedor.acreedor_id;
            factura.acreedor = acreedorFactura;

            SoftPacBusiness.FacturasWS.monedasDTO monedaFactura = new SoftPacBusiness.FacturasWS.monedasDTO();
            monedaFactura.moneda_id = moneda.moneda_id;
            factura.moneda = monedaFactura;

            return factura;
        }

        private void MostrarResultados(List<ResultadoCarga> resultados, int archivosProcesados, int archivosConError)
        {
            pnlResultados.Visible = true;

            int total = resultados.Count;
            int exitosas = resultados.Count(r => r.Estado == "Exitosa");
            int fallidas = resultados.Count(r => r.Estado == "Fallida");
            int duplicadas = resultados.Count(r => r.Estado == "Duplicada");

            lblTotalProcesadas.Text = total.ToString();
            lblExitosas.Text = exitosas.ToString();
            lblFallidas.Text = fallidas.ToString();
            lblDuplicadas.Text = duplicadas.ToString();

            gvResultados.DataSource = resultados;
            gvResultados.DataBind();

            string tipoMensaje = archivosConError == 0 ? (exitosas == total ? "success" : "warning") : "danger";
            string mensaje = $"Procesados {archivosProcesados} archivo(s). Facturas: {exitosas} exitosas, {fallidas} fallidas, {duplicadas} duplicadas";
            if (archivosConError > 0)
            {
                mensaje += $". {archivosConError} archivo(s) con errores";
            }
            MostrarMensaje(mensaje, tipoMensaje);
        }

        protected string GetEstadoBadge(string estado)
        {
            switch (estado)
            {
                case "Exitosa": return "success";
                case "Fallida": return "danger";
                case "Duplicada": return "warning";
                default: return "secondary";
            }
        }

        protected void btnNuevaCarga_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void btnVerFacturas_Click(object sender, EventArgs e)
        {
            Response.Redirect("Facturas.aspx");
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string mensajeEscapado = mensaje.Replace("'", "\\'").Replace("\"", "\\\"");
            string script = $@"
                $(document).ready(function() {{
                    var alertHtml = '<div class=""alert alert-{tipo} alert-dismissible fade show animated"" role=""alert"">' +
                        '<button type=""button"" class=""btn-close"" data-bs-dismiss=""alert""></button>' +
                        '{mensajeEscapado}' +
                        '</div>';
                    $('.page-title').after(alertHtml);
                    setTimeout(function() {{
                        $('.alert').fadeOut();
                    }}, 5000);
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje", script, true);
        }
    }

    public class ResultadoCarga
    {
        public string Estado { get; set; }
        public string NumeroFactura { get; set; }
        public string RazonSocial { get; set; }
        public decimal MontoTotal { get; set; }
        public string Mensaje { get; set; }
        public string Archivo { get; set; }  // Nueva propiedad para identificar el archivo
    }
}
