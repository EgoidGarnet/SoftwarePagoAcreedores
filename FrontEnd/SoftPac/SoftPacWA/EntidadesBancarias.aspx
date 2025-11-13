<%@ Page Title="Entidades Bancarias" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="EntidadesBancarias.aspx.cs" Inherits="SoftPacWA.EntidadesBancarias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .filter-section {
            background-color: white;
            padding: 1.5rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .lista-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 0;
            overflow: hidden;
        }

        .lista-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 1.5rem;
            border-bottom: 2px solid var(--color-light-1);
            background-color: #f8f9fa;
        }

        .lista-title {
            color: var(--color-primary);
            font-size: 1.2rem;
            font-weight: 600;
            margin: 0;
        }

        .lista-body {
            padding: 1.5rem;
        }

        .lista-footer {
            padding: 1rem 1.5rem;
            border-top: 1px solid var(--color-light-1);
            background-color: #f8f9fa;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .total-registros {
            color: var(--color-secondary);
            font-size: 0.9rem;
            margin: 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h3 class="pb-1">
            <i class="fas fa-university"></i> Entidades Bancarias
        </h3>
    </div>

    <!-- Filtros -->
    <div class="filter-section">
        <asp:UpdatePanel ID="upFiltros" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="row gx-3 gy-4">
                    <div class="col-12 col-md-4">
                        <label class="form-label">País</label>
                        <asp:DropDownList ID="ddlFiltroPais" runat="server" CssClass="form-select" 
                            AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroPais_SelectedIndexChanged">
                            <asp:ListItem Value="">Todos los países</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-4">
                        <label class="form-label">Entidad Bancaria</label>
                        <asp:DropDownList ID="ddlFiltroEntidad" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">Todas las entidades</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4 d-flex align-items-end gap-2 flex-wrap">
                        <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary" OnClick="btnFiltrar_Click" />
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <!-- Tabla de Entidades Bancarias -->
    <div class="lista-card">
        <div class="lista-header">
            <h3 class="lista-title">Lista de Entidades Bancarias</h3>
        </div>
    
        <div class="lista-body">
            <asp:UpdatePanel ID="upEntidades" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                        <asp:GridView ID="gvEntidades" runat="server" CssClass="table table-hover" 
                            AutoGenerateColumns="False" AllowPaging="True" PageSize="20"
                            OnPageIndexChanging="gvEntidades_PageIndexChanging"
                            EmptyDataText="No se encontraron entidades bancarias"
                            GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="entidad_bancaria_id" HeaderText="ID" />
                                <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                                <asp:BoundField DataField="formato_aceptado" HeaderText="Formato Aceptado" />
                                <asp:BoundField DataField="codigo_swift" HeaderText="Código SWIFT" />
                                <asp:TemplateField HeaderText="País">
                                    <ItemTemplate>
                                        <%# Eval("pais.nombre") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pagination-aw" HorizontalAlign="Center" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" 
                                PageButtonCount="10" />
                        </asp:GridView>
                    </div>
                    <asp:Label ID="Label1" runat="server" CssClass="total-registros"></asp:Label>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnFiltrar" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnLimpiar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
        
        <div class="lista-footer">
            <asp:Label ID="lblRegistros" runat="server" CssClass="total-registros"></asp:Label>
        </div>
    </div>
</asp:Content>