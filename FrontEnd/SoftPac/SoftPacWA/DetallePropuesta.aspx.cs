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
        private CuentasPropiasBO cuentasPropiasBO = new CuentasPropiasBO();

        private SoftPacBusiness.UsuariosWS.usuariosDTO UsuarioLogueado
        {
            get
            {
                return (SoftPacBusiness.UsuariosWS.usuariosDTO)Session["UsuarioLogueado"];
            }
        }

        // Clase auxiliar para Cuentas Propias
        [Serializable]
        public class CuentaPropiaViewModel
        {
            public int CuentaId { get; set; }
            public string Banco { get; set; }
            public string NumeroCuenta { get; set; }
            public decimal SaldoDisponible { get; set; }
            public decimal SaldoUsado { get; set; }
            public bool SaldoInsuficiente { get; set; }
        }

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

                // Cargar cuentas propiasif
                if (propuesta.estado?.ToLower() == "pendiente" && propuesta.detalles_propuesta != null && propuesta.detalles_propuesta.Length!=0)
                {
                    CargarCuentasPropias(propuesta);
                }
                else
                {
                    // Ocultar toda la card de cuentas (si le agregas runat="server" al div)
                    // o simplemente no cargar datos y el EmptyDataTemplate se mostrará
                    pnlCuentasPropias.Visible = false;
                    gvCuentasPropias.DataSource = null;
                    gvCuentasPropias.DataBind();
                }

                if (propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0)
                {
                    pnlTotalesMoneda.Visible = false;
                }
                else pnlTotalesMoneda.Visible = true;

                // Validar alertas
                if (propuesta.estado.ToLower() == "pendiente")
                    ValidarAlertas(propuesta);

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
            btnEnviar.Visible = esPendiente;
            if (esPendiente)
            {
                btnAnular.OnClientClick = "return mostrarModalAnular();";
            }
        }

        private void CargarCuentasPropias(propuestasPagoDTO propuesta)
        {
            if (propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0)
            {
                gvCuentasPropias.DataSource = null;
                gvCuentasPropias.DataBind();
                return;
            }

            // Agrupar por cuenta propia
            var cuentasVM = propuesta.detalles_propuesta
                .Where(d => d.cuenta_propia != null && d.fecha_eliminacionSpecified == false)
                .GroupBy(d => d.cuenta_propia.cuenta_bancaria_id)
                .Select(g => {
                    var cuenta = g.First().cuenta_propia;
                    decimal saldoUsado = g.Sum(d => d.monto_pago);
                    return new CuentaPropiaViewModel
                    {
                        CuentaId = cuenta.cuenta_bancaria_id,
                        Banco = cuenta.entidad_bancaria?.nombre ?? "-",
                        NumeroCuenta = cuenta.numero_cuenta ?? "-",
                        SaldoDisponible = cuenta.saldo_disponible,
                        SaldoUsado = saldoUsado,
                        SaldoInsuficiente = cuenta.saldo_disponible < saldoUsado
                    };
                })
                .ToList();

            gvCuentasPropias.DataSource = cuentasVM;
            gvCuentasPropias.DataBind();

            // Guardar en ViewState
            ViewState["CuentasViewModel"] = cuentasVM;
        }

        private void CargarTotalesPorMoneda(propuestasPagoDTO propuesta)
        {
            if (propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0)
                return;

            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToList();

            var totales = detallesActivos
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

            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToList();

            var detallesVM = detallesActivos
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

        private void ValidarAlertas(propuestasPagoDTO propuesta)
        {

            if (propuesta.detalles_propuesta == null || propuesta.detalles_propuesta.Length == 0)
            {
                pnlAlertaFacturasPagadas.Visible = false;
                pnlAlertaMontosDistintos.Visible = false;

                MostrarMensaje("La propuesta no tiene pagos. Considere anularla.", "warning");

                // Deshabilitar todos los botones excepto anular
                btnEditar.Visible = false;
                btnEnviar.Visible = false;
                btnExportar.Visible = false;
                btnAnular.Visible = true; // Solo anular visible

                return;
            }

            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToList();

            // Verificar facturas pagadas o eliminadas
            bool hayFacturasPagadas = detallesActivos
                .Any(d => d.factura != null &&
                         (d.factura.estado != "Pendiente" || d.factura.monto_restante == 0));

            pnlAlertaFacturasPagadas.Visible = hayFacturasPagadas;

            // Verificar montos distintos (solo en facturas pendientes con monto restante > 0)
            bool hayMontosDistintos = detallesActivos
                .Any(d => d.factura != null &&
                         d.factura.estado == "Pendiente" &&
                         d.factura.monto_restante > 0 &&
                         Math.Abs(d.factura.monto_restante - d.monto_pago) > 0.01m);

            pnlAlertaMontosDistintos.Visible = hayMontosDistintos;
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

        protected void gvCuentasPropias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ActualizarSaldo")
            {
                try
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = gvCuentasPropias.Rows[index];

                    int cuentaId = Convert.ToInt32(gvCuentasPropias.DataKeys[index].Value);
                    TextBox txtSaldo = (TextBox)row.FindControl("txtSaldoNuevo");

                    if (txtSaldo != null && !string.IsNullOrWhiteSpace(txtSaldo.Text))
                    {
                        decimal nuevoSaldo;
                        if (decimal.TryParse(txtSaldo.Text, out nuevoSaldo) && nuevoSaldo >= 0)
                        {
                            // Obtener el DTO de la cuenta
                            var cuenta = cuentasPropiasBO.ObtenerPorId(cuentaId);

                            if (cuenta != null)
                            {
                                cuenta.saldo_disponible = nuevoSaldo;

                                if (cuentasPropiasBO.Modificar(cuenta) == 1)
                                {
                                    MostrarMensaje("Saldo actualizado correctamente", "success");
                                    CargarDetalle();
                                }
                                else
                                {
                                    MostrarMensaje("Error al actualizar el saldo", "danger");
                                }
                            }
                        }
                        else
                        {
                            MostrarMensaje("Ingrese un saldo válido", "warning");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"Error al actualizar saldo: {ex.Message}", "danger");
                }
            }
        }

        protected void gvCuentasPropias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var cuenta = (CuentaPropiaViewModel)e.Row.DataItem;

                // Aplicar estilo rojo si el saldo es insuficiente
                if (cuenta.SaldoInsuficiente)
                {
                    Label lblSaldoDisponible = (Label)e.Row.FindControl("lblSaldoDisponible");
                    if (lblSaldoDisponible != null)
                    {
                        lblSaldoDisponible.CssClass = "text-danger fw-bold";
                    }
                }
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

        protected void btnEliminarPagos_Click(object sender, EventArgs e)
        {
            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null || propuesta.detalles_propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                var usuarioDTO = DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO,
                    SoftPacBusiness.PropuestaPagoWS.usuariosDTO>(UsuarioLogueado);

                // Marcar como eliminados los detalles con facturas pagadas/eliminadas
                foreach (var detalle in propuesta.detalles_propuesta)
                {
                    if (detalle.factura != null &&
                        (detalle.factura.estado != "Pendiente" || detalle.factura.monto_restante == 0))
                    {
                        detalle.fecha_eliminacion = DateTime.Now;
                        detalle.fecha_eliminacionSpecified = true;
                        detalle.usuario_eliminacion = usuarioDTO;
                    }
                }

                if (propuestasBO.Modificar(propuesta) == 1)
                {
                    MostrarMensaje("Pagos eliminados correctamente", "success");
                    CargarDetalle();
                }
                else
                {
                    MostrarMensaje("Error al eliminar los pagos", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al eliminar pagos: {ex.Message}", "danger");
            }
        }

        protected void btnActualizarMontos_Click(object sender, EventArgs e)
        {
            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null || propuesta.detalles_propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                // Actualizar montos donde no coincidan (solo facturas pendientes)
                foreach (var detalle in propuesta.detalles_propuesta)
                {
                    if (detalle.factura != null &&
                        detalle.factura.estado == "Pendiente" &&
                        detalle.factura.monto_restante > 0 &&
                        Math.Abs(detalle.factura.monto_restante - detalle.monto_pago) > 0.01m)
                    {
                        detalle.monto_pago = detalle.factura.monto_restante;
                    }
                }

                if (propuestasBO.Modificar(propuesta) == 1)
                {
                    MostrarMensaje("Montos actualizados correctamente", "success");
                    CargarDetalle();
                }
                else
                {
                    MostrarMensaje("Error al actualizar los montos", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al actualizar montos: {ex.Message}", "danger");
            }
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

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                var propuesta = propuestasBO.ObtenerPorId(PropuestaId);

                if (propuesta == null || propuesta.detalles_propuesta == null)
                {
                    MostrarMensaje("No se encontró la propuesta", "danger");
                    return;
                }

                // Validar saldos de cuentas propias
                var cuentasConProblemas = propuesta.detalles_propuesta
                    .Where(d => d.cuenta_propia != null && d.fecha_eliminacionSpecified == false)
                    .GroupBy(d => d.cuenta_propia.cuenta_bancaria_id)
                    .Select(g => new
                    {
                        Cuenta = g.First().cuenta_propia,
                        SaldoUsado = g.Sum(d => d.monto_pago)
                    })
                    .Where(c => c.Cuenta.saldo_disponible < c.SaldoUsado)
                    .ToList();

                if (cuentasConProblemas.Any())
                {
                    MostrarMensaje("No se puede enviar la propuesta. Hay cuentas con saldo insuficiente (marcadas en rojo).", "danger");
                    return;
                }

                // Intentar confirmar envío
                if (propuestasBO.confirmarEnvioPropuesta(PropuestaId,
                    DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO,
                    SoftPacBusiness.PropuestaPagoWS.usuariosDTO>(UsuarioLogueado)) == 1)
                {
                    MostrarMensaje("Se envió la propuesta correctamente.", "success");
                    CargarDetalle();
                }
                else
                {
                    MostrarMensaje("La propuesta no pudo enviarse correctamente, revise el saldo disponible en las cuentas propias.", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al enviar la propuesta: {ex.Message}", "danger");
            }
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


            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false);

            foreach (var detalle in detallesActivos)
            {
                string numeroFactura = (detalle.factura?.numero_factura ?? "").Replace("-", "");

                string proveedor = RemoverTildes(detalle.factura?.acreedor?.razon_social ?? "");
                string moneda = detalle.factura?.moneda?.codigo_iso ?? "";

                string monto = detalle.monto_pago.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

                string cuentaOrigen = detalle.cuenta_propia?.numero_cuenta ?? "";
                string bancoOrigen = RemoverTildes(detalle.cuenta_propia?.entidad_bancaria?.nombre ?? "");
                string cuentaDestino = detalle.cuenta_acreedor?.numero_cuenta ?? "";
                string bancoDestino = RemoverTildes(detalle.cuenta_acreedor?.entidad_bancaria?.nombre ?? "");
                string formaPago = RemoverTildes(MapearFormaPago((char?)detalle.forma_pago));

                string fecha = DateTime.Now.ToString("yyyyMMdd");

                sb.AppendLine($"{numeroFactura},{proveedor},{moneda},{monto},{cuentaOrigen},{bancoOrigen},{cuentaDestino},{bancoDestino},{formaPago},{fecha}");
            }

            DescargarArchivo(sb.ToString(), $"PropuestaPago_{propuesta.propuesta_id}.csv", "text/csv");
        }

        private string RemoverTildes(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            string normalized = texto.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        private void ExportarXML(propuestasPagoDTO propuesta)
        {
            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToList();

            var propuestaXML = new PropuestaExportXML
            {
                PropuestaId = propuesta.propuesta_idSpecified ? propuesta.propuesta_id : 0,
                Estado = propuesta.estado ?? "",
                FechaCreacion = propuesta.fecha_hora_creacion.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                EntidadBancaria = propuesta.entidad_bancaria?.nombre ?? "",
                Detalles = detallesActivos.Select(d => new DetalleExportXML
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
            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToArray();

            // Formato de texto con ancho fijo
            sb.AppendLine("=".PadRight(120, '='));
            sb.AppendLine($"PROPUESTA DE PAGO #{propuesta.propuesta_id}".PadLeft(70));
            sb.AppendLine("=".PadRight(120, '='));
            sb.AppendLine($"Estado: {propuesta.estado}");
            sb.AppendLine($"Fecha: {propuesta.fecha_hora_creacion:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Banco: {propuesta.entidad_bancaria?.nombre}");
            sb.AppendLine($"Total de pagos: {detallesActivos.Length}");
            sb.AppendLine("=".PadRight(120, '='));
            sb.AppendLine();

            // Encabezados con formato fijo
            sb.AppendLine($"{"Factura".PadRight(15)} {"Proveedor".PadRight(30)} {"Mon".PadRight(4)} {"Monto".PadLeft(12)} {"Cta.Origen".PadRight(20)} {"Cta.Destino".PadRight(20)}");
            sb.AppendLine("-".PadRight(120, '-'));

            // Datos
            foreach (var detalle in detallesActivos)
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
            var detallesActivos = propuesta.detalles_propuesta
                .Where(d => d.fecha_eliminacionSpecified == false)
                .ToArray();

            // Formato MT101 (SWIFT) simplificado
            var sb = new StringBuilder();

            sb.AppendLine("{1:F01BANKXXXXAXXX0000000000}");
            sb.AppendLine("{2:I101BANKXXXXAXXXN}");
            sb.AppendLine("{4:");
            sb.AppendLine($":20:PROP{propuesta.propuesta_id:D10}");
            sb.AppendLine($":28D:1/{detallesActivos.Length}");
            sb.AppendLine($":50H:/{propuesta.entidad_bancaria?.codigo_swift ?? ""}");
            sb.AppendLine($"{propuesta.entidad_bancaria?.nombre ?? ""}");
            sb.AppendLine($":30:{DateTime.Now:yyyyMMdd}");

            int secuencia = 1;
            foreach (var detalle in detallesActivos)
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

                // Actualizar estado
                propuesta.estado = "Anulada";

                propuesta.usuario_modificacion = DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO, SoftPacBusiness.PropuestaPagoWS.usuariosDTO>(UsuarioLogueado);
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