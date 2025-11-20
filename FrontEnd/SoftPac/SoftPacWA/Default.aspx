<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" 
    CodeBehind="Default.aspx.cs" Inherits="SoftPacWA.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- ApexCharts -->
    <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
    
    <style>
        :root {
            --gradient-primary: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            --gradient-success: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
            --gradient-warning: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            --gradient-info: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
        }

        .dashboard-hero {
            background: linear-gradient(135deg, #0B1F34 0%, #1a3a52 100%);
            color: white;
            padding: 2.5rem 2rem;
            border-radius: 20px;
            margin-bottom: 1.5rem;
            box-shadow: 0 10px 40px rgba(0,0,0,0.15);
            position: relative;
            overflow: hidden;
        }

        .dashboard-hero::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -20%;
            width: 400px;
            height: 400px;
            background: rgba(255,255,255,0.05);
            border-radius: 50%;
        }

        .dashboard-hero h1 {
            font-size: 2.2rem;
            font-weight: 700;
            margin: 0;
            position: relative;
            z-index: 1;
        }

        .dashboard-hero p {
            font-size: 1rem;
            margin: 0.75rem 0 0 0;
            opacity: 0.9;
            position: relative;
            z-index: 1;
        }

        .welcome-badge {
            background: rgba(255,255,255,0.2);
            padding: 0.4rem 1rem;
            border-radius: 20px;
            display: inline-block;
            margin-top: 0.75rem;
            backdrop-filter: blur(10px);
            font-size: 0.9rem;
        }

        .metric-card {
            background: white;
            border-radius: 16px;
            padding: 1.25rem;
            height: 100%;
            position: relative;
            overflow: hidden;
            transition: all 0.3s ease;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            border-left: 4px solid transparent;
        }

        .metric-card:hover {
            transform: translateY(-8px);
            box-shadow: 0 12px 28px rgba(0,0,0,0.15);
        }

        .metric-card.primary {
            border-left-color: #667eea;
        }

        .metric-card.success {
            border-left-color: #11998e;
        }

        .metric-card.warning {
            border-left-color: #f5576c;
        }

        .metric-card.info {
            border-left-color: #4facfe;
        }

        .metric-icon {
            width: 56px;
            height: 56px;
            border-radius: 14px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.75rem;
            color: white;
            margin-bottom: 0.75rem;
        }

        .metric-card.primary .metric-icon {
            background: var(--gradient-primary);
        }

        .metric-card.success .metric-icon {
            background: var(--gradient-success);
        }

        .metric-card.warning .metric-icon {
            background: var(--gradient-warning);
        }

        .metric-card.info .metric-icon {
            background: var(--gradient-info);
        }

        .metric-value {
            font-size: 2.5rem;
            font-weight: 700;
            color: #2d3748;
            margin: 0.25rem 0;
            line-height: 1;
        }

        .metric-label {
            font-size: 0.9rem;
            color: #718096;
            font-weight: 500;
            text-transform: capitalize;  
            letter-spacing: 0.5px;
        }

        .metric-change {
            font-size: 0.8rem;
            margin-top: 0.5rem;
            padding: 0.2rem 0.6rem;
            border-radius: 12px;
            display: inline-block;
        }

        .metric-change.positive {
            background: rgba(72, 187, 120, 0.1);
            color: #48bb78;
        }

        .metric-change.negative {
            background: rgba(245, 101, 101, 0.1);
            color: #f56565;
        }

        .chart-card {
            background: white;
            border-radius: 16px;
            padding: 1.25rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            margin-bottom: 1.5rem;
            height: auto;
            min-height: 400px;
        }

        .chart-card.compact {
            padding: 1rem;
        }

        .chart-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .chart-title i {
            color: #667eea;
            font-size: 1.2rem;
        }

        .stats-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
            gap: 1.25rem;
            margin-bottom: 1.5rem;
        }

        .quick-actions {
            background: white;
            border-radius: 16px;
            padding: 1.25rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            margin-bottom: 1.5rem;
        }

        .quick-action-btn {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            padding: 0.75rem 1rem;
            border-radius: 12px;
            border: 2px solid #e2e8f0;
            background: white;
            transition: all 0.3s ease;
            text-decoration: none;
            color: #2d3748;
            margin-bottom: 0.5rem;
            cursor: pointer;
        }

        .quick-action-btn:hover {
            border-color: #667eea;
            background: #f7fafc;
            transform: translateX(8px);
            text-decoration: none;
            color: #667eea;
        }

        .quick-action-icon {
            width: 42px;
            height: 42px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.25rem;
            background: var(--gradient-primary);
            color: white;
            flex-shrink: 0;
        }

        .refresh-section {
            background: white;
            padding: 0.85rem 1.25rem;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.06);
            margin-bottom: 1.5rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 1rem;
        }

        .btn-refresh, .btn-pdf {
            border: none;
            color: white;
            padding: 0.6rem 1.25rem;
            border-radius: 10px;
            font-weight: 600;
            transition: all 0.3s ease;
            font-size: 0.9rem;
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
            text-decoration: none;
            cursor: pointer;
        }

        .btn-refresh {
            background: var(--gradient-primary);
            box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
        }

        .btn-refresh:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
            color: white;
        }

        .btn-pdf {
            background: var(--gradient-info);
            box-shadow: 0 4px 12px rgba(79, 172, 254, 0.3);
        }

        .btn-pdf:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(79, 172, 254, 0.4);
            color: white;
            text-decoration: none;
        }

        .action-buttons {
            display: flex;
            gap: 0.75rem;
            flex-wrap: wrap;
        }

        .info-card {
            background: white;
            border-radius: 14px;
            padding: 1rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.06);
            margin-bottom: 1rem;
        }

        .info-card-title {
            font-size: 0.95rem;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 0.75rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .info-card-title i {
            color: #667eea;
            font-size: 1rem;
        }

        .info-card-content {
            color: #4a5568;
            line-height: 1.6;
            font-size: 0.9rem;
        }

        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .animate-card {
            animation: fadeInUp 0.6s ease-out;
        }

        /* Optimización de espacios en gráficos */
        #chartTendencias, #chartBancos {
            min-height: 320px !important;
            width: 100% !important;
            position: relative !important;
        }

        #chartPropuestas {
            min-height: 260px !important;
            width: 100% !important;
            position: relative !important;
        }

        .apexcharts-toolbar {
            top: -5px !important;
        }

        .chart-card {
            position: relative;
            overflow: visible !important;
            clear: both;
        }

        .info-card {
            position: relative;
            clear: both;
        }

        .row.g-3 {
            display: flex;
            flex-wrap: wrap;
        }

        /* En pantallas XL (≥1400px): Layout 70/30 */
        @media (min-width: 1400px) {
            .row.g-3 > .col-12.col-xl-8 {
                flex: 0 0 66.666667%;
                max-width: 66.666667%;
            }
    
            .row.g-3 > .col-12.col-xl-4 {
                flex: 0 0 33.333333%;
                max-width: 33.333333%;
            }
        }

        /* En pantallas menores a XL: Apilar verticalmente */
        @media (max-width: 1399px) {
            .row.g-3 > div[class*="col-"] {
                flex: 0 0 100% !important;
                max-width: 100% !important;
                margin-bottom: 2rem !important;
            }
    
            .chart-card {
                margin-bottom: 2rem !important;
                min-height: auto !important;
                height: auto !important;
            }
    
            .info-card {
                margin-bottom: 2rem !important;
            }
    
            /* Asegurar que cada gráfico tenga su espacio */
            .chart-card:nth-child(1) {
                margin-bottom: 2rem !important;
            }
    
            .chart-card:nth-child(2) {
                margin-top: 2rem !important;
            }
        }

        /* Fix para móviles */
        @media (max-width: 768px) {
            .dashboard-hero h1 {
                font-size: 1.6rem;
            }
    
            .metric-value {
                font-size: 2rem;
            }
    
            .stats-row {
                grid-template-columns: 1fr;
            }

            .refresh-section {
                flex-direction: column;
                align-items: stretch;
            }

            .action-buttons {
                width: 100%;
            }

            .btn-refresh, .btn-pdf {
                flex: 1;
                justify-content: center;
            }
    
            /* Mayor espaciado en móviles */
            .chart-card {
                margin-bottom: 3rem !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="upDashboard" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <!-- Hero Section -->
            <div class="dashboard-hero animate-card">
                <h1>
                    <i class="fas fa-home me-3"></i>
                    ¡Bienvenido, <asp:Literal ID="litNombreUsuario" runat="server" />!
                </h1>
                <p>Panel de Control del Sistema de Pago a Acreedores</p>
                <div class="welcome-badge">
                    <i class="fas fa-check-circle me-2"></i>
                    Sistema Activo
                </div>
            </div>

            <!-- Refresh Section con ambos botones -->
            <div class="refresh-section animate-card" style="animation-delay: 0.1s;">
                <div>
                    <small class="text-muted">
                        <i class="far fa-clock me-1"></i>
                        Última actualización: <asp:Label ID="lblUltimaActualizacion" runat="server"></asp:Label>
                    </small>
                </div>
                <div class="action-buttons">
                    <asp:Button ID="btnRefrescar" runat="server" OnClick="btnRefrescar_Click"
                        CssClass="btn-refresh" Text="🔄 Actualizar Dashboard" />
                    
                    <asp:LinkButton ID="btnGenerarReportePDF" runat="server" OnClick="btnGenerarReportePDF_Click"
                        CssClass="btn-pdf">
                        <i class="fas fa-file-pdf"></i>
                        <span>Generar Reporte PDF</span>
                    </asp:LinkButton>
                </div>
            </div>

            <!-- Tarjetas de Métricas -->
            <div class="stats-row">
                <div class="metric-card primary animate-card" style="animation-delay: 0.2s;">
                    <div class="metric-icon">
                        <i class="fas fa-file-invoice"></i>
                    </div>
                    <div class="metric-value">
                        <asp:Label ID="lblTotalFacturas" runat="server" Text="0"></asp:Label>
                    </div>
                    <div class="metric-label">Total Facturas</div>
                    <div class="metric-change positive">
                        <i class="fas fa-arrow-up me-1"></i>
                        <asp:Label ID="lblCambioFacturas" runat="server" Text="+0%"></asp:Label> este mes
                    </div>
                </div>

                <div class="metric-card warning animate-card" style="animation-delay: 0.3s;">
                    <div class="metric-icon">
                        <i class="fas fa-exclamation-circle"></i>
                    </div>
                    <div class="metric-value">
                        <asp:Label ID="lblFacturasPendientes" runat="server" Text="0"></asp:Label>
                    </div>
                    <div class="metric-label">Facturas Pendientes</div>
                    <div class="metric-change negative">
                        <i class="fas fa-clock me-1"></i>
                        Requiere atención
                    </div>
                </div>

                <div class="metric-card info animate-card" style="animation-delay: 0.4s;">
                    <div class="metric-icon">
                        <i class="fas fa-wallet"></i>
                    </div>
                    <div class="metric-value">
                        <asp:Label ID="lblTotalCuentas" runat="server" Text="0"></asp:Label>
                    </div>
                    <div class="metric-label">Cuentas Bancarias</div>
                    <div class="metric-change positive">
                        <i class="fas fa-check-circle me-1"></i>
                        <asp:Label ID="lblCuentasActivas" runat="server" Text="0"></asp:Label> activas
                    </div>
                </div>

                <div class="metric-card success animate-card" style="animation-delay: 0.5s;">
                    <div class="metric-icon">
                        <i class="fas fa-file-contract"></i>
                    </div>
                    <div class="metric-value">
                        <asp:Label ID="lblTotalPropuestas" runat="server" Text="0"></asp:Label>
                    </div>
                    <div class="metric-label">Propuestas Generadas</div>
                    <div class="metric-change positive">
                        <i class="fas fa-arrow-up me-1"></i>
                        Este mes
                    </div>
                </div>
            </div>

            <!-- Contenido Principal -->
            <div class="row g-3">
                <!-- Columna Izquierda - Gráficos Principales -->
                <div class="col-12 col-xl-8">
                    <!-- Tendencia de Facturas -->
                    <div class="chart-card animate-card" style="animation-delay: 0.6s;">
                        <div class="chart-title">
                            <i class="fas fa-chart-line"></i>
                            Facturas Registradas (Últimos 6 Meses)
                        </div>
                        <div id="chartTendencias"></div>
                    </div>

                    <!-- Distribución por Banco -->
                    <div class="chart-card animate-card" style="animation-delay: 0.7s;">
                        <div class="chart-title">
                            <i class="fas fa-chart-bar"></i>
                            Distribución de Propuestas de Pago por Banco
                        </div>
                        <div id="chartBancos"></div>
                    </div>
                </div>

                <!-- Columna Derecha - Panel Lateral -->
                <div class="col-12 col-xl-4">
                    <!-- Resumen Rápido -->
                    <div class="info-card animate-card" style="animation-delay: 0.8s;">
                        <div class="info-card-title">
                            <i class="fas fa-info-circle"></i>
                            Resumen Rápido
                        </div>
                        <div class="info-card-content">
                            <div class="d-flex justify-content-between mb-2">
                                <span>Facturas Pagadas:</span>
                                <strong><asp:Label ID="lblFacturasPagadas" runat="server" Text="0"></asp:Label></strong>
                            </div>
                            <div class="d-flex justify-content-between mb-2">
                                <span>Facturas Vencidas:</span>
                                <strong style="color: #f56565;">
                                    <asp:Label ID="lblFacturasVencidas" runat="server" Text="0"></asp:Label>
                                </strong>
                            </div>
                            <div class="d-flex justify-content-between">
                                <span>Propuestas Este Mes:</span>
                                <strong><asp:Label ID="lblPropuestasEsteMes" runat="server" Text="0"></asp:Label></strong>
                            </div>
                        </div>
                    </div>

                    <!-- Gráfico de Propuestas (más compacto) -->
                    <div class="chart-card compact animate-card" style="animation-delay: 0.9s;">
                        <div class="chart-title">
                            <i class="fas fa-chart-pie"></i>
                            Propuestas por Estado
                        </div>
                        <div id="chartPropuestas"></div>
                    </div>
                </div>
            </div>

            <!-- Hidden Fields para JavaScript -->
            <asp:HiddenField ID="hfMesesLabels" runat="server" />
            <asp:HiddenField ID="hfMesesData" runat="server" />
            <asp:HiddenField ID="hfBancosLabels" runat="server" />
            <asp:HiddenField ID="hfBancosData" runat="server" />
            <asp:HiddenField ID="hfPropuestasLabels" runat="server" />
            <asp:HiddenField ID="hfPropuestasData" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Scripts ApexCharts -->
    <script type="text/javascript">
        var chartTendencias, chartBancos, chartPropuestas;

        function inicializarGraficos() {
            try {
                var mesesLabels = JSON.parse(document.getElementById('<%= hfMesesLabels.ClientID %>').value || '[]');
                var mesesData = JSON.parse(document.getElementById('<%= hfMesesData.ClientID %>').value || '[]');
                var bancosLabels = JSON.parse(document.getElementById('<%= hfBancosLabels.ClientID %>').value || '[]');
                var bancosData = JSON.parse(document.getElementById('<%= hfBancosData.ClientID %>').value || '[]');
                var propuestasLabels = JSON.parse(document.getElementById('<%= hfPropuestasLabels.ClientID %>').value || '[]');
                var propuestasData = JSON.parse(document.getElementById('<%= hfPropuestasData.ClientID %>').value || '[]');

                // Destruir gráficos existentes
                if (chartTendencias) chartTendencias.destroy();
                if (chartBancos) chartBancos.destroy();
                if (chartPropuestas) chartPropuestas.destroy();

                // Gráfico de Tendencias
                var optionsTendencias = {
                    series: [{
                        name: 'Facturas',
                        data: mesesData
                    }],
                    chart: {
                        type: 'area',
                        height: 320,
                        toolbar: {
                            show: true,
                            tools: {
                                download: true,
                                selection: false,
                                zoom: false,
                                zoomin: false,
                                zoomout: false,
                                pan: false,
                                reset: false
                            }
                        },
                        animations: {
                            enabled: true,
                            speed: 800
                        }
                    },
                    dataLabels: {
                        enabled: false
                    },
                    stroke: {
                        curve: 'smooth',
                        width: 3
                    },
                    fill: {
                        type: 'gradient',
                        gradient: {
                            shadeIntensity: 1,
                            opacityFrom: 0.7,
                            opacityTo: 0.2
                        }
                    },
                    colors: ['#667eea'],
                    xaxis: {
                        categories: mesesLabels,
                        labels: {
                            style: {
                                fontSize: '11px'
                            }
                        }
                    },
                    yaxis: {
                        labels: {
                            style: {
                                fontSize: '11px'
                            }
                        }
                    },
                    tooltip: {
                        theme: 'dark'
                    },
                    grid: {
                        padding: {
                            top: 0,
                            right: 0,
                            bottom: 0,
                            left: 0
                        }
                    }
                };
                chartTendencias = new ApexCharts(document.querySelector("#chartTendencias"), optionsTendencias);
                chartTendencias.render();

                // Gráfico de Bancos
                var optionsBancos = {
                    series: [{
                        name: 'Cuentas',
                        data: bancosData
                    }],
                    chart: {
                        type: 'bar',
                        height: 320,
                        toolbar: {
                            show: true,
                            tools: {
                                download: true,
                                selection: false,
                                zoom: false,
                                zoomin: false,
                                zoomout: false,
                                pan: false,
                                reset: false
                            }
                        },
                        animations: {
                            enabled: true,
                            speed: 800
                        }
                    },
                    plotOptions: {
                        bar: {
                            borderRadius: 8,
                            horizontal: true,
                            dataLabels: {
                                position: 'top'
                            }
                        }
                    },
                    dataLabels: {
                        enabled: true,
                        offsetX: -20,
                        style: {
                            fontSize: '11px',
                            colors: ['#fff']
                        }
                    },
                    fill: {
                        type: 'gradient',
                        gradient: {
                            shade: 'dark',
                            type: 'horizontal',
                            shadeIntensity: 0.5,
                            gradientToColors: ['#38ef7d'],
                            opacityFrom: 0.85,
                            opacityTo: 0.85
                        }
                    },
                    colors: ['#11998e'],
                    xaxis: {
                        categories: bancosLabels,
                        labels: {
                            style: {
                                fontSize: '11px'
                            }
                        }
                    },
                    yaxis: {
                        labels: {
                            style: {
                                fontSize: '11px'
                            }
                        }
                    },
                    grid: {
                        padding: {
                            top: 0,
                            right: 0,
                            bottom: 0,
                            left: 0
                        }
                    }
                };
                chartBancos = new ApexCharts(document.querySelector("#chartBancos"), optionsBancos);
                chartBancos.render();

                // Gráfico de Propuestas
                var optionsPropuestas = {
                    series: propuestasData,
                    chart: {
                        type: 'donut',
                        height: 260,
                        toolbar: {
                            show: false
                        },
                        animations: {
                            enabled: true,
                            speed: 800
                        }
                    },
                    labels: propuestasLabels,
                    colors: ['#ffc107', '#28a745', '#dc3545'],
                    legend: {
                        position: 'bottom',
                        fontSize: '11px',
                        offsetY: 0
                    },
                    plotOptions: {
                        pie: {
                            donut: {
                                size: '65%',
                                labels: {
                                    show: true,
                                    total: {
                                        show: true,
                                        label: 'Total',
                                        fontSize: '14px',
                                        fontWeight: 600
                                    }
                                }
                            }
                        }
                    },
                    dataLabels: {
                        enabled: true,
                        style: {
                            fontSize: '11px'
                        }
                    },
                    tooltip: {
                        theme: 'dark'
                    }
                };
                chartPropuestas = new ApexCharts(document.querySelector("#chartPropuestas"), optionsPropuestas);
                chartPropuestas.render();

            } catch (error) {
                console.error('Error al inicializar gráficos:', error);
            }
        }

        $(document).ready(function () {
            inicializarGraficos();
        });

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            inicializarGraficos();
        });
    </script>
</asp:Content>