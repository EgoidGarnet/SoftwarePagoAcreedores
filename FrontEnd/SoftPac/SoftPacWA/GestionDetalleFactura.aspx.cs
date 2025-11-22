using SoftPac.Business;
using SoftPacBusiness.FacturasWS;
using System;
using System.Linq;
using System.Web.UI;

namespace SoftPacWA
{
    public partial class GestionDetalleFactura : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();

        // Propiedad para obtener la acción de la URL
        private string Accion => Request.QueryString["accion"] ?? "insertar";

        // Propiedad para manejar el objeto Detalle guardado en Sesión
        private detallesFacturaDTO DetalleActual
        {
            get { return Session["Detalle"] as detallesFacturaDTO; }
            set { Session["Detalle"] = value; }
        }

        // Propiedad para saber a qué factura regresar
        private int? FacturaIdPadre
        {
            get { return Session["facturaId"] as int?; }
            set { Session["facturaId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (DetalleActual != null) lblMoneda.Text = DetalleActual.factura.moneda.codigo_iso;
                ConfigurarPaginaPorAccion();
            }
        }

        private void ConfigurarPaginaPorAccion()
        {
            if (Accion.Equals("editar", StringComparison.OrdinalIgnoreCase))
            {
                lblTitulo.Text = "Editar Detalle de Factura";
                CargarDatosDetalle();
            }
            else
            {
                lblTitulo.Text = "Nuevo Detalle de Factura";
                DetalleActual = new detallesFacturaDTO(); // Limpiamos por si acaso
            }
        }

        private void CargarDatosDetalle()
        {
            if (DetalleActual != null)
            {
                txtDescripcion.Text = DetalleActual.descripcion;
                txtSubtotal.Text = DetalleActual.subtotal.ToString("F2");
            }
            else
            {
                // Si no hay detalle en sesión, no podemos editar. Regresamos.
                MostrarMensaje("No se encontró el detalle a editar.", "warning");
                Response.Redirect("Facturas.aspx");
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {

                if (Accion.Equals("insertar"))
                {
                    // Crear un nuevo detalle
                    detallesFacturaDTO nuevoDetalle = new detallesFacturaDTO();
                    nuevoDetalle.descripcion = txtDescripcion.Text;
                    nuevoDetalle.subtotal = Convert.ToDecimal(txtSubtotal.Text);
                    facturasDTO facturaAux = new facturasDTO();
                    facturaAux.factura_id = FacturaIdPadre ?? 0;
                    facturaAux = facturasBO.ObtenerPorId(FacturaIdPadre ?? 0);
                    if (facturaAux.detalles_Factura == null)
                        facturaAux.detalles_Factura = new detallesFacturaDTO[] { };
                    var lista = (facturaAux.detalles_Factura ?? Array.Empty<detallesFacturaDTO>()).ToList();
                    lista.Add(nuevoDetalle);
                    facturaAux.detalles_Factura = lista.ToArray();
                    nuevoDetalle.factura = facturaAux;
                    facturasBO.InsertarDetalle(nuevoDetalle);
                }
                else // Editar
                {
                    // Actualizar el detalle existente
                    var detalleModificado = DetalleActual;
                    detalleModificado.descripcion = txtDescripcion.Text;
                    detalleModificado.subtotal = Convert.ToDecimal(txtSubtotal.Text);
                    detalleModificado.factura = facturasBO.ObtenerPorId(FacturaIdPadre ?? 0);
                    facturasBO.ModificarDetalle(detalleModificado);
                }

                // Limpiar sesión y redirigir
                DetalleActual = null;
                Response.Redirect($"GestionFactura.aspx?accion=editar");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar el detalle: " + ex.Message, "danger");
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            DetalleActual = null; // Limpiar la sesión
            Response.Redirect($"GestionFactura.aspx?accion=editar");
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MostrarMensaje("El campo 'Descripción' es obligatorio.", "warning");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSubtotal.Text))
            {
                MostrarMensaje("El campo 'Subtotal' es obligatorio.", "warning");
                return false;
            }
            return true;
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
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarMensajeDetalle", script, true);
        }
    }
}