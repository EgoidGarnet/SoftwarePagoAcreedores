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
        private CuentasPropiasBO cuentasPropiasBO = new CuentasPropiasBO();
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();


        private usuariosDTO UsuarioLogueado { get { return (usuariosDTO)Session["UsuarioLogueado"]; } }
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

        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*_-";
            var sb = new System.Text.StringBuilder();
            var rnd = new System.Random();

            for (int i = 0; i < length; i++)
            {
                int idx = rnd.Next(validChars.Length);
                sb.Append(validChars[idx]);
            }

            return sb.ToString();
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
            else if (e.CommandName == "Activar")
            {
                usuariosDTO usuarioModificar = usuariosBO.ObtenerPorId(usuarioId);
                int resultado = usuariosBO.ModificarAccesoUsuario(usuarioId, usuarioModificar.nombre_de_usuario, true,
                    usuarioModificar.usuario_pais.Where(a => a.acceso == true)
                                                              .Select(a => a.pais.pais_id).ToList(), UsuarioLogueado, "");
                if (resultado == 0)
                {
                    MostrarMensaje("Error al activar el usuario.", "danger");
                }
                else
                {
                    MostrarMensaje("Usuario activado correctamente", "success");
                    CargarUsuarios();
                }
            }
            else if (e.CommandName == "MostrarModalDesactivar")
            {
                // Guardar ID del usuario
                ViewState["UsuarioDesactivar"] = usuarioId;

                // Abrir modal
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(),
                    "abrirModalDesactivar",
                    "var m = new bootstrap.Modal(document.getElementById('modalDesactivar')); m.show();",
                    true
                );

            }
            else if (e.CommandName == "VerActividad")
            {
                CargarActividadUsuario(usuarioId);
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(),
                    "abrirModalActividad",
                    "var m = new bootstrap.Modal(document.getElementById('modalActividad')); m.show();",
                    true
                );
            }
            //else if (e.CommandName == "Eliminar")
            //{
            //    divMensaje.Visible = false; // Limpiar mensaje previo antes de eliminar
            //    usuariosDTO usuarioActual = (usuariosDTO)Session["UsuarioLogueado"];
            //    int resultado = usuariosBO.EliminarUsuario(usuarioId, usuarioActual.usuario_id);
            //    if (resultado > 0)
            //    {
            //        MostrarMensaje("Usuario desactivado correctamente.", "success");
            //        CargarUsuarios();
            //    }
            //    else
            //    {
            //        MostrarMensaje("Error al desactivar el usuario.", "danger");
            //    }
            //}
        }
        protected void btnConfirmarDesactivar_Click(object sender, EventArgs e)
        {
            int usuarioId = (int)ViewState["UsuarioDesactivar"];
            divMensaje.Visible = false; // Limpiar mensaje previo antes de eliminar
            usuariosDTO usuarioModificar = usuariosBO.ObtenerPorId(usuarioId);
            int resultado = usuariosBO.ModificarAccesoUsuario(usuarioId, usuarioModificar.nombre_de_usuario, false,
                usuarioModificar.usuario_pais.Where(a => a.acceso == true)
                                                          .Select(a => a.pais.pais_id).ToList(), UsuarioLogueado, "");
            //usuariosDTO usuarioActual = (usuariosDTO)Session["UsuarioLogueado"];
            //int resultado = usuariosBO.EliminarUsuario(usuarioId, usuarioActual.usuario_id);
            if (resultado > 0)
            {
                MostrarMensaje("Usuario desactivado correctamente.", "success");
                CargarUsuarios();
            }
            else
            {
                MostrarMensaje("Error al desactivar el usuario.", "danger");
            }
        }
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(Request.Form["usuarioIdSeleccionado"] ?? "0");
            bool activar = bool.Parse(Request.Form["activarUsuario"] ?? "false");


            CargarUsuarios(); // recargar grid
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
            LimpiarMensajesError();

            // Campos no editables
            txtNombre.Text = usuario.nombre;
            txtNombre.Enabled = false;
            txtApellidos.Text = usuario.apellidos;
            txtApellidos.Enabled = false;
            txtCorreo.Text = usuario.correo_electronico;
            txtCorreo.Enabled = false;

            // Campo de contraseña - IMPORTANTE: vacío, no "********"
            txtPasswordModal.Text = string.Empty;
            txtPasswordModal.Enabled = false;
            txtPasswordModal.CssClass = "form-control"; // Reset estilo
            txtPasswordModal.Attributes["placeholder"] = "No se modificará";
            btnRegenerarPassword.Visible = true; // MOSTRAR botón regenerar
            hfPasswordRegenerada.Value = string.Empty; // Limpiar flag
            litPasswordHint.Text = "La contraseña no se modificará. Use 'Regenerar' para crear una nueva.";

            chkSuperusuario.Checked = usuario.superusuario;
            chkSuperusuario.Disabled = true;

            // Campos editables
            txtNombreUsuario.Text = usuario.nombre_de_usuario;
            txtNombreUsuario.Enabled = true;
            chkActivo.Checked = usuario.activo;
            chkActivo.Disabled = false;

            // Países
            cblPaises.ClearSelection();
            if (usuario.usuario_pais != null)
            {
                foreach (var acceso in usuario.usuario_pais)
                {
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

            // Campo de contraseña para NUEVO usuario
            txtPasswordModal.Text = string.Empty;
            txtPasswordModal.Enabled = false;
            txtPasswordModal.CssClass = "form-control"; // Reset estilo
            txtPasswordModal.Attributes["placeholder"] = "Se generará automáticamente";
            btnRegenerarPassword.Visible = false; // Ocultar en modo crear
            hfPasswordRegenerada.Value = string.Empty;
            litPasswordHint.Text = "La contraseña se generará automáticamente y se enviará por correo.";

            chkActivo.Checked = true;
            chkActivo.Disabled = true;
            chkSuperusuario.Checked = false;
            chkSuperusuario.Disabled = false;
            cblPaises.ClearSelection();

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
                else
                {
                    // *** VALIDACIÓN DE DUPLICADO: Nombre de Usuario ***
                    if (ExisteNombreUsuario(txtNombreUsuario.Text.Trim()))
                    {
                        lblErrorNombreUsuario.Text = "Este nombre de usuario ya existe. Elija otro.";
                        lblErrorNombreUsuario.Visible = true;
                        esValido = false;
                    }
                }

                // Validar Correo
                if (string.IsNullOrWhiteSpace(txtCorreo.Text))
                {
                    lblErrorCorreo.Text = "El correo electrónico es obligatorio.";
                    lblErrorCorreo.Visible = true;
                    esValido = false;
                }
                else if (!EsCorreoValido(txtCorreo.Text.Trim()))
                {
                    lblErrorCorreo.Text = "El formato del correo electrónico no es válido.";
                    lblErrorCorreo.Visible = true;
                    esValido = false;
                }
                else
                {
                    // *** VALIDACIÓN DE DUPLICADO: Correo Electrónico ***
                    if (ExisteCorreoElectronico(txtCorreo.Text.Trim()))
                    {
                        lblErrorCorreo.Text = "Este correo electrónico ya está registrado. Use otro.";
                        lblErrorCorreo.Visible = true;
                        esValido = false;
                    }
                }
            }
            else // Modo modificación
            {
                // Validar Nombre de Usuario en modificación (puede estar cambiando el username)
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
                else
                {
                    // Validar que no exista otro usuario con ese nombre (excepto el actual)
                    if (ExisteNombreUsuarioExcepto(txtNombreUsuario.Text.Trim(), usuarioId))
                    {
                        lblErrorNombreUsuario.Text = "Este nombre de usuario ya existe. Elija otro.";
                        lblErrorNombreUsuario.Visible = true;
                        esValido = false;
                    }
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


        private bool EsCorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            try
            {
                // Validación básica de formato de correo
                var addr = new System.Net.Mail.MailAddress(correo);
                return addr.Address == correo;
            }
            catch
            {
                return false;
            }
        }

        private bool ExisteNombreUsuario(string nombreUsuario)
        {
            try
            {
                List<usuariosDTO> listaUsuarios = usuariosBO.ListarTodos().ToList();

                // Buscar si existe algún usuario con ese nombre (sin importar mayúsculas/minúsculas)
                return listaUsuarios.Any(u =>
                    u.nombre_de_usuario != null &&
                    u.nombre_de_usuario.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase)
                );
            }
            catch (Exception ex)
            {
                // En caso de error, mejor prevenir la duplicación
                System.Diagnostics.Debug.WriteLine($"Error al verificar nombre de usuario: {ex.Message}");
                return false;
            }
        }

        private bool ExisteCorreoElectronico(string correo)
        {
            try
            {
                List<usuariosDTO> listaUsuarios = usuariosBO.ListarTodos().ToList();

                // Buscar si existe algún usuario con ese correo (sin importar mayúsculas/minúsculas)
                return listaUsuarios.Any(u =>
                    u.correo_electronico != null &&
                    u.correo_electronico.Equals(correo, StringComparison.OrdinalIgnoreCase)
                );
            }
            catch (Exception ex)
            {
                // En caso de error, mejor prevenir la duplicación
                System.Diagnostics.Debug.WriteLine($"Error al verificar correo electrónico: {ex.Message}");
                return false;
            }
        }

        private bool ExisteNombreUsuarioExcepto(string nombreUsuario, int usuarioIdActual)
        {
            try
            {
                List<usuariosDTO> listaUsuarios = usuariosBO.ListarTodos().ToList();

                // Buscar si existe otro usuario (que no sea el actual) con ese nombre
                return listaUsuarios.Any(u =>
                    u.usuario_id != usuarioIdActual &&
                    u.nombre_de_usuario != null &&
                    u.nombre_de_usuario.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase)
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar nombre de usuario: {ex.Message}");
                return false;
            }
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
                        accesoPais.accesoSpecified = true;
                        paisesSeleccionados.Add(accesoPais);
                    }

                    // GENERAR CONTRASEÑA AUTOMÁTICAMENTE
                    string contraseniaGenerada = GenerateRandomPassword(12);

                    usuariosDTO nuevoUsuario = new usuariosDTO
                    {
                        nombre = txtNombre.Text.Trim(),
                        apellidos = txtApellidos.Text.Trim(),
                        nombre_de_usuario = txtNombreUsuario.Text.Trim(),
                        correo_electronico = txtCorreo.Text.Trim(),
                        password_hash = contraseniaGenerada, // Contraseña generada automáticamente
                        activo = chkActivo.Checked,
                        superusuario = chkSuperusuario.Checked,
                        usuario_pais = paisesSeleccionados.ToArray()
                    };
                    resultado = usuariosBO.InsertarUsuario(nuevoUsuario, UsuarioLogueado);
                }
                else // Es una MODIFICACIÓN
                {
                    usuariosDTO usuarioModificar = usuariosBO.ObtenerPorId(usuarioId);

                    if (usuarioModificar != null)
                    {
                        usuarioModificar.nombre_de_usuario = txtNombreUsuario.Text.Trim();
                        usuarioModificar.activo = chkActivo.Checked;

                        // Verificar si se regeneró la contraseña
                        if (!string.IsNullOrEmpty(hfPasswordRegenerada.Value))
                        {
                            // SI se regeneró, enviar la nueva contraseña
                            usuarioModificar.password_hash = hfPasswordRegenerada.Value;
                        }
                        else
                        {
                            // SI NO se regeneró, enviar string VACÍO (no null)
                            usuarioModificar.password_hash = string.Empty; // ← CAMBIO AQUÍ
                        }

                        var nuevosAccesos = new BindingList<usuarioPaisAccesoDTO>();
                        foreach (ListItem item in cblPaises.Items)
                        {
                            usuarioPaisAccesoDTO accesoPais = new usuarioPaisAccesoDTO();
                            accesoPais.pais = new paisesDTO { pais_id = Convert.ToInt32(item.Value) };
                            accesoPais.acceso = item.Selected;
                            accesoPais.accesoSpecified = true;
                            nuevosAccesos.Add(accesoPais);
                        }

                        List<int> paisesAcceso = nuevosAccesos.Where(a => a.acceso == true)
                                                              .Select(a => a.pais.pais_id).ToList();

                        resultado = usuariosBO.ModificarAccesoUsuario(
                            usuarioModificar.usuario_id,
                            usuarioModificar.nombre_de_usuario,
                            usuarioModificar.activo,
                            paisesAcceso,
                            UsuarioLogueado,
                            usuarioModificar.password_hash // Aquí irá "" o la nueva contraseña
                        );
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

        #region Actividad de Usuario
        private void CargarActividadUsuario(int usuarioId)
        {
            try
            {
                ViewState["UsuarioActividadId"] = usuarioId;
                // Obtener datos del usuario
                usuariosDTO usuario = usuariosBO.ObtenerPorId(usuarioId);
                if (usuario != null)
                {
                    lblActividadUsuario.Text = usuario.nombre_de_usuario;
                    lblActividadNombre.Text = $"{usuario.nombre} {usuario.apellidos}";
                    lblActividadCorreo.Text = usuario.correo_electronico;
                }

                // Obtener kardex del usuario
                var kardexList = cuentasPropiasBO.ObtenerKardexCuentaPropiaPorUsuario(usuarioId);

                if (kardexList != null && kardexList.Count > 0)
                {
                    // Ordenar por fecha descendente (más recientes primero)
                    var listaOrdenada = kardexList.OrderByDescending(k => k.fecha_modificacion).ToList();

                    gvActividad.DataSource = listaOrdenada;
                    gvActividad.DataBind();
                    gvActividad.Visible = true;
                    pnlSinActividad.Visible = false;

                    lblTotalActividad.Text = $"Total de movimientos: {kardexList.Count}";
                }
                else
                {
                    gvActividad.DataSource = null;
                    gvActividad.DataBind();
                    gvActividad.Visible = false;
                    pnlSinActividad.Visible = true;

                    lblTotalActividad.Text = "Total de movimientos: 0";
                }
                var propuestasDelUsuario = propuestasBO.ListarActividadPorUsuario(usuarioId);

                var logDeAcciones = new List<AccionUsuario>();

                foreach (var propuesta in propuestasDelUsuario)
                {
                    // Acción de Creación
                    if (propuesta.usuario_creacion?.usuario_id == usuarioId && propuesta.fecha_hora_creacionSpecified)
                    {
                        logDeAcciones.Add(new AccionUsuario
                        {
                            PropuestaId = propuesta.propuesta_id,
                            FechaAccion = propuesta.fecha_hora_creacion,
                            TipoAccion = "Creación",
                            Estado = propuesta.estado,
                            NumFacturas = propuesta.detalles_propuesta == null ? 0 : propuesta.detalles_propuesta.Length
                        });
                    }

                    // Acción de Modificación
                    if (propuesta.usuario_modificacion?.usuario_id == usuarioId && propuesta.fecha_hora_modificacionSpecified)
                    {
                        logDeAcciones.Add(new AccionUsuario
                        {
                            PropuestaId = propuesta.propuesta_id,
                            FechaAccion = propuesta.fecha_hora_modificacion,
                            TipoAccion = "Modificación",
                            Estado = propuesta.estado,
                            NumFacturas = propuesta.detalles_propuesta == null ? 0 : propuesta.detalles_propuesta.Length
                        });
                    }

                    // Acción de Eliminación
                    if (propuesta.usuario_eliminacion?.usuario_id == usuarioId && propuesta.fecha_eliminacionSpecified)
                    {
                        logDeAcciones.Add(new AccionUsuario
                        {
                            PropuestaId = propuesta.propuesta_id,
                            FechaAccion = propuesta.fecha_eliminacion,
                            TipoAccion = "Eliminación",
                            Estado = "Eliminada",
                            NumFacturas = propuesta.detalles_propuesta == null ? 0 : propuesta.detalles_propuesta.Length
                        });
                    }
                }

                if (logDeAcciones.Count > 0)
                {
                    gvUltimasAcciones.Visible = true;
                    pnlSinAcciones.Visible = false;

                    gvUltimasAcciones.DataSource =
                        logDeAcciones.OrderByDescending(a => a.FechaAccion)
                                     .Take(10)
                                     .ToList();

                    gvUltimasAcciones.DataBind();
                }
                else
                {
                    gvUltimasAcciones.Visible = false;
                    pnlSinAcciones.Visible = true;

                    gvUltimasAcciones.DataSource = null;
                    gvUltimasAcciones.DataBind();
                }



                upActividad.Update();



            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar la actividad: {ex.Message}", "danger");
            }
        }

        protected void gvActividad_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var kardex = (SoftPacBusiness.CuentasPropiasWS.kardexCuentasPropiasDTO)e.Row.DataItem;

                if (kardex != null && kardex.cuenta_propia != null)
                {
                    // Formatear monto modificado
                    Label lblMontoModificado = (Label)e.Row.FindControl("lblMontoModificado");
                    if (lblMontoModificado != null)
                    {
                        decimal monto = kardex.saldo_modificacion;
                        string codigoISO = kardex.cuenta_propia.moneda?.codigo_iso ?? "";
                        string cssClass = monto < 0 ? "monto-negativo" : "monto-positivo";

                        lblMontoModificado.Text = $"{codigoISO} {monto:N2}";
                        lblMontoModificado.CssClass = cssClass;
                    }

                    // Formatear saldo resultante
                    Label lblSaldoResultante = (Label)e.Row.FindControl("lblSaldoResultante");
                    if (lblSaldoResultante != null)
                    {
                        decimal saldoResultante = kardex.saldo_resultante;
                        string codigoISO = kardex.cuenta_propia.moneda?.codigo_iso ?? "";

                        lblSaldoResultante.Text = $"{codigoISO} {saldoResultante:N2}";
                    }
                }
            }
        }

        protected void gvActividad_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvActividad.PageIndex = e.NewPageIndex;

            // Necesitamos recargar los datos del usuario guardado en ViewState
            if (ViewState["UsuarioActividadId"] != null)
            {
                int usuarioId = (int)ViewState["UsuarioActividadId"];
                CargarActividadUsuario(usuarioId);
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


        protected void btnRegenerarPassword_Click(object sender, EventArgs e)
        {
            // Generar nueva contraseña
            string nuevaPassword = GenerateRandomPassword(12);

            // Guardarla en el HiddenField (NO en el TextBox visible)
            hfPasswordRegenerada.Value = nuevaPassword;

            // Mostrar confirmación visual
            txtPasswordModal.Attributes["placeholder"] = "✓ Nueva contraseña generada";
            txtPasswordModal.CssClass = "form-control border-success";
            litPasswordHint.Text = "✓ Nueva contraseña generada. Se enviará por correo al guardar los cambios.";

            MostrarMensaje("Contraseña regenerada exitosamente. Guarde para confirmar.", "success");
        }



    }
}