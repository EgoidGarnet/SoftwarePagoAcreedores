<%@ Page Title="Resumen de Propuesta" Language="C#" MasterPageFile="~/SoftPac.Master"
    AutoEventWireup="true" CodeBehind="ResumenPropuesta.aspx.cs"
    Inherits="SoftPacWA.ResumenPropuesta" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
    <style>
        .wizard-container {
            max-width: 1400px;
            margin: 0 auto;
        }

        .page-header {
            text-align: center;
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 3px solid var(--color-light-1);
        }

        .page-header h2 {
            color: var(--color-primary);
            font-size: 1.8rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
        }

        .step-indicator {
            background-color: var(--color-primary);
            color: white;
            padding: 1rem 2rem;
            border-radius: 8px;
            margin-bottom: 2rem;
            text-align: center;
            font-weight: 600;
            font-size: 1.1rem;
        }

        .resumen-cards {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }

        .resumen-card {
            background: white;
            border: 1px solid #e0e0e0;
            border-radius: 10px;
            padding: 1.5rem;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }

        .resumen-card h4 {
            margin: 0 0 0.5rem 0;
            font-weight: 600;
            font-size: 0.9rem;
            color: var(--color-secondary);
        }

        .resumen-card p {
            margin: 0;
            font-size: 1.1rem;
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

        .pagos-card {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 2rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .pagos-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid var(--color-light-1);
        }

        .pagos-title {
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

        .actions-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 2rem;
            padding-top: 2rem;
            border-top: 2px solid var(--color-light-1);
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

        /* Modal de confirmación */
        .modal-header {
            background-color: var(--color-primary);
            color: white;
        }

        .modal-body {
            padding: 2rem;
        }

        .confirmacion-icon {
            font-size: 4rem;
            color: var(--color-secondary);
            margin-bottom: 1rem;
        }

        @media (max-width: 768px) {
            .resumen-cards {
                grid-template-columns: 1fr;
            }

            .totales-grid {
                flex-direction: column;
            }

            .actions-container {
                flex-direction: column-reverse;
                gap: 1rem;
            }

            .btn {
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="wizard-container">
        
        <!-- Mensajes -->
        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-dismissible fade show" role="alert">
            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </asp:Panel>

        <!-- Page Header -->
        <div class="page-header">
            <h2>
                <i class="fas fa-money-check-alt"></i>
                Nueva Propuesta de Pago
            </h2>
            <p>Revise y confirme los detalles de la propuesta</p>
        </div>

        <!-- Step Indicator -->
        <div class="step-indicator">
            <i class="fas fa-check-circle"></i>
            Paso 4 de 4: Resumen y Confirmación
        </div>

        <!-- Resumen Cards -->
        <div class="resumen-cards">
            <div class="resumen-card">
                <h4>País</h4>
                <p><asp:Label ID="lblPais" runat="server"></asp:Label></p>
            </div>
            <div class="resumen-card">
                <h4>Entidad Bancaria</h4>
                <p><asp:Label ID="lblBanco" runat="server"></asp:Label></p>
            </div>
            <div class="resumen-card">
                <h4>Total de Facturas</h4>
                <p><asp:Label ID="lblTotalFacturas" runat="server"></asp:Label></p>
            </div>
            <div class="resumen-card">
                <h4>Total de Pagos</h4>
                <p><asp:Label ID="lblTotalPagos" runat="server"></asp:Label></p>
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

        <!-- Detalle de Pagos -->
        <div class="pagos-card">
            <div class="pagos-header">
                <h3 class="pagos-title">
                    <i class="fas fa-list"></i>
                    Detalle de Pagos
                </h3>
                <small class="text-muted">Puede modificar la forma de pago de cada registro</small>
            </div>

            <asp:UpdatePanel ID="upPagos" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:GridView ID="gvPagos" runat="server" 
                            AutoGenerateColumns="false" 
                            CssClass="table table-hover align-middle"
                            AllowPaging="true" 
                            PageSize="20"
                            OnPageIndexChanging="gvPagos_PageIndexChanging"
                            GridLines="None"
                            EmptyDataText="No hay pagos en esta propuesta.">
                            
                            <HeaderStyle CssClass="table-light" />
                            
                            <Columns>
                                <asp:BoundField DataField="NumeroFactura" HeaderText="N° Factura" />
                                
                                <asp:TemplateField HeaderText="Proveedor">
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
                                            SelectedValue='<%# Eval("FormaPago") %>'>
                                            <asp:ListItem Value="T" Text="Transferencia"></asp:ListItem>
                                            <asp:ListItem Value="C" Text="Cheque"></asp:ListItem>
                                            <asp:ListItem Value="E" Text="Efectivo"></asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <PagerStyle CssClass="pagination justify-content-center mt-3" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" PageButtonCount="10" />
                            
                            <EmptyDataTemplate>
                                <div class="empty-state">
                                    <i class="fas fa-inbox"></i>
                                    <h5>No hay pagos en esta propuesta</h5>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Actions -->
        <div class="actions-container">
            <asp:Button ID="btnCancelar" runat="server" 
                Text="Cancelar" 
                CssClass="btn btn-secondary"
                OnClick="btnCancelar_Click"
                CausesValidation="false" />
            
            <div style="display: flex; gap: 0.5rem;">
                <asp:Button ID="btnVolver" runat="server" 
                    Text="Volver" 
                    CssClass="btn btn-outline-secondary"
                    OnClick="btnVolver_Click"
                    CausesValidation="false" />
                
                <asp:Button ID="btnGuardar" runat="server" 
                    Text="Guardar Propuesta" 
                    CssClass="btn btn-primary"
                    OnClick="btnGuardar_Click" />
            </div>
        </div>
    </div>

    <!-- Modal de Confirmación -->
    <div class="modal fade" id="modalConfirmacion" tabindex="-1" aria-hidden="true" ClientIDMode="Static">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <i class="fas fa-check-circle"></i>
                        Propuesta Guardada
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body text-center">
                    <i class="fas fa-check-circle confirmacion-icon"></i>
                    <h4>¡Propuesta guardada exitosamente!</h4>
                    <p class="text-muted">La propuesta ha sido creada con estado "Pendiente"</p>
                    <p><strong>ID de Propuesta: <asp:Label ID="lblPropuestaId" runat="server"></asp:Label></strong></p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnIrADetalle" runat="server" 
                        Text="Ver Detalle" 
                        CssClass="btn btn-primary"
                        OnClick="btnIrADetalle_Click" />
                    <asp:Button ID="btnIrAListado" runat="server" 
                        Text="Ir al Listado" 
                        CssClass="btn btn-secondary"
                        OnClick="btnIrAListado_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>