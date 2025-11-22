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

        /* Estilos para validación */
        .error-message {
            font-size: 0.875rem;
            margin-top: 0.25rem;
            display: block;
        }

        .form-control.is-invalid, .form-select.is-invalid {
            border-color: #dc3545;
        }

        .btn-icon {
            padding: .25rem .5rem;
            font-size: .9rem;
        }

        .action-buttons {
            display: flex;
            gap: .5rem;
            flex-wrap: wrap;
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
                    <div class="row gx-3 gy-3">
                        <div class="col-12 col-md-3">
                            <label class="form-label">Buscar por Número de Cuenta</label>
                            <asp:TextBox ID="txtBuscarCuenta" runat="server" CssClass="form-control"
                                placeholder="Ingrese número de cuenta" autocomplete="off"></asp:TextBox>
                            <asp:HiddenField ID="hfCuentasJson" runat="server" />
                        </div>

                        <div class="col-12 col-md-2">
                            <label class="form-label">Entidad Bancaria</label>
                            <asp:DropDownList ID="ddlFiltroEntidad" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-12 col-md-2">
                            <label class="form-label">Moneda</label>
                            <asp:DropDownList ID="ddlFiltroMoneda" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-12 col-md-2">
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
                        <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                            <asp:GridView ID="gvCuentasPropias" runat="server" AutoGenerateColumns="False"
                                CssClass="table table-hover" GridLines="None" DataKeyNames="cuenta_bancaria_id"
                                AllowPaging="True" OnPageIndexChanging="gvCuentasPropias_PageIndexChanging" PageSize="10"
                                OnRowCommand="gvCuentasPropias_RowCommand"
                                EmptyDataText="No se encontraron cuentas propias">
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
                                            <div class="action-buttons">
                                                <asp:LinkButton ID="btnVerDetalle" runat="server" CssClass="btn btn-sm btn-info btn-icon"
                                                    CommandName="VerDetalle" CommandArgument='<%# Eval("cuenta_bancaria_id") %>' ToolTip="Ver detalle">
                                                    <i class="fas fa-eye"></i>
                                                </asp:LinkButton>

                                                <asp:LinkButton ID="btnModificar" runat="server" CssClass="btn btn-sm btn-warning btn-icon"
                                                    CommandName="Modificar" CommandArgument='<%# Eval("cuenta_bancaria_id") %>' ToolTip="Modificar">
                                                    <i class="fas fa-edit"></i>
                                                </asp:LinkButton>

                                                <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-danger btn-icon"
                                                    CommandName="MostrarModalEliminar" CommandArgument='<%# Eval("cuenta_bancaria_id") %>'
                                                    ToolTip="Eliminar">
                                                    <i class="fas fa-trash"></i>
                                                </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pagination-aw" HorizontalAlign="Center" />
                                <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última"
                                    PageButtonCount="10" />
                            </asp:GridView>
                        </div>
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
                                    <asp:Literal ID="litModalTitulo" runat="server"></asp:Literal>
                                </h5>
                                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <asp:HiddenField ID="hfCuentaId" runat="server" Value="0" />
                                <div class="row g-3">
                                    <div class="col-md-6">
                                        <label class="form-label">Entidad Bancaria <span class="text-danger">*</span></label>
                                        <asp:DropDownList ID="ddlEntidadBancaria" runat="server" CssClass="form-select"></asp:DropDownList>
                                        <asp:Label ID="lblEntidadError" runat="server" CssClass="text-danger error-message"></asp:Label>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Moneda <span class="text-danger">*</span></label>
                                        <asp:DropDownList ID="ddlMoneda" runat="server" CssClass="form-select"></asp:DropDownList>
                                        <asp:Label ID="lblMonedaError" runat="server" CssClass="text-danger error-message"></asp:Label>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Tipo de Cuenta <span class="text-danger">*</span></label>
                                        <asp:DropDownList ID="ddlTipoCuenta" runat="server" CssClass="form-select">
                                            <asp:ListItem Value="">--Seleccione--</asp:ListItem>
                                            <asp:ListItem Value="Ahorro">Ahorro</asp:ListItem>
                                            <asp:ListItem Value="Corriente">Corriente</asp:ListItem>
                                            <asp:ListItem Value="Cuenta Maestra">Cuenta Maestra</asp:ListItem>
                                            <asp:ListItem Value="Interbancaria">Interbancaria</asp:ListItem>
                                            <asp:ListItem Value="Detracción">Detracción</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblTipoCuentaError" runat="server" CssClass="text-danger error-message"></asp:Label>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Número de Cuenta <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtNumeroCuenta" runat="server" CssClass="form-control" MaxLength="20" placeholder="Ej: 00123456789012345678"></asp:TextBox>
                                        <asp:Label ID="lblNumeroCuentaError" runat="server" CssClass="text-danger error-message"></asp:Label>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">CCI (Código de Cuenta Interbancario) <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtCci" runat="server" CssClass="form-control" MaxLength="20" placeholder="Ej: 00123456789012345678"></asp:TextBox>
                                        <asp:Label ID="lblCciError" runat="server" CssClass="text-danger error-message"></asp:Label>
                                    </div>
                                    <div class="col-md-6">
                                        <label class="form-label">Saldo Disponible <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtSaldoDisponible" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="0.00"></asp:TextBox>
                                        <asp:Label ID="lblSaldoError" runat="server" CssClass="text-danger error-message"></asp:Label>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-check form-switch mt-4">
                                            <input id="chkActiva" runat="server" type="checkbox" class="form-check-input" checked="checked" />
                                            <label for="chkActiva" class="form-check-label">Cuenta Activa</label>
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-3">
                                    <small class="text-muted"><span class="text-danger">*</span> Campos obligatorios</small>
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
        <!-- Modal de Confirmación de Eliminación -->
        <div class="modal fade" id="modalConfirmarEliminar" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header bg-danger text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-exclamation-triangle me-2"></i>Confirmar Eliminación
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <p class="mb-0">¿Está seguro que desea eliminar esta cuenta propia?</p>
                        <p class="text-muted mb-0 mt-2"><small>Esta acción no se puede deshacer.</small></p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                        <asp:Button ID="btnConfirmarEliminar" runat="server" CssClass="btn btn-danger" 
                            OnClick="btnConfirmarEliminar_Click" Text="Eliminar definitivamente" />
                    </div>
                </div>
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