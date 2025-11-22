using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace SoftPacWA
{
    public partial class Default : System.Web.UI.Page
    {
        // Instanciar los Business Objects
        private FacturasBO facturasBO = new FacturasBO();
        private CuentasPropiasBO cuentasBO = new CuentasPropiasBO();
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] != null)
                {
                    CargarDatosUsuario();
                    CargarDashboard();

                    // Notificar que el dashboard se actualizó (para limpiar flag si existe)
                    Session["DashboardNeedsRefresh"] = false;
                }
                else
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        private void CargarDatosUsuario()
        {
            usuariosDTO usuario = (usuariosDTO)Session["UsuarioLogueado"];
            litNombreUsuario.Text = Server.HtmlEncode(usuario.nombre + " " + usuario.apellidos);
        }

        private void CargarDashboard()
        {
            try
            {
                // Obtener datos del usuario logueado
                usuariosDTO usuario = (usuariosDTO)Session["UsuarioLogueado"];

                // Obtener los países a los que el usuario tiene acceso
                var paisesUsuario = usuario.usuario_pais
                    .Where(up => up.acceso == true)
                    .Select(up => up.pais.pais_id)
                    .ToList();

                // Total de facturas (filtradas por países del usuario)
                var todasFacturas = facturasBO.ListarTodos()
                    .Where(f => f.acreedor != null &&
                                f.acreedor.pais != null &&
                                paisesUsuario.Contains(f.acreedor.pais.pais_id))
                    .ToList();

                lblTotalFacturas.Text = todasFacturas.Count.ToString();

                // Facturas pendientes
                var facturasPendientes = todasFacturas
                    .Count(f => f.estado != null && f.estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase));
                lblFacturasPendientes.Text = facturasPendientes.ToString();

                // Total de cuentas propias
                var todasCuentas = cuentasBO.ListarTodos().ToList();
                lblTotalCuentas.Text = todasCuentas.Count.ToString();

                // Cuentas activas
                var cuentasActivas = todasCuentas.Count(c => c.activa);
                lblCuentasActivas.Text = cuentasActivas.ToString();

                // Total de propuestas generadas por el usuario
                var todasPropuestas = propuestasBO.ListarUltimasPorUsuario(usuario.usuario_id, 100);
                lblTotalPropuestas.Text = todasPropuestas != null ? todasPropuestas.Count.ToString() : "0";

                // Facturas pagadas
                var facturasPagadas = todasFacturas
                    .Count(f => f.estado != null && f.estado.Equals("Pagada", StringComparison.OrdinalIgnoreCase));
                lblFacturasPagadas.Text = facturasPagadas.ToString();

                // Facturas vencidas
                var facturasVencidas = todasFacturas
                    .Count(f => f.estado != null && f.estado.Equals("Vencida", StringComparison.OrdinalIgnoreCase));
                lblFacturasVencidas.Text = facturasVencidas.ToString();

                // Propuestas este mes
                var propuestasEsteMes = 0;
                if (todasPropuestas != null)
                {
                    propuestasEsteMes = todasPropuestas
                        .Count(p => p.fecha_hora_creacion.Month == DateTime.Now.Month &&
                                   p.fecha_hora_creacion.Year == DateTime.Now.Year);
                }
                lblPropuestasEsteMes.Text = propuestasEsteMes.ToString();

                // Facturas por mes (últimos 6 meses)
                var fechaInicio = DateTime.Now.AddMonths(-6);
                var facturasPorMes = todasFacturas
                    .Where(f => f.fecha_emision >= fechaInicio)
                    .GroupBy(f => new { f.fecha_emision.Year, f.fecha_emision.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new
                    {
                        Mes = ObtenerNombreMes(g.Key.Month) + " " + g.Key.Year.ToString().Substring(2),
                        Cantidad = g.Count()
                    })
                    .ToList();

                // Llenar con ceros los meses sin datos
                var mesesLabels = new List<string>();
                var mesesData = new List<int>();

                for (int i = 5; i >= 0; i--)
                {
                    var mes = DateTime.Now.AddMonths(-i);
                    var nombreMes = ObtenerNombreMes(mes.Month) + " " + mes.Year.ToString().Substring(2);
                    mesesLabels.Add(nombreMes);

                    var cantidad = facturasPorMes
                        .FirstOrDefault(f => f.Mes == nombreMes)?.Cantidad ?? 0;
                    mesesData.Add(cantidad);
                }

                // Bancos más utilizados en cuentas propias (top 5)
                var bancosMasUsados = todasCuentas
                    .Where(c => c.entidad_bancaria != null && c.entidad_bancaria.nombre != null)
                    .GroupBy(c => c.entidad_bancaria.nombre)
                    .Select(g => new
                    {
                        Nombre = g.Key,
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(b => b.Cantidad)
                    .Take(5)
                    .ToList();

                var bancosLabels = bancosMasUsados.Select(b => b.Nombre).ToList();
                var bancosData = bancosMasUsados.Select(b => b.Cantidad).ToList();

                // Si no hay datos de bancos, agregar datos de ejemplo
                if (bancosLabels.Count == 0)
                {
                    bancosLabels.Add("Sin datos");
                    bancosData.Add(0);
                }

                // Propuestas por estado (para gráfico de dona)
                var propuestasPorEstado = new Dictionary<string, int>
                {
                    { "Pendiente", 0 },
                    { "Enviada", 0 },
                    { "Anulada", 0 }
                };

                if (todasPropuestas != null)
                {
                    foreach (var propuesta in todasPropuestas)
                    {
                        var estado = propuesta.estado ?? "Pendiente";
                        if (propuestasPorEstado.ContainsKey(estado))
                        {
                            propuestasPorEstado[estado]++;
                        }
                    }
                }

                var propuestasLabels = propuestasPorEstado.Keys.ToList();
                var propuestasData = propuestasPorEstado.Values.ToList();

                // Serializar a JSON para JavaScript
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                hfMesesLabels.Value = serializer.Serialize(mesesLabels);
                hfMesesData.Value = serializer.Serialize(mesesData);
                hfBancosLabels.Value = serializer.Serialize(bancosLabels);
                hfBancosData.Value = serializer.Serialize(bancosData);
                hfPropuestasLabels.Value = serializer.Serialize(propuestasLabels);
                hfPropuestasData.Value = serializer.Serialize(propuestasData);


                // Calcular cambio porcentual de facturas (mes actual vs mes anterior)
                var facturasMesActual = todasFacturas
                    .Count(f => f.fecha_emision.Month == DateTime.Now.Month &&
                               f.fecha_emision.Year == DateTime.Now.Year);

                var facturasMesAnterior = todasFacturas
                    .Count(f => f.fecha_emision.Month == DateTime.Now.AddMonths(-1).Month &&
                               f.fecha_emision.Year == DateTime.Now.AddMonths(-1).Year);

                if (facturasMesAnterior > 0)
                {
                    var cambio = ((facturasMesActual - facturasMesAnterior) * 100.0 / facturasMesAnterior);
                    lblCambioFacturas.Text = (cambio >= 0 ? "+" : "") + cambio.ToString("F1") + "%";
                }
                else if (facturasMesActual > 0)
                {
                    lblCambioFacturas.Text = "+100%";
                }
                else
                {
                    lblCambioFacturas.Text = "0%";
                }

                // Última actualización
                lblUltimaActualizacion.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Debug.WriteLine($"Error al cargar dashboard: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Mostrar valores por defecto en caso de error
                lblTotalFacturas.Text = "0";
                lblFacturasPendientes.Text = "0";
                lblTotalCuentas.Text = "0";
                lblCuentasActivas.Text = "0";
                lblTotalPropuestas.Text = "0";
                lblFacturasPagadas.Text = "0";
                lblFacturasVencidas.Text = "0";
                lblPropuestasEsteMes.Text = "0";
                lblCambioFacturas.Text = "0%";

                // Datos vacíos para gráficos
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                hfMesesLabels.Value = serializer.Serialize(new List<string> { "Sin datos" });
                hfMesesData.Value = serializer.Serialize(new List<int> { 0 });
                hfBancosLabels.Value = serializer.Serialize(new List<string> { "Sin datos" });
                hfBancosData.Value = serializer.Serialize(new List<int> { 0 });
                hfPropuestasLabels.Value = serializer.Serialize(new List<string> { "Sin datos" });
                hfPropuestasData.Value = serializer.Serialize(new List<int> { 0 });

                // Mostrar mensaje de error al usuario
                MostrarMensaje($"Error al cargar algunos datos del dashboard. Por favor, actualice la página.", "warning");
            }
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarDashboard();
            MostrarMensaje("Dashboard actualizado correctamente", "success");
        }

        protected void btnGenerarReportePDF_Click(object sender, EventArgs e)
        {
            try
            {
                // Redirigir a página de reporte con parámetros del usuario
                usuariosDTO usuario = (usuariosDTO)Session["UsuarioLogueado"];
                Response.Redirect($"ReporteDashboard.aspx?usuario={usuario.usuario_id}");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al generar reporte: {ex.Message}", "danger");
            }
        }

        // Helper: Obtener nombre del mes en español
        private string ObtenerNombreMes(int mes)
        {
            string[] meses = {
                "", "Ene", "Feb", "Mar", "Abr", "May", "Jun",
                "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
            };
            return meses[mes];
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string mensajeEscapado = mensaje.Replace("'", "\\'").Replace("\"", "\\\"");
            string script = $@"
                $(document).ready(function() {{
                    var alertHtml = '<div class=""alert alert-{tipo} alert-dismissible fade show"" role=""alert"">' +
                                    '{mensajeEscapado}' +
                                    '<button type=""button"" class=""btn-close"" data-bs-dismiss=""alert""></button>' +
                                    '</div>';
                    $('.container-fluid').prepend(alertHtml);
                    setTimeout(function() {{
                        $('.alert').fadeOut();
                    }}, 3000);
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje", script, true);
        }
    }
}
