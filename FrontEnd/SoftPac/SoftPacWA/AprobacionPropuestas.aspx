<%@ Page Title="Gestión de Propuestas" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="AprobacionPropuestas.aspx.cs" Inherits="SoftPacWA.AprobacionPropuestas" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </asp:Panel>

            <div class="card shadow-sm">
                <div class="card-body">
                    <asp:GridView ID="gvPropuestasPendientes" runat="server" 
                        AutoGenerateColumns="False" 
                        CssClass="table table-hover align-middle" 
                        GridLines="None" 
                        OnRowCommand="gvPropuestasPendientes_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="propuesta_id" HeaderText="ID" />
                            
                            <asp:TemplateField HeaderText="Fecha Creación">
                                <ItemTemplate>
                                    <%# Eval("fecha_hora_creacion") != null ? Convert.ToDateTime(Eval("fecha_hora_creacion")).ToString("dd/MM/yyyy HH:mm") : "-" %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Usuario">
                                <ItemTemplate>
                                    <%# Eval("usuario_creacion.nombre") + " " + Eval("usuario_creacion.apellidos") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="País">
                                <ItemTemplate>
                                    <%# Eval("entidad_bancaria.pais.nombre") ?? "Sin país" %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Banco">
                                <ItemTemplate>
                                    <%# Eval("entidad_bancaria.nombre") ?? "Sin banco" %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Nº Pagos">
                                <ItemTemplate>
                                    <%# ((SoftPacBusiness.PropuestaPagoWS.propuestasPagoDTO)Container.DataItem).detalles_propuesta.Length %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <div class="d-flex flex-wrap gap-2">
                                        <asp:LinkButton ID="btnVisualizar" runat="server" 
                                            CssClass="btn btn-sm btn-outline-info btn-action"
                                            CommandName="Visualizar"
                                            CommandArgument='<%# Eval("propuesta_id") %>'>
                                            <i class="fas fa-eye me-1"></i> Visualizar
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center text-muted py-4">
                                <i class="fas fa-clipboard-check fa-2x mb-3"></i>
                                <p class="mb-0">No se encontraron propuestas pendientes</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>