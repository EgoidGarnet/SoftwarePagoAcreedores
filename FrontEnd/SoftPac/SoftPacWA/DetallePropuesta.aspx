<%@ Page Title="Detalle de Propuesta" Language="C#" MasterPageFile="~/SoftPac.Master"
    AutoEventWireup="true" CodeBehind="DetallePropuesta.aspx.cs"
    Inherits="SoftPacWA.DetallePropuesta" %>

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
        }

        .estado-pendiente {
            background-color: #fff3cd;
            color: #856404;
        }

        .estado-enviada {
            background-color: #cfe2ff;
            color: #084298;
        }

        .estado-anulada {
            background-color: #f8d7da;
            color: #842029;
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
                    <i class="fas fa-file-alt"></i>
                    Detalle de Propuesta #<asp:Label ID="lblPropuestaId" runat="server"></asp:Label>
                </h2>
            </div>
            <div class="actions-buttons">
                <asp:HyperLink ID="lnkVolver" runat="server" 
                    NavigateUrl="~/PropuestasPago.aspx" 
                    CssClass="btn btn-secondary">
                    <i class="fas fa-arrow-left"></i> Volver
                </asp:HyperLink>

                <asp:LinkButton ID="btnEditar" runat="server" 
                    CssClass="btn btn-warning"
                    OnClick="btnEditar_Click"
                    ToolTip="Editar propuesta">
                    <i class="fas fa-edit"></i> Editar
                </asp:LinkButton>

                <asp:LinkButton ID="btnAnular" runat="server" 
                    CssClass="btn btn-danger"
                    OnClick="btnAnular_Click"
                    ToolTip="Anular propuesta"
                    OnClientClick="return false;">
                    <i class="fas fa-ban"></i> Anular
                </asp:LinkButton>

                <asp:LinkButton ID="btnExportar" runat="server" 
                    CssClass="btn btn-primary"
                    OnClick="btnExportar_Click"
                    ToolTip="Exportar propuesta">
                    <i class="fas fa-paper-plane"></i> Exportar
                </asp:LinkButton>
            </div>
        </div>

        <!-- Info Cards -->
        <div class="info-cards">
            <div class="info-card">
                <div class="info-card-label">Estado</div>
                <div class="info-card-value">
                    <asp:Label ID="lblEstado" runat="server" CssClass="badge-estado"></asp:Label>
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
        <div class="totales-section">
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
                            GridLines="None"
                            EmptyDataText="No hay detalles en esta propuesta.">
                            
                            <HeaderStyle CssClass="table-light" />
                            
                            <Columns>
                                <asp:BoundField DataField="NumeroFactura" HeaderText="N° Factura" />
                                
                                <asp:TemplateField HeaderText="acreedor">
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
                                
                                <asp:TemplateField HeaderText="Banco Origen">
                                    <ItemTemplate>
                                        <%# Eval("BancoOrigen") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Cuenta Destino">
                                    <ItemTemplate>
                                        <%# Eval("CuentaDestino") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Banco Destino">
                                    <ItemTemplate>
                                        <%# Eval("BancoDestino") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:BoundField DataField="FormaPago" HeaderText="Forma de Pago" />
                            </Columns>

                            <PagerStyle CssClass="pagination justify-content-center mt-3" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" PageButtonCount="10" />
                            
                            <EmptyDataTemplate>
                                <div class="empty-state">
                                    <i class="fas fa-inbox"></i>
                                    <h5>No hay detalles disponibles</h5>
                                    <p>Esta propuesta no tiene pagos registrados.</p>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Modal de Anulación -->
    <div class="modal fade" id="modalAnular" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-ban"></i>
                        Anular Propuesta
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i>
                        <strong>Advertencia:</strong> Esta acción no se puede deshacer.
                    </div>
                    <%--<div class="mb-3">
                        <label class="form-label fw-bold">Motivo de Anulación <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtMotivoAnulacion" runat="server" 
                            CssClass="form-control" 
                            TextMode="MultiLine" 
                            Rows="4"
                            placeholder="Ingrese el motivo por el cual se anula esta propuesta..."
                            MaxLength="500"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvMotivo" runat="server"
                            ControlToValidate="txtMotivoAnulacion"
                            ErrorMessage="Debe ingresar un motivo de anulación"
                            CssClass="text-danger"
                            Display="Dynamic"
                            ValidationGroup="Anular" />
                    </div>--%>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnConfirmarAnulacion" runat="server" 
                        Text="Confirmar Anulación" 
                        CssClass="btn btn-danger"
                        OnClick="btnConfirmarAnulacion_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function mostrarModalAnular() {
            $('#modalAnular').modal('show');
            return false;
        }
    </script>
</asp:Content>