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
        .filter-section .form-label {
            height: 44px;
            display: flex;
            align-items: center;
            margin-bottom: 0;
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

        .badge-elimin {
            background-color: #ff0000;
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

        .btn-reporte {
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
        }

        @media (max-width: 768px) {
            .btn-reporte .btn-text {
                display: none;
            }
            
            .lista-header {
                flex-direction: column;
                gap: 1rem;
                align-items: flex-start;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-title">
        <h3 class="pb-1">
            <i class="fas fa-file-invoice"></i> Facturas
        </h3>
    </div>

    <!-- Filtros -->
    <div class="filter-section">
        <div class="row gx-3 gy-4">
            <div class="col-12 col-md-3">
                <label class="form-label">Buscar por Número de Factura</label>
                <asp:TextBox ID="txtBuscarFactura" runat="server" CssClass="form-control" 
                    placeholder="Ingrese número de factura" autocomplete="off"></asp:TextBox>
                <asp:HiddenField ID="hfFacturasJson" runat="server" />
            </div>
            <div class="col-12 col-md-2">
                <label class="form-label">País</label>
                <asp:DropDownList ID="ddlFiltroPais" runat="server" CssClass="form-select" 
                    AutoPostBack="true" OnSelectedIndexChanged="AplicarFiltros">
                    <asp:ListItem Value="">Todos los países</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-12 col-md-3">
                <label class="form-label">Acreedor</label>
                <asp:DropDownList ID="ddlFiltroProveedor" runat="server" CssClass="form-select" 
                    AutoPostBack="true" OnSelectedIndexChanged="AplicarFiltros">
                    <asp:ListItem Value="">Todos los acreedores</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-2">
                <label class="form-label">Vencimiento Desde</label>
                <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-6 col-md-2">
                <label class="form-label">Vencimiento Hasta</label>
                <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
        </div>
        <div class="row gx-3 gy-4 mt-2">
            <div class="col-md-12 d-flex gap-2">
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
    <div class="lista-card">
        <div class="lista-header">
            <h3 class="lista-title">Lista de Facturas</h3>
            <asp:LinkButton ID="btnGenerarReporte" runat="server" 
                CssClass="btn btn-primary btn-reporte" 
                OnClick="btnGenerarReporte_Click"
                ToolTip="Generar reporte">
                <i class="fas fa-file-pdf"></i>
                <span class="btn-text">Generar reporte</span>
            </asp:LinkButton>
        </div>
        
        <div class="lista-body">
            <asp:UpdatePanel ID="upFacturas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                        <asp:GridView ID="gvFacturas" runat="server" CssClass="table table-hover" 
                            AutoGenerateColumns="False" AllowPaging="True" PageSize="20"
                            OnPageIndexChanging="gvFacturas_PageIndexChanging"
                            EmptyDataText="No se encontraron facturas"
                            GridLines="None"
                            OnRowDataBound="gvFacturas_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="numero_factura" HeaderText="Número de factura" />
                                <asp:TemplateField HeaderText="Acreedor">
                                    <ItemTemplate>
                                        <%# Eval("acreedor.razon_social") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="País">
                                    <ItemTemplate>
                                        <%# Eval("acreedor.pais.nombre") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha de emisión">
                                    <ItemTemplate>
                                        <%# Eval("fecha_emision") != null ? Convert.ToDateTime(Eval("fecha_emision")).ToString("dd/MM/yyyy") : "" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha límite de pago">
                                    <ItemTemplate>
                                        <%# Eval("fecha_limite_pago") != null ? Convert.ToDateTime(Eval("fecha_limite_pago")).ToString("dd/MM/yyyy") : "" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Monto total">
                                    <ItemTemplate>
                                        <%# string.Format("{0} {1:N2}", Eval("moneda.codigo_iso"), Eval("monto_total")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='badge-estado <%# GetEstadoClass(Eval("estado").ToString()) %>'>
                                            <%# Eval("estado") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <div class="action-buttons">
                                            <asp:LinkButton ID="btnVer" runat="server" CssClass="btn btn-sm btn-info btn-icon" 
                                                CommandName="Ver" CommandArgument='<%# Eval("factura_id") %>' 
                                                OnClick="btnAccion_Click" ToolTip="Ver detalles">
                                                <i class="fas fa-eye"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-warning btn-icon" 
                                                CommandName="Editar" CommandArgument='<%# Eval("factura_id") %>' 
                                                OnClick="btnAccion_Click" ToolTip="Editar">
                                                <i class="fas fa-edit"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-danger btn-icon" 
                                                CommandName="Eliminar" CommandArgument='<%# Eval("factura_id") %>' 
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        
        <div class="lista-footer">
            <asp:Label ID="lblRegistros" runat="server" CssClass="total-registros"></asp:Label>
        </div>
    </div>

    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            var facturasJson = $('#<%= hfFacturasJson.ClientID %>').val();
            var facturas = facturasJson ? JSON.parse(facturasJson) : [];
        
            $('#<%= txtBuscarFactura.ClientID %>').autocomplete({
                source: function (request, response) {
                    var term = request.term.toLowerCase();
                    var matches = $.grep(facturas, function (factura) {
                        return factura.numero_factura.toLowerCase().includes(term) ||
                               factura.acreedor.toLowerCase().includes(term) ||
                               factura.monto_total.toString().includes(term);
                    });

                    response(matches.slice(0, 10).map(function (factura) {
                        return {
                            label: factura.numero_factura + ' - ' + factura.acreedor + ' (' + factura.moneda + ' ' + factura.monto_total + ') - ' + factura.estado,
                            value: factura.numero_factura
                        };
                    }));
                },
                minLength: 1,
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    return false;
                }
            });
        });
    </script>


</asp:Content>