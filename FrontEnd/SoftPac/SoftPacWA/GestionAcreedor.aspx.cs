using SoftPac.Business;
using SoftPacBusiness.AcreedoresWS;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // <-- agregado
using System.Web.UI;
using System.Web.UI.WebControls;
using paisesDTO = SoftPacBusiness.UsuariosWS.paisesDTO;
using usuariosDTO = SoftPacBusiness.UsuariosWS.usuariosDTO;

namespace SoftPacWA
{
    public partial class GestionAcreedor : Page
    {
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();
        private List<paisesDTO> paisesUsuario;

        private usuariosDTO UsuarioLogueado { get { return (usuariosDTO)Session["UsuarioLogueado"]; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");
            paisesUsuario = UsuarioLogueado.usuario_pais.Where(up => up.acceso).Select(up => up.pais).ToList();

            if (!IsPostBack)
            {
                CargarPaises();

                string accion = (Request.QueryString["accion"] ?? "insertar").ToLower();
                hfAccion.Value = accion;
                if (accion == "editar")
                {
                    int id;
                    if (int.TryParse(Request.QueryString["id"], out id))
                    {
                        hfId.Value = id.ToString();
                        lblTitulo.Text = "Editar acreedor";
                        CargarAcreedor(id);
                    }
                    else
                    {
                        Response.Redirect("Acreedores.aspx");
                    }
                }
                else
                {
                    lblTitulo.Text = "Nuevo acreedor";
                    ddlActivo.SelectedValue = "S";
                }
            }
        }

        private void CargarPaises()
        {
            ddlPais.Items.Clear();

            // Opción vacía al inicio
            ddlPais.Items.Add(new ListItem("Seleccione un país", ""));

            ddlPais.DataSource = paisesUsuario;
            ddlPais.DataTextField = "nombre";
            ddlPais.DataValueField = "pais_id";
            ddlPais.DataBind();
        }


        private void CargarAcreedor(int id)
        {
            acreedoresDTO a = acreedoresBO.obtenerPorId(id);
            if (a == null) { Response.Redirect("Acreedores.aspx"); return; }

            txtRazon.Text = a.razon_social;
            txtRuc.Text = a.ruc;
            txtDir.Text = a.direccion_fiscal;
            txtCondicion.Text = a.condicion;
            txtPlazo.Text = a.plazo_de_pagoSpecified ? a.plazo_de_pago.ToString() : "0";
            if (a.pais != null && a.pais.pais_idSpecified)
            {
                if (ddlPais.Items.FindByValue(a.pais.pais_id.ToString()) != null)
                    ddlPais.SelectedValue = a.pais.pais_id.ToString();
            }
            ddlActivo.SelectedValue = a.activo ? "S" : "N";
        }

        // ------------------- VALIDACIÓN -------------------
        private bool ValidarCampos()
        {
            bool esValido = true;

            // Limpiar mensajes previos
            lblRazonError.Text = string.Empty;
            lblRucError.Text = string.Empty;
            lblPaisError.Text = string.Empty;
            lblDirError.Text = string.Empty;
            lblCondicionError.Text = string.Empty;
            lblPlazoError.Text = string.Empty;
            lblActivoError.Text = string.Empty;

            string razon = (txtRazon.Text ?? string.Empty).Trim();
            string ruc = (txtRuc.Text ?? string.Empty).Trim();
            string dir = (txtDir.Text ?? string.Empty).Trim();
            string cond = (txtCondicion.Text ?? string.Empty).Trim();
            string plazoTexto = (txtPlazo.Text ?? string.Empty).Trim();
            string paisValor = ddlPais.SelectedValue;
            string activoValor = ddlActivo.SelectedValue;

            if (string.IsNullOrEmpty(razon))
            {
                lblRazonError.Text = "Ingrese la razón social.";
                esValido = false;
            }

            if (string.IsNullOrEmpty(ruc))
            {
                lblRucError.Text = "Ingrese el RUC / NIT / RFC.";
                esValido = false;
            }
            else
            {
                int paisId = 0;
                int.TryParse(paisValor, out paisId);

                const int PAIS_PERU = 1;
                const int PAIS_MEXICO = 2;
                const int PAIS_COLOMBIA = 3;

                ruc = ruc.Trim().ToUpperInvariant();
                txtRuc.Text = ruc;

                bool idFiscalValido = true;

                if (paisId == PAIS_PERU)
                {
                    // RUC Perú: 11 dígitos
                    if (!Regex.IsMatch(ruc, @"^\d{11}$"))
                        idFiscalValido = false;
                }
                else if (paisId == PAIS_MEXICO)
                {
                    // RFC México empresa: 13 caracteres alfanuméricos
                    if (!Regex.IsMatch(ruc, @"^[A-Z0-9]{13}$"))
                        idFiscalValido = false;
                }
                else if (paisId == PAIS_COLOMBIA)
                {
                    // NIT Colombia: 10 dígitos
                    if (!Regex.IsMatch(ruc, @"^\d{10}$"))
                        idFiscalValido = false;
                }
                if (!idFiscalValido)
                {
                    lblRucError.Text = "Debe ser un RUC, NIT o RFC válido.";
                    esValido = false;
                }
            }

            if (string.IsNullOrEmpty(paisValor))
            {
                lblPaisError.Text = "Seleccione un país.";
                ddlPais.ClearSelection();
                ddlPais.SelectedValue = "";
                esValido = false;
            }


            if (string.IsNullOrEmpty(dir))
            {
                lblDirError.Text = "Ingrese la dirección fiscal.";
                esValido = false;
            }

            if (string.IsNullOrEmpty(cond))
            {
                lblCondicionError.Text = "Ingrese la condición.";
                esValido = false;
            }

            int plazo = 0;
            if (string.IsNullOrEmpty(plazoTexto))
            {
                lblPlazoError.Text = "Ingrese el plazo de pago en días.";
                esValido = false;
            }
            else if (!int.TryParse(plazoTexto, out plazo))
            {
                lblPlazoError.Text = "El plazo de pago debe ser un número entero.";
                esValido = false;
            }
            else
            {
                // 3) Plazo debe estar entre 15 y 60 días
                if (plazo < 15 || plazo > 60)
                {
                    lblPlazoError.Text = "El plazo de pago debe estar entre 15 y 60 días.";
                    esValido = false;
                }
            }

            if (string.IsNullOrEmpty(activoValor))
            {
                lblActivoError.Text = "Seleccione si el acreedor está activo o no.";
                esValido = false;
            }

            return esValido;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            string razon = (txtRazon.Text ?? "").Trim();
            string ruc = (txtRuc.Text ?? "").Trim();
            string dir = (txtDir.Text ?? "").Trim();
            string cond = (txtCondicion.Text ?? "").Trim();
            int plazo = 0; int.TryParse(txtPlazo.Text, out plazo);
            string activo = ddlActivo.SelectedValue;
            int paisId = int.Parse(ddlPais.SelectedValue);

            if (hfAccion.Value == "editar")
            {
                int id = int.Parse(hfId.Value);
                int ok = acreedoresBO.modificar(id, razon, ruc, dir, cond, plazo, activo, paisId);
                Response.Redirect("Acreedores.aspx");
            }
            else
            {
                int ok = acreedoresBO.insertar(razon, ruc, dir, cond, plazo, activo, paisId);
                Response.Redirect("Acreedores.aspx");
            }
        }
    }
}