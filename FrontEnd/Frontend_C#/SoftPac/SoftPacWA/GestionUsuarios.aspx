<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="SoftPacWA.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        :root {
            --color-primary: #0B1F34; 
            --color-secondary: #60748A;
            --color-accent: #5E4923;
            --color-light-1: #CED6DE;
            --color-light-2: #DED8CE;
            --color-neutral: #A3957D;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f8f9fa;
            color: var(--color-primary);
        }

        h1 {
            font-weight: 600;
            color: var(--color-primary);
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
    </style>

    <asp:UpdatePanel ID="upMain" runat="server">
        <ContentTemplate>
            <div id="divMensaje" runat="server" visible="false"></div>

            <div class="d-flex justify-content-between align-items-center mb-4">
                <h1>Administración de Usuarios</h1>
                <asp:LinkButton ID="btnAbrirModalNuevo" runat="server" CssClass="btn btn-primary" OnClick="btnAbrirModalNuevo_Click">
                    <i class="fas fa-plus me-2"></i> Nuevo Usuario
                </asp:LinkButton>
            </div>

            <div class="card shadow-sm">
                <div class="card-body">
                    <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False"
                        CssClass="table table-hover" GridLines="None"
                        DataKeyNames="usuario_id" OnRowCommand="gvUsuarios_RowCommand">
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
                                    <asp:LinkButton ID="btnModificar" runat="server" CssClass="btn btn-sm btn-outline-primary me-2"
                                        CommandName="Modificar" CommandArgument='<%# Eval("usuario_id") %>'>
                                        <i class="fas fa-edit"></i> Modificar
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-outline-danger"
                                        CommandName="Eliminar" CommandArgument='<%# Eval("usuario_id") %>'
                                        OnClientClick="return confirm('¿Está seguro de que desea eliminar a este usuario?');">
                                        <i class="fas fa-trash"></i> Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalUsuario" tabindex="-1" aria-labelledby="modalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="upModal" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <h5 class="modal-title" id="modalLabel">
                                <asp:Literal ID="litModalTitulo" runat="server">Nuevo Usuario</asp:Literal>
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfUsuarioId" runat="server" Value="0" />
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Nombre</label>
                                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Apellidos</label>
                                    <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Nombre de Usuario</label>
                                    <asp:TextBox ID="txtNombreUsuario" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Correo Electrónico</label>
                                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Contraseña</label>
                                    <asp:TextBox ID="txtPasswordModal" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                    <small class="form-text text-muted">Dejar en blanco para no cambiarla al modificar.</small>
                                </div>
                            </div>
                            <hr />
                            <div class="row">
                                <div class="col-md-4 mb-3">
                                    <label class="form-label">Países con Acceso</label>
                                    <asp:CheckBoxList ID="cblPaises" runat="server" CssClass="form-check"></asp:CheckBoxList>
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

    <script type="text/javascript">
        function abrirModal() {
            var modal = new bootstrap.Modal(document.getElementById('modalUsuario'), {});
            modal.show();
        }
    </script>
</asp:Content>
