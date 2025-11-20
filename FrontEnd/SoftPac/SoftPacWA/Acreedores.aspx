<%@ Page Title="Acreedores" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="Acreedores.aspx.cs" Inherits="SoftPacWA.Acreedores" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
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
            display: flex;
            align-items: center;
            gap: 1rem;
        }

            .page-title h2 {
                color: var(--color-primary);
                font-size: 1.8rem;
                font-weight: 600;
                margin: 0;
            }

        .filter-section {
            background: #fff;
            padding: .9rem 1rem;
            border-radius: 8px;
            margin-bottom: 1rem;
            box-shadow: 0 2px 8px rgba(0,0,0,.08)
        }

            .filter-section .form-label {
                height: 36px;
                display: flex;
                align-items: center;
                margin-bottom: 0
            }

        .badge-estado {
            padding: .35rem .7rem;
            border-radius: 4px;
            font-size: .85rem;
            font-weight: 600
        }

        .badge-pagado {
            background: #28a745;
            color: #fff
        }
        /* Activo */
        .badge-vencido {
            background: #dc3545;
            color: #fff
        }
        /* Inactivo */
        .action-buttons {
            display: flex;
            gap: .5rem
        }

        .btn-icon {
            padding: .25rem .5rem;
            font-size: .9rem
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

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <!-- Page Header -->
        <div class="page-header">
            <div class="page-title">
                <i class="fas fa-users" style="font-size: 2rem"></i>
                <h2>Acreedores</h2>
            </div>
        </div>

        <div class="filter-section">
            <div class="row gx-3 gy-3 pb-2">
                <div class="col-12 col-md-5">
                    <label class="form-label">Buscar</label>
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Razón social o RUC"></asp:TextBox>
                </div>
                <div class="col-12 col-md-3">
                    <label class="form-label">País</label>
                    <asp:DropDownList ID="ddlFiltroPais" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="AplicarFiltros">
                        <asp:ListItem Value="">Todos los países</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-12 col-md-4 d-flex align-items-end gap-2 flex-wrap">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary" OnClick="AplicarFiltros" />
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="LimpiarFiltros" />
                </div>
            </div>
        </div>

        <!-- Botón Nuevo Acreedor -->
        <div class="mb-3">
            <asp:Button ID="btnNuevoAcreedor" runat="server" Text="Nuevo acreedor" CssClass="btn btn-primary" OnClick="btnNuevoAcreedor_Click" />
        </div>

        <!-- Lista de Acreedores -->
        <div class="lista-card">
            <div class="lista-header">
                <h3 class="lista-title">Lista de Acreedores</h3>
            </div>

            <div class="lista-body">
                <asp:UpdatePanel ID="upAcreedores" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                            <asp:GridView ID="gvAcreedores" runat="server" CssClass="table table-hover"
                                AutoGenerateColumns="False" AllowPaging="True" PageSize="20"
                                OnPageIndexChanging="gvAcreedores_PageIndexChanging"
                                OnRowDataBound="gvAcreedores_RowDataBound"
                                EmptyDataText="No se encontraron acreedores" GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="razon_social" HeaderText="Razón social" />
                                    <asp:TemplateField HeaderText="País">
                                        <ItemTemplate><%# Eval("PaisNombre") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ruc" HeaderText="RUC" />
                                    <asp:TemplateField HeaderText="Estado">
                                        <ItemTemplate>
                                            <span class='badge-estado <%# GetEstadoClass(Eval("Estado")) %>'><%# Eval("Estado") %></span>
                                            <asp:HiddenField ID="hfId" runat="server" Value='<%# Eval("acreedor_id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <div class="action-buttons">
                                                <asp:LinkButton ID="btnVer" runat="server" CssClass="btn btn-sm btn-info btn-icon"
                                                    CommandName="Ver" CommandArgument='<%# Eval("acreedor_id") %>' OnClick="btnAccion_Click" ToolTip="Ver">
                                                    <i class="fas fa-eye"></i>
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-warning btn-icon"
                                                    CommandName="Editar" CommandArgument='<%# Eval("acreedor_id") %>' OnClick="btnAccion_Click" ToolTip="Editar">
                                                    <i class="fas fa-edit"></i>
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnToggle" runat="server" CssClass="btn btn-sm btn-danger btn-icon"
                                                    CommandName="Eliminar" CommandArgument='<%# Eval("acreedor_id") %>' OnClick="btnAccion_Click" ToolTip="Inactivar/Activar">
                                                    <i class="fas fa-x"></i>
                                                </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pagination-aw" HorizontalAlign="Center" />
                                <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última"
                                    PageButtonCount="10" />
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>
        <div class="lista-footer">
            <asp:Label ID="lblRegistros" runat="server" CssClass="total-registros"></asp:Label>
        </div>
    </div>
</asp:Content>
