<%@ Page Title="Agregar Detalle a Propuesta" Language="C#" MasterPageFile="~/SoftPac.Master"
    AutoEventWireup="true" CodeBehind="AgregarDetallePropuesta.aspx.cs"
    Inherits="SoftPacWA.AgregarDetallePropuesta" %>

<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server">
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
            color: var(--color-primary);
            font-size: 1.8rem;
            font-weight: 600;
            margin: 0;
        }

        .section-card {
            background: white;
            border-radius: 12px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
        }

        .section-title {
            color: var(--color-primary);
            font-size: 1.2rem;
            font-weight: 600;
            margin-bottom: 1.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1rem;
            margin-bottom: 1.5rem;
        }

        .info-item {
            display: flex;
            flex-direction: column;
            padding: 1rem;
            background-color: #f8f9fa;
            border-radius: 8px;
        }

        .info-label {
            font-size: 0.85rem;
            color: var(--color-secondary);
            font-weight: 500;
            margin-bottom: 0.25rem;
        }

        .info-value {
            font-weight: 600;
            color: var(--color-primary);
            font-size: 1rem;
        }

        .search-section {
            background: linear-gradient(135deg, #f8f9fa, #ffffff);
            border: 2px dashed var(--color-light-1);
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 2rem;
        }

        .search-row {
            display: flex;
            gap: 1rem;
            align-items: end;
            flex-wrap: wrap;
        }

        .search-field {
            flex: 1;
            min-width: 200px;
        }

        .detalle-preview {
            background: linear-gradient(135deg, rgba(96, 116, 138, 0.05), rgba(11, 31, 52, 0.03));
            border-left: 4px solid var(--color-secondary);
            border-radius: 8px;
            padding: 1.5rem;
            margin-bottom: 2rem;
        }

        .detalle-preview-title {
            color: var(--color-primary);
            font-weight: 600;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .cuenta-card {
            background: white;
            border: 2px solid #e0e0e0;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 1rem;
            transition: all 0.3s ease;
            cursor: pointer;
            position: relative;
        }

        .cuenta-card:hover {
            border-color: var(--color-secondary);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            transform: translateY(-2px);
        }

        .cuenta-card.selected {
            border-color: var(--color-secondary);
            background: linear-gradient(135deg, rgba(96, 116, 138, 0.08), rgba(11, 31, 52, 0.05));
            box-shadow: 0 0 0 3px rgba(96, 116, 138, 0.2);
        }

        .cuenta-header {
            display: flex;
            align-items: flex-start;
            gap: 1rem;
            margin-bottom: 1rem;
        }

        .cuenta-radio {
            margin-top: 0.25rem;
        }

        .cuenta-radio input[type="radio"] {
            width: 20px;
            height: 20px;
            cursor: pointer;
            accent-color: var(--color-secondary);
        }

        .cuenta-info {
            flex: 1;
        }

        .cuenta-numero {
            color: var(--color-primary);
            font-weight: 600;
            font-size: 1.1rem;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .cuenta-tipo {
            color: var(--color-secondary);
            font-size: 0.9rem;
        }

        .cuenta-saldo {
            text-align: right;
        }

        .saldo-label {
            color: var(--color-secondary);
            font-size: 0.8rem;
            font-weight: 500;
        }

        .saldo-amount {
            color: #198754;
            font-weight: 700;
            font-size: 1.4rem;
        }

        .cuenta-details {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            padding-top: 1rem;
            border-top: 1px solid #e0e0e0;
            margin-top: 1rem;
        }

        .detail-item {
            display: flex;
            flex-direction: column;
        }

        .detail-label {
            color: var(--color-secondary);
            font-size: 0.8rem;
            font-weight: 500;
            margin-bottom: 0.25rem;
        }

        .detail-value {
            color: var(--color-primary);
            font-weight: 600;
        }

        .forma-pago-section {
            background: #f8f9fa;
            border-radius: 8px;
            padding: 1.5rem;
            margin: 1.5rem 0;
        }

        .forma-pago-label {
            color: var(--color-primary);
            font-weight: 600;
            margin-bottom: 1rem;
            display: block;
        }

        .action-buttons {
            display: flex;
            gap: 1rem;
            justify-content: center;
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
            .page-header {
                flex-direction: column;
                gap: 1rem;
                align-items: flex-start;
            }

            .search-row {
                flex-direction: column;
            }

            .search-field {
                width: 100%;
            }

            .cuenta-header {
                flex-direction: column;
            }

            .cuenta-saldo {
                text-align: left;
            }

            .action-buttons {
                flex-direction: column;
            }

            .action-buttons .btn {
                width: 100%;
            }
        }

        .btn-nv-dt{
            font-size: 1rem;
        }

        .btn-accionfinal{
            font-size: 1rem;
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
            <h2 class="page-title">
                <i class="fas fa-plus-circle"></i>
                Agregar Detalle a Propuesta #<asp:Label ID="lblPropuestaId" runat="server" />
            </h2>
            
            <div class="actions-buttons">
                <asp:LinkButton ID="LbVolver" runat="server" 
                    CssClass="btn btn-secondary"
                    OnClick="btnVolver_Click">
                    <i class="fas fa-arrow-left"></i> Volver
                </asp:LinkButton>
            </div>
        </div>
        <!-- Información de la Propuesta -->
        <div class="section-card">
            <div class="section-title">
                <i class="fas fa-info-circle"></i>
                Información de la Propuesta
            </div>

            <div class="info-grid">
                <div class="info-item">
                    <span class="info-label">Banco / Entidad Bancaria</span>
                    <span class="info-value">
                        <asp:Label ID="lblBanco" runat="server" />
                    </span>
                </div>
                <div class="info-item">
                    <span class="info-label">País</span>
                    <span class="info-value">
                        <asp:Label ID="lblPais" runat="server" />
                    </span>
                </div>
            </div>
        </div>

        <!-- Búsqueda de Facturas -->
        <div class="section-card">
            <div class="section-title">
                <i class="fas fa-search"></i>
                Buscar Facturas Pendientes
            </div>

            <div class="search-section">
                <div class="search-row">
                    <div class="search-field">
                        <label class="form-label">Intervalo de Vencimiento</label>
                        <asp:DropDownList ID="ddlIntervaloDias" runat="server" CssClass="form-select">
                            <asp:ListItem Value="7">Próximos 7 días</asp:ListItem>
                            <asp:ListItem Value="15" Selected="True">Próximos 15 días</asp:ListItem>
                            <asp:ListItem Value="30">Próximos 30 días</asp:ListItem>
                            <asp:ListItem Value="60">Próximos 60 días</asp:ListItem>
                            <asp:ListItem Value="90">Próximos 90 días</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div>
                        <asp:LinkButton ID="btnBuscarFacturas" runat="server" 
                            CssClass="btn btn-primary btn-lg btn-nv-dt"
                            OnClick="btnBuscarFacturas_Click">
                            <i class="fas fa-search me-2"></i> Buscar Facturas
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="search-row mt-3">
                    <div class="search-field" style="flex: 3;">
                        <label class="form-label">Seleccionar Factura</label>
                        <asp:DropDownList ID="ddlFacturas" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Primero busque facturas --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div>
                        <asp:LinkButton ID="btnGenerarDetalle" runat="server" 
                            CssClass="btn btn-secondary btn-lg btn-nv-dt"
                            OnClick="btnGenerarDetalle_Click">
                            <i class="fas fa-check me-2"></i> Seleccionar
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <!-- Panel de Detalle Generado -->
        <asp:Panel ID="pnlDetalleGenerado" runat="server" Visible="false">
            
            <!-- Preview del Detalle -->
            <div class="section-card">
                <div class="section-title">
                    <i class="fas fa-file-invoice"></i>
                    Detalle del Pago
                </div>

                <div class="detalle-preview">
                    <div class="detalle-preview-title">
                        <i class="fas fa-receipt"></i>
                        Vista Previa
                    </div>
                    <div class="info-grid">
                        <div class="info-item">
                            <span class="info-label" style="font-size: 1.05rem;">Número de Factura</span>
                            <span class="info-value" style="font-size: 1.05rem;">
                                <asp:Label ID="lblFacturaGenerada" runat="server" />
                            </span>
                        </div>
                        <div class="info-item">
                            <span class="info-label" style="font-size: 1.05rem;">Acreedor</span>
                            <span class="info-value" style="font-size: 1.05rem;">
                                <asp:Label ID="lblAcreedorFactura" runat="server" />
                            </span>
                        </div>
                        <div class="info-item">
                            <span class="info-label" style="font-size: 1.05rem;">Moneda</span>
                            <span class="info-value" style="font-size: 1.05rem;">
                                <asp:Label ID="lblMonedaGenerada" runat="server" />
                            </span>
                        </div>
                        <div class="info-item">
                            <span class="info-label" style="font-size: 1.05rem;">Monto a Pagar</span>
                            <span class="info-value" style="color: #198754; font-size: 1.05rem;">
                                <asp:Label ID="lblMontoGenerado" runat="server" />
                            </span>
                        </div>
                    </div>
                </div>

                <!-- Selección de Cuenta Propia -->
                <div class="section-title mt-4">
                    <i class="fas fa-wallet"></i>
                    Seleccionar Cuenta de Origen
                </div>

                <asp:Repeater ID="rptCuentas" runat="server">
                    <ItemTemplate>
                        <div class="cuenta-card" onclick="toggleCuenta(event, 'rdo_<%# Eval("cuenta_bancaria_id") %>')">
                            <div class="cuenta-header">
                                <div class="cuenta-radio">
                                    <input type="radio"
                                           id='rdo_<%# Eval("cuenta_bancaria_id") %>'
                                           name="cuentaSeleccionada"
                                           value='<%# Eval("cuenta_bancaria_id") %>'
                                           onclick="event.stopPropagation(); seleccionarCard(this);" />
                                </div>

                                <div class="cuenta-info">
                                    <div class="cuenta-numero">
                                        <i class="fas fa-credit-card"></i>
                                        <%# Eval("numero_cuenta") %>
                                    </div>
                                    <div class="cuenta-tipo">
                                        <%# Eval("tipo_cuenta") %> • <%# Eval("entidad_bancaria.nombre") %>
                                    </div>
                                </div>

                                <div class="cuenta-saldo">
                                    <div class="saldo-label">Saldo Disponible</div>
                                    <div class="saldo-amount">
                                        <%# Eval("moneda.codigo_iso") %> <%# string.Format("{0:N2}", Eval("saldo_disponible")) %>
                                    </div>
                                </div>
                            </div>

                            <div class="cuenta-details">
                                <div class="detail-item">
                                    <span class="detail-label">CCI</span>
                                    <span class="detail-value"><%# Eval("cci") ?? "-" %></span>
                                </div>
                                <div class="detail-item">
                                    <span class="detail-label">Estado</span>
                                    <span class="detail-value">
                                        <i class="fas fa-check-circle text-success"></i> Activa
                                    </span>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- Forma de Pago -->
                <div class="forma-pago-section">
                    <label class="forma-pago-label">
                        <i class="fas fa-money-check-alt me-2"></i>
                        Forma de Pago
                    </label>
                    <asp:DropDownList ID="ddlFormaPago" runat="server" CssClass="form-select">
                        <asp:ListItem Value="T" Selected="True">Transferencia Bancaria</asp:ListItem>
                        <asp:ListItem Value="C">Cheque</asp:ListItem>
                        <asp:ListItem Value="E">Efectivo</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- Botones de Acción -->
                <div class="action-buttons">
                    <asp:LinkButton ID="btnConfirmar" runat="server" 
                        CssClass="btn btn-success btn-lg btn-accionfinal"
                        OnClick="btnConfirmar_Click">
                        <i class="fas fa-check-circle me-2"></i> Confirmar y Agregar Detalle
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnCancelar" runat="server" 
                        CssClass="btn btn-secondary btn-lg btn-accionfinal"
                        OnClick="btnCancelar_Click" 
                        CausesValidation="false">
                        <i class="fas fa-times me-2"></i> Cancelar
                    </asp:LinkButton>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        function toggleCuenta(e, inputId) {
            e = e || window.event;
            var input = document.getElementById(inputId);
            if (!input) return;

            input.checked = true;
            actualizarSeleccion();
        }

        function seleccionarCard(radioElem) {
            actualizarSeleccion();
        }

        function actualizarSeleccion() {
            document.querySelectorAll('.cuenta-card').forEach(function (card) {
                card.classList.remove('selected');
            });

            var radioSeleccionado = document.querySelector('input[name="cuentaSeleccionada"]:checked');
            if (radioSeleccionado) {
                var card = radioSeleccionado.closest('.cuenta-card');
                if (card) {
                    card.classList.add('selected');
                }
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            actualizarSeleccion();
        });
    </script>
</asp:Content>