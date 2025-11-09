<%@ Page Title="Gestión de Detalle de Factura" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="GestionDetalleFactura.aspx.cs" Inherits="SoftPacWA.GestionDetalleFactura" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-section {
            background-color: white;
            padding: 2rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            max-width: 800px; /* Limita el ancho para formularios simples */
            margin-left: auto;
            margin-right: auto;
        }
        .section-title {
            font-size: 1.5rem;
            font-weight: 600;
            color: var(--color-primary);
            margin-bottom: 1.5rem;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid var(--color-light-1);
        }
        .footer-buttons {
            display: flex;
            justify-content: flex-end;
            gap: 1rem;
            margin-top: 2rem;
            padding-top: 1rem;
            border-top: 1px solid #dee2e6;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <asp:UpdatePanel ID="updPanelDetalle" runat="server">
            <ContentTemplate>
                <div class="form-section">
                    <h2 class="section-title">
                        <asp:Label ID="lblTitulo" runat="server" Text="Gestión de Detalle"></asp:Label>
                    </h2>

                    <%-- Formulario de Detalle --%>
                    <div class="mb-3">
                        <label class="form-label">Descripción</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Subtotal</label>
                        <div class="input-group">
                            <span class="input-group-text">
                                <asp:Label ID="lblMoneda" runat="server"></asp:Label>
                            </span>
                            <asp:TextBox ID="txtSubtotal" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                        </div>
                    </div>
                    
                    <%-- Botones de Acción --%>
                    <div class="footer-buttons">
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" />
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>