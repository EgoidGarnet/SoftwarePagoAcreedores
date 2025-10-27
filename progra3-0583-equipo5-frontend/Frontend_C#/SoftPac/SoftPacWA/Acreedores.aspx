<%@ Page Title="Acreedores" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="Acreedores.aspx.cs" Inherits="SoftPacWA.Acreedores" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
    <style>
        .filter-section{background:#fff;padding:.9rem 1rem;border-radius:8px;margin-bottom:1rem;box-shadow:0 2px 8px rgba(0,0,0,.08)}
        .filter-section .form-label{height:36px;display:flex;align-items:center;margin-bottom:0}
        .badge-estado{padding:.35rem .7rem;border-radius:4px;font-size:.85rem;font-weight:600}
        .badge-pagado{background:#28a745;color:#fff}   /* Activo */
        .badge-vencido{background:#dc3545;color:#fff}  /* Inactivo */
        .action-buttons{display:flex;gap:.5rem}
        .btn-icon{padding:.25rem .5rem;font-size:.9rem}
        .table-responsive{border-radius:8px;overflow:hidden}
    </style>
</asp:Content>

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h3 class="pb-1"><i class="fas fa-users"></i> Acreedores</h3>
    </div>

    <div class="filter-section">
        <div class="row gx-3 gy-3">
            <div class="col-12 col-md-3">
                <label class="form-label">País</label>
                <asp:DropDownList ID="ddlFiltroPais" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="AplicarFiltros">
                    <asp:ListItem Value="">Todos los países</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-12 col-md-5">
                <label class="form-label">Buscar</label>
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Razón social o RUC"></asp:TextBox>
            </div>
            <div class="col-12 col-md-4 d-flex align-items-end gap-2 flex-wrap">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary" OnClick="AplicarFiltros" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="LimpiarFiltros" />
                <div class="ms-auto">
                    <asp:Button ID="btnNuevoAcreedor" runat="server" Text="Nuevo acreedor" CssClass="btn btn-primary" OnClick="btnNuevoAcreedor_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="card">
        <div class="card-body">
            <asp:UpdatePanel ID="upAcreedores" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive" style="overflow-x:auto; white-space:nowrap;">
                        <asp:GridView ID="gvAcreedores" runat="server" CssClass="table table-hover"
                            AutoGenerateColumns="False" AllowPaging="True" PageSize="20"
                            OnPageIndexChanging="gvAcreedores_PageIndexChanging"
                            OnRowDataBound="gvAcreedores_RowDataBound"
                            EmptyDataText="No se encontraron acreedores" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="RazonSocial" HeaderText="Razón social" />
                                <asp:TemplateField HeaderText="País">
                                    <ItemTemplate><%# Eval("PaisNombre") %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='badge-estado <%# GetEstadoClass(Eval("Estado")) %>'><%# Eval("Estado") %></span>
                                        <asp:HiddenField ID="hfId" runat="server" Value='<%# Eval("AcreedorId") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <div class="action-buttons">
                                            <asp:LinkButton ID="btnVer" runat="server" CssClass="btn btn-sm btn-info btn-icon"
                                                CommandName="Ver" CommandArgument='<%# Eval("AcreedorId") %>' OnClick="btnAccion_Click" ToolTip="Ver">
                                                <i class="fas fa-eye"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-warning btn-icon"
                                                CommandName="Editar" CommandArgument='<%# Eval("AcreedorId") %>' OnClick="btnAccion_Click" ToolTip="Editar">
                                                <i class="fas fa-edit"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnToggle" runat="server" CssClass="btn btn-sm btn-danger btn-icon"
                                                CommandName="Eliminar" CommandArgument='<%# Eval("AcreedorId") %>' OnClick="btnAccion_Click" ToolTip="Inactivar/Activar">
                                                <i class="fas fa-trash"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pagination-aw" HorizontalAlign="Center" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" PageButtonCount="10" />
                        </asp:GridView>
                    </div>
                    <div class="mt-2">
                        <asp:Label ID="lblRegistros" runat="server" CssClass="text-muted"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
