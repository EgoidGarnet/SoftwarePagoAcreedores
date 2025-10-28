<%@ Page Title="Reporte de Facturas Pendientes" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="ReporteFacturas.aspx.cs" Inherits="SoftPacWA.ReporteFacturas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .reporte-container {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 2rem;
            margin-bottom: 2rem;
        }

        .reporte-header {
            text-align: center;
            border-bottom: 3px solid var(--color-primary);
            padding-bottom: 1.5rem;
            margin-bottom: 2rem;
        }

            .reporte-header h2 {
                color: var(--color-primary);
                font-size: 1.8rem;
                font-weight: 700;
                margin-bottom: 0.5rem;
            }

            .reporte-header .subtitle {
                color: var(--color-secondary);
                font-size: 1rem;
            }

        .auditoria-block {
            background-color: #f8f9fa;
            border-left: 4px solid var(--color-secondary);
            padding: 1rem 1.5rem;
            margin-bottom: 2rem;
            border-radius: 4px;
        }

            .auditoria-block h4 {
                color: var(--color-primary);
                font-size: 1rem;
                font-weight: 600;
                margin-bottom: 1rem;
            }

        .auditoria-item {
            display: flex;
            margin-bottom: 0.5rem;
        }

            .auditoria-item strong {
                color: var(--color-primary);
                min-width: 150px;
            }

            .auditoria-item span {
                color: var(--color-secondary);
            }

        .filtros-aplicados {
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 1rem 1.5rem;
            margin-bottom: 2rem;
            border-radius: 4px;
        }

            .filtros-aplicados h4 {
                color: #856404;
                font-size: 1rem;
                font-weight: 600;
                margin-bottom: 0.5rem;
            }

        .grupo-rango {
            margin-bottom: 2rem;
        }

        .grupo-header {
            background: linear-gradient(135deg, var(--color-primary), var(--color-secondary));
            color: white;
            padding: 0.75rem 1.5rem;
            border-radius: 6px;
            font-weight: 600;
            font-size: 1.1rem;
            margin-bottom: 1rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .grupo-total {
            background-color: #f8f9fa;
            padding: 0.75rem 1.5rem;
            border-radius: 4px;
            font-weight: 600;
            margin-top: 0.5rem;
            text-align: right;
            color: var(--color-primary);
        }

        .total-general {
            background: var(--color-primary);
            color: white;
            padding: 1rem 1.5rem;
            border-radius: 6px;
            font-size: 1.2rem;
            font-weight: 700;
            text-align: right;
            margin-top: 2rem;
        }

        .botones-accion {
            display: flex;
            gap: 1rem;
            justify-content: center;
            margin-top: 2rem;
            padding-top: 2rem;
            border-top: 2px solid var(--color-light-1);
        }

        .filter-section {
            background-color: #f8f9fa;
            padding: 1.5rem;
            border-radius: 8px;
            margin-bottom: 2rem;
            border: 1px solid var(--color-light-1);
        }

        @media (max-width: 768px) {
            .reporte-container {
                padding: 1rem;
            }

            .auditoria-item {
                flex-direction: column;
            }

                .auditoria-item strong {
                    min-width: auto;
                }

            .botones-accion {
                flex-direction: column;
            }

                .botones-accion .btn {
                    width: 100%;
                }
        }

        @media print {
            .filter-section,
            .botones-accion,
            .top-navbar,
            .sidebar {
                display: none !important;
            }

            .reporte-container {
                box-shadow: none;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title mb-4">
        <h3>
            <i class="fas fa-file-invoice"></i>Reporte de Facturas Pendientes por Vencimiento
        </h3>
    </div>

    <!-- Sección de Filtros -->
    <div class="filter-section">
        <h5 class="mb-3"><i class="fas fa-filter"></i>Filtros del Reporte</h5>
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">Acreedor</label>
                <asp:DropDownList ID="ddlAcreedor" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">Todos</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">País</label>
                <asp:DropDownList ID="ddlPais" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">Todos</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Moneda</label>
                <asp:DropDownList ID="ddlMoneda" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">Todas</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Rango de Vencimiento</label>
                <asp:DropDownList ID="ddlRango" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">Todos</asp:ListItem>
                    <asp:ListItem Value="0-30">0-30 días</asp:ListItem>
                    <asp:ListItem Value="31-60">31-60 días</asp:ListItem>
                    <asp:ListItem Value="61-90">61-90 días</asp:ListItem>
                    <asp:ListItem Value="90+">+90 días</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-12">
                <asp:Button ID="btnAplicarFiltros" runat="server" Text="Aplicar Filtros / Vista Previa"
                    CssClass="btn btn-primary me-2" OnClick="btnAplicarFiltros_Click" />
                <asp:Button ID="btnLimpiarFiltros" runat="server" Text="Limpiar Filtros"
                    CssClass="btn btn-secondary" OnClick="btnLimpiarFiltros_Click" />
            </div>
        </div>
    </div>

    <!-- Reporte -->
    <div class="reporte-container">
        <!-- Encabezado del Reporte -->
        <div class="reporte-header">
            <h2><i class="fas fa-file-invoice-dollar"></i>REPORTE DE FACTURAS PENDIENTES</h2>
            <p class="subtitle">Agrupadas por Rangos de Vencimiento</p>
        </div>

        <!-- Bloque de Auditoría -->
        <div class="auditoria-block">
            <h4><i class="fas fa-info-circle"></i>Información del Reporte</h4>
            <div class="auditoria-item">
                <strong>Fecha de Generación:</strong>
                <span>
                    <asp:Label ID="lblFechaGeneracion" runat="server"></asp:Label></span>
            </div>
            <div class="auditoria-item">
                <strong>Hora de Generación:</strong>
                <span>
                    <asp:Label ID="lblHoraGeneracion" runat="server"></asp:Label></span>
            </div>
            <div class="auditoria-item">
                <strong>Usuario:</strong>
                <span>
                    <asp:Label ID="lblUsuario" runat="server"></asp:Label></span>
            </div>
        </div>

        <!-- Filtros Aplicados -->
        <asp:Panel ID="pnlFiltrosAplicados" runat="server" CssClass="filtros-aplicados" Visible="false">
            <h4><i class="fas fa-filter"></i>Filtros Aplicados</h4>
            <asp:Label ID="lblFiltrosAplicados" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Contenido del Reporte -->
        <asp:UpdatePanel ID="upReporte" runat="server">
            <ContentTemplate>
                <asp:Repeater ID="rptRangos" runat="server" OnItemDataBound="rptRangos_ItemDataBound">
                    <ItemTemplate>
                        <div class="grupo-rango">
                            <div class="grupo-header">
                                <span><%# Eval("Rango") %></span>
                                <span><%# Eval("CantidadFacturas") %> factura(s)</span>
                            </div>

                            <div class="table-responsive">
                                <asp:GridView ID="gvFacturas" runat="server"
                                    AutoGenerateColumns="false"
                                    CssClass="table table-hover table-sm"
                                    GridLines="None">
                                    <HeaderStyle CssClass="table-light" />
                                    <Columns>
                                        <asp:BoundField DataField="NumeroFactura" HeaderText="N° Factura" />
                                        <asp:BoundField DataField="Acreedor" HeaderText="Proveedor" />
                                        <asp:BoundField DataField="Pais" HeaderText="País" />
                                        <asp:TemplateField HeaderText="Fecha Vencimiento">
                                            <ItemTemplate>
                                                <%# Eval("FechaVencimiento", "{0:dd/MM/yyyy}") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Días Vencimiento">
                                            <ItemTemplate>
                                                <%# Eval("DiasVencimiento") %> días
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Moneda" HeaderText="Moneda" />
                                        <asp:TemplateField HeaderText="Monto">
                                            <ItemTemplate>
                                                <%# Eval("Monto", "{0:N2}") %>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Right" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>

                            <div class="grupo-total">
                                Subtotal:
                                <asp:Label ID="lblSubtotal" runat="server"></asp:Label>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="total-general">
                    TOTAL GENERAL:
                    <asp:Label ID="lblTotalGeneral" runat="server"></asp:Label>
                </div>

                <asp:Panel ID="pnlSinDatos" runat="server" Visible="false" CssClass="text-center py-5">
                    <i class="fas fa-inbox fa-4x text-muted mb-3"></i>
                    <h4 class="text-muted">No hay facturas que cumplan con los criterios seleccionados</h4>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <!-- Botones de Acción -->
        <div class="botones-accion">
            <asp:LinkButton ID="btnExportarPDF" runat="server"
                CssClass="btn btn-danger btn-lg d-flex align-items-center justify-content-center"
                OnClick="btnExportarPDF_Click"
                ToolTip="Exportar reporte en PDF">
        <i class="fas fa-file-pdf me-2"></i>
        <span>Exportar a PDF</span>
            </asp:LinkButton>

            <asp:LinkButton ID="btnVolver" runat="server"
                CssClass="btn btn-secondary btn-lg d-flex align-items-center justify-content-center"
                OnClick="btnVolver_Click"
                ToolTip="Volver a la vista de Facturas">
        <i class="fas fa-arrow-left me-2"></i>
        <span>Volver</span>
            </asp:LinkButton>
        </div>

    </div>
</asp:Content>
