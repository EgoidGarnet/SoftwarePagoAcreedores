using SoftPac.Business;
using SoftPacBusiness.PropuestaPagoWS;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftPacWA
{
    public partial class AprobacionPropuestas : System.Web.UI.Page
    {
        private readonly PropuestasPagoBO propuestasPagoBO = new PropuestasPagoBO();

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

                var propuestas = propuestasPagoBO.ListarConFiltros(null, null, "pendiente")
                    ?? new List<propuestasPagoDTO>();

                var modelo = propuestas
                    .Where(p => p != null)
                    .Select(p => new PropuestaPendienteViewModel
                    {
                        PropuestaId = p.propuesta_id,
                        FechaCreacion = p.fecha_hora_creacion,
                        Usuario = FormatearNombreUsuario(p.usuario_creacion),
                        Pais = p.entidad_bancaria?.pais?.nombre ?? "Sin país",
                        Banco = p.entidad_bancaria?.nombre ?? "Sin banco",
                        NumeroPagos = p.detalles_propuesta?.Length ?? 0
                    })
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToList();

                gvPropuestasPendientes.DataSource = modelo;
                gvPropuestasPendientes.DataBind();

                lblTotalPendientes.Text = $"Propuestas pendientes: {modelo.Count}";
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

        private string FormatearNombreUsuario(usuariosDTO usuario)
        {
            if (usuario == null)
            {
                return "Sin usuario";
            }

            string nombre = usuario.nombre ?? string.Empty;
            string apellidos = usuario.apellidos ?? string.Empty;
            string nombreCompleto = $"{nombre} {apellidos}".Trim();

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                return usuario.nombre_de_usuario ?? "Sin usuario";
            }

            return nombreCompleto;
        }

        private class PropuestaPendienteViewModel
        {
            public int PropuestaId { get; set; }
            public DateTime FechaCreacion { get; set; }
            public string Usuario { get; set; }
            public string Pais { get; set; }
            public string Banco { get; set; }
            public int NumeroPagos { get; set; }
        }
    }
}