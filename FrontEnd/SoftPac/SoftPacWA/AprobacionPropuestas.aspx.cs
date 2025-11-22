using SoftPac.Business;
using SoftPacBusiness.PropuestaPagoWS;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class AprobacionPropuestas : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasPagoBO = new PropuestasPagoBO();

        private SoftPacBusiness.UsuariosWS.usuariosDTO UsuarioLogueado
        {
            get
            {
                return (SoftPacBusiness.UsuariosWS.usuariosDTO)Session["UsuarioLogueado"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!ValidarSesion())
                    return;

                CargarPropuestasPendientes();
            }
        }

        private bool ValidarSesion()
        {
            if (UsuarioLogueado == null)
            {
                Response.Redirect("~/Login.aspx");
                return false;
            }
            return true;
        }

        private void CargarPropuestasPendientes()
        {
            try
            {
                pnlError.Visible = false;

                // Filtrando por "En revisión" como mencionaste
                var propuestas = propuestasPagoBO.ListarConFiltros(null, null, "En revisión")
                    .Where(p => p != null)
                    .OrderByDescending(p => p.fecha_hora_creacion)
                    .ToList();

                // Asegurarse de que detalles_propuesta no sea null
                propuestas.ForEach(p =>
                {
                    if (p.detalles_propuesta == null)
                    {
                        p.detalles_propuesta = Array.Empty<detallesPropuestaDTO>();
                    }
                });

                gvPropuestasPendientes.DataSource = propuestas;
                gvPropuestasPendientes.DataBind();

                lblTotalPendientes.Text = $"Propuestas pendientes: {propuestas.Count}";
            }
            catch (Exception ex)
            {
                gvPropuestasPendientes.DataSource = null;
                gvPropuestasPendientes.DataBind();

                lblTotalPendientes.Text = "Propuestas pendientes: 0";
                pnlError.Visible = true;
                lblError.Text = $"Error al cargar las propuestas: {ex.Message}";
            }
        }

        protected void gvPropuestasPendientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Visualizar")
            {
                int propuestaId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/DetallePropuestaAdmin.aspx?id={propuestaId}");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlError.Visible = true;
            pnlError.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblError.Text = mensaje;
        }
    }
}