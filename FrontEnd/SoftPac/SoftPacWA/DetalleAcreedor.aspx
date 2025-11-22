<%@ Page Title="Detalle de Acreedor" Language="C#" MasterPageFile="~/SoftPac.Master"
    AutoEventWireup="true" CodeBehind="DetalleAcreedor.aspx.cs" Inherits="SoftPacWA.DetalleAcreedor" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
            flex-wrap: wrap;
            gap: 1rem;
        }

        .page-header h3 {
            color: var(--color-primary);
            font-size: 1.8rem;
            font-weight: 600;
            margin: 0;
        }

        .header-actions {
            display: flex;
            gap: .5rem;
            flex-wrap: wrap;
        }

        .card-info {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            margin-bottom: 1.5rem;
        }

        .card-info .card-body {
            padding: 1.5rem;
        }

        .kv {
            display: flex;
            gap: .5rem;
            align-items: center;
            flex-wrap: wrap;
        }

        .kv .k {
            min-width: 130px;
            color: #6c757d;
            font-weight: 600;
        }

        .kv .v {
            font-weight: 500;
            color: var(--color-primary);
        }

        .small-cols > .col {
            padding-top: .5rem;
            padding-bottom: .5rem;
        }

        .badge-estado {
            padding: .35rem .7rem;
            border-radius: 4px;
            font-size: .85rem;
            font-weight: 600;
        }

        .badge-activo {
            background: #28a745;
            color: #fff;
        }

        .badge-inactivo {
            background: #dc3545;
            color: #fff;
        }

        .action-buttons {
            display: flex;
            gap: .5rem;
            flex-wrap: wrap;
        }

        .btn-icon {
            padding: .25rem .5rem;
            font-size: .9rem;
        }

        /* Sección de cuentas bancarias */
        .cuentas-section {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            overflow: hidden;
        }

        .cuentas-header {
            padding: 1rem 1.5rem;
            border-bottom: 2px solid var(--color-light-1);
            background-color: #f8f9fa;
        }

        .cuentas-header h5 {
            color: var(--color-primary);
            font-size: 1.2rem;
            font-weight: 600;
            margin: 0;
        }

        .cuentas-body {
            padding: 1.5rem;
        }

        .cuentas-footer {
            padding: 1rem 1.5rem;
            border-top: 1px solid var(--color-light-1);
            background-color: #f8f9fa;
        }

        /* para el iframe del modal */
        .modal-iframe-container {
            width: 100%;
            height: 70vh;
        }

        .modal-iframe-container iframe {
            width: 100%;
            height: 100%;
            border: none;
        }

        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .header-actions {
                width: 100%;
            }

            .header-actions .btn {
                flex: 1;
                min-width: auto;
            }

            .kv {
                flex-direction: column;
                align-items: flex-start;
            }

            .kv .k {
                min-width: auto;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <div class="page-header">
            <h3><i class="fa-solid fa-id-card me-2"></i>Detalle del acreedor</h3>
            <div class="header-actions">
                <!-- BOTÓN EDITAR ACREEDOR -->
                <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-warning btn-sm" 
                    OnClick="btnEditar_Click">
                    <i class="fa fa-edit"></i> Editar acreedor
                </asp:LinkButton>
                
                <!-- BOTÓN ELIMINAR ACREEDOR -->
                <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-danger btn-sm" 
                    OnClientClick="return false;"
                    data-bs-toggle="modal" data-bs-target="#modalConfirmarEliminar">
                    <i class="fa fa-trash"></i> Eliminar acreedor
                </asp:LinkButton>
                
                <!-- MARCAMOS PARA ABRIR EN MODAL -->
                <asp:HyperLink ID="lnkNuevaCuenta" runat="server" CssClass="btn btn-primary btn-sm" data-modal-cuenta="true">
                    <i class="fa fa-plus"></i> Nueva cuenta
                </asp:HyperLink>
                <asp:HyperLink ID="lnkVolver" runat="server" CssClass="btn btn-secondary btn-sm" NavigateUrl="Acreedores.aspx">
                    <i class="fa fa-arrow-left"></i> Volver
                </asp:HyperLink>
            </div>
        </div>

        <asp:HiddenField ID="hfAcreedorId" runat="server" />

        <!-- Datos del acreedor -->
        <div class="card-info">
            <div class="card-body">
                <div class="row row-cols-1 row-cols-md-3 g-3 small-cols">
                    <div class="col">
                        <div class="kv">
                            <span class="k">Razón social</span>
                            <asp:Label ID="lblRazon" runat="server" CssClass="v" />
                        </div>
                    </div>
                    <div class="col">
                        <div class="kv">
                            <span class="k">RUC / NIT / RFC</span>
                            <asp:Label ID="lblRuc" runat="server" CssClass="v" />
                        </div>
                    </div>
                    <div class="col">
                        <div class="kv">
                            <span class="k">País</span>
                            <asp:Label ID="lblPais" runat="server" CssClass="v" />
                        </div>
                    </div>
                    <div class="col-12">
                        <div class="kv">
                            <span class="k">Dirección fiscal</span>
                            <asp:Label ID="lblDir" runat="server" CssClass="v" />
                        </div>
                    </div>
                    <div class="col">
                        <div class="kv">
                            <span class="k">Plazo pago</span>
                            <span class="v"><asp:Label ID="lblPlazo" runat="server" /> días</span>
                        </div>
                    </div>
                    <div class="col">
                        <div class="kv">
                            <span class="k">Estado</span>
                            <span class="v"><asp:Literal ID="litEstado" runat="server" /></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Cuentas bancarias -->
        <div class="cuentas-section">
            <div class="cuentas-header">
                <h5><i class="fa-solid fa-building-columns me-2"></i>Cuentas bancarias</h5>
            </div>

            <div class="cuentas-body">
                <asp:UpdatePanel ID="upCuentas" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                            <asp:GridView ID="gvCuentas" runat="server" CssClass="table table-hover mb-0"
                                AutoGenerateColumns="False" GridLines="None" 
                                EmptyDataText="Sin cuentas registradas"
                                OnRowDataBound="gvCuentas_RowDataBound" 
                                OnRowCommand="gvCuentas_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Entidad">
                                        <ItemTemplate><%# Eval("Entidad") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="N° Cuenta">
                                        <ItemTemplate><%# Eval("NumeroCuenta") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CCI/IBAN">
                                        <ItemTemplate><%# Eval("CCI") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo">
                                        <ItemTemplate><%# Eval("TipoCuenta") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Divisa">
                                        <ItemTemplate><%# Eval("Divisa") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Estado">
                                        <ItemTemplate>
                                            <span class='badge-estado <%# GetCuentaEstadoClass(Eval("Estado")) %>'><%# Eval("Estado") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <div class="action-buttons">
                                                <!-- MARCAMOS EDITAR PARA MODAL -->
                                                <asp:HyperLink ID="btnEditarCuenta" runat="server" CssClass="btn btn-sm btn-warning btn-icon"
                                                    NavigateUrl='<%# "GestionDetalleAcreedor.aspx?acreedorId=" + Eval("AcreedorId") + "&cuentaId=" + Eval("CuentaBancariaId") + "&popup=1" %>'
                                                    ToolTip="Editar" data-modal-cuenta="true">
                                                    <i class="fas fa-edit"></i>
                                                </asp:HyperLink>

                                                <asp:LinkButton ID="btnToggleCuenta" runat="server" CssClass="btn btn-sm btn-danger btn-icon"
                                                    CommandName="Toggle" CommandArgument='<%# Eval("CuentaBancariaId") %>' ToolTip="Inactivar/Activar">
                                                    <i class="fas fa-trash"></i>
                                                </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="text-center py-4">
                                        <i class="fa-solid fa-building-columns fa-2x text-muted mb-3"></i>
                                        <p class="text-muted mb-0">Sin cuentas registradas</p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="cuentas-footer">
                <asp:Label ID="lblTotalCuentas" runat="server" CssClass="text-muted"></asp:Label>
            </div>
        </div>

        <!-- MODAL PARA CONFIRMAR ELIMINACIÓN -->
        <div class="modal fade" id="modalConfirmarEliminar" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header bg-danger text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-exclamation-triangle me-2"></i>Confirmar eliminación
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <p class="mb-0">¿Está seguro que desea eliminar permanentemente este acreedor?</p>
                        <p class="text-muted mb-0 mt-2"><small>Esta acción no se puede deshacer.</small></p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                        <asp:Button ID="btnConfirmarEliminar" runat="server" CssClass="btn btn-danger" 
                            OnClick="btnEliminar_Click" Text="Eliminar definitivamente" />
                    </div>
                </div>
            </div>
        </div>

        <!-- MODAL PARA GESTION DE CUENTAS DE ACREEDOR -->
        <div class="modal fade" id="modalCuentaAcreedor" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header text-white" style="background-color: var(--color-primary);">
                        <h5 class="modal-title"><i class="fa fa-building-columns me-2"></i>Gestión de cuenta de acreedor</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body p-0">
                        <div class="modal-iframe-container">
                            <iframe id="iframeCuentaAcreedor"></iframe>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function abrirModalCuenta(url) {
            var iframe = document.getElementById('iframeCuentaAcreedor');
            iframe.src = url;
            var modal = new bootstrap.Modal(document.getElementById('modalCuentaAcreedor'));
            modal.show();
        }

        // Interceptar clics en enlaces marcados con data-modal-cuenta="true"
        document.addEventListener('click', function (e) {
            var link = e.target.closest('a[data-modal-cuenta="true"]');
            if (link) {
                e.preventDefault();
                abrirModalCuenta(link.href);
            }
        });

        // Escuchar mensajes del iframe para cerrar el modal y recargar
        window.addEventListener('message', function (e) {
            if (e.data === 'cerrarModalCuenta') {
                var modal = bootstrap.Modal.getInstance(document.getElementById('modalCuentaAcreedor'));
                if (modal) {
                    modal.hide();
                }
                // Recargar la página para actualizar la lista de cuentas
                location.reload();
            }
        });

        // Función para mostrar mensajes (toast o alert)
        function mostrarMensaje(mensaje, tipo) {
            var alertHtml = '<div class="alert alert-' + tipo + ' alert-dismissible fade show" role="alert">' +
                mensaje +
                '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                '</div>';
            $('.page-container').prepend(alertHtml);
            setTimeout(function () {
                $('.alert').fadeOut();
            }, 5000);
        }
    </script>
</asp:Content>