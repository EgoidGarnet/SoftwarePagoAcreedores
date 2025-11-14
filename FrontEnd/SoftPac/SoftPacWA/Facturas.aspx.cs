using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPac.Business;
using SoftPacBusiness.FacturasWS;
using SoftPacBusiness.UsuariosWS;
using paisesDTO = SoftPacBusiness.FacturasWS.paisesDTO;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;
using System.Web.Script.Serialization;

namespace SoftPacWA
{
    public partial class Facturas : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();
        private AcreedoresBO acreedoresBO = new AcreedoresBO();
        private List<paisesDTO> paisesUsuario;

        private List<facturasDTO> ListaFacturas;

        private usuariosDTO UsuarioLogueado
        {
            get
            {
                return (usuariosDTO)Session["UsuarioLogueado"];
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
            SoftPacBusiness.FacturasWS.usuariosDTO user = new SoftPacBusiness.FacturasWS.usuariosDTO();
            user.usuario_id = UsuarioLogueado.usuario_id;
            user.usuario_pais = DTOConverter.ConvertirArray<SoftPacBusiness.UsuariosWS.usuarioPaisAccesoDTO, SoftPacBusiness.FacturasWS.usuarioPaisAccesoDTO>(UsuarioLogueado.usuario_pais);
            paisesUsuario = user.usuario_pais.Where(up => up.acceso == true).Select(up => up.pais).ToList();
            if (!IsPostBack)
            {
                CargarFiltros();
                CargarFacturasParaAutocomplete();
                CargarFacturas();
            }
        }

        #region Autocomplete
        private void CargarFacturasParaAutocomplete()
        {
            try
            {
                List<facturasDTO> listaFacturas = facturasBO.ListarTodos()
                    .Where(f => paisesUsuario.Select(pu => pu.pais_id).ToList().Contains(f.acreedor.pais.pais_id))
                    .ToList();

                var facturasSimplificadas = listaFacturas.Select(f => new
                {
                    numero_factura = f.numero_factura ?? "",
                    acreedor = f.acreedor?.razon_social ?? "",
                    monto_total = f.monto_total,
                    moneda = f.moneda?.codigo_iso ?? "",
                    estado = f.estado ?? ""
                }).ToList();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(facturasSimplificadas);
                hfFacturasJson.Value = json;
            }
            catch (Exception ex)
            {
                hfFacturasJson.Value = "[]";
            }
        }
        #endregion

        private void CargarFiltros()
        {
            try
            {
                // Cargar países
                ddlFiltroPais.DataSource = paisesUsuario;
                ddlFiltroPais.DataTextField = "nombre";
                ddlFiltroPais.DataValueField = "pais_id";
                ddlFiltroPais.DataBind();
                if (paisesUsuario.Count > 1)
                    ddlFiltroPais.Items.Insert(0, new ListItem("Todos los países", ""));

                // Cargar proveedores
                var proveedores = acreedoresBO.ListarTodos().Where(a => a.activo).ToList();
                ddlFiltroProveedor.DataSource = proveedores;
                ddlFiltroProveedor.DataTextField = "razon_social";
                ddlFiltroProveedor.DataValueField = "acreedor_id";
                ddlFiltroProveedor.DataBind();
                ddlFiltroProveedor.Items.Insert(0, new ListItem("Todos los acreedores", ""));
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
                ListaFacturas = ListaFacturas.Where(f => paisesUsuario.Select(pu => pu.pais_id).ToList().Contains(f.acreedor.pais.pais_id)).ToList();

                // Filtro por número de factura (búsqueda)
                string numeroFacturaBusqueda = (txtBuscarFactura.Text ?? string.Empty).Trim().ToLower();
                if (!string.IsNullOrEmpty(numeroFacturaBusqueda))
                {
                    ListaFacturas = ListaFacturas.Where(f =>
                        (f.numero_factura ?? string.Empty).ToLower().Contains(numeroFacturaBusqueda) ||
                        (f.acreedor?.razon_social ?? string.Empty).ToLower().Contains(numeroFacturaBusqueda)
                    ).ToList();
                }

                // Aplicar filtros
                if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    int paisId = int.Parse(ddlFiltroPais.SelectedValue);
                    ListaFacturas = ListaFacturas.Where(f => f.acreedor?.pais?.pais_id == paisId).ToList();
                }

                if (!string.IsNullOrEmpty(ddlFiltroProveedor.SelectedValue))
                {
                    int acreedorId = int.Parse(ddlFiltroProveedor.SelectedValue);
                    ListaFacturas = ListaFacturas.Where(f => f.acreedor?.acreedor_id == acreedorId).ToList();
                }

                if (!string.IsNullOrEmpty(txtFechaDesde.Text))
                {
                    DateTime fechaDesde = DateTime.Parse(txtFechaDesde.Text);
                    ListaFacturas = ListaFacturas.Where(f => f.fecha_limite_pago >= fechaDesde).ToList();
                }

                if (!string.IsNullOrEmpty(txtFechaHasta.Text))
                {
                    DateTime fechaHasta = DateTime.Parse(txtFechaHasta.Text);
                    ListaFacturas = ListaFacturas.Where(f => f.fecha_limite_pago <= fechaHasta).ToList();
                }

                // Ordenar por fecha de emisión descendente
                ListaFacturas = ListaFacturas.OrderByDescending(f => f.fecha_emision).ToList();

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
            txtBuscarFactura.Text = string.Empty;
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
                facturasDTO factura = (facturasDTO)e.Row.DataItem;

                LinkButton btnEditar = (LinkButton)e.Row.FindControl("btnEditar");
                LinkButton btnEliminar = (LinkButton)e.Row.FindControl("btnEliminar");
                if (btnEditar != null && (factura.monto_restante != factura.monto_total || factura.estado != "Pendiente"))
                {
                    btnEditar.Enabled = false;
                    btnEditar.Visible = false;
                    //btnEditar.CssClass += " disabled";
                    btnEditar.ToolTip = "No se puede editar una factura con pagos registrados.";
                }
                if (btnEliminar != null && (factura.monto_restante != factura.monto_total || factura.estado != "Pendiente"))
                {
                    btnEliminar.Enabled = false;
                    btnEliminar.Visible = false;
                    //btnEliminar.CssClass += " disabled";
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
                        SoftPacBusiness.FacturasWS.usuariosDTO usuarioEliminacion = new SoftPacBusiness.FacturasWS.usuariosDTO();
                        usuarioEliminacion.usuario_id = UsuarioLogueado.usuario_id;
                        var resultado = facturasBO.Eliminar(facturaId, usuarioEliminacion);
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
                case "eliminado":
                    return "badge-elimin";
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