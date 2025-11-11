using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using SoftPac.Business;
using SoftPacBusiness.AcreedoresWS;
using SoftPacBusiness.UsuariosWS;
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

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
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