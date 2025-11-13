<%@ Page Title="Editar Propuesta" Language="C#" MasterPageFile="~/SoftPac.Master"
    AutoEventWireup="true" CodeBehind="EditarPropuesta.aspx.cs"
    Inherits="SoftPacWA.EditarPropuesta" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-container {
            max-width: 1400px;
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
            color: var(--color-primary);
            font-size: 1.8rem;
            font-weight: 600;
            margin: 0;
        }

        .actions-buttons {
            display: flex;
            gap: 0.5rem;
        }

        .info-cards {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }

        .info-card {
            background: white;
            border: 1px solid #e0e0e0;
            border-radius: 10px;
            padding: 1.5rem;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }

        .info-card-label {
            font-size: 0.85rem;
            color: var(--color-secondary);
            margin-bottom: 0.5rem;
            font-weight: 500;
        }

        .info-card-value {
            font-size: 1.2rem;
            font-weight: 700;
            color: var(--color-primary);
        }

        .badge-estado {
            display: inline-block;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 1rem;
            font-weight: 600;
            background-color: #fff3cd;
            color: #856404;
        }

        .alert-info-box {
            background-color: #e7f3ff;
            border-left: 4px solid #2196F3;
            padding: 1rem 1.25rem;
            border-radius: 6px;
            margin-bottom: 2rem;
        }

        .alert-info-box i {
            color: #2196F3;
            margin-right: 0.5rem;
        }

        .totales-section {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 2rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .totales-title {
            color: var(--color-primary);
            font-size: 1.1rem;
            font-weight: 600;
            margin-bottom: 1rem;
        }

        .totales-grid {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
        }

        .total-badge {
            background: linear-gradient(135deg, var(--color-secondary), var(--color-primary));
            color: white;
            padding: 0.75rem 1.5rem;
            border-radius: 999px;
            font-weight: 600;
            font-size: 1.1rem;
        }

        .detalles-card {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .detalles-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid var(--color-light-1);
        }

        .detalles-title {
            color: var(--color-primary);
            font-size: 1.2rem;
            font-weight: 600;
        }

        .forma-pago-select {
            padding: 0.25rem 0.5rem;
            border: 1px solid var(--color-light-1);
            border-radius: 4px;
            font-size: 0.85rem;
        }

        .empty-state {
            text-align: center;
            padding: 3rem;
            color: var(--color-secondary);
        }

        .empty-state i {
            font-size: 3rem;
            margin-bottom: 1rem;
            opacity: 0.5;
        }

        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                gap: 1rem;
                align-items: flex-start;
            }

            .info-cards {
                grid-template-columns: 1fr;
            }

            .actions-buttons {
                width: 100%;
                flex-direction: column;
            }

            .actions-buttons .btn {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        
        <!-- Mensajes -->
        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-dismissible fade show" role="alert">
            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </asp:Panel>

        <!-- Page Header -->
        <div class="page-header">
            <div>
                <h2 class="page-title">
                    <i class="fas fa-edit"></i>
                    Editar Propuesta #<asp:Label ID="lblPropuestaId" runat="server"></asp:Label>
                </h2>
            </div>
            <div class="actions-buttons">
                <asp:Button ID="btnCancelar" runat="server" 
                    Text="Cancelar" 
                    CssClass="btn btn-secondary"
                    OnClick="btnCancelar_Click"
                    CausesValidation="false" />
                
                <asp:Button ID="btnGuardar" runat="server" 
                    Text="Guardar Cambios" 
                    CssClass="btn btn-success"
                    OnClick="btnGuardar_Click" />
            </div>
        </div>

        <!-- Alert Info -->
        <div class="alert-info-box">
            <i class="fas fa-info-circle"></i>
            <strong>Nota:</strong> Puede modificar la forma de pago de cada registro o eliminar pagos de esta propuesta.
            Los cambios no se guardarán hasta que presione "Guardar Cambios".
        </div>

        <!-- Info Cards -->
        <div class="info-cards">
            <div class="info-card">
                <div class="info-card-label">Estado</div>
                <div class="info-card-value">
                    <span class="badge-estado">Pendiente</span>
                </div>
            </div>
            <div class="info-card">
                <div class="info-card-label">Fecha de Creación</div>
                <div class="info-card-value">
                    <asp:Label ID="lblFechaCreacion" runat="server"></asp:Label>
                </div>
            </div>
            <div class="info-card">
                <div class="info-card-label">Usuario Creador</div>
                <div class="info-card-value">
                    <asp:Label ID="lblUsuarioCreador" runat="server"></asp:Label>
                </div>
            </div>
            <div class="info-card">
                <div class="info-card-label">País</div>
                <div class="info-card-value">
                    <asp:Label ID="lblPais" runat="server"></asp:Label>
                </div>
            </div>
            <div class="info-card">
                <div class="info-card-label">Entidad Bancaria</div>
                <div class="info-card-value">
                    <asp:Label ID="lblBanco" runat="server"></asp:Label>
                </div>
            </div>
            <div class="info-card">
                <div class="info-card-label">Total de Pagos</div>
                <div class="info-card-value">
                    <asp:Label ID="lblTotalPagos" runat="server"></asp:Label>
                </div>
            </div>
        </div>

        <!-- Totales por Moneda -->
        <asp:Panel class="totales-section" runat="server" ID="PnlTotalesMoneda">
            <div class="totales-title">
                <i class="fas fa-coins"></i>
                Totales por Moneda
            </div>
            <div class="totales-grid">
                <asp:Repeater ID="rptTotales" runat="server">
                    <ItemTemplate>
                        <span class="total-badge">
                            <%# Eval("CodigoMoneda") %>: <%# string.Format("{0:N2}", Eval("Total")) %>
                        </span>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>

        <!-- Botón para agregar nuevo detalle (colocar justo encima de la grilla de detalles) -->
        <div style="margin-bottom:1rem;">
            <asp:Button ID="btnAgregarDetalle" runat="server" Text="Agregar Detalle" CssClass="btn btn-primary"
                OnClick="btnAgregarDetalle_Click" />
        </div>

        <!-- Detalles de Pagos -->
        <div class="detalles-card">
            <div class="detalles-header">
                <h3 class="detalles-title">
                    <i class="fas fa-list"></i>
                    Detalle de Pagos
                </h3>
                <span class="text-muted">
                    <asp:Label ID="lblCantidadPagos" runat="server"></asp:Label>
                </span>
            </div>

            <asp:UpdatePanel ID="upDetalles" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:GridView ID="gvDetalles" runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-hover align-middle"
                            AllowPaging="true"
                            PageSize="20"
                            OnPageIndexChanging="gvDetalles_PageIndexChanging"
                            OnRowCommand="gvDetalles_RowCommand"
                            GridLines="None"
                            EmptyDataText="No hay detalles en esta propuesta.">
                            
                            <HeaderStyle CssClass="table-light" />
                            
                            <Columns>
                                <asp:BoundField DataField="NumeroFactura" HeaderText="N° Factura" />
                                
                                <asp:TemplateField HeaderText="Acreedor">
                                    <ItemTemplate>
                                        <%# Eval("RazonSocialAcreedor") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:BoundField DataField="CodigoMoneda" HeaderText="Moneda" />
                                
                                <asp:TemplateField HeaderText="Monto">
                                    <ItemTemplate>
                                        <%# string.Format("{0:N2}", Eval("Monto")) %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cuenta Origen">
                                    <ItemTemplate>
                                        <%# Eval("CuentaOrigen") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cuenta Destino">
                                    <ItemTemplate>
                                        <%# Eval("CuentaDestino") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Forma de Pago">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlFormaPago" runat="server" 
                                            CssClass="forma-pago-select"
                                            SelectedValue='<%# Eval("FormaPagoChar") %>'>
                                            <asp:ListItem Value="T" Text="Transferencia"></asp:ListItem>
                                            <asp:ListItem Value="C" Text="Cheque"></asp:ListItem>
                                            <asp:ListItem Value="E" Text="Efectivo"></asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEliminar" runat="server"
                                            CssClass="btn btn-sm btn-danger"
                                            CommandName="EliminarDetalle"
                                            CommandArgument='<%# Eval("DetalleId") %>'
                                            ToolTip="Eliminar pago"
                                            OnClientClick="return false;">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <PagerStyle CssClass="pagination justify-content-center mt-3" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" PageButtonCount="10" />
                            
                            <EmptyDataTemplate>
                                <div class="empty-state">
                                    <i class="fas fa-inbox"></i>
                                    <h5>No hay detalles disponibles</h5>
                                    <p>Esta propuesta no tiene pagos para editar.</p>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Modal de Confirmación de Eliminación -->
    <div class="modal fade" id="modalEliminar" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-exclamation-triangle"></i>
                        Confirmar Eliminación
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p>¿Está seguro de que desea eliminar este pago de la propuesta?</p>
                    <p class="text-muted mb-0">Esta acción se guardará al presionar "Guardar Cambios".</p>
                    <asp:HiddenField ID="hdnDetalleIdEliminar" runat="server" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnConfirmarEliminar" runat="server" 
                        Text="Eliminar" 
                        CssClass="btn btn-danger"
                        OnClick="btnConfirmarEliminar_Click"
                        CausesValidation="false" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function mostrarModalEliminar(detalleId) {
            document.getElementById('<%= hdnDetalleIdEliminar.ClientID %>').value = detalleId;
            $('#modalEliminar').modal('show');
            return false;
        }
    </script>
</asp:Content>