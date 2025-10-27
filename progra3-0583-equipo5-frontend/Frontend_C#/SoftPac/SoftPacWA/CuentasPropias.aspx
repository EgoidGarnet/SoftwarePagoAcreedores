<%@ Page Title="Cuentas Propias" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="CuentasPropias.aspx.cs" Inherits="SoftPacWA.CuentasPropias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .filter-section {
            background-color: white;
            padding: 1.5rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="upMain" runat="server">
        <ContentTemplate>
            <div id="divMensaje" runat="server" visible="false"></div>

            <div class="d-flex justify-content-between align-items-center mb-4">
                <h1>Cuentas Propias</h1>
                <asp:LinkButton ID="btnAbrirModalNuevo" runat="server" CssClass="btn btn-primary" OnClick="btnAbrirModalNuevo_Click">
    <i class="fas fa-plus me-2"></i> Nueva Cuenta Propia
                </asp:LinkButton>
            </div>

            <!-- Sección de Filtros -->
            <div class="filter-section">
                <div class="row g-3 align-items-end">
                    <div class="col-md-4">
                        <label class="form-label">Entidad Bancaria</label>
                        <asp:DropDownList ID="ddlFiltroEntidad" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Saldo Disponible (mayor o igual que)</label>
                        <asp:TextBox ID="txtFiltroSaldo" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div class="col-md-4 d-flex">
                        <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-secondary me-2" OnClick="btnFiltrar_Click" />
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary" OnClick="btnLimpiar_Click" />
                    </div>
                </div>
            </div>

            <!-- Grilla de Cuentas -->
            <div class="card shadow-sm">
                <div class="card-body">
                    <asp:GridView ID="gvCuentasPropias" runat="server" AutoGenerateColumns="False"
                        CssClass="table table-hover" GridLines="None" DataKeyNames="CuentaBancariaId"
                        AllowPaging="True" OnPageIndexChanging="gvCuentasPropias_PageIndexChanging" PageSize="10"
                        OnRowCommand="gvCuentasPropias_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="EntidadBancaria.Nombre" HeaderText="Entidad Bancaria" />
                            <asp:BoundField DataField="TipoCuenta" HeaderText="Tipo de Cuenta" />
                            <asp:BoundField DataField="NumeroCuenta" HeaderText="Número de Cuenta" />
                            <asp:BoundField DataField="Cci" HeaderText="CCI" />
                            <asp:BoundField DataField="Moneda.Simbolo" HeaderText="Moneda" />
                            <asp:BoundField DataField="SaldoDisponible" HeaderText="Saldo Disponible" DataFormatString="{0:N2}" />
                            <asp:TemplateField HeaderText="Activa">
                                <ItemTemplate>
                                    <span class='badge <%# (bool)Eval("Activa") ? "bg-success" : "bg-danger" %>'>
                                        <%# (bool)Eval("Activa") ? "Sí" : "No" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnModificar" runat="server" CssClass="btn btn-sm btn-outline-primary me-2"
                                        CommandName="Modificar" CommandArgument='<%# Eval("CuentaBancariaId") %>' ToolTip="Modificar">
            <i class="fas fa-edit"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-outline-danger"
                                        CommandName="Eliminar" CommandArgument='<%# Eval("CuentaBancariaId") %>'
                                        ToolTip="Eliminar" OnClientClick="return confirm('¿Está seguro de eliminar esta cuenta?');">
            <i class="fas fa-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination justify-content-center" />
                    </asp:GridView>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Modal para Crear/Editar Cuenta -->
    <div class="modal fade" id="modalCuenta" tabindex="-1" aria-labelledby="modalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <asp:UpdatePanel ID="upModal" runat="server">
                    <ContentTemplate>
                        <div class="modal-header text-white" style="background-color: var(--color-primary);">
                            <h5 class="modal-title">
                                <asp:Literal ID="litModalTitulo" runat="server"></asp:Literal></h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <asp:HiddenField ID="hfCuentaId" runat="server" Value="0" />
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Entidad Bancaria</label>
                                    <asp:DropDownList ID="ddlEntidadBancaria" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Moneda</label>
                                    <asp:DropDownList ID="ddlMoneda" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Tipo de Cuenta (Ej. Ahorros, Corriente)</label>
                                    <asp:TextBox ID="txtTipoCuenta" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Número de Cuenta</label>
                                    <asp:TextBox ID="txtNumeroCuenta" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">CCI (Código de Cuenta Interbancario)</label>
                                    <asp:TextBox ID="txtCci" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label class="form-label">Saldo Disponible</label>
                                    <asp:TextBox ID="txtSaldoDisponible" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <div class="form-check form-switch mt-4">
                                        <div class="col-md-4 mb-3">
                                            <div class="form-check form-switch mt-4">
                                                <input id="chkActiva" runat="server" type="checkbox" class="form-check-input" checked="checked" />
                                                <label for="chkActiva" class="form-check-label">Cuenta Activa</label>
                                            </div>
                                        </div>
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
            var modal = new bootstrap.Modal(document.getElementById('modalCuenta'), {});
            modal.show();
        }
    </script>
</asp:Content>
