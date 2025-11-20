<%@ Page Title="Detalle de Cuenta Propia" Language="C#" MasterPageFile="~/SoftPac.Master"
    AutoEventWireup="true" CodeBehind="DetalleCuentaPropia.aspx.cs" Inherits="SoftPacWA.DetalleCuentaPropia" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card-info .card-body{padding:1rem}
        .kv{display:flex;gap:.5rem;align-items:center}
        .kv .k{min-width:150px;color:#6c757d;font-weight:600}
        .kv .v{font-weight:500}
        .small-cols>.col{padding-top:.25rem;padding-bottom:.25rem}
        .badge-estado{padding:.35rem .7rem;border-radius:4px;font-size:.85rem;font-weight:600}
        .badge-activo{background:#28a745;color:#fff}
        .badge-inactivo{background:#dc3545;color:#fff}
        .monto-positivo{color:#28a745;font-weight:600}
        .monto-negativo{color:#dc3545;font-weight:600}
        .card-saldo{
              background: linear-gradient(135deg, #f8f9fa 0%, #eef4f9 100%);
              color: #333;
            }
        .card-saldo .card-body{padding:1.5rem}
        .saldo-item{text-align:center;padding:1rem}
        .saldo-label{font-size:1rem;margin-bottom:.5rem;font-weight:700}
        .saldo-valor{font-size:1.5rem;font-weight:700}
    </style>
</asp:Content>

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="pb-1"><i class="fa-solid fa-wallet"></i> Detalle de cuenta propia</h3>
        <div class="d-flex gap-2">
            <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-danger btn-sm" 
                OnClientClick="return false;" Text="Eliminar cuenta"
                data-bs-toggle="modal" data-bs-target="#modalConfirmarEliminar">
                <i class="fa fa-trash"></i> Eliminar cuenta
            </asp:LinkButton>
            <asp:HyperLink ID="lnkVolver" runat="server" CssClass="btn btn-secondary btn-sm" NavigateUrl="CuentasPropias.aspx">
                Volver
            </asp:HyperLink>
        </div>
    </div>

    <asp:HiddenField ID="hfCuentaId" runat="server" />

    <!-- Datos de la cuenta -->
    <div class="card card-info mb-3">
        <div class="card-body">
            <div class="row row-cols-1 row-cols-md-3 g-2 small-cols">
                <div class="col">
                    <div class="kv"><span class="k">Entidad bancaria</span><asp:Label ID="lblEntidad" runat="server" CssClass="v" /></div>
                </div>
                <div class="col">
                    <div class="kv"><span class="k">Número de cuenta</span><asp:Label ID="lblNumeroCuenta" runat="server" CssClass="v" /></div>
                </div>
                <div class="col">
                    <div class="kv"><span class="k">Tipo de cuenta</span><asp:Label ID="lblTipoCuenta" runat="server" CssClass="v" /></div>
                </div>
                <div class="col">
                    <div class="kv"><span class="k">CCI</span><asp:Label ID="lblCCI" runat="server" CssClass="v" /></div>
                </div>
                <div class="col">
                    <div class="kv"><span class="k">Moneda</span><asp:Label ID="lblMoneda" runat="server" CssClass="v" /></div>
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

    <!-- Resumen de movimientos -->
    <div class="card card-saldo mb-3">
        <div class="card-body">
            <div class="row">
                <div class="col-md-3 saldo-item">
                    <div class="saldo-label">Total Ingresos</div>
                    <div class="saldo-valor"><asp:Label ID="lblTotalIngresos" runat="server" /></div>
                </div>
                <div class="col-md-3 saldo-item">
                    <div class="saldo-label">Total Egresos</div>
                    <div class="saldo-valor"><asp:Label ID="lblTotalEgresos" runat="server" /></div>
                </div>
                <div class="col-md-3 saldo-item">
                    <div class="saldo-label">Diferencia</div>
                    <div class="saldo-valor"><asp:Label ID="lblDiferencia" runat="server" /></div>
                </div>
                <div class="col-md-3 saldo-item">
                    <div class="saldo-label">Saldo Disponible</div>
                    <div class="saldo-valor"><asp:Label ID="lblSaldoTotal" runat="server" /></div>
                </div>
            </div>
        </div>
    </div>

    <!-- Historial de cambios (Kardex) -->
    <h5 class="mb-2"><i class="fa-solid fa-clock-rotate-left me-2 py-2"></i> Historial de cambios en saldo</h5>
    <div class="card">
        <div class="card-body">
            <asp:UpdatePanel ID="upKardex" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:GridView ID="gvKardex" runat="server" CssClass="table table-sm table-striped mb-3"
                        AutoGenerateColumns="False" GridLines="None" EmptyDataText="Sin movimientos registrados"
                        AllowPaging="True" PageSize="20" OnPageIndexChanging="gvKardex_PageIndexChanging"
                        OnRowDataBound="gvKardex_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Fecha">
                                <ItemTemplate><%# Eval("fecha_modificacion", "{0:dd/MM/yyyy HH:mm}") %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Usuario">
                                <ItemTemplate><%# Eval("usuario_modificacion.nombre") + " " + Eval("usuario_modificacion.apellidos") + " (" + Eval("usuario_modificacion.correo_electronico") + ")" %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Monto Modificado">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontoModificado" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Saldo Resultante">
                                <ItemTemplate><%# FormatearMonto((decimal)Eval("saldo_resultante")) %></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-ger" HorizontalAlign="Center" />
                        <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última" 
                            PageButtonCount="5" Position="Bottom" />
                    </asp:GridView>
                    <asp:Panel ID="pnlSinDatos" runat="server" CssClass="text-center py-5">
                        <i class="fa-solid fa-inbox fa-3x text-muted mb-3"></i>
                        <p class="text-muted h5">No hay movimientos registrados</p>
                        <p class="text-muted">Esta cuenta aún no tiene historial de cambios en el saldo.</p>
                    </asp:Panel>
                    <asp:Label ID="lblTotalMovimientos" runat="server" CssClass="text-muted d-block"></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- MODAL PARA CONFIRMAR ELIMINACIÓN -->
    <div class="modal fade" id="modalConfirmarEliminar" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-exclamation-triangle me-2"></i>Confirmar eliminación
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p class="mb-0">¿Está seguro que desea eliminar permanentemente esta cuenta propia?</p>
                    <p class="text-muted mb-0 mt-2"><small>Esta acción no se puede deshacer.</small></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnConfirmarEliminar" runat="server" CssClass="btn btn-danger" 
                        OnClick="btnConfirmarEliminar_Click" Text="Eliminar definitivamente" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>