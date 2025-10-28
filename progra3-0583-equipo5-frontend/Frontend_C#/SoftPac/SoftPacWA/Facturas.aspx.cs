using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPac.Business;
using SoftPac.Model;

namespace SoftPacWA
{
    public partial class Facturas : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();
        private AcreedoresBO acreedoresBO = new AcreedoresBO();
        private List<PaisesDTO> paisesUsuario;

        private List<FacturasDTO> ListaFacturas;

        private UsuariosDTO UsuarioLogueado
        {
            get
            {
                return (UsuariosDTO)Session["UsuarioLogueado"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");
            if ((string)Session["MensajeExito"] != null)
            {
                MostrarMensaje((string)Session["MensajeExito"], "success");
                Session["MensajeExito"] = null;
            }
            paisesUsuario = UsuarioLogueado.UsuarioPais.Where(up => up.Acceso == true).Select(up => up.Pais).ToList();
            if (!IsPostBack)
            {
                CargarFiltros();
                CargarFacturas();
            }
        }

        private void CargarFiltros()
        {
            try
            {
                // Cargar países
                ddlFiltroPais.DataSource = paisesUsuario;
                ddlFiltroPais.DataTextField = "Nombre";
                ddlFiltroPais.DataValueField = "PaisId";
                ddlFiltroPais.DataBind();
                ddlFiltroPais.Items.Insert(0, new ListItem("Todos los países", ""));

                // Cargar proveedores
                var proveedores = acreedoresBO.ListarTodos().Where(a => a.Activo).ToList();
                ddlFiltroProveedor.DataSource = proveedores;
                ddlFiltroProveedor.DataTextField = "RazonSocial";
                ddlFiltroProveedor.DataValueField = "AcreedorId";
                ddlFiltroProveedor.DataBind();
                ddlFiltroProveedor.Items.Insert(0, new ListItem("Todos los proveedores", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar filtros: " + ex.Message, "danger");
            }
        }

        private void CargarFacturas()
        {
            try
            {
                ListaFacturas = facturasBO.ListarTodos().ToList();
                ListaFacturas = ListaFacturas.Where(f => paisesUsuario.Select(pu => pu.PaisId).ToList().Contains(f.Acreedor?.Pais.PaisId)).ToList();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    int paisId = int.Parse(ddlFiltroPais.SelectedValue);
                    ListaFacturas = ListaFacturas.Where(f => f.Acreedor?.Pais?.PaisId == paisId).ToList();
                }

                if (!string.IsNullOrEmpty(ddlFiltroProveedor.SelectedValue))
                {
                    int acreedorId = int.Parse(ddlFiltroProveedor.SelectedValue);
                    ListaFacturas = ListaFacturas.Where(f => f.Acreedor?.AcreedorId == acreedorId).ToList();
                }

                if (!string.IsNullOrEmpty(txtFechaDesde.Text))
                {
                    DateTime fechaDesde = DateTime.Parse(txtFechaDesde.Text);
                    ListaFacturas = ListaFacturas.Where(f => f.FechaLimitePago >= fechaDesde).ToList();
                }

                if (!string.IsNullOrEmpty(txtFechaHasta.Text))
                {
                    DateTime fechaHasta = DateTime.Parse(txtFechaHasta.Text);
                    ListaFacturas = ListaFacturas.Where(f => f.FechaLimitePago <= fechaHasta).ToList();
                }

                // Ordenar por fecha de emisión descendente
                ListaFacturas = ListaFacturas.OrderByDescending(f => f.FechaEmision).ToList();

                gvFacturas.DataSource = ListaFacturas;
                gvFacturas.DataBind();

                // Actualizar contador de registros
                lblRegistros.Text = $"Mostrando {ListaFacturas.Count} factura(s)";
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar facturas: " + ex.Message, "danger");
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            string url = "ReporteFacturas.aspx?";

            if (!string.IsNullOrEmpty(ddlFiltroProveedor.SelectedValue))
                url += $"acreedor={ddlFiltroProveedor.SelectedValue}&";

            if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                url += $"pais={ddlFiltroPais.SelectedValue}&";

            Response.Redirect(url.TrimEnd('&'));
        }

        protected void AplicarFiltros(object sender, EventArgs e)
        {
            CargarFacturas();
        }

        protected void LimpiarFiltros(object sender, EventArgs e)
        {
            ddlFiltroPais.SelectedIndex = 0;
            ddlFiltroProveedor.SelectedIndex = 0;
            txtFechaDesde.Text = string.Empty;
            txtFechaHasta.Text = string.Empty;
            CargarFacturas();
        }

        protected void gvFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFacturas.PageIndex = e.NewPageIndex;
            CargarFacturas();
        }

        protected void gvFacturas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FacturasDTO factura = (FacturasDTO)e.Row.DataItem;

                LinkButton btnEditar = (LinkButton)e.Row.FindControl("btnEditar");
                LinkButton btnEliminar = (LinkButton)e.Row.FindControl("btnEliminar");
                if (btnEditar != null && factura.MontoRestante != factura.MontoTotal)
                {
                    btnEditar.Enabled = false;
                    btnEditar.CssClass += " disabled";
                    btnEditar.ToolTip = "No se puede editar una factura con pagos registrados.";
                }
                if (btnEliminar != null && factura.MontoRestante != factura.MontoTotal)
                {
                    btnEliminar.Enabled = false;
                    btnEliminar.CssClass += " disabled";
                    btnEliminar.ToolTip = "No se puede eliminar una factura con pagos registrados.";
                }
            }
        }

        protected void btnNuevaFactura_Click(object sender, EventArgs e)
        {
            Session["FacturaId"] = null;
            Response.Redirect("GestionFactura.aspx?accion=insertar");
        }

        protected void btnAccion_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int facturaId = int.Parse(btn.CommandArgument);
            string accion = btn.CommandName;

            try
            {
                switch (accion)
                {
                    case "Ver":
                        Session["facturaId"] = facturaId;
                        Response.Redirect($"GestionFactura.aspx?accion=verdetalle");
                        break;
                    case "Editar":
                        Session["facturaId"] = facturaId;
                        Response.Redirect($"GestionFactura.aspx?accion=editar");
                        break;
                    case "Eliminar":
                        var resultado = facturasBO.Eliminar(facturaId, UsuarioLogueado);
                        if (resultado == 1)
                        {
                            MostrarMensaje("Factura eliminada correctamente", "success");
                            CargarFacturas();
                        }
                        else
                        {
                            MostrarMensaje("Error al eliminar la factura", "danger");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected string GetEstadoClass(string estado)
        {
            switch (estado?.ToLower())
            {
                case "pendiente":
                    return "badge-pendiente";
                case "pagada":
                    return "badge-pagado";
                case "vencida":
                    return "badge-vencido";
                default:
                    return "badge-pendiente";
            }
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
                    $('.content-area').prepend(alertHtml);
                    setTimeout(function() {{
                        $('.alert').fadeOut();
                    }}, 5000);
                }});
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje", script, true);
        }
    }
}