using SoftPac.Business;
using SoftPacBusiness.PropuestaPagoWS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftPacWA
{
    public partial class AprobacionPropuestas : System.Web.UI.Page
    {
        private PropuestasPagoBO propuestasPagoBO = new PropuestasPagoBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPropuestasPendientes();
            }
        }

        private void CargarPropuestasPendientes()
        {
            try
            {
                pnlError.Visible = false;

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

                if (propuestas.Count > 0)
                {
                    gvPropuestasPendientes.DataSource = propuestas;
                    gvPropuestasPendientes.DataBind();
                    gvPropuestasPendientes.Visible = true;
                    //pnlEmptyState.Visible = false;
                    //lblTotalRegistros.Text = $"Mostrando {propuestas.Count} propuesta(s)";
                }
                else
                {
                    gvPropuestasPendientes.Visible = false;
                    //pnlEmptyState.Visible = true;
                    //lblTotalRegistros.Text = "0 propuestas";
                }
                upPropuestas.Update();


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
    }
}