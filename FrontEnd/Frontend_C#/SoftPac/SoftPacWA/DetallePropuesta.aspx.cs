using SoftPac.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using SoftPacBusiness.UsuariosWS;
using SoftPacBusiness.PropuestaPagoWS;

namespace SoftPacWA
{
    public partial class DetallePropuesta : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();

        // Clase auxiliar para el GridView
        [Serializable]
        public class DetalleViewModel
        {
            public string NumeroFactura { get; set; }
            public string RazonSocialAcreedor { get; set; }
            public string CodigoMoneda { get; set; }
            public decimal Monto { get; set; }
            public string CuentaOrigen { get; set; }
            public string BancoOrigen { get; set; }
            public string CuentaDestino { get; set; }
            public string BancoDestino { get; set; }
            public string FormaPago { get; set; }
        }

        // Clase para exportación XML
        [Serializable]
        [XmlRoot("PropuestaPago")]
        public class PropuestaExportXML
        {
            public int PropuestaId { get; set; }
            public string Estado { get; set; }
            public string FechaCreacion { get; set; }
            public string EntidadBancaria { get; set; }
            public List<DetalleExportXML> Detalles { get; set; }
        }

        [Serializable]
        public class DetalleExportXML
        {
            public string NumeroFactura { get; set; }
            public string Proveedor { get; set; }
            public decimal Monto { get; set; }
            public string Moneda { get; set; }
            public string CuentaOrigen { get; set; }
            public string BancoOrigen { get; set; }
            public string CuentaDestino { get; set; }
            public string BancoDestino { get; set; }
            public string FormaPago { get; set; }
            public string Fecha { get; set; }
        }

        private int PropuestaId
        {
            get
            {
                int.TryParse(Request.QueryString["id"], out var id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PropuestaId <= 0)
                {
                    MostrarMensaje("ID de propuesta inválido", "danger");
                    btnEditar.Visible = false;
                    btnAnular.Visible = false;
                    return;
                }

                CargarDetalle();
            }
        }

        private void CargarDetalle()
        {
            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta solicitada", "danger");
                    btnEditar.Visible = false;
                    btnAnular.Visible = false;
                    return;
                }

                // Cargar información general
                lblPropuestaId.Text = propuesta.propuesta_id.ToString() ?? "-";
                lblEstado.Text = propuesta.estado ?? "-";
                lblEstado.CssClass = $"badge-estado estado-{propuesta.estado?.ToLower()}";
                lblFechaCreacion.Text = propuesta.fecha_hora_creacion.ToString("dd/MM/yyyy HH:mm") ?? "-";
                lblUsuarioCreador.Text = propuesta.usuario_creacion != null
                    ? $"{propuesta.usuario_creacion.nombre} {propuesta.usuario_creacion.apellidos}"
                    : "-";
                lblPais.Text = propuesta.entidad_bancaria?.pais?.nombre ?? "-";
                lblBanco.Text = propuesta.entidad_bancaria?.nombre ?? "-";
                lblTotalPagos.Text = propuesta.detalles_propuesta?.Length.ToString() ?? "0";

                // Controlar visibilidad de botones según estado
                ConfigurarBotonesPorEstado(propuesta.estado);

                // Cargar totales por moneda
                CargarTotalesPorMoneda(propuesta);

                // Cargar detalles
                CargarDetallesPagos(propuesta);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar el detalle de la propuesta: {ex.Message}", "danger");
            }
        }

        private void ConfigurarBotonesPorEstado(string estado)
        {
            // Solo se puede editar y anular propuestas en estado "Pendiente"
            bool esPendiente = estado == "Pendiente";
            btnEditar.Visible = esPendiente;
            btnAnular.Visible = esPendiente;

            if (esPendiente)
            {
                btnAnular.OnClientClick = "return mostrarModalAnular();";
            }
        }

        private void CargarTotalesPorMoneda(propuestasPagoDTO propuesta)
        {
            if (propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0)
                return;

            var totales = propuesta.detalles_propuesta
                .GroupBy(d => d.factura?.moneda?.codigo_iso ?? "-")
                .Select(g => new
                {
                    CodigoMoneda = g.Key,
                    Total = g.Sum(d => d.monto_pago)
                })
                .OrderBy(t => t.CodigoMoneda)
                .ToList();

            rptTotales.DataSource = totales;
            rptTotales.DataBind();
        }

        private void CargarDetallesPagos(propuestasPagoDTO propuesta)
        {
            if (propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0)
            {
                lblCantidadPagos.Text = "0 pago(s)";
                return;
            }

            var detallesVM = propuesta.detalles_propuesta
                .Select(d => new DetalleViewModel
                {
                    NumeroFactura = d.factura?.numero_factura ?? "-",
                    RazonSocialAcreedor = d.factura?.acreedor?.razon_social ?? "-",
                    CodigoMoneda = d.factura?.moneda?.codigo_iso ?? "-",
                    Monto = d.monto_pago,
                    CuentaOrigen = d.cuenta_propia?.numero_cuenta ?? "-",
                    BancoOrigen = d.cuenta_propia?.entidad_bancaria?.nombre ?? "-",
                    CuentaDestino = d.cuenta_acreedor?.numero_cuenta ?? "-",
                    BancoDestino = d.cuenta_acreedor?.entidad_bancaria?.nombre ?? "-",
                    FormaPago = MapearFormaPago((char?)d.forma_pago)
                })
                .ToList();

            gvDetalles.DataSource = detallesVM;
            gvDetalles.DataBind();

            lblCantidadPagos.Text = $"{detallesVM.Count} pago(s)";

            // Guardar en ViewState para paginación
            ViewState["DetallesViewModel"] = detallesVM;
        }

        private string MapearFormaPago(char? formaPago)
        {
            if (!formaPago.HasValue)
                return "-";

            switch (char.ToUpper(formaPago.Value))
            {
                case 'T':
                    return "Transferencia";
                case 'C':
                    return "Cheque";
                case 'E':
                    return "Efectivo";
                default:
                    return formaPago.ToString();
            }
        }

        protected void gvDetalles_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDetalles.PageIndex = e.NewPageIndex;

            var detallesVM = ViewState["DetallesViewModel"] as List<DetalleViewModel>;
            if (detallesVM != null)
            {
                gvDetalles.DataSource = detallesVM;
                gvDetalles.DataBind();
            }

            upDetalles.Update();
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/EditarPropuesta.aspx?id={PropuestaId}");
        }

        protected void btnAnular_Click(object sender, EventArgs e)
        {
            // Este botón abre el modal mediante JavaScript
            // La anulación real se hace en btnConfirmarAnulacion_Click
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                if (propuesta.entidad_bancaria == null)
                {
                    MostrarMensaje("La propuesta no tiene una entidad bancaria asignada", "danger");
                    return;
                }

                string formatoAceptado = propuesta.entidad_bancaria.formato_aceptado?.ToUpper() ?? "CSV";

                // Exportar según el formato de la entidad bancaria
                switch (formatoAceptado)
                {
                    case "CSV":
                        ExportarCSV(propuesta);
                        break;
                    case "XML":
                        ExportarXML(propuesta);
                        break;
                    case "TXT":
                        ExportarTXT(propuesta);
                        break;
                    case "MT101":
                        ExportarMT101(propuesta);
                        break;
                    case "PDF":
                        MostrarMensaje("La exportación a PDF aún no está implementada", "warning");
                        break;
                    default:
                        // Por defecto exportar como CSV
                        ExportarCSV(propuesta);
                        break;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al exportar la propuesta: {ex.Message}", "danger");
            }
        }

        private void ExportarCSV(propuestasPagoDTO propuesta)
        {
            var sb = new StringBuilder();

            // Encabezados
            sb.AppendLine("NumeroFactura,Proveedor,Moneda,Monto,CuentaOrigen,BancoOrigen,CuentaDestino,BancoDestino,FormaPago,Fecha");

            // Datos
            foreach (var detalle in propuesta.detalles_propuesta)
            {
                sb.AppendLine($"\"{detalle.factura?.numero_factura ?? ""}\",\"{detalle.factura?.acreedor?.razon_social ?? ""}\",\"{detalle.factura?.moneda?.codigo_iso ?? ""}\",{detalle.monto_pago:F2},\"{detalle.cuenta_propia?.numero_cuenta ?? ""}\",\"{detalle.cuenta_propia?.entidad_bancaria?.nombre ?? ""}\",\"{detalle.cuenta_acreedor?.numero_cuenta ?? ""}\",\"{detalle.cuenta_acreedor?.entidad_bancaria?.nombre ?? ""}\",\"{MapearFormaPago((char?)detalle.forma_pago)}\",\"{DateTime.Now:yyyy-MM-dd}\"");
            }

            // Descargar archivo
            DescargarArchivo(sb.ToString(), $"PropuestaPago_{propuesta.propuesta_id}.csv", "text/csv");
        }

        private void ExportarXML(propuestasPagoDTO propuesta)
        {
            var propuestaXML = new PropuestaExportXML
            {
                PropuestaId = propuesta.propuesta_idSpecified? propuesta.propuesta_id : 0,
                Estado = propuesta.estado ?? "",
                FechaCreacion = propuesta.fecha_hora_creacion.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                EntidadBancaria = propuesta.entidad_bancaria?.nombre ?? "",
                Detalles = propuesta.detalles_propuesta.Select(d => new DetalleExportXML
                {
                    NumeroFactura = d.factura?.numero_factura ?? "",
                    Proveedor = d.factura?.acreedor?.razon_social ?? "",
                    Monto = d.monto_pago,
                    Moneda = d.factura?.moneda?.codigo_iso ?? "",
                    CuentaOrigen = d.cuenta_propia?.numero_cuenta ?? "",
                    BancoOrigen = d.cuenta_propia?.entidad_bancaria?.nombre ?? "",
                    CuentaDestino = d.cuenta_acreedor?.numero_cuenta ?? "",
                    BancoDestino = d.cuenta_acreedor?.entidad_bancaria?.nombre ?? "",
                    FormaPago = MapearFormaPago((char?)d.forma_pago),
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd")
                }).ToList()
            };

            var serializer = new XmlSerializer(typeof(PropuestaExportXML));
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    serializer.Serialize(xmlWriter, propuestaXML);
                    DescargarArchivo(stringWriter.ToString(), $"PropuestaPago_{propuesta.propuesta_id}.xml", "application/xml");
                }
            }
        }

        private void ExportarTXT(propuestasPagoDTO propuesta)
        {
            var sb = new StringBuilder();

            // Formato de texto con ancho fijo
            sb.AppendLine("=".PadRight(120, '='));
            sb.AppendLine($"PROPUESTA DE PAGO #{propuesta.propuesta_id}".PadLeft(70));
            sb.AppendLine("=".PadRight(120, '='));
            sb.AppendLine($"Estado: {propuesta.estado}");
            sb.AppendLine($"Fecha: {propuesta.fecha_hora_creacion:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Banco: {propuesta.entidad_bancaria?.nombre}");
            sb.AppendLine($"Total de pagos: {propuesta.detalles_propuesta.Length}");
            sb.AppendLine("=".PadRight(120, '='));
            sb.AppendLine();

            // Encabezados con formato fijo
            sb.AppendLine($"{"Factura".PadRight(15)} {"Proveedor".PadRight(30)} {"Mon".PadRight(4)} {"Monto".PadLeft(12)} {"Cta.Origen".PadRight(20)} {"Cta.Destino".PadRight(20)}");
            sb.AppendLine("-".PadRight(120, '-'));

            // Datos
            foreach (var detalle in propuesta.detalles_propuesta)
            {
                string factura = (detalle.factura?.numero_factura ?? "").PadRight(15);
                string proveedor = TruncarTexto(detalle.factura?.acreedor?.razon_social ?? "", 30).PadRight(30);
                string moneda = (detalle.factura?.moneda?.codigo_iso ?? "").PadRight(4);
                string monto = detalle.monto_pago.ToString("N2").PadLeft(12);
                string cuentaOrigen = (detalle.cuenta_propia?.numero_cuenta ?? "").PadRight(20);
                string cuentaDestino = (detalle.cuenta_acreedor?.numero_cuenta ?? "").PadRight(20);

                sb.AppendLine($"{factura} {proveedor} {moneda} {monto} {cuentaOrigen} {cuentaDestino}");
            }

            sb.AppendLine("=".PadRight(120, '='));

            DescargarArchivo(sb.ToString(), $"PropuestaPago_{propuesta.propuesta_id}.txt", "text/plain");
        }

        private void ExportarMT101(propuestasPagoDTO propuesta)
        {
            // Formato MT101 (SWIFT) simplificado
            var sb = new StringBuilder();

            sb.AppendLine("{1:F01BANKXXXXAXXX0000000000}");
            sb.AppendLine("{2:I101BANKXXXXAXXXN}");
            sb.AppendLine("{4:");
            sb.AppendLine($":20:PROP{propuesta.propuesta_id:D10}");
            sb.AppendLine($":28D:1/{propuesta.detalles_propuesta.Length}");
            sb.AppendLine($":50H:/{propuesta.entidad_bancaria?.codigo_swift ?? ""}");
            sb.AppendLine($"{propuesta.entidad_bancaria?.nombre ?? ""}");
            sb.AppendLine($":30:{DateTime.Now:yyyyMMdd}");

            int secuencia = 1;
            foreach (var detalle in propuesta.detalles_propuesta)
            {
                sb.AppendLine($":21:{secuencia:D10}");
                sb.AppendLine($":32B:{detalle.factura?.moneda?.codigo_iso ?? "USD"}{detalle.monto_pago:F2}");
                sb.AppendLine($":50K:/{detalle.cuenta_propia?.numero_cuenta ?? ""}");
                sb.AppendLine($"{detalle.cuenta_propia?.entidad_bancaria?.nombre ?? ""}");
                sb.AppendLine($":59:/{detalle.cuenta_acreedor?.numero_cuenta ?? ""}");
                sb.AppendLine($"{detalle.factura?.acreedor?.razon_social ?? ""}");
                sb.AppendLine($":70:{detalle.factura?.numero_factura ?? ""}");
                secuencia++;
            }

            sb.AppendLine("-}");

            DescargarArchivo(sb.ToString(), $"PropuestaPago_{propuesta.propuesta_id}.mt101", "text/plain");
        }

        private string TruncarTexto(string texto, int maxLength)
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            return texto.Length <= maxLength ? texto : texto.Substring(0, maxLength - 3) + "...";
        }

        private void DescargarArchivo(string contenido, string nombreArchivo, string contentType)
        {
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");
            Response.ContentEncoding = Encoding.UTF8;
            Response.Write(contenido);
            Response.End();
        }

        protected void btnConfirmarAnulacion_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                if (propuesta.estado != "Pendiente")
                {
                    MostrarMensaje("Solo se pueden anular propuestas en estado Pendiente", "warning");
                    return;
                }

                // Actualizar estado y motivo
                propuesta.estado = "Anulada";
                // Aquí deberías tener un campo en el DTO para guardar el motivo de anulación
                // propuesta.MotivoAnulacion = txtMotivoAnulacion.Text;

                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                SoftPacBusiness.PropuestaPagoWS.usuariosDTO usuarioModificacion = new SoftPacBusiness.PropuestaPagoWS.usuariosDTO();
                usuarioModificacion.usuario_id = usuarioId;
                propuesta.usuario_modificacion = usuarioModificacion;
                propuesta.fecha_hora_modificacion = DateTime.Now;

                bool resultado = propuestasBO.Modificar(propuesta) == 1;

                if (resultado)
                {
                    // Cerrar modal y recargar
                    ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal",
                        "$('#modalAnular').modal('hide');", true);

                    MostrarMensaje("Propuesta anulada exitosamente", "success");
                    CargarDetalle();
                }
                else
                {
                    MostrarMensaje("Error al anular la propuesta", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al anular la propuesta: {ex.Message}", "danger");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }
    }
}