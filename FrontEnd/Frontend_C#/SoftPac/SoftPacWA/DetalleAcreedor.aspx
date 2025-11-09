<%@ Page Title="Detalle de Acreedor" Language="C#" MasterPageFile="~/SoftPac.Master"
    AutoEventWireup="true" CodeBehind="DetalleAcreedor.aspx.cs" Inherits="SoftPacWA.DetalleAcreedor" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card-info .card-body{padding:1rem}
        .kv{display:flex;gap:.5rem;align-items:center}
        .kv .k{min-width:130px;color:#6c757d;font-weight:600}
        .kv .v{font-weight:500}
        .small-cols>.col{padding-top:.25rem;padding-bottom:.25rem}
        .badge-estado{padding:.35rem .7rem;border-radius:4px;font-size:.85rem;font-weight:600}
        .badge-activo{background:#28a745;color:#fff}
        .badge-inactivo{background:#dc3545;color:#fff}
        .action-buttons{display:flex;gap:.5rem}
        .btn-icon{padding:.25rem .5rem;font-size:.9rem}
    </style>
</asp:Content>

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="pb-1"><i class="fa-solid fa-id-card"></i> Detalle del acreedor</h3>
        <div class="d-flex gap-2">
            <asp:HyperLink ID="lnkNuevaCuenta" runat="server" CssClass="btn btn-primary btn-sm">
                <i class="fa fa-plus"></i> Nueva cuenta
            </asp:HyperLink>
            <asp:HyperLink ID="lnkVolver" runat="server" CssClass="btn btn-secondary btn-sm" NavigateUrl="Acreedores.aspx">
                Volver
            </asp:HyperLink>
        </div>
    </div>

    <asp:HiddenField ID="hfAcreedorId" runat="server" />

    <!-- Datos del acreedor -->
    <div class="card card-info mb-3">
        <div class="card-body">
            <div class="row row-cols-1 row-cols-md-3 g-2 small-cols">
                <div class="col">
                    <div class="kv"><span class="k">Razón social</span><asp:Label ID="lblRazon" runat="server" CssClass="v" /></div>
                </div>
                <div class="col">
                    <div class="kv"><span class="k">RUC</span><asp:Label ID="lblRuc" runat="server" CssClass="v" /></div>
                </div>
                <div class="col">
                    <div class="kv"><span class="k">País</span><asp:Label ID="lblPais" runat="server" CssClass="v" /></div>
                </div>
                <div class="col-12">
                    <div class="kv"><span class="k">Dirección fiscal</span><asp:Label ID="lblDir" runat="server" CssClass="v" /></div>
                </div>
                <div class="col">
                    <div class="kv"><span class="k">Plazo pago</span><span class="v"><asp:Label ID="lblPlazo" runat="server" /> días</span></div>
                </div>
                <div class="col">
                    <div class="kv">
                        <span class="k">Estado</span>
                        <span class="v"><asp:Literal ID="litEstado" runat="server" /></span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Cuentas bancarias -->
    <h5 class="mb-2"><i class="fa-solid fa-building-columns me-2"></i> Cuentas bancarias</h5>
    <div class="card">
        <div class="card-body">
            <asp:UpdatePanel ID="upCuentas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="gvCuentas" runat="server" CssClass="table table-sm table-striped mb-0"
                        AutoGenerateColumns="False" GridLines="None" EmptyDataText="Sin cuentas registradas"
                        OnRowDataBound="gvCuentas_RowDataBound" OnRowCommand="gvCuentas_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="Entidad">
                                <ItemTemplate><%# Eval("Entidad") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="N° Cuenta">
                                <ItemTemplate><%# Eval("NumeroCuenta") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CCI/IBAN">
                                <ItemTemplate><%# Eval("CCI") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo">
                                <ItemTemplate><%# Eval("TipoCuenta") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Divisa">
                                <ItemTemplate><%# Eval("Divisa") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <span class='badge-estado <%# GetCuentaEstadoClass(Eval("Estado")) %>'><%# Eval("Estado") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <div class="action-buttons">
                                        <asp:HyperLink ID="btnEditarCuenta" runat="server" CssClass="btn btn-sm btn-warning btn-icon"
                                            NavigateUrl='<%# "GestionDetalleAcreedor.aspx?acreedorId=" + Eval("AcreedorId") + "&cuentaId=" + Eval("CuentaBancariaId") %>'
                                            ToolTip="Editar">
                                            <i class="fas fa-edit"></i>
                                        </asp:HyperLink>
                                        <asp:LinkButton ID="btnToggleCuenta" runat="server" CssClass="btn btn-sm btn-danger btn-icon"
                                            CommandName="Toggle" CommandArgument='<%# Eval("CuentaBancariaId") %>' ToolTip="Inactivar/Activar">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:Label ID="lblTotalCuentas" runat="server" CssClass="text-muted mt-2 d-block"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
