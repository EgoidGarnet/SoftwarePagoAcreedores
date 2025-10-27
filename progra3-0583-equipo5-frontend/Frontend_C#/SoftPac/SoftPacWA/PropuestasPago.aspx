<%@ Page Title="Propuestas de Pago" Language="C#" MasterPageFile="~/SoftPac.Master" 
    AutoEventWireup="true" CodeBehind="PropuestasPago.aspx.cs" 
    Inherits="SoftPacWA.PropuestasPago" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

        .filters-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 1.5rem;
            margin-bottom: 2rem;
        }

        .filters-title {
            color: var(--color-primary);
            font-size: 1.1rem;
            font-weight: 600;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .filters-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            align-items: end;
        }

        .filter-group {
            display: flex;
            flex-direction: column;
        }

        .filter-label {
            font-weight: 500;
            color: var(--color-primary);
            margin-bottom: 0.5rem;
            font-size: 0.9rem;
        }

        .lista-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            padding: 1.5rem;
        }

        .lista-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid var(--color-light-1);
        }

        .lista-title {
            color: var(--color-primary);
            font-size: 1.2rem;
            font-weight: 600;
            margin: 0;
        }

        .total-registros {
            color: var(--color-secondary);
            font-size: 0.9rem;
        }

        .acciones-buttons {
            display: flex;
            gap: 0.5rem;
            justify-content: left;
        }

        .empty-state {
            text-align: center;
            padding: 3rem;
            color: var(--color-secondary);
        }

        .empty-state i {
            font-size: 4rem;
            margin-bottom: 1rem;
            opacity: 0.5;
        }

        .empty-state h3 {
            color: var(--color-primary);
            margin-bottom: 0.5rem;
        }

        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                gap: 1rem;
                align-items: flex-start;
            }

            .filters-row {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        
        <!-- Mensajes -->
        <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-dismissible fade show" role="alert">
            <asp:Label ID="lblMensaje" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </asp:Panel>

        <!-- Page Header -->
        <div class="page-header">
            <div class="page-title">
                <i class="fas fa-money-check-alt" style="font-size: 2rem; color: var(--color-secondary);"></i>
                <h2>Propuestas de Pago</h2>
            </div>
            <div>
                <asp:Button ID="btnCrearPropuesta" runat="server" 
                    Text="Nueva Propuesta" 
                    CssClass="btn btn-primary"
                    OnClick="btnCrearPropuesta_Click" />
            </div>
        </div>

        <!-- Filtros -->
        <div class="filters-card">
            <div class="filters-title">
                <i class="fas fa-filter"></i>
                Filtros de Búsqueda
            </div>
            <div class="filters-row">
                <div class="filter-group">
                    <label class="filter-label">País</label>
                    <asp:DropDownList ID="ddlFiltroPais" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroPais_SelectedIndexChanged">
                        <asp:ListItem Value="">-- Todos --</asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <div class="filter-group">
                    <label class="filter-label">Banco</label>
                    <asp:DropDownList ID="ddlFiltroBanco" runat="server" CssClass="form-select">
                        <asp:ListItem Value="">-- Todos --</asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <div class="filter-group">
                    <label class="filter-label">Estado</label>
                    <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-select">
                        <asp:ListItem Value="">-- Todos --</asp:ListItem>
                        <asp:ListItem Value="Pendiente">Pendiente</asp:ListItem>
                        <asp:ListItem Value="Enviada">Enviada</asp:ListItem>
                        <asp:ListItem Value="Anulada">Anulada</asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <div class="filter-group" style="display: flex; gap: 0.5rem; flex-direction: row;">
                    <asp:Button ID="btnFiltrar" runat="server" 
                        Text="Filtrar" 
                        CssClass="btn btn-primary"
                        OnClick="btnFiltrar_Click" />
                    <asp:Button ID="btnLimpiarFiltros" runat="server" 
                        Text="Limpiar" 
                        CssClass="btn btn-secondary"
                        OnClick="btnLimpiarFiltros_Click" />
                </div>
            </div>
        </div>

        <!-- Lista de Propuestas -->
        <div class="lista-card">
            <div class="lista-header">
                <h3 class="lista-title">Lista de Propuestas</h3>
                <span class="total-registros">
                    <asp:Label ID="lblTotalRegistros" runat="server" Text="0 propuestas"></asp:Label>
                </span>
            </div>

            <asp:UpdatePanel ID="upPropuestas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <!-- GridView -->
                    <div class="table-responsive">
                        <asp:GridView ID="gvPropuestas" runat="server" 
                            AutoGenerateColumns="false"
                            CssClass="table table-hover align-middle"
                            AllowPaging="true"
                            PageSize="20"
                            OnPageIndexChanging="gvPropuestas_PageIndexChanging"
                            GridLines="None"
                            EmptyDataText="No se encontraron propuestas de pago.">
                            
                            <HeaderStyle CssClass="table-light" />
                            
                            <Columns>
                                <asp:BoundField DataField="PropuestaId" HeaderText="ID" />
                                
                                <asp:TemplateField HeaderText="Fecha Creación">
                                    <ItemTemplate>
                                        <%# Eval("FechaHoraCreacion") != null ? Convert.ToDateTime(Eval("FechaHoraCreacion")).ToString("dd/MM/yyyy HH:mm") : "-" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Usuario">
                                    <ItemTemplate>
                                        <%# Eval("UsuarioCreacion.Nombre") + " " + Eval("UsuarioCreacion.Apellidos") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="País">
                                    <ItemTemplate>
                                        <%# Eval("EntidadBancaria.Pais.Nombre") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Banco">
                                    <ItemTemplate>
                                        <%# Eval("EntidadBancaria.Nombre") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Nº Pagos">
                                    <ItemTemplate>
                                        <%# Eval("DetallesPropuesta.Count") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='badge <%# GetEstadoClass(Eval("Estado").ToString()) %>'>
                                            <%# Eval("Estado") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <div class="acciones-buttons">
                                            <asp:HyperLink ID="lnkVer" runat="server"
                                                NavigateUrl='<%# "~/DetallePropuesta.aspx?id=" + Eval("PropuestaId") %>'
                                                CssClass="btn btn-sm btn-info btn-icon"
                                                ToolTip="Ver detalle">
                                                <i class="fas fa-eye"></i>
                                            </asp:HyperLink>

                                            <asp:HyperLink ID="lnkEditar" runat="server"
                                                NavigateUrl='<%# "~/EditarPropuesta.aspx?id=" + Eval("PropuestaId") %>'
                                                CssClass="btn btn-sm btn-warning btn-icon"
                                                ToolTip="Editar"
                                                Visible='<%# Eval("Estado").ToString() == "Pendiente" %>'>
                                                <i class="fas fa-edit"></i>
                                            </asp:HyperLink>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            
                            <PagerStyle CssClass="pagination justify-content-center mt-3" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" PageButtonCount="10" />
                        </asp:GridView>
                    </div>

                    <!-- Estado vacío -->
                    <asp:Panel ID="pnlEmptyState" runat="server" Visible="false" CssClass="empty-state">
                        <i class="fas fa-inbox"></i>
                        <h3>No hay propuestas de pago</h3>
                        <p class="text-muted">Comienza creando tu primera propuesta de pago.</p>
                        <asp:Button ID="btnCrearPrimera" runat="server" 
                            Text="Crear primera propuesta" 
                            CssClass="btn btn-primary"
                            OnClick="btnCrearPropuesta_Click" />
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>