using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SoftPacWA
{
    public partial class CrearPropuestaPaso1 : System.Web.UI.Page
    {
        private PaisesBO paisesBO = new PaisesBO();
        private EntidadesBancariasBO bancosBO = new EntidadesBancariasBO();
        private UsuariosBO usuariosBO = new UsuariosBO();
        private usuariosDTO UsuarioLogueado
        {
            get
            {
                return (usuariosDTO)Session["UsuarioLogueado"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!ValidarSesion())
                    return;

                CargarPaises();
                RestaurarDatos();
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

        private void CargarPaises()
        {
            try
            {
                var paisesAcceso = UsuarioLogueado.usuario_pais.Where(up => up.acceso== true)
                    .Select(up => up.pais)
                    .OrderBy(p => p.nombre)
                    .ToList();

                if (paisesAcceso.Count == 0)
                {
                    MostrarMensaje("No tiene acceso a ningún país. Contacte al administrador.", "warning");
                    btnSiguiente.Enabled = false;
                    return;
                }

                ddlPais.DataSource = paisesAcceso;
                ddlPais.DataTextField = "nombre";
                ddlPais.DataValueField = "pais_id";
                ddlPais.DataBind();
                ddlPais.Items.Insert(0, new ListItem("-- Seleccione un país --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar países: {ex.Message}", "danger");
            }
        }

        private void RestaurarDatos()
        {
            // Si hay datos guardados en sesión (usuario retrocedió), restaurarlos
            if (Session["PropuestaPago_PaisId"] != null)
            {
                ddlPais.SelectedValue = Session["PropuestaPago_PaisId"].ToString();
                CargarBancosPorPais(Convert.ToInt32(Session["PropuestaPago_PaisId"]));
                ddlEntidadBancaria.Enabled = true;

                if (Session["PropuestaPago_BancoId"] != null)
                {
                    ddlEntidadBancaria.SelectedValue = Session["PropuestaPago_BancoId"].ToString();
                }

                if (Session["PropuestaPago_PlazoVencimiento"] != null)
                {
                    ddlPlazoVencimiento.SelectedValue = Session["PropuestaPago_PlazoVencimiento"].ToString();
                }
            }
        }

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlPais.SelectedValue))
            {
                CargarBancosPorPais(Convert.ToInt32(ddlPais.SelectedValue));
                ddlEntidadBancaria.Enabled = true;
            }
            else
            {
                ddlEntidadBancaria.Items.Clear();
                ddlEntidadBancaria.Items.Add(new ListItem("-- Primero seleccione un país --", ""));
                ddlEntidadBancaria.Enabled = false;
            }
        }

        private void CargarBancosPorPais(int paisId)
        {
            try
            {
                var bancos = bancosBO.ListarTodos()
                    .Where(b => b.pais?.pais_id == paisId)
                    .OrderBy(b => b.nombre)
                    .ToList();

                ddlEntidadBancaria.DataSource = bancos;
                ddlEntidadBancaria.DataTextField = "nombre";
                ddlEntidadBancaria.DataValueField = "entidad_bancaria_id";
                ddlEntidadBancaria.DataBind();
                ddlEntidadBancaria.Items.Insert(0, new ListItem("-- Seleccione un banco --", ""));

                if (bancos.Count == 0)
                {
                    MostrarMensaje("No hay bancos disponibles para este país", "warning");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar bancos: {ex.Message}", "danger");
            }
        }

        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                // Guardar datos en sesión
                Session["PropuestaPago_PaisId"] = ddlPais.SelectedValue;
                Session["PropuestaPago_BancoId"] = ddlEntidadBancaria.SelectedValue;
                Session["PropuestaPago_PlazoVencimiento"] = ddlPlazoVencimiento.SelectedValue;

                // Calcular fecha límite de vencimiento
                int diasPlazo = Convert.ToInt32(ddlPlazoVencimiento.SelectedValue);
                DateTime fechaLimite = DateTime.Now.AddDays(diasPlazo);
                Session["PropuestaPago_FechaLimiteVencimiento"] = fechaLimite;

                // Redirigir al Paso 2
                Response.Redirect("~/CrearPropuestaPaso2.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al procesar los datos: {ex.Message}", "danger");
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            // Limpiar sesiones del flujo
            LimpiarSesionesPropuesta();
            Response.Redirect("~/PropuestasPago.aspx");
        }

        private void LimpiarSesionesPropuesta()
        {
            Session.Remove("PropuestaPago_PaisId");
            Session.Remove("PropuestaPago_BancoId");
            Session.Remove("PropuestaPago_PlazoVencimiento");
            Session.Remove("PropuestaPago_FechaLimiteVencimiento");
            Session.Remove("PropuestaPago_DetallesParciales");
            Session.Remove("PropuestaPago_CuentasSeleccionadas");
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }
    }
}