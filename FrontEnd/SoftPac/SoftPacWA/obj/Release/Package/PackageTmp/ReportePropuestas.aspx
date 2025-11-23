<%@ Page Title="Reporte de Propuestas de Pago" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="ReportePropuestas.aspx.cs" Inherits="SoftPacWA.ReportePropuestas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .reporte-container {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 2rem;
            margin-bottom: 2rem;
        }

        .stat-value {
            white-space: pre-line;
            display: block;
            text-align: center;
            font-size: 1.5rem;
            font-weight: 700;
        }

        .auditoria-item {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            margin-bottom: 0.5rem;
        }

            .auditoria-item strong {
                color: var(--color-primary);
                min-width: 160px;
                font-weight: 600;
            }

            .auditoria-item span {
                color: var(--color-secondary);
                flex: 1;
                white-space: nowrap;
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

        .propuesta-card {
            border: 1px solid var(--color-light-1);
            border-radius: 8px;
            margin-bottom: 2rem;
            overflow: hidden;
        }

        .propuesta-header {
            background: linear-gradient(135deg, var(--color-primary), var(--color-secondary));
            color: white;
            padding: 1rem 1.5rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

            .propuesta-header h5 {
                margin: 0;
                font-weight: 600;
            }

        .propuesta-body {
            padding: 1.5rem;
        }

        .info-propuesta {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            margin-bottom: 1.5rem;
            padding: 1rem;
            background-color: #f8f9fa;
            border-radius: 6px;
        }

        .info-item {
            display: flex;
            flex-direction: column;
        }

            .info-item label {
                font-size: 0.85rem;
                color: var(--color-secondary);
                font-weight: 500;
                margin-bottom: 0.25rem;
            }

            .info-item span {
                font-weight: 600;
                color: var(--color-primary);
            }

        .totales-moneda {
            background-color: #e9ecef;
            padding: 1rem;
            border-radius: 6px;
            margin-top: 1rem;
        }

            .totales-moneda h6 {
                color: var(--color-primary);
                font-weight: 600;
                margin-bottom: 0.75rem;
            }

        .total-item {
            display: flex;
            justify-content: space-between;
            padding: 0.5rem;
            border-bottom: 1px solid #dee2e6;
        }

            .total-item:last-child {
                border-bottom: none;
                font-weight: 700;
                background-color: var(--color-primary);
                color: white;
                border-radius: 4px;
                margin-top: 0.5rem;
            }

        .resumen-general {
            background: var(--color-primary);
            color: white;
            padding: 1.5rem;
            border-radius: 8px;
            margin-top: 2rem;
        }

            .resumen-general h4 {
                margin-bottom: 1rem;
                font-weight: 700;
            }

        .resumen-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
        }

        .stat-item {
            text-align: center;
            padding: 1rem;
            background-color: rgba(255, 255, 255, 0.1);
            border-radius: 6px;
        }

        .stat-value {
            font-size: 2rem;
            font-weight: 700;
            display: block;
            margin-bottom: 0.5rem;
        }

        .stat-label {
            font-size: 0.9rem;
            opacity: 0.9;
        }

        .botones-accion {
            display: flex;
            gap: 1rem;
            justify-content: center;
            margin-top: 2rem;
            margin-bottom: 2rem;
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

            .propuesta-header {
                flex-direction: column;
                gap: 0.5rem;
                align-items: flex-start;
            }

            .info-propuesta {
                grid-template-columns: 1fr;
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

            .propuesta-card {
                page-break-inside: avoid;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title mb-4">
        <h3 class="pb-1">
            <i class="fas fa-money-check-alt me-2"></i>Reporte Detallado de Propuestas de Pago
        </h3>
    </div>

    <!-- Sección de Filtros -->
    <div class="filter-section">
        <h5 class="mb-3"><i class="fas fa-filter me-2"></i>Filtros del Reporte</h5>
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">País <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlPais" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPais_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Banco <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlBanco" runat="server" CssClass="form-select">
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Estado</label>
                <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">Todos</asp:ListItem>
                    <asp:ListItem Value="Pendiente">Pendiente</asp:ListItem>
                    <asp:ListItem Value="Enviada">Enviada</asp:ListItem>
                    <asp:ListItem Value="Anulada">Anulada</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Días desde hoy</label>
                <asp:TextBox ID="txtDiasDesde" runat="server" CssClass="form-control" 
                    placeholder="Por defecto: 90 días" TextMode="Number" min="1"></asp:TextBox>
                <small class="text-muted">Si está vacío, se usan 90 días</small>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-12">
                <small class="text-muted d-block mb-2">
                    <span class="text-danger">*</span> Debe seleccionar al menos País o Banco
                </small>
                <asp:Button ID="btnAplicarFiltros" runat="server" Text="Aplicar Filtros / Vista Previa"
                    CssClass="btn btn-primary me-2" OnClick="btnAplicarFiltros_Click" />
                <asp:Button ID="btnLimpiarFiltros" runat="server" Text="Limpiar Filtros"
                    CssClass="btn btn-secondary" OnClick="btnLimpiarFiltros_Click" />
            </div>
        </div>
    </div>

    <!-- Alerta de Validación -->
    <asp:Panel ID="pnlAlertaValidacion" runat="server" Visible="false" CssClass="alert alert-danger alert-dismissible fade show" role="alert">
        <i class="fas fa-exclamation-triangle me-2"></i>
        <asp:Label ID="lblMensajeValidacion" runat="server"></asp:Label>
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </asp:Panel>
    
    <!-- Botones de Acción -->
    <div class="botones-accion">
        <asp:LinkButton ID="btnExportarPDF" runat="server"
            CssClass="btn btn-danger btn-lg"
            OnClick="btnExportarPDF_Click"
            ToolTip="Exportar a PDF">
    <i class="fas fa-file-pdf me-2"></i> Exportar a PDF
        </asp:LinkButton>

        <asp:LinkButton ID="btnVolver" runat="server"
            CssClass="btn btn-secondary btn-lg"
            OnClick="btnVolver_Click"
            ToolTip="Volver a Propuestas">
    <i class="fas fa-arrow-left me-2"></i> Volver
        </asp:LinkButton>
    </div>

    <!-- Reporte -->
    <div class="reporte-container">
        <!-- Encabezado del Reporte -->
        <div class="reporte-header">
            <h2 class="pb-1"><i class="fas fa-file-invoice-dollar me-2"></i>REPORTE DE PROPUESTAS DE PAGO</h2>
            <p class="subtitle">Detalle de Pagos Individuales por Propuesta</p>
        </div>

        <!-- Bloque de Auditoría -->
        <div class="auditoria-block">
            <h4><i class="fas fa-info-circle me-2"></i>Información del Reporte</h4>
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
                <asp:Repeater ID="rptPropuestas" runat="server" OnItemDataBound="rptPropuestas_ItemDataBound">
                    <ItemTemplate>
                        <div class="propuesta-card">
                            <div class="propuesta-header">
                                <h5>
                                    <i class="fas fa-file-invoice"></i>
                                    Propuesta #<%# Eval("PropuestaId") %> - <%# Eval("EntidadBancaria") %>
                                </h5>
                                <span class="badge bg-light text-dark">
                                    <%# Eval("Estado") %>
                                </span>
                            </div>

                            <div class="propuesta-body">
                                <!-- Información de la Propuesta -->
                                <div class="info-propuesta">
                                    <div class="info-item">
                                        <label>Fecha Creación</label>
                                        <span><%# Eval("FechaCreacion", "{0:dd/MM/yyyy HH:mm}") %></span>
                                    </div>
                                    <div class="info-item">
                                        <label>Usuario Creador</label>
                                        <span><%# Eval("UsuarioCreador") %></span>
                                    </div>
                                    <div class="info-item">
                                        <label>País</label>
                                        <span><%# Eval("Pais") %></span>
                                    </div>
                                    <div class="info-item">
                                        <label>Total de Pagos</label>
                                        <span><%# Eval("TotalPagos") %></span>
                                    </div>
                                </div>

                                <!-- Detalle de Pagos -->
                                <h6 class="mb-3" style="color: var(--color-primary); font-weight: 600;">
                                    <i class="fas fa-list m-2"></i>Detalle de Pagos Individuales
                                </h6>

                                <div class="table-responsive">
                                    <asp:GridView ID="gvDetalles" runat="server"
                                        AutoGenerateColumns="false"
                                        CssClass="table table-hover table-sm table-bordered"
                                        GridLines="None">
                                        <HeaderStyle CssClass="table-light" />
                                        <Columns>
                                            <asp:BoundField DataField="NumeroFactura" HeaderText="N° Factura" />
                                            <asp:BoundField DataField="Acreedor" HeaderText="Proveedor" />
                                            <asp:BoundField DataField="Moneda" HeaderText="Mon." />
                                            <asp:TemplateField HeaderText="Monto">
                                                <ItemTemplate>
                                                    <%# Eval("Monto", "{0:N2}") %>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Right" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="CuentaOrigen" HeaderText="Cuenta Origen" />
                                            <asp:BoundField DataField="BancoOrigen" HeaderText="Banco Origen" />
                                            <asp:BoundField DataField="CuentaDestino" HeaderText="Cuenta Destino" />
                                            <asp:BoundField DataField="BancoDestino" HeaderText="Banco Destino" />
                                            <asp:BoundField DataField="FormaPago" HeaderText="Forma de Pago" />
                                        </Columns>
                                    </asp:GridView>
                                </div>

                                <!-- Totales por Moneda -->
                                <div class="totales-moneda">
                                    <h6><i class="fas fa-coins m-2"></i>Totales por Moneda</h6>
                                    <asp:Repeater ID="rptTotales" runat="server">
                                        <ItemTemplate>
                                            <div class="total-item">
                                                <span><%# Eval("Moneda") %></span>
                                                <strong><%# Eval("Total", "{0:N2}") %></strong>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- Resumen General -->
                <asp:Panel ID="pnlResumenGeneral" runat="server" CssClass="resumen-general" Visible="false">
                    <h4><i class="fas fa-chart-bar m-2"></i>Resumen General del Reporte</h4>
                    <div class="resumen-stats">
                        <div class="stat-item">
                            <span class="stat-value">
                                <asp:Label ID="lblTotalPropuestas" runat="server"></asp:Label></span>
                            <span class="stat-label">Propuestas</span>
                        </div>
                        <div class="stat-item">
                            <span class="stat-value">
                                <asp:Label ID="lblTotalPagos" runat="server"></asp:Label></span>
                            <span class="stat-label">Pagos Individuales</span>
                        </div>
                        <div class="stat-item">
                            <span class="stat-value">
                                <asp:Label ID="lblTotalMontos" runat="server"></asp:Label></span>
                            <span class="stat-label">Monto Total</span>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlSinDatos" runat="server" Visible="false" CssClass="text-center py-5">
                    <i class="fas fa-inbox fa-4x text-muted mb-3"></i>
                    <h4 class="text-muted">Seleccione filtros y haga clic en "Aplicar Filtros" para generar el reporte</h4>
                    <p class="text-muted">Debe seleccionar al menos un País o un Banco</p>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>