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
    public partial class GestionUsuarios : System.Web.UI.Page
    {
        private UsuariosBO usuariosBO = new UsuariosBO();
        private PaisesBO paisesBO = new PaisesBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarUsuarios();
                CargarPaises();
            }
        }

        #region Carga de Datos
        private void CargarUsuarios()
        {
            gvUsuarios.DataSource = usuariosBO.ListarTodos();
            gvUsuarios.DataBind();
        }

        private void CargarPaises()
        {
            cblPaises.DataSource = paisesBO.ListarTodos();
            cblPaises.DataTextField = "Nombre";
            cblPaises.DataValueField = "PaisId";
            cblPaises.DataBind();
        }
        #endregion

        #region Acciones del Grid
        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int usuarioId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Modificar")
            {
                hfUsuarioId.Value = usuarioId.ToString();
                UsuariosDTO usuario = usuariosBO.ObtenerPorId(usuarioId);
                if (usuario != null)
                {
                    CargarDatosEnModal(usuario);
                    ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
                }
            }
            else if (e.CommandName == "Eliminar")
            {
                UsuariosDTO usuarioActual = (UsuariosDTO)Session["UsuarioLogueado"];
                int resultado = usuariosBO.EliminarUsuario(usuarioId, usuarioActual.UsuarioId.Value);
                if (resultado > 0)
                {
                    MostrarMensaje("Usuario eliminado correctamente.", "success");
                    CargarUsuarios();
                }
                else
                {
                    MostrarMensaje("Error al eliminar el usuario.", "danger");
                }
            }
        }
        #endregion

        #region Lógica del Modal (Crear/Editar)
        protected void btnAbrirModalNuevo_Click(object sender, EventArgs e)
        {
            LimpiarModal();
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
        }

        private void CargarDatosEnModal(UsuariosDTO usuario)
        {
            litModalTitulo.Text = "Modificar Usuario";

            // Campos no editables (en gris)
            txtNombre.Text = usuario.Nombre;
            txtNombre.Enabled = false;
            txtApellidos.Text = usuario.Apellidos;
            txtApellidos.Enabled = false;
            txtCorreo.Text = usuario.CorreoElectronico;
            txtCorreo.Enabled = false;
            chkSuperusuario.Checked = usuario.Superusuario;
            chkSuperusuario.Disabled = true;

            // Campos editables
            txtNombreUsuario.Text = usuario.NombreDeUsuario;
            txtNombreUsuario.Enabled = true;
            chkActivo.Checked = usuario.Activo;
            chkActivo.Disabled = false;

            // Limpiar selección de países y marcar los que tiene acceso
            cblPaises.ClearSelection();
            if (usuario.UsuarioPais != null)
            {
                foreach (var acceso in usuario.UsuarioPais)
                {
                    ListItem item = cblPaises.Items.FindByValue(acceso.Pais.PaisId.ToString());
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
            }
        }

        private void LimpiarModal()
        {
            litModalTitulo.Text = "Nuevo Usuario";
            hfUsuarioId.Value = "0";
            txtNombre.Text = string.Empty;
            txtNombre.Enabled = true;
            txtApellidos.Text = string.Empty;
            txtApellidos.Enabled = true;
            txtNombreUsuario.Text = string.Empty;
            txtNombreUsuario.Enabled = true;
            txtCorreo.Text = string.Empty;
            txtCorreo.Enabled = true;
            txtPasswordModal.Text = string.Empty;
            chkActivo.Checked = true;
            chkActivo.Disabled = false;
            chkSuperusuario.Checked = false;
            chkSuperusuario.Disabled = false;
            cblPaises.ClearSelection();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int usuarioId = Convert.ToInt32(hfUsuarioId.Value);

                // Obtener países seleccionados
                var paisesSeleccionados = new BindingList<UsuarioPaisAccesoDTO>();
                foreach (ListItem item in cblPaises.Items)
                {
                    if (item.Selected)
                    {
                        paisesSeleccionados.Add(new UsuarioPaisAccesoDTO
                        {
                            Pais = new PaisesDTO { PaisId = Convert.ToInt32(item.Value) },
                            Acceso = true
                        });
                    }
                }

                int resultado;

                if (usuarioId == 0) // Es un nuevo usuario
                {
                    UsuariosDTO nuevoUsuario = new UsuariosDTO
                    {
                        Nombre = txtNombre.Text,
                        Apellidos = txtApellidos.Text,
                        NombreDeUsuario = txtNombreUsuario.Text,
                        CorreoElectronico = txtCorreo.Text,
                        PasswordHash = txtPasswordModal.Text, // El BO se encargará de "hashear"
                        Activo = chkActivo.Checked,
                        Superusuario = chkSuperusuario.Checked,
                        UsuarioPais = paisesSeleccionados
                    };
                    resultado = usuariosBO.InsertarUsuario(nuevoUsuario);
                }
                else // Es una modificación
                {
                    resultado = usuariosBO.ModificarAccesoUsuario(usuarioId, txtNombreUsuario.Text, chkActivo.Checked,
                                                                 paisesSeleccionados.Select(p => p.Pais.PaisId.Value).ToList());
                }

                if (resultado > 0)
                {
                    MostrarMensaje("Usuario guardado correctamente.", "success");
                    CargarUsuarios();

                    ScriptManager.RegisterStartupScript(this, GetType(), "CerrarModalScript",
                        "$('#modalUsuario').modal('hide');", true);
                }
                else
                {
                    MostrarMensaje("Error al guardar el usuario.", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error: {ex.Message}", "danger");
            }
        }
        #endregion

        private void MostrarMensaje(string mensaje, string tipo)
        {
            divMensaje.Visible = true;
            divMensaje.InnerHtml = $"<div class='alert alert-{tipo} alert-dismissible fade show' role='alert'>" +
                                   $"{mensaje}" +
                                   "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>" +
                                   "</div>";
        }
    }
}