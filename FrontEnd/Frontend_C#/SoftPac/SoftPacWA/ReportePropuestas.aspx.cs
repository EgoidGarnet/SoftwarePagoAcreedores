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
    public partial class ReportePropuestas : System.Web.UI.Page
    {
        private ReportePropuestasBO reporteBO = new ReportePropuestasBO();
        private PaisesBO paisesBO = new PaisesBO();
        private EntidadesBancariasBO bancosBO = new EntidadesBancariasBO();

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
                // Cargar países
                var paises = UsuarioLogueado.usuario_pais
                    .Where(up => up.acceso)
                    .Select(up => up.pais)
                    .ToList();
                ddlPais.DataSource = paises;
                ddlPais.DataTextField = "Nombre";
                ddlPais.DataValueField = "PaisId";
                ddlPais.DataBind();
                ddlPais.Items.Insert(0, new ListItem("Todos", ""));

                // Cargar bancos (se actualizará cuando se seleccione un país)
                ddlBanco.Items.Clear();
                ddlBanco.Items.Insert(0, new ListItem("Todos", ""));
            }
            catch (Exception ex)
            {
                // Log error
            }
        }

        private void CargarFiltrosDesdeQueryString()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["pais"]))
            {
                ddlPais.SelectedValue = Request.QueryString["pais"];
                CargarBancosPorPais();
            }

            if (!string.IsNullOrEmpty(Request.QueryString["banco"]))
                ddlBanco.SelectedValue = Request.QueryString["banco"];

            if (!string.IsNullOrEmpty(Request.QueryString["estado"]))
                ddlEstado.SelectedValue = Request.QueryString["estado"];
        }

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarBancosPorPais();
        }

        private void CargarBancosPorPais()
        {
            ddlBanco.Items.Clear();

            if (string.IsNullOrEmpty(ddlPais.SelectedValue))
            {
                ddlBanco.Items.Insert(0, new ListItem("Todos", ""));
                return;
            }

            int paisId = int.Parse(ddlPais.SelectedValue);
            var bancos = bancosBO.ListarTodos()
                .Where(b => b.pais.pais_id == paisId)
                .OrderBy(b => b.nombre)
                .ToList();

            ddlBanco.DataSource = bancos;
            ddlBanco.DataTextField = "Nombre";
            ddlBanco.DataValueField = "EntidadBancariaId";
            ddlBanco.DataBind();

            ddlBanco.Items.Insert(0, new ListItem("Todos", ""));
        }

        protected void btnAplicarFiltros_Click(object sender, EventArgs e)
        {
            GenerarReporte();
        }

        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            ddlPais.SelectedIndex = 0;
            ddlBanco.Items.Clear();
            ddlBanco.Items.Insert(0, new ListItem("Todos", ""));
            ddlEstado.SelectedIndex = 0;
            GenerarReporte();
        }

        private void GenerarReporte()
        {
            try
            {
                int? paisId = string.IsNullOrEmpty(ddlPais.SelectedValue) ?
                    (int?)null : int.Parse(ddlPais.SelectedValue);
                int? bancoId = string.IsNullOrEmpty(ddlBanco.SelectedValue) ?
                    (int?)null : int.Parse(ddlBanco.SelectedValue);
                string estado = ddlEstado.SelectedValue;

                var datos = reporteBO.GenerarReporteDetallado(paisId, bancoId, estado);

                MostrarFiltrosAplicados();

                if (datos.Count > 0)
                {
                    rptPropuestas.DataSource = datos;
                    rptPropuestas.DataBind();

                    pnlSinDatos.Visible = false;
                    pnlResumenGeneral.Visible = true;

                    // Calcular estadísticas
                    lblTotalPropuestas.Text = datos.Count.ToString();
                    lblTotalPagos.Text = datos.Sum(p => p.Detalles.Count).ToString();

                    var montosAgrupados = datos
                        .SelectMany(p => p.Detalles)
                        .GroupBy(d => d.Moneda)
                        .Select(g => $"{g.Key}: {g.Sum(x => x.Monto):N2}")
                        .ToList();

                    lblTotalMontos.Text = string.Join("\n", montosAgrupados);
                }
                else
                {
                    rptPropuestas.DataSource = null;
                    rptPropuestas.DataBind();
                    pnlSinDatos.Visible = true;
                    pnlResumenGeneral.Visible = false;
                }
            }
            catch (Exception ex)
            {
                pnlSinDatos.Visible = true;
                pnlResumenGeneral.Visible = false;
                rptPropuestas.DataSource = null;
                rptPropuestas.DataBind();
            }
        }

        private void MostrarFiltrosAplicados()
        {
            var filtros = new List<string>();

            if (!string.IsNullOrEmpty(ddlPais.SelectedValue))
                filtros.Add($"País: {ddlPais.SelectedItem.Text}");

            if (!string.IsNullOrEmpty(ddlBanco.SelectedValue))
                filtros.Add($"Banco: {ddlBanco.SelectedItem.Text}");

            if (!string.IsNullOrEmpty(ddlEstado.SelectedValue))
                filtros.Add($"Estado: {ddlEstado.SelectedItem.Text}");

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

        protected void rptPropuestas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var propuesta = (ReportePropuestasBO.PropuestaDetalle)e.Item.DataItem;

                GridView gvDetalles = (GridView)e.Item.FindControl("gvDetalles");
                if (gvDetalles != null)
                {
                    gvDetalles.DataSource = propuesta.Detalles;
                    gvDetalles.DataBind();
                }

                Repeater rptTotales = (Repeater)e.Item.FindControl("rptTotales");
                if (rptTotales != null)
                {
                    var totalesPorMoneda = propuesta.Detalles
                        .GroupBy(d => d.Moneda)
                        .Select(g => new
                        {
                            Moneda = g.Key,
                            Total = g.Sum(x => x.Monto)
                        })
                        .ToList();

                    rptTotales.DataSource = totalesPorMoneda;
                    rptTotales.DataBind();
                }
            }
        }

        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                int? paisId = string.IsNullOrEmpty(ddlPais.SelectedValue) ?
                    (int?)null : int.Parse(ddlPais.SelectedValue);
                int? bancoId = string.IsNullOrEmpty(ddlBanco.SelectedValue) ?
                    (int?)null : int.Parse(ddlBanco.SelectedValue);
                string estado = ddlEstado.SelectedValue;

                var datos = reporteBO.GenerarReporteDetallado(paisId, bancoId, estado);

                if (datos == null || datos.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('No hay datos para exportar. Por favor, aplique filtros que devuelvan resultados.');", true);
                    return;
                }

                var filtrosInfo = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(ddlPais.SelectedValue))
                    filtrosInfo["País"] = ddlPais.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlBanco.SelectedValue))
                    filtrosInfo["Banco"] = ddlBanco.SelectedItem.Text;
                if (!string.IsNullOrEmpty(ddlEstado.SelectedValue))
                    filtrosInfo["Estado"] = ddlEstado.SelectedItem.Text;

                byte[] pdfBytes = reporteBO.GenerarPDF(
                    datos,
                    UsuarioLogueado.nombre + " " + UsuarioLogueado.apellidos,
                    filtrosInfo
                );

                // Limpiar la respuesta
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition",
                    $"attachment; filename=ReportePropuestas_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                Response.BinaryWrite(pdfBytes);
                Response.Flush();

                // Importante: usar HttpContext.Current.ApplicationInstance.CompleteRequest()
                // en lugar de Response.End() para evitar ThreadAbortException
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Error al generar el PDF: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PropuestasPago.aspx");
        }
    }
}