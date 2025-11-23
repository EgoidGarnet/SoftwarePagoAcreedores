<%@ Page Title="Nueva Propuesta - Paso 2" Language="C#" MasterPageFile="~/SoftPac.Master" 
    AutoEventWireup="true" CodeBehind="CrearPropuestaPaso2.aspx.cs" 
    Inherits="SoftPacWA.CrearPropuestaPaso2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .wizard-container {
            max-width: 1200px;
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

        .criterios-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 1.5rem;
            margin-bottom: 2rem;
            border-left: 4px solid var(--color-secondary);
        }

        .criterios-title {
            color: var(--color-primary);
            font-size: 1.1rem;
            font-weight: 600;
            margin-bottom: 1rem;
        }

        .criterios-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
        }

        .criterio-item {
            display: flex;
            flex-direction: column;
        }

        .criterio-label {
            font-size: 0.85rem;
            color: var(--color-secondary);
            margin-bottom: 0.25rem;
        }

        .criterio-value {
            font-size: 1rem;
            color: var(--color-primary);
            font-weight: 600;
        }

        .facturas-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 2rem;
            margin-bottom: 2rem;
        }

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid var(--color-light-1);
        }

        .card-title {
            color: var(--color-primary);
            font-size: 1.2rem;
            font-weight: 600;
        }

        .total-registros {
            color: var(--color-secondary);
            font-size: 0.95rem;
            font-weight: 500;
        }

        .monto-total-container {
            background: linear-gradient(135deg, var(--color-secondary), var(--color-primary));
            color: white;
            padding: 1.5rem;
            border-radius: 8px;
            margin-top: 1.5rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .monto-label {
            font-size: 1.1rem;
            font-weight: 600;
        }

        .monto-value {
            font-size: 1.5rem;
            font-weight: 700;
        }

        .info-box {
            background-color: #e7f3ff;
            border-left: 4px solid #2196F3;
            padding: 1rem 1.25rem;
            border-radius: 6px;
            margin-top: 1.5rem;
        }

        .info-box-content {
            color: #1565c0;
            font-size: 0.95rem;
            line-height: 1.5;
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

        @media (max-width: 768px) {
            .criterios-grid {
                grid-template-columns: 1fr;
            }

            .card-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 0.5rem;
            }

            .monto-total-container {
                flex-direction: column;
                gap: 0.5rem;
                text-align: center;
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

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
            <p>Seleccione las facturas a incluir en la propuesta</p>
        </div>

        <!-- Step Indicator -->
        <div class="step-indicator">
            <i class="fas fa-file-invoice"></i>
            Paso 2 de 4: Selección de Facturas
        </div>

        <!-- Criterios Seleccionados -->
        <div class="criterios-card">
            <div class="criterios-title">
                <i class="fas fa-info-circle"></i>
                Criterios de Filtrado
            </div>
            <div class="criterios-grid">
                <div class="criterio-item">
                    <span class="criterio-label">País:</span>
                    <asp:Label ID="lblPais" runat="server" CssClass="criterio-value"></asp:Label>
                </div>
                <div class="criterio-item">
                    <span class="criterio-label">Entidad Bancaria:</span>
                    <asp:Label ID="lblBanco" runat="server" CssClass="criterio-value"></asp:Label>
                </div>
                <div class="criterio-item">
                    <span class="criterio-label">Plazo de Vencimiento:</span>
                    <asp:Label ID="lblPlazo" runat="server" CssClass="criterio-value"></asp:Label>
                </div>
            </div>
        </div>

        <!-- Listado de Facturas -->
        <div class="facturas-card">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="fas fa-file-invoice-dollar"></i>
                    Facturas Disponibles
                </h3>
                <span class="total-registros">
                    <asp:Label ID="lblTotalRegistros" runat="server"></asp:Label>
                </span>
            </div>

            <asp:UpdatePanel ID="upFacturas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:GridView ID="gvFacturas" runat="server" 
                            AutoGenerateColumns="false"
                            CssClass="table table-hover align-middle"
                            AllowPaging="true"
                            PageSize="20"
                            OnPageIndexChanging="gvFacturas_PageIndexChanging"
                            GridLines="None"
                            EmptyDataText="No se encontraron facturas que cumplan con los criterios.">
                            
                            <HeaderStyle CssClass="table-light" />
                            
                            <Columns>
                                <asp:TemplateField HeaderText="">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkSeleccionarTodos" runat="server" 
                                            ToolTip="Seleccionar todas"
                                            onclick="seleccionarTodos(this);" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSeleccionar" runat="server" 
                                            CssClass="factura-checkbox" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:BoundField DataField="numero_factura" HeaderText="N° Factura" />
                                
                                <asp:TemplateField HeaderText="Acreedor">
                                    <ItemTemplate>
                                        <%# Eval("acreedor.razon_social") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Fecha Emisión">
                                    <ItemTemplate>
                                        <%# Eval("fecha_emision") != null ? Convert.ToDateTime(Eval("fecha_emision")).ToString("dd/MM/yyyy") : "-" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Fecha Vencimiento">
                                    <ItemTemplate>
                                        <%# Eval("fecha_limite_pago") != null ? Convert.ToDateTime(Eval("fecha_limite_pago")).ToString("dd/MM/yyyy") : "-" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Moneda">
                                    <ItemTemplate>
                                        <%# Eval("moneda.codigo_iso") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Monto Total">
                                    <ItemTemplate>
                                        <%# String.Format("{0:N2}", Eval("monto_total")) %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Monto Restante">
                                    <ItemTemplate>
                                        <%# String.Format("{0:N2}", Eval("monto_restante")) %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                </asp:TemplateField>
                            </Columns>

                            <PagerStyle CssClass="pagination justify-content-center mt-3" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" PageButtonCount="10" />
                            
                            <EmptyDataTemplate>
                                <div class="empty-state">
                                    <i class="fas fa-inbox"></i>
                                    <h5>No hay facturas disponibles</h5>
                                    <p>No se encontraron facturas pendientes que cumplan con los criterios seleccionados.</p>
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>

                    <div class="info-box">
                        <div class="info-box-content">
                            <strong>Nota:</strong> Solo se muestran facturas pendientes de pago que venzan dentro del plazo seleccionado.
                            Seleccione las facturas que desea incluir en esta propuesta.
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Actions -->
        <div class="actions-container">
            <asp:Button ID="btnVolver" runat="server"
                Text="Volver"
                CssClass="btn btn-secondary"
                OnClick="btnVolver_Click"
                CausesValidation="false" />
            <asp:Button ID="btnContinuar" runat="server" 
                Text="Continuar" 
                CssClass="btn btn-primary"
                OnClick="btnContinuar_Click" />
        </div>
    </div>

    <script type="text/javascript">
        function seleccionarTodos(checkbox) {
            var checkboxes = document.querySelectorAll('.factura-checkbox input[type="checkbox"]');
            checkboxes.forEach(function (cb) {
                cb.checked = checkbox.checked;
            });
        }
    </script>
</asp:Content>