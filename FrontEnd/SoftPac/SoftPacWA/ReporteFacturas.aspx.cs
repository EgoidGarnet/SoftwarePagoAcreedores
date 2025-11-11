using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class ReporteFacturas : System.Web.UI.Page
    {
        private ReporteFacturasBO reporteBO = new ReporteFacturasBO();
        private AcreedoresBO acreedoresBO = new AcreedoresBO();
        private PaisesBO paisesBO = new PaisesBO();
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
                CargarDatosAuditoria();
                CargarFiltros();
                CargarFiltrosDesdeQueryString();
                GenerarReporte();
            }
        }

        private void CargarDatosAuditoria()
        {
            lblFechaGeneracion.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblHoraGeneracion.Text = DateTime.Now.ToString("HH:mm:ss");
            lblUsuario.Text = $"{UsuarioLogueado.nombre} {UsuarioLogueado.apellidos} ({UsuarioLogueado.correo_electronico})";
        }

        private void CargarFiltros()
        {
            try
            {
                // Cargar acreedores
                var acreedores = acreedoresBO.ListarTodos().Where(a => a.activo).ToList();
                ddlAcreedor.DataSource = acreedores;
                ddlAcreedor.DataTextField = "razon_social";
                ddlAcreedor.DataValueField = "acreedor_id";
                ddlAcreedor.DataBind();
                ddlAcreedor.Items.Insert(0, new ListItem("Todos", ""));

                // Cargar países
                var paises = UsuarioLogueado.usuario_pais
                    .Where(up => up.acceso)
                    .Select(up => up.pais)
                    .ToList();
                ddlPais.DataSource = paises;
                ddlPais.DataTextField = "nombre";
                ddlPais.DataValueField = "pais_id";
                ddlPais.DataBind();
                ddlPais.Items.Insert(0, new ListItem("Todos", ""));

                // Cargar monedas
                var monedas = monedasBO.ListarTodos();
                ddlMoneda.DataSource = monedas;
                ddlMoneda.DataTextField = "codigo_iso";
                ddlMoneda.DataValueField = "moneda_id";
                ddlMoneda.DataBind();
                ddlMoneda.Items.Insert(0, new ListItem("Todas", ""));
            }
            catch (Exception ex)
            {
                // Log error
            }
        }

        private void CargarFiltrosDesdeQueryString()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["acreedor"]))
                ddlAcreedor.SelectedValue = Request.QueryString["acreedor"];

            if (!string.IsNullOrEmpty(Request.QueryString["pais"]))
                ddlPais.SelectedValue = Request.QueryString["pais"];

            if (!string.IsNullOrEmpty(Request.QueryString["moneda"]))
                ddlMoneda.SelectedValue = Request.QueryString["moneda"];

            if (!string.IsNullOrEmpty(Request.QueryString["rango"]))
                ddlRango.SelectedValue = Request.QueryString["rango"];
        }

        protected void btnAplicarFiltros_Click(object sender, EventArgs e)
        {
            GenerarReporte();
        }

        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            ddlAcreedor.SelectedIndex = 0;
            ddlPais.SelectedIndex = 0;
            ddlMoneda.SelectedIndex = 0;
            ddlRango.SelectedIndex = 0;
            GenerarReporte();
        }

        private void GenerarReporte()
        {
            try
            {
                int? acreedorId = string.IsNullOrEmpty(ddlAcreedor.SelectedValue) ?
                    (int?)null : int.Parse(ddlAcreedor.SelectedValue);
                int? paisId = string.IsNullOrEmpty(ddlPais.SelectedValue) ?
                    (int?)null : int.Parse(ddlPais.SelectedValue);
                int? monedaId = string.IsNullOrEmpty(ddlMoneda.SelectedValue) ?
                    (int?)null : int.Parse(ddlMoneda.SelectedValue);
                string rango = ddlRango.SelectedValue;

                var datos = reporteBO.GenerarReportePorVencimiento(acreedorId, paisId, monedaId, rango);

                MostrarFiltrosAplicados();

                if (datos.Count > 0)
                {
                    rptRangos.DataSource = datos;
                    rptRangos.DataBind();
                    pnlSinDatos.Visible = false;

                    // Calcular total general
                    var totalesGenerales = datos
                        .SelectMany(r => r.Facturas)
                        .GroupBy(f => f.Moneda)
                        .Select(g => $"{g.Key}: {g.Sum(x => x.Monto):N2}")
                        .ToList();
                    lblTotalGeneral.Text = string.Join("\n", totalesGenerales);
                }
                else
                {
                    rptRangos.DataSource = null;
                    rptRangos.DataBind();
                    pnlSinDatos.Visible = true;
                    lblTotalGeneral.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                pnlSinDatos.Visible = true;
                rptRangos.DataSource = null;
                rptRangos.DataBind();
            }
        }

        private void MostrarFiltrosAplicados()
        {
            var filtros = new List<string>();

            if (!string.IsNullOrEmpty(ddlAcreedor.SelectedValue))
                filtros.Add($"Acreedor: {ddlAcreedor.SelectedItem.Text}");

            if (!string.IsNullOrEmpty(ddlPais.SelectedValue))
                filtros.Add($"País: {ddlPais.SelectedItem.Text}");

            if (!string.IsNullOrEmpty(ddlMoneda.SelectedValue))
                filtros.Add($"Moneda: {ddlMoneda.SelectedItem.Text}");

            if (!string.IsNullOrEmpty(ddlRango.SelectedValue))
                filtros.Add($"Rango: {ddlRango.SelectedItem.Text}");

            if (filtros.Count > 0)
            {
                pnlFiltrosAplicados.Visible = true;
                lblFiltrosAplicados.Text = string.Join(" | ", filtros);
            }
            else
            {
                pnlFiltrosAplicados.Visible = false;
            }
        }

        protected void rptRangos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rango = (ReporteFacturasBO.RangoVencimiento)e.Item.DataItem;

                GridView gv = (GridView)e.Item.FindControl("gvFacturas");
                if (gv != null)
                {
                    gv.DataSource = rango.Facturas;
                    gv.DataBind();
                }

                Label lblSubtotal = (Label)e.Item.FindControl("lblSubtotal");
                if (lblSubtotal != null)
                {
                    var subtotalesPorMoneda = rango.Facturas
                        .GroupBy(f => f.Moneda)
                        .Select(g => $"{g.Key}: {g.Sum(x => x.Monto):N2}")
                        .ToList();
                    lblSubtotal.Text = string.Join(" | ", subtotalesPorMoneda);
                }
            }
        }

        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                int? acreedorId = string.IsNullOrEmpty(ddlAcreedor.SelectedValue) ?
                    (int?)null : int.Parse(ddlAcreedor.SelectedValue);
                int? paisId = string.IsNullOrEmpty(ddlPais.SelectedValue) ?
                    (int?)null : int.Parse(ddlPais.SelectedValue);
                int? monedaId = string.IsNullOrEmpty(ddlMoneda.SelectedValue) ?
                    (int?)null : int.Parse(ddlMoneda.SelectedValue);
                string rango = ddlRango.SelectedValue;

                var datos = reporteBO.GenerarReportePorVencimiento(acreedorId, paisId, monedaId, rango);

                if (datos == null || datos.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('No hay datos para exportar. Por favor, aplique filtros que devuelvan resultados.');", true);
                    return;
                }

                var filtrosInfo = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(ddlAcreedor.SelectedValue))
                    filtrosInfo["Acreedor"] = ddlAcreedor.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlPais.SelectedValue))
                    filtrosInfo["País"] = ddlPais.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlMoneda.SelectedValue))
                    filtrosInfo["Moneda"] = ddlMoneda.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlRango.SelectedValue))
                    filtrosInfo["Rango"] = ddlRango.SelectedItem.Text;

                byte[] pdfBytes = reporteBO.GenerarPDF(
                    datos,
                    UsuarioLogueado.nombre + " " + UsuarioLogueado.apellidos,
                    filtrosInfo
                );

                // IMPORTANTE: Verificar que pdfBytes no sea null
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Error: No se pudo generar el PDF.');", true);
                    return;
                }

                // Limpiar completamente la respuesta
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition",
                    $"attachment; filename=ReporteFacturas_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                Response.AddHeader("Content-Length", pdfBytes.Length.ToString());
                Response.BinaryWrite(pdfBytes);
                Response.Flush();

                // Usar CompleteRequest en lugar de End para evitar ThreadAbortException
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                // Mostrar error detallado
                string errorMsg = $"Error al generar el PDF: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $" | Detalle: {ex.InnerException.Message}";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('{errorMsg.Replace("'", "\\'").Replace("\n", "\\n")}');", true);
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Facturas.aspx");
        }
    }
}