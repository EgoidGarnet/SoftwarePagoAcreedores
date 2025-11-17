<%@ Page Title="Gestión de Propuestas" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="AprobacionPropuestas.aspx.cs" Inherits="SoftPacWA.AprobacionPropuestas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .page-title {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 1.5rem;
        }

            .page-title h3 {
                color: var(--color-primary);
                margin: 0;
                font-weight: 600;
            }

        .summary-card {
            background: #fff;
            border-radius: 10px;
            padding: 1rem 1.25rem;
            border: 1px solid rgba(0, 0, 0, 0.05);
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
            display: inline-flex;
            align-items: center;
            gap: .5rem;
            color: var(--color-secondary);
            font-weight: 600;
        }

        .table thead {
            background-color: var(--color-primary);
            color: white;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(12, 31, 52, 0.05);
        }

        .btn-action {
            border-radius: 999px;
        }
    </style>

    <asp:UpdatePanel ID="upPropuestas" runat="server">
        <ContentTemplate>
            <div class="page-title">
                <h3><i class="fas fa-clipboard-check me-2"></i>Propuestas pendientes de aprobación</h3>
                <asp:Label ID="lblTotalPendientes" runat="server" CssClass="summary-card" Text="Propuestas pendientes: 0"></asp:Label>
            </div>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger" role="alert">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <div class="card shadow-sm">
                <div class="card-body">
                    <asp:GridView ID="gvPropuestasPendientes" runat="server" AutoGenerateColumns="False" CssClass="table table-hover align-middle" GridLines="None" EmptyDataText="No se encontraron propuestas pendientes de aprobación.">
                        <Columns>
                            <asp:BoundField DataField="PropuestaId" HeaderText="ID" />
                            <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha Creación" DataFormatString="{0:dd/MM/yyyy HH:mm}" HtmlEncode="False" />
                            <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                            <asp:BoundField DataField="Pais" HeaderText="País" />
                            <asp:BoundField DataField="Banco" HeaderText="Banco" />
                            <asp:BoundField DataField="NumeroPagos" HeaderText="Nº Pagos" />
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <div class="d-flex flex-wrap gap-2">
                                        <asp:LinkButton ID="btnVisualizar" runat="server" CssClass="btn btn-sm btn-outline-info btn-action">
                                            <i class="fas fa-eye me-1"></i> Visualizar
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnAprobar" runat="server" CssClass="btn btn-sm btn-success btn-action">
                                            <i class="fas fa-check me-1"></i> Aprobar
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnRechazar" runat="server" CssClass="btn btn-sm btn-outline-danger btn-action">
                                            <i class="fas fa-times me-1"></i> Rechazar
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>