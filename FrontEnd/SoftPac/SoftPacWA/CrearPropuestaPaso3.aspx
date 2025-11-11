<%@ Page Title="Nueva Propuesta - Paso 3" Language="C#" MasterPageFile="~/SoftPac.Master" 
    AutoEventWireup="true" CodeBehind="CrearPropuestaPaso3.aspx.cs" 
    Inherits="SoftPacWA.CrearPropuestaPaso3" %>

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

        .saldo-requerido-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 1.5rem;
            margin-bottom: 2rem;
            border-left: 4px solid #198754;
        }

        .saldo-title {
            color: var(--color-primary);
            font-size: 1.1rem;
            font-weight: 600;
            margin-bottom: 1rem;
        }

        .moneda-requirement {
            background-color: #f8f9fa;
            border-left: 4px solid #0d6efd;
            padding: 1rem;
            margin-bottom: 0.75rem;
            border-radius: 4px;
        }

        .moneda-name {
            color: var(--color-primary);
            font-weight: 600;
            font-size: 0.95rem;
            margin-bottom: 0.5rem;
        }

        .moneda-details {
            display: flex;
            gap: 2rem;
            font-size: 0.875rem;
        }

        .detail-item {
            display: flex;
            flex-direction: column;
        }

        .detail-label {
            color: var(--color-secondary);
            font-size: 0.8rem;
        }

        .detail-value {
            color: var(--color-primary);
            font-weight: 600;
        }

        .detail-value.success {
            color: #198754;
        }

        .detail-value.danger {
            color: #dc3545;
        }

        .cuentas-section {
            margin-bottom: 2rem;
        }

        .moneda-header {
            background: linear-gradient(135deg, var(--color-secondary), var(--color-primary));
            color: white;
            padding: 1rem 1.5rem;
            border-radius: 8px;
            margin-bottom: 1rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .moneda-header-title {
            font-size: 1.1rem;
            font-weight: 600;
        }

        .moneda-header-info {
            font-size: 0.9rem;
            opacity: 0.95;
        }

        .cuenta-card {
            background-color: white;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            padding: 1.25rem;
            margin-bottom: 1rem;
            transition: all 0.3s ease;
            cursor: pointer;
        }

        .cuenta-card:hover {
            border-color: var(--color-secondary);
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }

        .cuenta-card.selected {
            border-color: var(--color-secondary);
            background-color: rgba(96, 116, 138, 0.05);
        }

        .cuenta-header {
            display: flex;
            align-items: flex-start;
            gap: 1rem;
            margin-bottom: 0.75rem;
        }

        .cuenta-checkbox input[type="checkbox"] {
            width: 20px;
            height: 20px;
            cursor: pointer;
        }

        .cuenta-info {
            flex: 1;
        }

        .cuenta-numero {
            color: var(--color-primary);
            font-weight: 600;
            font-size: 1rem;
            margin-bottom: 0.25rem;
        }

        .cuenta-tipo {
            color: var(--color-secondary);
            font-size: 0.875rem;
        }

        .cuenta-saldo {
            text-align: right;
        }

        .saldo-label {
            color: var(--color-secondary);
            font-size: 0.75rem;
        }

        .saldo-amount {
            color: #198754;
            font-weight: 700;
            font-size: 1.25rem;
        }

        .cuenta-details {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 0.75rem;
            padding-top: 0.75rem;
            border-top: 1px solid #e0e0e0;
            font-size: 0.875rem;
        }

        .detail-row {
            display: flex;
            flex-direction: column;
        }

        .detail-row-label {
            color: var(--color-secondary);
            font-size: 0.8rem;
        }

        .detail-row-value {
            color: var(--color-primary);
            font-weight: 500;
        }

        .actions-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 2rem;
            padding-top: 2rem;
            border-top: 2px solid var(--color-light-1);
        }

        .alert-no-cuentas {
            text-align: center;
            padding: 3rem;
            color: var(--color-secondary);
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
        }

        .alert-no-cuentas i {
            font-size: 3rem;
            opacity: 0.5;
            margin-bottom: 1rem;
        }

        @media (max-width: 768px) {
            .criterios-grid {
                grid-template-columns: 1fr;
            }

            .moneda-details {
                flex-direction: column;
                gap: 0.5rem;
            }

            .cuenta-header {
                flex-wrap: wrap;
            }

            .cuenta-saldo {
                text-align: left;
                width: 100%;
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
            <p>Seleccione las cuentas bancarias para realizar los pagos</p>
        </div>

        <!-- Step Indicator -->
        <div class="step-indicator">
            <i class="fas fa-university"></i>
            Paso 3 de 4: Selección de Cuentas Propias
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
                    <span class="criterio-label">Facturas Seleccionadas:</span>
                    <asp:Label ID="lblCantidadFacturas" runat="server" CssClass="criterio-value"></asp:Label>
                </div>
            </div>
        </div>

        <!-- Saldo Requerido -->
        <div class="saldo-requerido-card">
            <div class="saldo-title">
                <i class="fas fa-balance-scale"></i>
                Saldo Requerido por Moneda
            </div>
            <asp:Repeater ID="rptSaldosRequeridos" runat="server">
                <ItemTemplate>
                    <div class="moneda-requirement">
                        <div class="moneda-name">
                            <%# Eval("Moneda.Nombre") %> (<%# Eval("Moneda.CodigoIso") %>)
                        </div>
                        <div class="moneda-details">
                            <div class="detail-item">
                                <span class="detail-label">Requerido (con 1% margen)</span>
                                <span class="detail-value">
                                    <%# string.Format("{0:N2}", Eval("MontoRequerido")) %>
                                </span>
                            </div>
                            <div class="detail-item">
                                <span class="detail-label">Seleccionado</span>
                                <span class="detail-value danger" id='status_<%# Eval("Moneda.CodigoIso") %>'>
                                    <span id='lblSaldo_<%# Eval("Moneda.CodigoIso") %>'>0.00</span>
                                </span>
                            </div>
                        </div>
                    </div>
                    <input type="hidden" id='hdnRequerido_<%# Eval("Moneda.CodigoIso") %>' value='<%# Eval("MontoRequerido") %>' />
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- Sección de Cuentas por Moneda -->
        <asp:Repeater ID="rptCuentasPorMoneda" runat="server" OnItemDataBound="rptCuentasPorMoneda_ItemDataBound">
            <ItemTemplate>
                <div class="cuentas-section">
                    <div class="moneda-header">
                        <div class="moneda-header-title">
                            <i class="fas fa-coins"></i>
                            <span>Cuentas en <%# Eval("Moneda.Nombre") %> (<%# Eval("Moneda.CodigoIso") %>)</span>
                        </div>
                        <div class="moneda-header-info">
                            <%# Eval("CantidadCuentas") %> cuenta(s) disponible(s)
                        </div>
                    </div>

                    <asp:Repeater ID="rptCuentas" runat="server">
                        <ItemTemplate>
                            <div class="cuenta-card" onclick="toggleCuenta(event, 'chk_<%# Eval("CuentaBancariaId") %>')">
                                <div class="cuenta-header">
                                    <div class="cuenta-checkbox">
                                        <input type="checkbox" 
                                            id='chk_<%# Eval("CuentaBancariaId") %>'
                                            name="cuentaSeleccionada"
                                            value='<%# Eval("CuentaBancariaId") %>'
                                            data-moneda='<%# Eval("Moneda.CodigoIso") %>'
                                            data-saldo='<%# Eval("SaldoDisponible") %>'
                                            onchange="actualizarSaldoSeleccionado('<%# Eval("Moneda.CodigoIso") %>'); event.stopPropagation();" />
                                    </div>
                                    <div class="cuenta-info">
                                        <div class="cuenta-numero">
                                            <i class="fas fa-credit-card me-2"></i>
                                            <%# Eval("NumeroCuenta") %>
                                        </div>
                                        <div class="cuenta-tipo">
                                            <%# Eval("TipoCuenta") %> - <%# Eval("EntidadBancaria.Nombre") %>
                                        </div>
                                    </div>
                                    <div class="cuenta-saldo">
                                        <div class="saldo-label">Saldo Disponible</div>
                                        <div class="saldo-amount">
                                            <%# string.Format("{0} {1:N2}", Eval("Moneda.CodigoIso"), Eval("SaldoDisponible")) %>
                                        </div>
                                    </div>
                                </div>
                                <div class="cuenta-details">
                                    <div class="detail-row">
                                        <span class="detail-row-label">CCI</span>
                                        <span class="detail-row-value"><%# Eval("Cci") ?? "-" %></span>
                                    </div>
                                    <div class="detail-row">
                                        <span class="detail-row-label">Estado</span>
                                        <span class="detail-row-value">
                                            <i class="fas fa-check-circle text-success"></i> Activa
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Mensaje si no hay cuentas -->
        <asp:Panel ID="pnlNoCuentas" runat="server" Visible="false" CssClass="alert-no-cuentas">
            <i class="fas fa-info-circle"></i>
            <h5>No hay cuentas disponibles</h5>
            <p>No se encontraron cuentas propias activas para esta entidad bancaria en las monedas requeridas.</p>
        </asp:Panel>

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
                OnClick="btnContinuar_Click"
                OnClientClick="return validarSeleccion();" />
        </div>

    </div>

    <script type="text/javascript">
        function toggleCuenta(event, checkboxId) {
            // Evita que al hacer click dentro del checkbox se dispare también el click del card
            if (event.target.type === "checkbox") return;

            var checkbox = document.getElementById(checkboxId);
            if (checkbox) {
                checkbox.checked = !checkbox.checked;
                var moneda = checkbox.getAttribute('data-moneda');
                actualizarSaldoSeleccionado(moneda);
            }
        }

        function actualizarSaldoSeleccionado(moneda) {
            var checkboxes = document.querySelectorAll('input[data-moneda="' + moneda + '"]');
            var totalSeleccionado = 0;

            checkboxes.forEach(function (cb) {
                var card = cb.closest('.cuenta-card');
                if (cb.checked) {
                    card.classList.add('selected');
                    var saldo = parseFloat(cb.getAttribute('data-saldo'));
                    if (!isNaN(saldo)) {
                        totalSeleccionado += saldo;
                    }
                } else {
                    card.classList.remove('selected');
                }
            });

            var lblSaldo = document.getElementById('lblSaldo_' + moneda);
            if (lblSaldo) {
                lblSaldo.innerText = totalSeleccionado.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            }

            var hdnRequerido = document.getElementById('hdnRequerido_' + moneda);
            var status = document.getElementById('status_' + moneda);

            if (hdnRequerido && status) {
                var requerido = parseFloat(hdnRequerido.value);
                status.classList.remove('success', 'danger');
                if (totalSeleccionado >= requerido) {
                    status.classList.add('success');
                } else {
                    status.classList.add('danger');
                }
            }
        }

        function validarSeleccion() {
            // Validación se hace en servidor, siempre retornar true
            return true;
        }

        document.addEventListener('DOMContentLoaded', function () {
            var hdnRequeridos = document.querySelectorAll('[id^="hdnRequerido_"]');
            hdnRequeridos.forEach(function (hdnRequerido) {
                var moneda = hdnRequerido.id.replace('hdnRequerido_', '');
                actualizarSaldoSeleccionado(moneda);
            });
        });
    </script>
</asp:Content>