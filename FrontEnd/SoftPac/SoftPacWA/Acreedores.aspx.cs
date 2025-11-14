// Acreedores.aspx.cs
using SoftPac.Business;
using SoftPacBusiness.AcreedoresWS;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using paisesDTO = SoftPacBusiness.UsuariosWS.paisesDTO;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;

namespace SoftPacWA
{
    public partial class Acreedores : Page
    {
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();

        private List<paisesDTO> paisesUsuario;
        private List<acreedoresDTO> listaAcreedores;

        private usuariosDTO UsuarioLogueado
        {
            get { return (usuariosDTO)Session["UsuarioLogueado"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");

            if ((string)Session["MensajeExito"] != null)
            {
                MostrarMensaje((string)Session["MensajeExito"], "success");
                Session["MensajeExito"] = null;
            }
            //países a los que el usuario tiene acceso
            paisesUsuario = UsuarioLogueado.usuario_pais?
                .Where(up => up.acceso)
                .Select(up => up.pais)
                .ToList();

            if (!IsPostBack)
            {
                CargarFiltros();
                CargarAcreedores();
            }
        }

        // ---------- Utilidades ----------
        private string PaisNombreById(int? paisId)
        {
            if (!paisId.HasValue) return string.Empty;
            var p = paisesUsuario?.FirstOrDefault(x => x.pais_id == paisId.Value);
            return p?.nombre ?? string.Empty;
        }

        private string GetPaisNombreSeguro(SoftPacBusiness.AcreedoresWS.paisesDTO pais)
        {
            if (pais == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(pais.nombre)) return pais.nombre;
            return PaisNombreById(pais.pais_id);
        }
        // --------------------------------

        private void CargarFiltros()
        {
            try
            {
                ddlFiltroPais.DataSource = paisesUsuario;
                ddlFiltroPais.DataTextField = "nombre";
                ddlFiltroPais.DataValueField = "pais_id";
                ddlFiltroPais.DataBind();
                ddlFiltroPais.Items.Insert(0, new ListItem("Todos los países", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar filtros: " + ex.Message, "danger");
            }
        }

        private void CargarAcreedores()
        {
            try
            {
                // Países habilitados para el usuario
                var paisesIdsUsuario = UsuarioLogueado?.usuario_pais?
                    .Where(up => up.acceso && up.pais != null)
                    .Select(up => up.pais.pais_id)
                    .Distinct()
                    .ToArray() ?? Array.Empty<int>();

                var lista = acreedoresBO.ListarPorPaises(paisesIdsUsuario); // BindingList<acreedoresDTO>
                IEnumerable<acreedoresDTO> query = lista; // trabajar en IEnumerable

                if (int.TryParse(ddlFiltroPais.SelectedValue, out int paisId) && paisId > 0)
                    query = query.Where(a => a.pais != null && a.pais.pais_id == paisId);

                string texto = (txtBuscar.Text ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(texto))
                    query = query.Where(a =>
                        (!string.IsNullOrEmpty(a.razon_social) && a.razon_social.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (!string.IsNullOrEmpty(a.ruc) && a.ruc.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0));

                var data = query.Select(a => new
                {
                    a.acreedor_id,
                    a.razon_social,
                    a.ruc,
                    PaisNombre = GetPaisNombreSeguro(a.pais),
                    Estado = a.activo ? "Activo" : "Inactivo"
                })
                .OrderBy(x => x.razon_social)
                .ToList();

                gvAcreedores.DataSource = data;
                gvAcreedores.DataBind();
            }
            catch (Exception ex)
            {
                gvAcreedores.DataSource = new List<object>();
                gvAcreedores.DataBind();
                MostrarMensaje("Error al cargar acreedores: " + ex.Message, "danger");
            }
        }

        protected void AplicarFiltros(object sender, EventArgs e) => CargarAcreedores();

        protected void LimpiarFiltros(object sender, EventArgs e)
        {
            ddlFiltroPais.SelectedIndex = 0;
            txtBuscar.Text = string.Empty;
            CargarAcreedores();
        }

        protected void gvAcreedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAcreedores.PageIndex = e.NewPageIndex;
            CargarAcreedores();
        }

        protected void gvAcreedores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            string estado = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Estado"));
            LinkButton btnToggle = (LinkButton)e.Row.FindControl("btnToggle");

            if (btnToggle != null)
            {
                if (string.Equals(estado, "Activo", StringComparison.OrdinalIgnoreCase))
                {
                    btnToggle.CssClass = "btn btn-sm btn-danger btn-icon";
                    btnToggle.ToolTip = "Inactivar";
                    btnToggle.CommandName = "Eliminar";
                    btnToggle.Text = "<i class='fas fa-ban'></i>";
                }
                else
                {
                    btnToggle.CssClass = "btn btn-sm btn-success btn-icon";
                    btnToggle.ToolTip = "Activar";
                    btnToggle.CommandName = "Eliminar";
                    btnToggle.Text = "<i class='fas fa-rotate-left'></i>";
                }
            }
        }

        protected void btnNuevoAcreedor_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionAcreedor.aspx?accion=insertar");
        }

        protected void btnAccion_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int acreedorId = int.Parse(btn.CommandArgument);
            string accion = btn.CommandName;

            try
            {
                switch (accion)
                {
                    case "Ver":
                        Response.Redirect("DetalleAcreedor.aspx?id=" + acreedorId);
                        break;

                    case "Editar":
                        Response.Redirect("GestionAcreedor.aspx?accion=editar&id=" + acreedorId);
                        break;

                    case "Eliminar":
                        {
                            //SoftPacBusiness.AcreedoresWS.usuariosDTO usuarioLog = new SoftPacBusiness.AcreedoresWS.usuariosDTO();
                            //usuarioLog.usuario_id = UsuarioLogueado.usuario_id;
                            //int result = acreedoresBO.Eliminar(acreedorId, usuarioLog);
                            acreedoresDTO acreedor = acreedoresBO.obtenerPorId(acreedorId);
                            acreedor.activo = !acreedor.activo;
                            int result = acreedoresBO.modificar(acreedor.acreedor_id, acreedor.razon_social, acreedor.ruc,
                                acreedor.direccion_fiscal, acreedor.condicion, acreedor.plazo_de_pago, acreedor.activo?"S":"N", acreedor.pais.pais_id);
                            if (result == 1)
                            {
                                MostrarMensaje("Estado actualizado", "success");
                                CargarAcreedores();
                            }
                            else
                            {
                                MostrarMensaje("No se pudo actualizar el estado del acreedor", "danger");
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, "danger");
            }
        }

        protected string GetEstadoClass(object estadoObj)
        {
            var e = Convert.ToString(estadoObj);
            if (string.Equals(e, "Activo", StringComparison.OrdinalIgnoreCase)) return "badge-pagado";
            if (string.Equals(e, "Inactivo", StringComparison.OrdinalIgnoreCase)) return "badge-vencido";
            return "badge-pendiente";
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
                    setTimeout(function() {{ $('.alert').fadeOut(); }}, 5000);
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensaje", script, true);
        }
    }
}