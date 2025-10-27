// Acreedores.aspx.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SoftPac.Business;
using SoftPac.Model;

namespace SoftPacWA
{
    public partial class Acreedores : Page
    {
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();

        private List<PaisesDTO> paisesUsuario;
        private List<AcreedoresDTO> listaAcreedores;

        private UsuariosDTO UsuarioLogueado
        {
            get { return (UsuariosDTO)Session["UsuarioLogueado"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");

            if ((string)Session["MensajeExito"] != null)
            {
                MostrarMensaje((string)Session["MensajeExito"], "success");
                Session["MensajeExito"] = null;
            }

            // países a los que el usuario tiene acceso
            paisesUsuario = UsuarioLogueado.UsuarioPais
                .Where(up => up.Acceso)
                .Select(up => up.Pais)
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
            var p = paisesUsuario?.FirstOrDefault(x => x.PaisId == paisId.Value);
            return p?.Nombre ?? string.Empty;
        }

        private string GetPaisNombreSeguro(PaisesDTO pais)
        {
            if (pais == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(pais.Nombre)) return pais.Nombre;
            return PaisNombreById(pais.PaisId);
        }
        // --------------------------------

        private void CargarFiltros()
        {
            try
            {
                ddlFiltroPais.DataSource = paisesUsuario;
                ddlFiltroPais.DataTextField = "Nombre";
                ddlFiltroPais.DataValueField = "PaisId";
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
                List<AcreedoresDTO> lista = acreedoresBO.ListarTodos().ToList();

                HashSet<int> paisesPermitidos = new HashSet<int>(paisesUsuario.Select(p => p.PaisId.Value));
                lista = lista.Where(a =>
                    a.Pais != null &&
                    a.Pais.PaisId.HasValue &&
                    paisesPermitidos.Contains(a.Pais.PaisId.Value)).ToList();

                if (!string.IsNullOrEmpty(ddlFiltroPais.SelectedValue))
                {
                    int paisId = int.Parse(ddlFiltroPais.SelectedValue);
                    lista = lista.Where(a => a.Pais != null &&
                                             a.Pais.PaisId.HasValue &&
                                             a.Pais.PaisId.Value == paisId).ToList();
                }

                string texto = (txtBuscar.Text ?? string.Empty).Trim().ToLower();
                if (!string.IsNullOrEmpty(texto))
                {
                    lista = lista.Where(a =>
                        (a.RazonSocial ?? string.Empty).ToLower().Contains(texto) ||
                        (a.Ruc ?? string.Empty).ToLower().Contains(texto)
                    ).ToList();
                }

                lista = lista.OrderBy(a => a.RazonSocial).ToList();
                listaAcreedores = lista;

                gvAcreedores.DataSource = lista.Select(a => new
                {
                    a.AcreedorId,
                    a.RazonSocial,
                    PaisNombre = GetPaisNombreSeguro(a.Pais), // <— Fallback seguro
                    Estado = a.Activo ? "Activo" : "Inactivo"
                }).ToList();

                gvAcreedores.DataBind();
                lblRegistros.Text = $"Mostrando {lista.Count} registro(s)";
            }
            catch (Exception ex)
            {
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
                    btnToggle.Text = "<i class='fas fa-trash'></i>";
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

                    case "Eliminar": // alterna Activo/Inactivo en BO
                        {
                            int result = acreedoresBO.Eliminar(acreedorId, UsuarioLogueado);
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
