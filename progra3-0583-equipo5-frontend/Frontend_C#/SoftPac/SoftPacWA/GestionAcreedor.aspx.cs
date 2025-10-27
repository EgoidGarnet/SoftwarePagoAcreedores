using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using SoftPac.Business;
using SoftPac.Model;

namespace SoftPacWA
{
    public partial class GestionAcreedor : Page
    {
        private readonly AcreedoresBO acreedoresBO = new AcreedoresBO();
        private List<PaisesDTO> paisesUsuario;

        private UsuariosDTO UsuarioLogueado { get { return (UsuariosDTO)Session["UsuarioLogueado"]; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UsuarioLogueado == null) Response.Redirect("~/Login.aspx");
            paisesUsuario = UsuarioLogueado.UsuarioPais.Where(up => up.Acceso).Select(up => up.Pais).ToList();

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
            ddlPais.DataTextField = "Nombre";
            ddlPais.DataValueField = "PaisId";
            ddlPais.DataBind();
        }

        private void CargarAcreedor(int id)
        {
            AcreedoresDTO a = acreedoresBO.obtenerPorId(id);
            if (a == null) { Response.Redirect("Acreedores.aspx"); return; }

            txtRazon.Text = a.RazonSocial;
            txtRuc.Text = a.Ruc;
            txtDir.Text = a.DireccionFiscal;
            txtCondicion.Text = a.Condicion;
            txtPlazo.Text = a.PlazoDePago.HasValue ? a.PlazoDePago.Value.ToString() : "0";
            if (a.Pais != null && a.Pais.PaisId.HasValue)
            {
                if (ddlPais.Items.FindByValue(a.Pais.PaisId.Value.ToString()) != null)
                    ddlPais.SelectedValue = a.Pais.PaisId.Value.ToString();
            }
            ddlActivo.SelectedValue = a.Activo ? "S" : "N";
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
