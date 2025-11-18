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


        //protected void gvPropuestasPendientes_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "MostrarModalRechazar")
        //    {
        //        // Guardar ID de la propuesta en ViewState
        //        ViewState["PropuestaRechazar"] = Convert.ToInt32(e.CommandArgument);

        //        // Abrir modal de confirmación
        //        ScriptManager.RegisterStartupScript(
        //            this, this.GetType(),
        //            "abrirModalRechazar",
        //            "var m = new bootstrap.Modal(document.getElementById('modalRechazar')); m.show();",
        //            true
        //        );
        //    }
        //}

        //protected void btnConfirmarRechazar_Click(object sender, EventArgs e)
        //{
        //    if (ViewState["PropuestaRechazar"] != null)
        //    {
        //        int propuestaId = (int)ViewState["PropuestaRechazar"];
        //        RechazarPropuesta(propuestaId);
        //    }
        //}

        //private void RechazarPropuesta(int propuestaId)
        //{
        //    try
        //    {
        //        // Obtener la propuesta actual
        //        var propuesta = propuestasPagoBO.ObtenerPorId(propuestaId);

        //        if (propuesta == null)
        //        {
        //            MostrarMensaje("No se encontró la propuesta especificada.", "danger");
        //            return;
        //        }

        //        // Cambiar el estado a "Pendiente"
        //        propuesta.estado = "Pendiente";
        //        propuesta.usuario_modificacion = DTOConverter.Convertir<SoftPacBusiness.UsuariosWS.usuariosDTO, SoftPacBusiness.PropuestaPagoWS.usuariosDTO>(UsuarioLogueado); ;
        //        propuesta.fecha_hora_modificacion = DateTime.Now;

        //        // Modificar la propuesta
        //        int resultado = propuestasPagoBO.Modificar(propuesta);

        //        if (resultado > 0)
        //        {
        //            MostrarMensaje("La propuesta ha sido rechazada exitosamente.", "success");
        //            CargarPropuestasPendientes();
        //        }
        //        else
        //        {
        //            MostrarMensaje("No se pudo rechazar la propuesta.", "danger");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MostrarMensaje($"Error al rechazar la propuesta: {ex.Message}", "danger");
        //    }
        //}

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlError.Visible = true;
            pnlError.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblError.Text = mensaje;
        }
    }
}