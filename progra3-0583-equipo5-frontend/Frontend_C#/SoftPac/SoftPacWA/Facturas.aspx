<%@ Page Title="Facturas" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="Facturas.aspx.cs" Inherits="SoftPacWA.Facturas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .filter-section {
            background-color: white;
            padding: 1.5rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }

        .badge-estado {
            padding: 0.4rem 0.8rem;
            border-radius: 4px;
            font-size: 0.85rem;
            font-weight: 600;
        }

        .badge-pendiente {
            background-color: #ffc107;
            color: #000;
        }

        .badge-pagado {
            background-color: #28a745;
            color: white;
        }

        .badge-vencido {
            background-color: #dc3545;
            color: white;
        }

        .action-buttons {
            display: flex;
            gap: 0.5rem;
        }

        .btn-icon {
            padding: 0.25rem 0.5rem;
            font-size: 0.9rem;
        }

        .pagination-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 1rem;
        }

        .table-responsive {
            border-radius: 8px;
            overflow: hidden;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <i class="fas fa-file-invoice"></i> Facturas
    </div>

    <!-- Filtros -->
    <div class="filter-section">
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">País</label>
                <asp:DropDownList ID="ddlFiltroPais" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="AplicarFiltros">
                    <asp:ListItem Value="">Todos los países</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label">Proveedor</label>
                <asp:DropDownList ID="ddlFiltroProveedor" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="AplicarFiltros">
                    <asp:ListItem Value="">Todos los proveedores</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label">Fecha Vencimiento Desde</label>
                <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <label class="form-label">Fecha Vencimiento Hasta</label>
                <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-2 d-flex align-items-end gap-2">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary" OnClick="AplicarFiltros" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="LimpiarFiltros" />
            </div>
        </div>
    </div>

    <!-- Botón Nueva Factura -->
    <div class="mb-3">
        <asp:Button ID="btnNuevaFactura" runat="server" Text="Nueva factura" CssClass="btn btn-primary" 
            OnClick="btnNuevaFactura_Click" />
    </div>

    <!-- Tabla de Facturas -->
    <div class="card">
        <div class="card-body">
            <asp:UpdatePanel ID="upFacturas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:GridView ID="gvFacturas" runat="server" CssClass="table table-hover" 
                            AutoGenerateColumns="False" AllowPaging="True" PageSize="20"
                            OnPageIndexChanging="gvFacturas_PageIndexChanging"
                            EmptyDataText="No se encontraron facturas"
                            GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="Numero_factura" HeaderText="Número de factura" />
                                <asp:TemplateField HeaderText="Proveedor">
                                    <ItemTemplate>
                                        <%# Eval("Acreedor.RazonSocial") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="País">
                                    <ItemTemplate>
                                        <%# Eval("Acreedor.Pais.Nombre") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha de emisión">
                                    <ItemTemplate>
                                        <%# Eval("Fecha_emision") != null ? Convert.ToDateTime(Eval("FechaEmision")).ToString("dd/MM/yyyy") : "" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha límite de pago">
                                    <ItemTemplate>
                                        <%# Eval("FechaLimitePago") != null ? Convert.ToDateTime(Eval("FechaLimitePago")).ToString("dd/MM/yyyy") : "" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Monto total">
                                    <ItemTemplate>
                                        <%# string.Format("{0} {1:N2}", Eval("Moneda.CodigoIso"), Eval("Monto_total")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='badge-estado <%# GetEstadoClass(Eval("Estado").ToString()) %>'>
                                            <%# Eval("Estado") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <div class="action-buttons">
                                            <asp:LinkButton ID="btnVer" runat="server" CssClass="btn btn-sm btn-info btn-icon" 
                                                CommandName="Ver" CommandArgument='<%# Eval("FacturaId") %>' 
                                                OnClick="btnAccion_Click" ToolTip="Ver detalles">
                                                <i class="fas fa-eye"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-warning btn-icon" 
                                                CommandName="Editar" CommandArgument='<%# Eval("FacturaId") %>' 
                                                OnClick="btnAccion_Click" ToolTip="Editar">
                                                <i class="fas fa-edit"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-danger btn-icon" 
                                                CommandName="Eliminar" CommandArgument='<%# Eval("FacturaId") %>' 
                                                OnClick="btnAccion_Click" ToolTip="Eliminar"
                                                OnClientClick="return confirm('¿Está seguro de eliminar esta factura?');">
                                                <i class="fas fa-trash"></i>
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

                    <div class="pagination-container">
                        <div>
                            <asp:Label ID="lblRegistros" runat="server" CssClass="text-muted"></asp:Label>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
