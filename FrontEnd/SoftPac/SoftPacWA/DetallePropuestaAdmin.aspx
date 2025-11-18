<%@ Page Title="Detalle de Propuesta - Admin" Language="C#" MasterPageFile="~/Admin.Master"
    AutoEventWireup="true" CodeBehind="DetallePropuestaAdmin.aspx.cs"
    Inherits="SoftPacWA.DetallePropuestaAdmin" %>

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
            margin-bottom: 2rem;
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

        .saldo-input {
            width: 120px;
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
    <asp:Panel class="page-container" runat="server">
        
        <!-- Mensajes -->
        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-dismissible fade show" role="alert">
            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </asp:Panel>

        <!-- Page Header -->
        <asp:Panel class="page-header" runat="server">
            <div>
                <h2 class="page-title">
                    <i class="fas fa-file-alt"></i>
                    Detalle de Propuesta #<asp:Label ID="lblPropuestaId" runat="server"></asp:Label>
                </h2>
            </div>
            <div class="actions-buttons">
                <asp:HyperLink ID="lnkVolver" runat="server" 
                    NavigateUrl="~/AprobacionPropuestas.aspx" 
                    CssClass="btn btn-secondary">
                    <i class="fas fa-arrow-left"></i> Volver
                </asp:HyperLink>

                <asp:LinkButton ID="btnRechazar" runat="server" 
                    CssClass="btn btn-danger"
                    OnClick="btnRechazar_Click"
                    ToolTip="Rechazar propuesta">
                    <i class="fas fa-times"></i> Rechazar
                </asp:LinkButton>

                <asp:LinkButton ID="btnExportar" runat="server" 
                    CssClass="btn btn-primary"
                    OnClick="btnExportar_Click"
                    ToolTip="Exportar propuesta">
                    <i class="fas fa-file-export"></i> Exportar
                </asp:LinkButton>
                
                <asp:LinkButton ID="btnConfirmarEnvio" runat="server" 
                    CssClass="btn btn-success"
                    OnClick="btnConfirmarEnvio_Click"
                    ToolTip="Confirmar envío">
                    <i class="fas fa-paper-plane"></i> Confirmar Envío
                </asp:LinkButton>
            </div>
        </asp:Panel>

        <!-- Info Cards (SIN ESTADO) -->
        <div class="info-cards">
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
        <asp:Panel ID="pnlTotalesMoneda" runat="server" class="totales-section">
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

        <!-- Cuentas Propias -->
        <asp:Panel ID="pnlCuentasPropias" runat="server" CssClass="detalles-card">
            <div class="detalles-header">
                <h3 class="detalles-title">
                    <i class="fas fa-wallet"></i>
                    Cuentas Propias y Saldos
                </h3>
            </div>

            <asp:UpdatePanel ID="upCuentas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:GridView ID="gvCuentasPropias" runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table table-hover align-middle"
                            GridLines="None"
                            DataKeyNames="CuentaId"
                            OnRowCommand="gvCuentasPropias_RowCommand"
                            OnRowDataBound="gvCuentasPropias_RowDataBound"
                            EmptyDataText="No hay cuentas asociadas a esta propuesta.">
                            
                            <HeaderStyle CssClass="table-light" />
                            
                            <Columns>
                                <asp:BoundField DataField="Banco" HeaderText="Banco" />
                                <asp:BoundField DataField="NumeroCuenta" HeaderText="Número de Cuenta" />
                                
                                <asp:TemplateField HeaderText="Saldo Disponible">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSaldoDisponible" runat="server" 
                                            Text='<%# string.Format("{0:N2}", Eval("SaldoDisponible")) %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Saldo Requerido">
                                    <ItemTemplate>
                                        <%# string.Format("{0:N2}", Eval("SaldoUsado")) %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Actualizar Saldo">
                                    <ItemTemplate>
                                        <div class="input-group input-group-sm">
                                            <asp:TextBox ID="txtSaldoNuevo" runat="server" 
                                                CssClass="form-control saldo-input" 
                                                TextMode="Number"
                                                step="0.01"
                                                placeholder="Nuevo saldo">
                                            </asp:TextBox>
                                            <asp:LinkButton ID="btnActualizarSaldo" runat="server"
                                                CssClass="btn btn-primary btn-sm"
                                                CommandName="ActualizarSaldo"
                                                CommandArgument='<%# Container.DataItemIndex %>'
                                                ToolTip="Actualizar saldo">
                                                <i class="fas fa-sync-alt"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <EmptyDataTemplate>
                                <div class="empty-state">
                                    <i class="fas fa-wallet"></i>
                                    <h5>No hay cuentas propias</h5>
                                    <p>Esta propuesta no tiene cuentas asociadas.</p>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>

        <!-- Detalles de Pagos (SIN COLUMNA ESTADO) -->
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
                            DataKeyNames="DetalleId"
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
    </asp:Panel>

    <!-- Modal de Rechazo -->
    <div class="modal fade" id="modalRechazar" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-times-circle me-2"></i>Rechazar Propuesta
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <p class="mb-0">¿Está seguro de que desea rechazar esta propuesta?</p>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                        <i class="fas fa-arrow-left me-1"></i>Cancelar
                    </button>
                    <asp:Button ID="btnConfirmarRechazar" runat="server"
                        CssClass="btn btn-danger"
                        Text="Rechazar"
                        OnClick="btnConfirmarRechazar_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>