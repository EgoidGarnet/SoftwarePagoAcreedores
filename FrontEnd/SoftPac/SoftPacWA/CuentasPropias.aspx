<%@ Page Title="Cuentas Propias" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="CuentasPropias.aspx.cs" Inherits="SoftPacWA.CuentasPropias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 3px solid var(--color-light-1);
        }

        .page-title {
            display: flex;
            align-items: center;
            gap: 1rem;
        }

            .page-title h2 {
                color: var(--color-primary);
                font-size: 1.8rem;
                font-weight: 600;
                margin: 0;
            }

        .filter-section {
            background-color: white;
            padding: 1.5rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .lista-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 0;
            overflow: hidden;
        }

        .lista-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 1.5rem;
            border-bottom: 2px solid var(--color-light-1);
            background-color: #f8f9fa;
        }

        .lista-title {
            color: var(--color-primary);
            font-size: 1.2rem;
            font-weight: 600;
            margin: 0;
        }

        .lista-body {
            padding: 1.5rem;
        }

        .lista-footer {
            padding: 1rem 1.5rem;
            border-top: 1px solid var(--color-light-1);
            background-color: #f8f9fa;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .total-registros {
            color: var(--color-secondary);
            font-size: 0.9rem;
            margin: 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <asp:UpdatePanel ID="upMain" runat="server">
            <ContentTemplate>
                <div id="divMensaje" runat="server" visible="false"></div>

                <!-- Page Header -->
                <div class="page-header">
                    <div class="page-title">
                        <i class="fas fa-wallet" style="font-size: 2rem"></i>
                        <h2>Cuentas Propias</h2>
                    </div>
                </div>

                <!-- Sección de Filtros -->
                <div class="filter-section">
                    <div class="row g-3 align-items-end">
                        <div class="col-md-3">
                            <label class="form-label">Buscar por Número de Cuenta</label>
                            <asp:TextBox ID="txtBuscarCuenta" runat="server" CssClass="form-control"
                                placeholder="Ingrese número de cuenta" autocomplete="off"></asp:TextBox>
                            <asp:HiddenField ID="hfCuentasJson" runat="server" />
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Entidad Bancaria</label>
                            <asp:DropDownList ID="ddlFiltroEntidad" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Moneda</label>
                            <asp:DropDownList ID="ddlFiltroMoneda" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-md-2">
                            <label class="form-label">Saldo Disponible (≥)</label>
                            <asp:TextBox ID="txtFiltroSaldo" runat="server" CssClass="form-control"
                                TextMode="Number" step="0.01"></asp:TextBox>
                        </div>

                        <div class="col-12 col-md-3 d-flex align-items-end gap-2 flex-wrap">
                            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar"
                                CssClass="btn btn-primary" OnClick="btnFiltrar_Click" />
                            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar"
                                CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" />
                        </div>
                    </div>
                </div>

                <!-- Botón Nueva Cuenta -->
                <div class="mb-3">
                    <asp:Button ID="btnAbrirModalNuevo" runat="server" Text="Nueva cuenta" CssClass="btn btn-primary"
                        OnClick="btnAbrirModalNuevo_Click" />
                </div>

                <!-- Grilla de Cuentas -->
                <div class="lista-card">
                    <div class="lista-header">
                        <h3 class="lista-title">Lista de Cuentas Propias</h3>
                    </div>

                    <div class="lista-body">
                        <asp:GridView ID="gvCuentasPropias" runat="server" AutoGenerateColumns="False"
                            CssClass="table table-hover" GridLines="None" DataKeyNames="cuenta_bancaria_id"
                            AllowPaging="True" OnPageIndexChanging="gvCuentasPropias_PageIndexChanging" PageSize="10"
                            OnRowCommand="gvCuentasPropias_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="entidad_bancaria.nombre" HeaderText="Entidad Bancaria" />
                                <asp:BoundField DataField="tipo_cuenta" HeaderText="Tipo de Cuenta" />
                                <asp:BoundField DataField="numero_cuenta" HeaderText="Número de Cuenta" />
                                <asp:BoundField DataField="cci" HeaderText="CCI" />
                                <asp:BoundField DataField="moneda.codigo_iso" HeaderText="Moneda" />
                                <asp:BoundField DataField="saldo_disponible" HeaderText="Saldo Disponible" DataFormatString="{0:N2}" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='badge <%# (bool)Eval("activa") ? "bg-success" : "bg-danger" %>'>
                                            <%# (bool)Eval("activa") ? "Activo" : "Inactivo" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnModificar" runat="server" CssClass="btn btn-sm btn-warning btn-icon"
                                            CommandName="Modificar" CommandArgument='<%# Eval("cuenta_bancaria_id") %>' ToolTip="Modificar">
                <i class="fas fa-edit"></i>
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-danger btn-icon"
                                            CommandName="Eliminar" CommandArgument='<%# Eval("cuenta_bancaria_id") %>'
                                            ToolTip="Eliminar" OnClientClick="return confirm('¿Está seguro de eliminar esta cuenta?');">
                <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pagination-aw" HorizontalAlign="Center" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última"
                                PageButtonCount="10" />
                        </asp:GridView>
                    </div>

                    <div class="lista-footer">
                        <asp:Label ID="lblRegistros" runat="server" CssClass="total-registros"></asp:Label>
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
            <div class="lista-footer">
                <asp:Label ID="Label1" runat="server" CssClass="total-registros"></asp:Label>
            </div>

        </div>

        <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css">
        <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

        <script type="text/javascript">
            function abrirModal() {
                var modal = new bootstrap.Modal(document.getElementById('modalCuenta'), {});
                modal.show();
            }

            function cerrarModal() {
                var modal = bootstrap.Modal.getInstance(document.getElementById('modalCuenta'));
                if (modal) {
                    modal.hide();
                }
                // Limpiar el backdrop si queda residual
                var backdrops = document.querySelectorAll('.modal-backdrop');
                backdrops.forEach(function (backdrop) {
                    backdrop.remove();
                });
                document.body.classList.remove('modal-open');
                document.body.style.removeProperty('overflow');
                document.body.style.removeProperty('padding-right');
            }

            // Autocomplete para búsqueda de cuentas
            $(document).ready(function () {
                var cuentasJson = $('#<%= hfCuentasJson.ClientID %>').val();
                var cuentas = cuentasJson ? JSON.parse(cuentasJson) : [];

                $('#<%= txtBuscarCuenta.ClientID %>').autocomplete({
                    source: function (request, response) {
                        var term = request.term.toLowerCase();
                        var matches = $.grep(cuentas, function (cuenta) {
                            return cuenta.numero_cuenta.toLowerCase().includes(term) ||
                                cuenta.entidad_bancaria.toLowerCase().includes(term) ||
                                cuenta.cci.toLowerCase().includes(term);
                        });

                        response(matches.slice(0, 10).map(function (cuenta) {
                            return {
                                label: cuenta.numero_cuenta + ' - ' + cuenta.entidad_bancaria + ' (' + cuenta.moneda + ')',
                                value: cuenta.numero_cuenta
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
    </div>
</asp:Content>
