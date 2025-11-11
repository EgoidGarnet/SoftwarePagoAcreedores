using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

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
                CargarUsuariosParaAutocomplete();
            }
        }

        private void CargarUsuariosParaAutocomplete()
        {
            try
            {
                List<usuariosDTO> listaUsuarios = usuariosBO.ListarTodos().ToList();

                var usuariosSimplificados = listaUsuarios.Select(u => new
                {
                    nombre_de_usuario = u.nombre_de_usuario ?? "",
                    nombre = u.nombre ?? "",
                    apellidos = u.apellidos ?? ""
                }).ToList();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(usuariosSimplificados);
                hfUsuariosJson.Value = json;
            }
            catch (Exception ex)
            {
                hfUsuariosJson.Value = "[]";
            }
        }

        public static bool IsAlphanumeric(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            foreach (char c in text)
            {
                if (!Char.IsLetterOrDigit(c) && !Char.IsWhiteSpace(c))
                {
                    return false;
                }
            }
            return true;
        }


        #region Carga de Datos
        private void CargarUsuarios()
        {
            try
            {
                List<usuariosDTO> listaUsuarios = usuariosBO.ListarTodos().ToList();

                // Aplicar filtro por nombre de usuario
                string textoBusqueda = (txtBuscar.Text ?? string.Empty).Trim().ToLower();
                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    listaUsuarios = listaUsuarios.Where(u =>
                        (u.nombre_de_usuario ?? string.Empty).ToLower().Contains(textoBusqueda) ||
                        (u.nombre ?? string.Empty).ToLower().Contains(textoBusqueda) ||
                        (u.apellidos ?? string.Empty).ToLower().Contains(textoBusqueda)
                    ).ToList();
                }

                // Aplicar filtro por estado
                if (!string.IsNullOrEmpty(ddlFiltroEstado.SelectedValue))
                {
                    bool filtroActivo = ddlFiltroEstado.SelectedValue == "activo";
                    listaUsuarios = listaUsuarios.Where(u => u.activo == filtroActivo).ToList();
                }

                gvUsuarios.DataSource = listaUsuarios;
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar usuarios: {ex.Message}", "danger");
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            divMensaje.Visible = false; // Ocultar mensaje al filtrar
            CargarUsuarios();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            divMensaje.Visible = false; // Ocultar mensaje al limpiar filtros
            txtBuscar.Text = string.Empty;
            ddlFiltroEstado.SelectedIndex = 0;
            CargarUsuarios();
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
                divMensaje.Visible = false; // Ocultar mensaje al modificar
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
                divMensaje.Visible = false; // Limpiar mensaje previo antes de eliminar
                usuariosDTO usuarioActual = (usuariosDTO)Session["UsuarioLogueado"];
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
            divMensaje.Visible = false; // Ocultar mensaje al abrir modal
            LimpiarModal();
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModal();", true);
        }

        private void CargarDatosEnModal(usuariosDTO usuario)
        {
            litModalTitulo.Text = "Modificar Usuario";

            // Limpiar mensajes de error PRIMERO
            LimpiarMensajesError();

            // Campos no editables (en gris)
            txtNombre.Text = usuario.nombre;
            txtNombre.Enabled = false;
            txtApellidos.Text = usuario.apellidos;
            txtApellidos.Enabled = false;
            txtCorreo.Text = usuario.correo_electronico;
            txtCorreo.Enabled = false;
            txtPasswordModal.Text = "********"; // Mostrar asteriscos en lugar de la contraseña real
            txtPasswordModal.Enabled = false; // Deshabilitar el campo
            chkSuperusuario.Checked = usuario.superusuario;
            chkSuperusuario.Disabled = true;

            // Campos editables
            txtNombreUsuario.Text = usuario.nombre_de_usuario;
            txtNombreUsuario.Enabled = true;
            chkActivo.Checked = usuario.activo;
            chkActivo.Disabled = false;

            // Limpiar selección de países y marcar solo los que tienen acceso = true
            cblPaises.ClearSelection();
            if (usuario.usuario_pais != null)
            {
                foreach (var acceso in usuario.usuario_pais)
                {
                    // Solo marcar si acceso es true
                    if (acceso.acceso)
                    {
                        ListItem item = cblPaises.Items.FindByValue(acceso.pais.pais_id.ToString());
                        if (item != null)
                        {
                            item.Selected = true;
                        }
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
            chkActivo.Disabled = true;
            chkSuperusuario.Checked = false;
            chkSuperusuario.Disabled = false;
            cblPaises.ClearSelection();

            // Limpiar mensajes de error
            LimpiarMensajesError();
        }

        private void LimpiarMensajesError()
        {
            lblErrorNombre.Visible = false;
            lblErrorApellidos.Visible = false;
            lblErrorNombreUsuario.Visible = false;
            lblErrorCorreo.Visible = false;
            lblErrorPassword.Visible = false;
            lblErrorPaises.Visible = false;
        }

        private bool ValidarCamposUsuario(int usuarioId)
        {
            bool esValido = true;
            LimpiarMensajesError();

            // Solo validar campos en creación de nuevo usuario
            if (usuarioId == 0)
            {
                // Validar Nombre
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    lblErrorNombre.Text = "El nombre es obligatorio.";
                    lblErrorNombre.Visible = true;
                    esValido = false;
                }
                else if (!IsAlphanumeric(txtNombre.Text.Trim()))
                {
                    lblErrorNombre.Text = "El nombre debe ser alfanumérico.";
                    lblErrorNombre.Visible = true;
                    esValido = false;
                }

                // Validar Apellidos
                if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    lblErrorApellidos.Text = "Los apellidos son obligatorios.";
                    lblErrorApellidos.Visible = true;
                    esValido = false;
                }
                else if (!IsAlphanumeric(txtApellidos.Text.Trim()))
                {
                    lblErrorApellidos.Text = "Los apellidos deben ser alfanuméricos.";
                    lblErrorApellidos.Visible = true;
                    esValido = false;
                }

                // Validar Nombre de Usuario
                if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
                {
                    lblErrorNombreUsuario.Text = "El nombre de usuario es obligatorio.";
                    lblErrorNombreUsuario.Visible = true;
                    esValido = false;
                }
                else if (!IsAlphanumeric(txtNombreUsuario.Text.Trim()))
                {
                    lblErrorNombreUsuario.Text = "El nombre de usuario debe ser alfanumérico.";
                    lblErrorNombreUsuario.Visible = true;
                    esValido = false;
                }

                // Validar Correo
                if (string.IsNullOrWhiteSpace(txtCorreo.Text))
                {
                    lblErrorCorreo.Text = "El correo electrónico es obligatorio.";
                    lblErrorCorreo.Visible = true;
                    esValido = false;
                }

                // Validar Contraseña
                if (string.IsNullOrWhiteSpace(txtPasswordModal.Text))
                {
                    lblErrorPassword.Text = "La contraseña es obligatoria.";
                    lblErrorPassword.Visible = true;
                    esValido = false;
                }
                else if (!IsAlphanumeric(txtPasswordModal.Text.Trim()))
                {
                    lblErrorPassword.Text = "La contraseña debe ser alfanumérica.";
                    lblErrorPassword.Visible = true;
                    esValido = false;
                }
            }

            // Validar que al menos un país esté seleccionado (PARA CREAR Y MODIFICAR)
            bool paisSeleccionado = false;
            foreach (ListItem item in cblPaises.Items)
            {
                if (item.Selected)
                {
                    paisSeleccionado = true;
                    break;
                }
            }

            if (!paisSeleccionado)
            {
                lblErrorPaises.Text = "Debe seleccionar al menos un país.";
                lblErrorPaises.Visible = true;
                esValido = false;
            }


            return esValido;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int usuarioId = Convert.ToInt32(hfUsuarioId.Value);

                // Validar campos antes de continuar
                if (!ValidarCamposUsuario(usuarioId))
                {
                    return;
                }

                int resultado;

                if (usuarioId == 0) // Es un nuevo usuario
                {
                    // Obtener TODOS los países con su estado de acceso
                    var paisesSeleccionados = new BindingList<usuarioPaisAccesoDTO>();
                    foreach (ListItem item in cblPaises.Items)
                    {
                        usuarioPaisAccesoDTO accesoPais = new usuarioPaisAccesoDTO();
                        accesoPais.pais = new paisesDTO { pais_id = Convert.ToInt32(item.Value) };
                        accesoPais.acceso = item.Selected;
                        accesoPais.accesoSpecified = true; // Crítico para la serialización

                        paisesSeleccionados.Add(accesoPais);
                    }

                    usuariosDTO nuevoUsuario = new usuariosDTO
                    {
                        nombre = txtNombre.Text.Trim(),
                        apellidos = txtApellidos.Text.Trim(),
                        nombre_de_usuario = txtNombreUsuario.Text.Trim(),
                        correo_electronico = txtCorreo.Text.Trim(),
                        password_hash = txtPasswordModal.Text.Trim(),
                        activo = chkActivo.Checked,
                        superusuario = chkSuperusuario.Checked,
                        usuario_pais = paisesSeleccionados.ToArray()
                    };
                    resultado = usuariosBO.InsertarUsuario(nuevoUsuario);
                }
                else // Es una modificación
                {
                    // Para modificación, obtenemos el usuario completo y actualizamos sus accesos
                    usuariosDTO usuarioModificar = usuariosBO.ObtenerPorId(usuarioId);

                    if (usuarioModificar != null)
                    {
                        // Actualizar campos básicos
                        usuarioModificar.nombre_de_usuario = txtNombreUsuario.Text.Trim();
                        usuarioModificar.activo = chkActivo.Checked;

                        // Actualizar accesos de países
                        // Primero limpiamos el array actual
                        var nuevosAccesos = new BindingList<usuarioPaisAccesoDTO>();

                        foreach (ListItem item in cblPaises.Items)
                        {
                            usuarioPaisAccesoDTO accesoPais = new usuarioPaisAccesoDTO();
                            accesoPais.pais = new paisesDTO { pais_id = Convert.ToInt32(item.Value) };
                            accesoPais.acceso = item.Selected;
                            accesoPais.accesoSpecified = true;

                            nuevosAccesos.Add(accesoPais);
                        }

                        usuarioModificar.usuario_pais = nuevosAccesos.ToArray();

                        // Llamar al método modificar normal
                        resultado = usuariosBO.ModificarUsuario(usuarioModificar);
                    }
                    else
                    {
                        resultado = 0;
                    }
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