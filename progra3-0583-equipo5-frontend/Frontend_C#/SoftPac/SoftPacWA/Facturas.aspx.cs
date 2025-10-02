using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPac.Business;

namespace SoftPacWA
{
    public partial class Facturas : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();
        private AcreedoresBO acreedoresBO = new AcreedoresBO();
        private PaisesBO paisesBO = new PaisesBO();

        protected void Page_Load(object sender, EventArgs e)
        {
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
                var paises = paisesBO.ListarTodos();
                ddlFiltroPais.DataSource = paises;
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
                var facturas = facturasBO.ListarTodos().ToList(); // convertir a lista para filtrar

                // Aplicar filtros
                if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    int paisId = int.Parse(ddlFiltroPais.SelectedValue);
                    facturas = facturas.Where(f => f.Acreedor?.Pais?.PaisId == paisId).ToList();
                }

                if (!string.IsNullOrEmpty(ddlFiltroProveedor.SelectedValue))
                {
                    int acreedorId = int.Parse(ddlFiltroProveedor.SelectedValue);
                    facturas = facturas.Where(f => f.Acreedor?.AcreedorId == acreedorId).ToList();
                }

                if (!string.IsNullOrEmpty(txtFechaDesde.Text))
                {
                    DateTime fechaDesde = DateTime.Parse(txtFechaDesde.Text);
                    facturas = facturas.Where(f => f.FechaLimitePago >= fechaDesde).ToList();
                }

                if (!string.IsNullOrEmpty(txtFechaHasta.Text))
                {
                    DateTime fechaHasta = DateTime.Parse(txtFechaHasta.Text);
                    facturas = facturas.Where(f => f.FechaLimitePago <= fechaHasta).ToList();
                }

                // Ordenar por fecha de emisión descendente
                facturas = facturas.OrderByDescending(f => f.FechaEmision).ToList();

                gvFacturas.DataSource = facturas;
                gvFacturas.DataBind();

                // Actualizar contador de registros
                lblRegistros.Text = $"Mostrando {facturas.Count} registro(s)";
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar facturas: " + ex.Message, "danger");
            }
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

        protected void btnNuevaFactura_Click(object sender, EventArgs e)
        {
            Response.Redirect("NuevaFactura.aspx");
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
                        Response.Redirect($"DetalleFactura.aspx?id={facturaId}");
                        break;
                    case "Editar":
                        Response.Redirect($"NuevaFactura.aspx?id={facturaId}");
                        break;
                    case "Eliminar":
                        // Implementar lógica de eliminación
                        var resultado = facturasBO.Eliminar(facturaId);
                        if (resultado ==1)
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
                case "pagado":
                    return "badge-pagado";
                case "vencido":
                    return "badge-vencido";
                default:
                    return "badge-pendiente";
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string script = $@"
                $(document).ready(function() {{
                    var alertHtml = '<div class=""alert alert-{tipo} alert-dismissible fade show"" role=""alert"">' +
                                    '{mensaje}' +
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