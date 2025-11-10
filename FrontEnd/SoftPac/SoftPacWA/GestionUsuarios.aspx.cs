using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
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
            paisesDTO pais = new paisesDTO();
            cblPaises.DataTextField = "nombre";
            cblPaises.DataValueField = "pais_id";
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
                usuariosDTO usuario = usuariosBO.ObtenerPorId(usuarioId);
                if (usuario != null)
                {
                    CargarDatosEnModal(usuario);
                    ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
                }
            }
            else if (e.CommandName == "Eliminar")
            {
                usuariosDTO usuarioActual = (usuariosDTO)Session["UsuarioLogueado"];
                MostrarMensaje(usuarioActual.usuario_id.ToString(),"success");
                int resultado = usuariosBO.EliminarUsuario(usuarioId, usuarioActual.usuario_id);
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

        private void CargarDatosEnModal(usuariosDTO usuario)
        {
            litModalTitulo.Text = "Modificar Usuario";

            // Campos no editables (en gris)
            txtNombre.Text = usuario.nombre;
            txtNombre.Enabled = false;
            txtApellidos.Text = usuario.apellidos;
            txtApellidos.Enabled = false;
            txtCorreo.Text = usuario.correo_electronico;
            txtCorreo.Enabled = false;
            chkSuperusuario.Checked = usuario.superusuario;
            chkSuperusuario.Disabled = true;

            // Campos editables
            txtNombreUsuario.Text = usuario.nombre_de_usuario;
            txtNombreUsuario.Enabled = true;
            chkActivo.Checked = usuario.activo;
            chkActivo.Disabled = false;

            // Limpiar selección de países y marcar los que tiene acceso
            cblPaises.ClearSelection();
            if (usuario.usuario_pais != null)
            {
                foreach (var acceso in usuario.usuario_pais)
                {
                    ListItem item = cblPaises.Items.FindByValue(acceso.pais.pais_id.ToString());
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

                // ✅ Obtener TODOS los países con su estado de acceso
                var paisesSeleccionados = new BindingList<usuarioPaisAccesoDTO>();
                bool tieneAlMenosUnPais = false;

                foreach (ListItem item in cblPaises.Items)
                {
                    if (item.Selected)
                    {
                        paisesSeleccionados.Add(new usuarioPaisAccesoDTO
                        {
                            pais = new paisesDTO { pais_id = Convert.ToInt32(item.Value) },
                            acceso = true
                        });
                        tieneAlMenosUnPais = true;
                    }
                    else
                    {
                        // ✅ AHORA SÍ agregamos los países NO seleccionados con acceso = false
                        paisesSeleccionados.Add(new usuarioPaisAccesoDTO
                        {
                            pais = new paisesDTO { pais_id = Convert.ToInt32(item.Value) },
                            acceso = false
                        });
                    }
                }

                // ✅ VALIDACIÓN: Debe tener al menos un país seleccionado
                if (!tieneAlMenosUnPais)
                {
                    MostrarMensaje("Debe seleccionar al menos un país permitido.", "warning");
                    return;
                }

                int resultado;

                if (usuarioId == 0) // Es un nuevo usuario
                {
                    usuariosDTO nuevoUsuario = new usuariosDTO
                    {
                        nombre = txtNombre.Text,
                        apellidos = txtApellidos.Text,
                        nombre_de_usuario = txtNombreUsuario.Text,
                        correo_electronico = txtCorreo.Text,
                        password_hash = txtPasswordModal.Text,
                        activo = chkActivo.Checked,
                        superusuario = chkSuperusuario.Checked,
                        usuario_pais = paisesSeleccionados.ToArray() // ✅ Ahora incluye TODOS los países
                    };
                    resultado = usuariosBO.InsertarUsuario(nuevoUsuario);
                }
                else // Es una modificación
                {
                    string nuevaPassword = string.IsNullOrWhiteSpace(txtPasswordModal.Text)
                                          ? null
                                          : txtPasswordModal.Text;

                    // ✅ Enviar TODOS los países (no solo los seleccionados)
                    resultado = usuariosBO.ModificarAccesoUsuario(
                        usuarioId,
                        txtNombreUsuario.Text,
                        chkActivo.Checked,
                        paisesSeleccionados.ToArray(), // ✅ Cambiado: ahora es el array completo
                        nuevaPassword
                    );
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