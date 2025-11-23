<%@ Page Title="Nueva Propuesta - Paso 1" Language="C#" MasterPageFile="~/SoftPac.Master" 
    AutoEventWireup="true" CodeBehind="CrearPropuestaPaso1.aspx.cs" 
    Inherits="SoftPacWA.CrearPropuestaPaso1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .wizard-container {
            max-width: 900px;
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

        .page-header p {
            color: var(--color-secondary);
            font-size: 1rem;
            margin: 0;
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

        .form-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 2.5rem;
            margin-bottom: 2rem;
        }

        .form-group {
            margin-bottom: 2rem;
        }

        .form-label {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-weight: 600;
            color: var(--color-primary);
            margin-bottom: 0.75rem;
            font-size: 1rem;
        }

        .form-label i {
            color: var(--color-secondary);
        }
        
        .form-select-lg {
            font-size: 1rem;
        }

        .required-asterisk {
            color: #dc3545;
            margin-left: 0.25rem;
        }

        .help-text {
            font-size: 0.875rem;
            color: var(--color-secondary);
            margin-top: 0.5rem;
            font-style: italic;
        }

        .info-box {
            background-color: #e7f3ff;
            border-left: 4px solid #2196F3;
            padding: 1rem 1.25rem;
            border-radius: 6px;
            margin-top: 2rem;
        }

        .info-box-title {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-weight: 600;
            color: #0d47a1;
            margin-bottom: 0.5rem;
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

        @media (max-width: 768px) {
            .form-card {
                padding: 1.5rem;
            }

            .actions-container {
                flex-direction: column;
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
        </div>

        <!-- Step Indicator -->
        <div class="step-indicator">
            <i class="fas fa-filter"></i>
            Paso 1 de 4: País y Entidad Bancaria
        </div>

        <!-- Form Card -->
        <div class="form-card">
            
            <!-- País -->
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-globe"></i>
                    País
                    <span class="required-asterisk">*</span>
                </label>
                <asp:DropDownList ID="ddlPais" runat="server" 
                    CssClass="form-select form-select-lg"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlPais_SelectedIndexChanged">
                    <asp:ListItem Value="">-- Seleccione un país --</asp:ListItem>
                </asp:DropDownList>
                <div class="help-text">
                    País donde se encuentran los proveedores a pagar
                </div>
                <asp:RequiredFieldValidator ID="rfvPais" runat="server"
                    ControlToValidate="ddlPais"
                    InitialValue=""
                    ErrorMessage="Debe seleccionar un país"
                    CssClass="text-danger"
                    Display="Dynamic"
                    ValidationGroup="Paso1" />
            </div>

            <!-- Entidad Bancaria -->
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-university"></i>
                    Entidad Bancaria
                    <span class="required-asterisk">*</span>
                </label>
                <asp:DropDownList ID="ddlEntidadBancaria" runat="server" 
                    CssClass="form-select form-select-lg"
                    Enabled="false">
                    <asp:ListItem Value="">-- Primero seleccione un país --</asp:ListItem>
                </asp:DropDownList>
                <div class="help-text">
                    El banco que procesará los pagos de esta propuesta
                </div>
                <asp:RequiredFieldValidator ID="rfvBanco" runat="server"
                    ControlToValidate="ddlEntidadBancaria"
                    InitialValue=""
                    ErrorMessage="Debe seleccionar una entidad bancaria"
                    CssClass="text-danger"
                    Display="Dynamic"
                    ValidationGroup="Paso1" />
            </div>

            <!-- Plazo de Vencimiento -->
            <div class="form-group">
                <label class="form-label">
                    <i class="fas fa-calendar-alt"></i>
                    Plazo de Vencimiento
                    <span class="required-asterisk">*</span>
                </label>
                <asp:DropDownList ID="ddlPlazoVencimiento" runat="server" 
                    CssClass="form-select form-select-lg">
                    <asp:ListItem Value="">-- Seleccione un plazo --</asp:ListItem>
                    <asp:ListItem Value="7">Próximos 7 días</asp:ListItem>
                    <asp:ListItem Value="15">Próximos 15 días</asp:ListItem>
                    <asp:ListItem Value="30">Próximos 30 días</asp:ListItem>
                    <asp:ListItem Value="60">Próximos 60 días</asp:ListItem>
                    <asp:ListItem Value="90">Próximos 90 días</asp:ListItem>
                </asp:DropDownList>
                <div class="help-text">
                    Solo se incluirán facturas que venzan dentro de este período
                </div>
                <asp:RequiredFieldValidator ID="rfvPlazo" runat="server"
                    ControlToValidate="ddlPlazoVencimiento"
                    InitialValue=""
                    ErrorMessage="Debe seleccionar un plazo de vencimiento"
                    CssClass="text-danger"
                    Display="Dynamic"
                    ValidationGroup="Paso1" />
            </div>

            <!-- Info Box -->
            <div class="info-box">
                <div class="info-box-title">
                    <i class="fas fa-info-circle"></i>
                    Información importante
                </div>
                <div class="info-box-content">
                    Los criterios seleccionados determinarán qué facturas pendientes estarán disponibles para 
                    incluir en la propuesta de pago en los siguientes pasos.
                </div>
            </div>

        </div>

        <!-- Actions -->
        <div class="actions-container">
            <asp:Button ID="btnCancelar" runat="server" 
                Text="Cancelar" 
                CssClass="btn btn-secondary"
                OnClick="btnCancelar_Click"
                CausesValidation="false" />
            
            <asp:Button ID="btnSiguiente" runat="server" 
                Text="Siguiente" 
                CssClass="btn btn-primary"
                OnClick="btnSiguiente_Click"
                ValidationGroup="Paso1" />
        </div>

    </div>
</asp:Content>