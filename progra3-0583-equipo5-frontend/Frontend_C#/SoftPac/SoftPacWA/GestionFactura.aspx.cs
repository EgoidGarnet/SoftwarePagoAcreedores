using SoftPac.Business;
using SoftPac.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class GestionFactura : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();
        private AcreedoresBO acreedoresBO = new AcreedoresBO();
        private PaisesBO paisesBO = new PaisesBO();
        private MonedasBO monedasBO = new MonedasBO();
        private FacturasDTO factura;

        private UsuariosDTO UsuarioLogueado
        {
            get
            {
                return (UsuariosDTO)Session["UsuarioLogueado"];
            }
        }

        private string Accion
        {
            get { return Request.QueryString["accion"] ?? "insertar"; }
        }

        private int? FacturaId
        {
            get { return Session["facturaId"] as int?; }
            set { Session["facturaId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (FacturaId.HasValue)
            {
                factura = facturasBO.ObtenerPorId(FacturaId.Value);
            }
            if (!IsPostBack)
            {
                if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");
                CargarControlesIniciales();
                ConfigurarPaginaPorAccion();
            }
        }

        private void CargarControlesIniciales()
        {
            CargarPaises();
            CargarAcreedores();
        }

        private void ConfigurarPaginaPorAccion()
        {
            switch (Accion.ToLower())
            {
                case "insertar":
                    ConfigurarModoInsertar();
                    btnEliminarFactura.Visible = false;
                    break;
                case "editar":
                    ConfigurarModoEditar();
                    break;
                case "verdetalle":
                    ConfigurarModoVerDetalle();
                    break;
                default:
                    Response.Redirect("Facturas.aspx");
                    break;
            }
        }

        #region Configuracion de Modos

        private void ConfigurarModoInsertar()
        {
            lblTitulo.Text = "Nueva Factura";
            btnGuardar.Visible = true;
            btnRegresar.Visible = true;
            btnCancelar.Visible = false;
            btnEditar.Visible = false;
            btnNuevoDetalle.Visible = true;
            HabilitarControles(true);
            ddlEstado.SelectedValue = "Pendiente";
            ddlEstado.Enabled = false; 
            txtMontoTotal.Attributes.Add("onkeyup", $"document.getElementById('{txtMontoRestante.ClientID}').value = this.value;");
        }

        private void ConfigurarModoEditar()
        {
            lblTitulo.Text = "Editar Factura";
            btnGuardar.Visible = true;
            btnRegresar.Visible = true;
            btnCancelar.Visible = true;
            btnEditar.Visible = false;
            btnNuevoDetalle.Visible = true;
            HabilitarControles(true); 
            txtNumeroFactura.ReadOnly = true;
            ddlPais.Enabled = false;
            ddlAcreedor.Enabled = false;
            btnEliminarFactura.Visible = true; 
            if (factura != null && factura.MontoRestante != factura.MontoTotal)
            {
                btnEliminarFactura.Enabled = false;
                btnEliminarFactura.ToolTip = "No se puede eliminar una factura con pagos registrados.";
            }

            CargarDatosFactura();
        }

        private void ConfigurarModoVerDetalle()
        {
            lblTitulo.Text = "Detalle de Factura";
            btnGuardar.Visible = false;
            btnRegresar.Visible = true;
            btnCancelar.Visible = false;
            btnEditar.Visible = true;
            btnNuevoDetalle.Visible = false;
            btnEliminarFactura.Visible = true;
            if (factura != null && factura.MontoRestante != factura.MontoTotal)
            {
                btnEditar.Enabled = false;
                btnEditar.ToolTip = "No se puede editar una factura con pagos registrados.";
                btnEliminarFactura.Enabled = false;
                btnEliminarFactura.ToolTip = "No se puede eliminar una factura con pagos registrados.";
            }
            HabilitarControles(false);
            CargarDatosFactura();
        }

        #endregion

        #region Carga de Datos

        private void CargarPaises()
        {
            ddlPais.DataSource = UsuarioLogueado.UsuarioPais.Where(up => up.Acceso == true).Select(up => up.Pais).ToList();
            ddlPais.DataTextField = "Nombre";
            ddlPais.DataValueField = "PaisId";
            ddlPais.DataBind();
            ddlPais.Items.Insert(0, new ListItem("-- Seleccione un País --", "0"));
        }

        private void CargarAcreedores()
        {
            var acreedores = acreedoresBO.ListarTodos().ToList();
            if (ddlPais.SelectedValue != "0")
            {
                int paisId = Convert.ToInt32(ddlPais.SelectedValue);
                acreedores = acreedores.Where(a => a.Pais.PaisId == paisId).ToList();
            }

            ddlAcreedor.DataSource = acreedores;
            ddlAcreedor.DataTextField = "RazonSocial";
            ddlAcreedor.DataValueField = "AcreedorId";
            ddlAcreedor.DataBind();
            ddlAcreedor.Items.Insert(0, new ListItem("-- Seleccione un Acreedor --", "0"));
        }

        private void CargarMonedas()
        {
            // 1. Obtener la lista completa de todas las monedas
            var todasLasMonedas = monedasBO.ListarTodos();
            var monedasFiltradas = new List<MonedasDTO>();

            // 2. Determinar qué monedas mostrar según el país
            string paisSeleccionado = ddlPais.SelectedItem.Text;

            switch (paisSeleccionado)
            {
                case "Perú":
                    monedasFiltradas = todasLasMonedas.Where(m => m.CodigoIso == "PEN" || m.CodigoIso == "USD").ToList();
                    break;

                case "Mexico":
                    monedasFiltradas = todasLasMonedas.Where(m => m.CodigoIso == "MXN" || m.CodigoIso == "USD").ToList();
                    break;

                case "Colombia":
                    monedasFiltradas = todasLasMonedas.Where(m => m.CodigoIso == "COP" || m.CodigoIso == "USD").ToList();
                    break;

                default:
                    // Si no hay país seleccionado, la lista quedará vacía.
                    // Opcionalmente, podrías cargar todas las monedas aquí si lo prefieres.
                    monedasFiltradas = todasLasMonedas.ToList();
                    break;
            }

            // 3. Enlazar la lista filtrada al DropDownList
            ddlMoneda.DataSource = monedasFiltradas;
            ddlMoneda.DataTextField = "Nombre";
            ddlMoneda.DataValueField = "MonedaId";
            ddlMoneda.DataBind();
        }

        private void CargarDatosFactura()
        {
            if (FacturaId.HasValue)
            {
                factura = facturasBO.ObtenerPorId(FacturaId.Value);
                if (factura != null)
                {
                    txtNumeroFactura.Text = factura.NumeroFactura;
                    ddlEstado.SelectedValue = factura.Estado;

                    if (factura.Acreedor != null && factura.Acreedor.Pais != null)
                    {
                        ddlPais.SelectedValue = factura.Acreedor.Pais.PaisId.ToString();
                        CargarAcreedores();
                        ddlAcreedor.SelectedValue = factura.Acreedor.AcreedorId.ToString();
                    }
                    CargarMonedas();
                    txtFechaEmision.Text = factura.FechaEmision?.ToString("yyyy-MM-dd");
                    txtFechaRecepcion.Text = factura.FechaRecepcion?.ToString("yyyy-MM-dd");
                    txtFechaLimitePago.Text = factura.FechaLimitePago?.ToString("yyyy-MM-dd");

                    ddlMoneda.SelectedValue = factura.Moneda.MonedaId.ToString();
                    txtMontoTotal.Text = factura.MontoTotal.ToString("F2");
                    txtMontoIgv.Text = factura.MontoIgv.ToString("F2");
                    txtMontoRestante.Text = factura.MontoRestante.ToString("F2");

                    txtRegimenFiscal.Text = factura.RegimenFiscal;
                    txtTasaIva.Text = factura.TasaIva.ToString("F2");
                    txtOtrosTributos.Text = factura.OtrosTributos.ToString("F2");

                    gvDetallesFactura.DataSource = factura.DetallesFactura;
                    if (Accion.ToLower() == "verdetalle")
                    {
                        gvDetallesFactura.Columns[2].Visible = false;
                    }
                    gvDetallesFactura.DataBind();

                    ActualizarCamposPorPais();
                }
            }
            else
            {
                MostrarMensaje("No se pudo cargar la factura", "danger");
            }
        }
        private bool ValidarCamposObligatorios()
        {
            // Lista de campos de texto obligatorios y su nombre amigable
            var camposTexto = new List<Tuple<TextBox, string>>
            {
                Tuple.Create(txtNumeroFactura, "Número de Factura"),
                Tuple.Create(txtFechaEmision, "Fecha de Emisión"),
                Tuple.Create(txtFechaRecepcion, "Fecha de Recepción"),
                Tuple.Create(txtFechaLimitePago, "Fecha Límite de Pago"),
                Tuple.Create(txtMontoTotal, "Monto Total"),
                Tuple.Create(txtMontoIgv, "Monto IGV"),
                Tuple.Create(txtRegimenFiscal, "Régimen Fiscal")
            };

            // Validar campos de texto
            foreach (var campo in camposTexto)
            {
                if (string.IsNullOrWhiteSpace(campo.Item1.Text))
                {
                    MostrarMensaje($"El campo '{campo.Item2}' es obligatorio.", "warning");
                    return false; // Detiene la validación en el primer error
                }
            }

            // Validar listas desplegables
            if (ddlPais.SelectedValue == "0")
            {
                MostrarMensaje("Debe seleccionar un País.", "warning");
                return false;
            }

            if (ddlAcreedor.SelectedValue == "0")
            {
                MostrarMensaje("Debe seleccionar un Acreedor.", "warning");
                return false;
            }

            return true; // Si todo está correcto
        }

        #endregion

        #region Acciones de Botones

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            FacturasDTO factura = new FacturasDTO();
            if (Accion == "editar" && FacturaId.HasValue)
            {
                factura = facturasBO.ObtenerPorId(FacturaId.Value);
            }
            if (!ValidarCamposObligatorios()) return;
            if(Accion == "insertar")
            {
                factura.NumeroFactura = txtNumeroFactura.Text;
                factura.Acreedor = acreedoresBO.obtenerPorId(Convert.ToInt32(ddlAcreedor.SelectedValue));
            }
            factura.Estado = ddlEstado.SelectedValue;
            if(!DateTime.TryParse(txtFechaEmision.Text, out DateTime fe))
            {
                MostrarMensaje("La fecha de emisión no es válida", "warning");
                return;
            }
            if (!DateTime.TryParse(txtFechaRecepcion.Text, out DateTime fr))
            {
                MostrarMensaje("La fecha de recepción no es válida", "warning");
                return;
            }
            if (!DateTime.TryParse(txtFechaLimitePago.Text, out DateTime fl))
            {
                MostrarMensaje("La fecha límite de pago no es válida", "warning");
                return;
            }
            factura.FechaEmision = Convert.ToDateTime(txtFechaEmision.Text);
            factura.FechaRecepcion = Convert.ToDateTime(txtFechaRecepcion.Text);
            factura.FechaLimitePago = Convert.ToDateTime(txtFechaLimitePago.Text);
            factura.Moneda = monedasBO.ObtenerPorId(Convert.ToInt32(ddlMoneda.SelectedValue));
            factura.MontoTotal = Convert.ToDecimal(txtMontoTotal.Text);
            factura.MontoIgv = Convert.ToDecimal(txtMontoIgv.Text);
            factura.RegimenFiscal = txtRegimenFiscal.Text;
            factura.TasaIva = string.IsNullOrEmpty(txtTasaIva.Text) ? 0 : Convert.ToDecimal(txtTasaIva.Text);
            factura.OtrosTributos = string.IsNullOrEmpty(txtOtrosTributos.Text) ? 0 : Convert.ToDecimal(txtOtrosTributos.Text);

            if (Accion == "insertar")
            {
                factura.MontoRestante = factura.MontoTotal;
                facturasBO.Insertar(factura);
                Session["MensajeExito"] = "Factura creada correctamente. Ahora puedes agregar detalles.";
                FacturaId = factura.FacturaId;
                Response.Redirect($"GestionFactura.aspx?accion=editar");
            }
            else
            {
                facturasBO.Modificar(factura);
                MostrarMensaje("Factura guardada correctamente", "success");
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Facturas.aspx");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"GestionFactura.aspx?accion=verDetalle");
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            Response.Redirect($"GestionFactura.aspx?accion=editar");
        }

        protected void btnAccion_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int detalleId = int.Parse(btn.CommandArgument);
            DetallesFacturaDTO detalleFactura = factura.DetallesFactura.FirstOrDefault(n => n.DetalleFacturaId == detalleId);
            Session["Detalle"] = detalleFactura;
            switch (btn.CommandName.ToLower())
            {
                case "editar":
                    Response.Redirect("GestionDetalleFactura.aspx?accion=editar");
                    break;
                case "eliminar":
                    var resultado = facturasBO.EliminarDetalle(detalleFactura,UsuarioLogueado);
                    if (resultado == 1)
                    {
                        MostrarMensaje("Detalle eliminado correctamente", "success");
                        CargarDatosFactura();
                    }
                    else
                    {
                        MostrarMensaje("Error al eliminar el detalle", "danger");
                    }
                    break;
            }
        }

        protected void btnNuevoDetalle_Click(object sender, EventArgs e)
        {
            // Guardar el ID de factura en sesión para el detalle
            if (FacturaId.HasValue)
            {
                Session["Detalle"] = new DetallesFacturaDTO(0,0,null,factura); // Es necesario incluir la factura para mostrar la moneda al insertar nuevo detalle
                Response.Redirect("GestionDetalleFactura.aspx?accion=insertar");
            }
            else
            {
                MostrarMensaje("Guarda primero la factura", "danger");
            }
        }
        protected void btnEliminarFactura_Click(object sender, EventArgs e)
        {
            try
            {
                if (FacturaId.HasValue)
                {
                    var resultado = facturasBO.Eliminar(FacturaId.Value, UsuarioLogueado);
                    if (resultado == 1)
                    {
                        Session["MensajeExito"] = "Factura eliminada correctamente.";
                        Response.Redirect("Facturas.aspx");
                    }
                    else
                    {
                        MostrarMensaje("Error: No se pudo eliminar la factura.", "danger");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error inesperado al eliminar: {ex.Message}", "danger");
                // AppLogger.RegistrarError(ex); // <-- Si tienes un logger
            }
        }

        #endregion
        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarAcreedores();
            CargarMonedas();
            ActualizarCamposPorPais();
        }

        private void HabilitarControles(bool habilitar)
        {
            // Habilitar o deshabilitar cada control individualmente
            txtNumeroFactura.ReadOnly = !habilitar;
            ddlEstado.Enabled = habilitar;
            ddlPais.Enabled = habilitar;
            ddlAcreedor.Enabled = habilitar;
            txtFechaEmision.ReadOnly = !habilitar;
            txtFechaRecepcion.ReadOnly = !habilitar;
            txtFechaLimitePago.ReadOnly = !habilitar;
            ddlMoneda.Enabled = habilitar;
            txtMontoTotal.ReadOnly = !habilitar;
            txtMontoIgv.ReadOnly = !habilitar;
            txtRegimenFiscal.ReadOnly = !habilitar;
            txtTasaIva.ReadOnly = !habilitar;
            txtOtrosTributos.ReadOnly = !habilitar;

            // Los campos calculados o que no edita el usuario siempre son de solo lectura
            txtMontoRestante.ReadOnly = true;
        }

        private void ActualizarCamposPorPais()
        {
            // Si no hay un país seleccionado, se oculta todo.
            if (ddlPais.SelectedValue == "0")
            {
                divMontos.Visible = false;
                divFiscal.Visible = false;
                return;
            }

            // Si hay un país, se muestran las secciones.
            divMontos.Visible = true;
            divFiscal.Visible = true;

            string paisSeleccionado = ddlPais.SelectedItem.Text;

            switch (paisSeleccionado)
            {
                case "Perú":
                    lblMontoIgv.Text = "IGV";
                    lblRegimenFiscal.Text = "Régimen";
                    divTasaIva.Visible = false;
                    divOtrosTributos.Visible = false;
                    break;

                case "Colombia":
                case "Mexico":
                    lblMontoIgv.Text = "IVA";
                    lblRegimenFiscal.Text = "Régimen Fiscal";
                    divTasaIva.Visible = true;
                    divOtrosTributos.Visible = true;
                    break;

                default: // Un valor por defecto por si acaso
                    lblMontoIgv.Text = "Impuesto";
                    lblRegimenFiscal.Text = "Régimen Fiscal";
                    divTasaIva.Visible = false;
                    divOtrosTributos.Visible = false;
                    break;
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            // Escapar comillas simples y dobles para evitar errores de JS
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