using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class NuevaFactura : Page
    {
        private const string DetallesSessionKey = "NuevaFacturaDetalles";
        private readonly FacturasBO facturasBO = new FacturasBO();
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();
        private readonly MonedasBO monedasBO = new MonedasBO();

        private BindingList<DetallesFacturaDTO> DetallesFactura
        {
            get
            {
                var detalles = Session[DetallesSessionKey] as BindingList<DetallesFacturaDTO>;
                if (detalles == null)
                {
                    detalles = new BindingList<DetallesFacturaDTO>();
                    Session[DetallesSessionKey] = detalles;
                }
                return detalles;
            }
            set
            {
                Session[DetallesSessionKey] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove(DetallesSessionKey);
                DetallesFactura = new BindingList<DetallesFacturaDTO>();
                CargarCombos();
                InicializarFormulario();

                string idParam = Request.QueryString["id"];
                if (int.TryParse(idParam, out int facturaId))
                {
                    CargarFactura(facturaId);
                }
                else
                {
                    ActualizarMontos();
                }
            }
        }

        private void CargarCombos()
        {
            try
            {
                var proveedores = acreedoresBO.ListarTodos()
                    .Where(a => a.Activo)
                    .OrderBy(a => a.RazonSocial)
                    .ToList();

                ddlProveedor.DataSource = proveedores;
                ddlProveedor.DataTextField = "RazonSocial";
                ddlProveedor.DataValueField = "AcreedorId";
                ddlProveedor.DataBind();
                ddlProveedor.Items.Insert(0, new ListItem("Seleccione un proveedor", string.Empty));

                var monedas = monedasBO.ListarTodos()
                    .OrderBy(m => m.Nombre)
                    .ToList();

                ddlMoneda.DataSource = monedas;
                ddlMoneda.DataTextField = "Nombre";
                ddlMoneda.DataValueField = "MonedaId";
                ddlMoneda.DataBind();
                ddlMoneda.Items.Insert(0, new ListItem("Seleccione una moneda", string.Empty));
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar catálogos: " + ex.Message, "danger");
            }
        }

        private void InicializarFormulario()
        {
            hfFacturaId.Value = string.Empty;
            txtNumeroFactura.Text = string.Empty;
            if (ddlProveedor.Items.Count > 0)
            {
                ddlProveedor.SelectedIndex = 0;
            }
            if (ddlMoneda.Items.Count > 0)
            {
                ddlMoneda.SelectedIndex = 0;
            }
            txtFechaEmision.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaRecepcion.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFechaLimitePago.Text = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
            ddlEstado.SelectedValue = "Pendiente";
            txtRegimenFiscal.Text = string.Empty;
            txtTasaIva.Text = "18";
            txtOtrosTributos.Text = "0";
            txtDescripcionDetalle.Text = string.Empty;
            txtSubtotalDetalle.Text = string.Empty;
            gvDetalles.DataSource = DetallesFactura;
            gvDetalles.DataBind();
        }

        private void CargarFactura(int facturaId)
        {
            try
            {
                var factura = facturasBO.ObtenerPorId(facturaId);
                if (factura == null)
                {
                    MostrarMensaje("No se encontró la factura solicitada.", "warning");
                    return;
                }

                hfFacturaId.Value = factura.FacturaId?.ToString();
                txtNumeroFactura.Text = factura.NumeroFactura;
                if (factura.Acreedor?.AcreedorId != null)
                {
                    var proveedorItem = ddlProveedor.Items.FindByValue(factura.Acreedor.AcreedorId.ToString());
                    if (proveedorItem != null)
                    {
                        ddlProveedor.SelectedValue = proveedorItem.Value;
                    }
                }
                if (factura.Moneda?.MonedaId != null)
                {
                    var monedaItem = ddlMoneda.Items.FindByValue(factura.Moneda.MonedaId.ToString());
                    if (monedaItem != null)
                    {
                        ddlMoneda.SelectedValue = monedaItem.Value;
                    }
                }
                txtFechaEmision.Text = factura.FechaEmision?.ToString("yyyy-MM-dd");
                txtFechaRecepcion.Text = factura.FechaRecepcion?.ToString("yyyy-MM-dd");
                txtFechaLimitePago.Text = factura.FechaLimitePago?.ToString("yyyy-MM-dd");
                string estado = string.IsNullOrWhiteSpace(factura.Estado) ? "Pendiente" : factura.Estado;
                var estadoItem = ddlEstado.Items.FindByValue(estado);
                ddlEstado.SelectedValue = estadoItem != null ? estadoItem.Value : "Pendiente";
                txtRegimenFiscal.Text = factura.RegimenFiscal;
                txtTasaIva.Text = factura.TasaIva.ToString("0.##");
                txtOtrosTributos.Text = factura.OtrosTributos.ToString("0.##");

                var detalles = factura.DetallesFactura != null
                    ? factura.DetallesFactura.Select(d => new DetallesFacturaDTO(d)).ToList()
                    : new List<DetallesFacturaDTO>();

                DetallesFactura = new BindingList<DetallesFacturaDTO>(detalles);
                gvDetalles.DataSource = DetallesFactura;
                gvDetalles.DataBind();
                ActualizarMontos();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar la factura: " + ex.Message, "danger");
            }
        }

        protected void btnAgregarDetalle_Click(object sender, EventArgs e)
        {
            Page.Validate("Detalle");
            if (!Page.IsValid)
            {
                return;
            }

            try
            {
                decimal subtotal = ParseDecimal(txtSubtotalDetalle.Text);
                var detalle = new DetallesFacturaDTO
                {
                    DetalleFacturaId = GenerarNuevoDetalleId(),
                    Descripcion = txtDescripcionDetalle.Text.Trim(),
                    Subtotal = subtotal
                };

                DetallesFactura.Add(detalle);
                txtDescripcionDetalle.Text = string.Empty;
                txtSubtotalDetalle.Text = string.Empty;

                gvDetalles.DataSource = DetallesFactura;
                gvDetalles.DataBind();
                ActualizarMontos();
            }
            catch (Exception ex)
            {
                MostrarMensaje("No se pudo agregar el detalle: " + ex.Message, "danger");
            }
        }

        protected void gvDetalles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EliminarDetalle")
            {
                return;
            }

            if (int.TryParse(e.CommandArgument.ToString(), out int detalleId))
            {
                var detalle = DetallesFactura.FirstOrDefault(d => d.DetalleFacturaId == detalleId);
                if (detalle != null)
                {
                    DetallesFactura.Remove(detalle);
                    gvDetalles.DataSource = DetallesFactura;
                    gvDetalles.DataBind();
                    ActualizarMontos();
                }
            }
        }

        protected void txtTasaIva_TextChanged(object sender, EventArgs e)
        {
            ActualizarMontos();
        }

        protected void txtOtrosTributos_TextChanged(object sender, EventArgs e)
        {
            ActualizarMontos();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Page.Validate("NuevaFactura");
            if (!Page.IsValid)
            {
                return;
            }

            if (!DetallesFactura.Any())
            {
                MostrarMensaje("Agregue al menos un detalle a la factura.", "warning");
                return;
            }

            try
            {
                var factura = ObtenerFacturaDesdeFormulario();
                bool esEdicion = !string.IsNullOrWhiteSpace(hfFacturaId.Value);

                int resultado = esEdicion ? facturasBO.Modificar(factura) : facturasBO.Insertar(factura);
                if (resultado == 1)
                {
                    Session.Remove(DetallesSessionKey);
                    MostrarMensaje("Factura guardada correctamente.", "success");
                    Response.Redirect("Facturas.aspx", false);
                }
                else
                {
                    MostrarMensaje("No se pudo guardar la factura. Inténtelo nuevamente.", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar la factura: " + ex.Message, "danger");
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove(DetallesSessionKey);
            Response.Redirect("Facturas.aspx", false);
        }

        private FacturasDTO ObtenerFacturaDesdeFormulario()
        {
            var factura = new FacturasDTO();

            if (int.TryParse(hfFacturaId.Value, out int facturaId))
            {
                factura.FacturaId = facturaId;
            }
            else
            {
                factura.FacturaId = GenerarNuevoFacturaId();
            }

            factura.NumeroFactura = txtNumeroFactura.Text.Trim();
            factura.FechaEmision = ParseNullableDate(txtFechaEmision.Text);
            factura.FechaRecepcion = ParseNullableDate(txtFechaRecepcion.Text);
            factura.FechaLimitePago = ParseNullableDate(txtFechaLimitePago.Text);
            factura.Estado = ddlEstado.SelectedValue;
            factura.RegimenFiscal = txtRegimenFiscal.Text.Trim();
            factura.TasaIva = ParseDecimal(txtTasaIva.Text);
            factura.OtrosTributos = ParseDecimal(txtOtrosTributos.Text);

            var detalles = DetallesFactura.Select(d => new DetallesFacturaDTO(d)).ToList();
            factura.DetallesFactura = new BindingList<DetallesFacturaDTO>(detalles);

            decimal subtotal = factura.DetallesFactura.Sum(d => d.Subtotal);
            decimal montoIgv = Math.Round(subtotal * (factura.TasaIva / 100), 2);
            factura.MontoIgv = montoIgv;
            factura.MontoTotal = subtotal + montoIgv + factura.OtrosTributos;
            factura.MontoRestante = factura.MontoTotal;

            if (!string.IsNullOrEmpty(ddlProveedor.SelectedValue) && int.TryParse(ddlProveedor.SelectedValue, out int acreedorId))
            {
                var proveedor = acreedoresBO.ListarTodos().FirstOrDefault(a => a.AcreedorId == acreedorId);
                factura.Acreedor = proveedor != null ? new AcreedoresDTO(proveedor) : new AcreedoresDTO { AcreedorId = acreedorId, RazonSocial = ddlProveedor.SelectedItem.Text };
            }

            if (!string.IsNullOrEmpty(ddlMoneda.SelectedValue) && int.TryParse(ddlMoneda.SelectedValue, out int monedaId))
            {
                var moneda = monedasBO.ListarTodos().FirstOrDefault(m => m.MonedaId == monedaId);
                if (moneda != null)
                {
                    factura.Moneda = new MonedasDTO
                    {
                        MonedaId = moneda.MonedaId,
                        Nombre = moneda.Nombre,
                        CodigoIso = moneda.CodigoIso,
                        Simbolo = moneda.Simbolo
                    };
                }
                else
                {
                    factura.Moneda = new MonedasDTO
                    {
                        MonedaId = monedaId,
                        Nombre = ddlMoneda.SelectedItem.Text
                    };
                }
            }

            foreach (var detalle in factura.DetallesFactura)
            {
                detalle.Factura = factura;
            }

            return factura;
        }

        private int GenerarNuevoFacturaId()
        {
            var facturas = facturasBO.ListarTodos();
            return facturas.Any() ? facturas.Max(f => f.FacturaId ?? 0) + 1 : 1;
        }

        private int GenerarNuevoDetalleId()
        {
            return DetallesFactura.Any() ? DetallesFactura.Max(d => d.DetalleFacturaId ?? 0) + 1 : 1;
        }

        private void ActualizarMontos()
        {
            decimal subtotal = DetallesFactura.Sum(d => d.Subtotal);
            decimal tasaIva = ParseDecimal(txtTasaIva.Text);
            decimal otrosTributos = ParseDecimal(txtOtrosTributos.Text);
            decimal montoIgv = Math.Round(subtotal * (tasaIva / 100), 2);
            decimal total = subtotal + montoIgv + otrosTributos;

            lblSubtotal.Text = subtotal.ToString("N2");
            lblIgv.Text = montoIgv.ToString("N2");
            lblTotal.Text = total.ToString("N2");
        }

        private DateTime? ParseNullableDate(string value)
        {
            return DateTime.TryParse(value, out var parsed) ? parsed : (DateTime?)null;
        }

        private decimal ParseDecimal(string value)
        {
            return decimal.TryParse(value, out decimal parsed) ? parsed : 0m;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            string mensajeCodificado = HttpUtility.JavaScriptStringEncode(mensaje);
            string script = $@"
                $(document).ready(function() {{
                    var alertHtml = '<div class=\"alert alert-{tipo} alert-dismissible fade show\" role=\"alert\">' +
                                    '{mensajeCodificado}' +
                                    '<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\" aria-label=\"Cerrar\"></button>' +
                                    '</div>';
                    $('.content-area').prepend(alertHtml);
                    setTimeout(function () {{
                        $('.alert').fadeOut();
                    }}, 5000);
                }});";

            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), script, true);
        }
    }
}
