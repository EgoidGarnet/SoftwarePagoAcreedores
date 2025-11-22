<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="SoftPacWA.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .page-title h3 {
            color: var(--color-primary);
            font-size: 1.8rem;
            font-weight: 600;
            margin-bottom: 1.5rem;
        }

        .filter-section {
            background: #fff;
            padding: .9rem 1rem;
            border-radius: 8px;
            margin-bottom: 1rem;
            box-shadow: 0 2px 8px rgba(0,0,0,.08);
        }

        .filter-section .form-label {
            height: 36px;
            display: flex;
            align-items: center;
            margin-bottom: 0;
        }

        .card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 0;
            overflow: hidden;
            margin-bottom: 1rem;
        }

        .card-body {
            padding: 1.5rem;
        }

        .table thead {
            background-color: var(--color-primary);
            color: white;
            font-weight: bold;
            text-transform: uppercase;
        }

        .table tbody tr {
            background-color: #fff !important;
        }

        .table-hover tbody tr:hover {
            background-color: var(--color-light-1) !important;
        }

        .btn-primary {
            background-color: var(--color-primary);
            border-color: var(--color-primary);
        }

        .btn-primary:hover {
            background-color: #092037;
            border-color: #092037;
        }

        .error-message {
            color: #dc3545;
            font-size: 0.875rem;
            margin-top: 0.25rem;
            display: block;
        }

        /* Estilos para el autocomplete */
        .ui-autocomplete {
            max-height: 200px;
            overflow-y: auto;
            overflow-x: hidden;
            z-index: 1000;
        }

        .ui-menu-item {
            padding: 8px 12px;
            cursor: pointer;
        }

        .ui-menu-item:hover {
            background-color: var(--color-light-1);
        }

        .monto-positivo {
            color: #28a745;
            font-weight: 600;
        }

        .monto-negativo {
            color: #dc3545;
            font-weight: 600;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <asp:UpdatePanel ID="upMain" runat="server">
            <ContentTemplate>
                <div id="divMensaje" runat="server" visible="false"></div>

                <div class="page-title">
                    <h3>
                        <i class="fas fa-users-cog me-2"></i>
                        Administración de Usuarios
                    </h3>
                </div>

                <!-- Sección de filtros -->
                <div class="filter-section">
                    <div class="row gx-3 gy-3">
                        <div class="col-12 col-md-5">
                            <label class="form-label">Buscar por nombre de usuario</label>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Ingrese nombre de usuario" autocomplete="off"></asp:TextBox>
                            <asp:HiddenField ID="hfUsuariosJson" runat="server" />
                        </div>
                        <div class="col-12 col-md-3">
                            <label class="form-label">Estado</label>
                            <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-select">
                                <asp:ListItem Value="" Selected="True">Todos</asp:ListItem>
                                <asp:ListItem Value="activo">Solo Activos</asp:ListItem>
                                <asp:ListItem Value="inactivo">Solo Inactivos</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-12 col-md-4 d-flex align-items-end gap-2 flex-wrap">
                            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary" OnClick="btnFiltrar_Click" />
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" />
                        </div>
                    </div>
                </div>

                <!-- Botón Nuevo Usuario -->
                <div class="mb-3">
                    <asp:LinkButton ID="btnAbrirModalNuevo" runat="server" CssClass="btn btn-primary" OnClick="btnAbrirModalNuevo_Click">
                        <i class="fas fa-plus me-2"></i> Nuevo Usuario
                    </asp:LinkButton>
                </div>

                <div class="card">
                    <div class="card-body">
                        <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                            <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False"
                                CssClass="table table-hover" GridLines="None"
                                DataKeyNames="usuario_id" OnRowCommand="gvUsuarios_RowCommand"
                                OnRowDataBound="gvUsuarios_RowDataBound"
                                EmptyDataText="No se encontraron usuarios">
                                <Columns>
                                    <asp:BoundField DataField="usuario_id" HeaderText="ID" />
                                    <asp:BoundField DataField="nombre" HeaderText="Nombre Completo" />
                                    <asp:BoundField DataField="nombre_de_usuario" HeaderText="Username" />
                                    <asp:BoundField DataField="correo_electronico" HeaderText="Correo" />
                                    <asp:TemplateField HeaderText="Activo">
                                        <ItemTemplate>
                                            <span class='badge <%# (bool)Eval("Activo") ? "bg-success" : "bg-danger" %>'>
                                                <%# (bool)Eval("Activo") ? "Sí" : "No" %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <div class="d-flex gap-2 flex-wrap">
                                                <asp:LinkButton ID="btnVerActividad" runat="server"
                                                    CssClass="btn btn-sm btn-outline-info"
                                                    CommandName="VerActividad"
                                                    CommandArgument='<%# Eval("usuario_id") %>'>
                                                    <i class="fas fa-history"></i> Actividad
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnModificar" runat="server" CssClass="btn btn-sm btn-outline-primary"
                                                    CommandName="Modificar" CommandArgument='<%# Eval("usuario_id") %>'>
                                                    <i class="fas fa-edit"></i> Modificar
                                                </asp:LinkButton>

                                                <asp:LinkButton ID="btnDesactivar" runat="server"
                                                    CssClass="btn btn-sm btn-outline-danger"
                                                    CommandName="MostrarModalDesactivar"
                                                    CommandArgument='<%# Eval("usuario_id") %>'
                                                    Visible='<%# (bool)Eval("activo") == true %>'>
                                                    <i class="fas fa-ban"></i> Desactivar
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnActivar" runat="server"
                                                    CssClass="btn btn-sm btn-success"
                                                    CommandName="Activar"
                                                    CommandArgument='<%# Eval("usuario_id") %>'
                                                    Visible='<%# (bool)Eval("activo") == false %>'>
                                                    <i class="fas fa-rotate-left"></i> Activar
                                                </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="text-center py-4" role="alert">
                                        <i class="fas fa-search fa-2x mb-3"></i>
                                        <h5>No se encontraron usuarios</h5>
                                        <p class="mb-0">No hay usuarios que coincidan con los filtros aplicados.</p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <div class="modal fade" id="modalUsuario" tabindex="-1" aria-labelledby="modalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <asp:UpdatePanel ID="upModal" runat="server">
                        <ContentTemplate>
                            <div class="modal-header" style="background-color: var(--color-primary); color: white;">
                                <h5 class="modal-title" id="modalLabel">
                                    <asp:Literal ID="litModalTitulo" runat="server">Nuevo Usuario</asp:Literal>
                                </h5>
                                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfUsuarioId" runat="server" Value="0" />
                                <div class="row">
                                    <div class="col-md-6 mb-3">
                                        <label class="form-label">Nombre</label>
                                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:Label ID="lblErrorNombre" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-md-6 mb-3">
                                        <label class="form-label">Apellidos</label>
                                        <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:Label ID="lblErrorApellidos" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-3">
                                        <label class="form-label">Nombre de Usuario</label>
                                        <asp:TextBox ID="txtNombreUsuario" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:Label ID="lblErrorNombreUsuario" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-md-6 mb-3">
                                        <label class="form-label">Correo Electrónico</label>
                                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                                        <asp:Label ID="lblErrorCorreo" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-3">
                                        <label class="form-label">Contraseña</label>
                                        <div class="input-group">
                                            <asp:TextBox ID="txtPasswordModal" runat="server" CssClass="form-control" TextMode="Password" Enabled="false" placeholder="Se generará automáticamente"></asp:TextBox>
                                            <asp:Button ID="btnRegenerarPassword" runat="server" Text="🔄 Regenerar" CssClass="btn btn-warning" OnClick="btnRegenerarPassword_Click" Visible="false" />
                                        </div>
                                        <small class="form-text text-muted">
                                            <asp:Literal ID="litPasswordHint" runat="server" Text="La contraseña se generará automáticamente y se enviará por correo."></asp:Literal>
                                        </small>
                                        <asp:HiddenField ID="hfPasswordRegenerada" runat="server" Value="" />
                                        <asp:Label ID="lblErrorPassword" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <hr />
                                <div class="row">
                                    <div class="col-md-4 mb-3">
                                        <label class="form-label">Países con Acceso</label>
                                        <asp:CheckBoxList ID="cblPaises" runat="server" CssClass="form-check"></asp:CheckBoxList>
                                        <asp:Label ID="lblErrorPaises" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-md-4 mb-3">
                                        <div class="form-check form-switch mt-4">
                                            <input id="chkActivo" runat="server" type="checkbox" class="form-check-input" />
                                            <label for="chkActivo" class="form-check-label">Usuario Activo</label>
                                        </div>
                                    </div>

                                    <div class="col-md-4 mb-3">
                                        <div class="form-check form-switch mt-4">
                                            <input id="chkSuperusuario" runat="server" type="checkbox" class="form-check-input" />
                                            <label for="chkSuperusuario" class="form-check-label">Es Superusuario</label>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <div class="modal fade" id="modalDesactivar" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="modal-header bg-danger text-white">
                        <h5 class="modal-title">Desactivar Usuario</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>

                    <div class="modal-body">
                        ¿Está seguro de que desea desactivar a este usuario?
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>

                        <asp:Button ID="btnConfirmarDesactivar" runat="server"
                            CssClass="btn btn-danger"
                            Text="Desactivar"
                            OnClick="btnConfirmarDesactivar_Click" />
                    </div>

                </div>
            </div>
        </div>

        <div class="modal fade" id="modalActividad" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-xl">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: var(--color-primary); color: white;">
                        <h5 class="modal-title">
                            <i class="fas fa-history me-2"></i>Actividad del Usuario
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <asp:UpdatePanel ID="upActividad" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <!-- Información del usuario -->
                                <div class="card mb-3">
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-4">
                                                <strong>Usuario:</strong>
                                                <asp:Label ID="lblActividadUsuario" runat="server" />
                                            </div>
                                            <div class="col-md-4">
                                                <strong>Nombre:</strong>
                                                <asp:Label ID="lblActividadNombre" runat="server" />
                                            </div>
                                            <div class="col-md-4">
                                                <strong>Correo:</strong>
                                                <asp:Label ID="lblActividadCorreo" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Tabla de actividad -->
                                <h6 class="mb-3"><i class="fas fa-exchange-alt me-2"></i>Movimientos en Cuentas Propias</h6>

                                <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                                    <asp:GridView ID="gvActividad" runat="server"
                                        CssClass="table table-sm table-striped"
                                        AutoGenerateColumns="False"
                                        GridLines="None"
                                        EmptyDataText="No hay movimientos registrados para este usuario"
                                        AllowPaging="True"
                                        PageSize="15"
                                        OnPageIndexChanging="gvActividad_PageIndexChanging"
                                        OnRowDataBound="gvActividad_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Fecha">
                                                <ItemTemplate><%# Eval("fecha_modificacion", "{0:dd/MM/yyyy HH:mm}") %></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Entidad Bancaria">
                                                <ItemTemplate><%# Eval("cuenta_propia.entidad_bancaria.nombre") %></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="N° Cuenta">
                                                <ItemTemplate><%# Eval("cuenta_propia.numero_cuenta") %></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Monto Modificado">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblMontoModificado" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Saldo Resultante">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSaldoResultante" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ger" HorizontalAlign="Center" />
                                        <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última"
                                            PageButtonCount="5" Position="Bottom" />
                                    </asp:GridView>
                                </div>

                                <asp:Panel ID="pnlSinActividad" runat="server" Visible="false" CssClass="text-center py-4">
                                    <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                                    <p class="text-muted h6">No hay movimientos registrados</p>
                                    <p class="text-muted">Este usuario aún no ha realizado cambios en cuentas propias.</p>
                                </asp:Panel>

                                <asp:Label ID="lblTotalActividad" runat="server" CssClass="text-muted mt-2 d-block"></asp:Label>

                                <!-- Últimas acciones -->
                                <div class="mt-4">
                                    <h6 class="mb-3"><i class="fas fa-history me-2"></i>Últimas acciones</h6>

                                    <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                                        <asp:GridView ID="gvUltimasAcciones" runat="server"
                                            CssClass="table table-sm table-striped"
                                            AutoGenerateColumns="False"
                                            GridLines="None">
                                            <Columns>
                                                <asp:BoundField DataField="PropuestaId" HeaderText="Propuesta ID" />
                                                <asp:BoundField DataField="FechaAccion" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                                <asp:BoundField DataField="TipoAccion" HeaderText="Tipo" />
                                                <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                                <asp:BoundField DataField="NumFacturas" HeaderText="# Facturas" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <asp:Panel ID="pnlSinAcciones" runat="server" Visible="false" CssClass="text-center py-4">
                                    <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                                    <p class="text-muted h6">No ha realizado acciones recientemente</p>
                                    <p class="text-muted">Este usuario aún no ha realizado acciones en las propuestas de pago.</p>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- jQuery UI para autocomplete -->
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
    <script type="text/javascript">
        function abrirModal() {
            var modal = new bootstrap.Modal(document.getElementById('modalUsuario'), {});
            modal.show();
        }

        $(document).ready(function () {
            var usuariosJson = $('#<%= hfUsuariosJson.ClientID %>').val();
            var usuarios = usuariosJson ? JSON.parse(usuariosJson) : [];
            $('#<%= txtBuscar.ClientID %>').autocomplete({
                source: function (request, response) {
                    var term = request.term.toLowerCase();
                    var matches = $.grep(usuarios, function (usuario) {
                        return usuario.nombre_de_usuario.toLowerCase().startsWith(term) ||
                            usuario.nombre.toLowerCase().startsWith(term) ||
                            usuario.apellidos.toLowerCase().startsWith(term);
                    });

                    response(matches.slice(0, 10).map(function (usuario) {
                        return {
                            label: usuario.nombre_de_usuario + ' (' + usuario.nombre + ' ' + usuario.apellidos + ')',
                            value: usuario.nombre_de_usuario
                        };
                    }));
                },
                minLength: 1,
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    return false;
                }
            });
        });
    </script>
</asp:Content>