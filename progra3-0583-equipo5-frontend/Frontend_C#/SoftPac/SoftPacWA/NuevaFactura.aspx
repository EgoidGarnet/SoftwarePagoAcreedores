<%@ Page Title="Nueva factura" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="NuevaFactura.aspx.cs" Inherits="SoftPacWA.NuevaFactura" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card {
            border: none;
            border-radius: 0.75rem;
            box-shadow: 0 10px 30px rgba(11, 31, 52, 0.08);
        }

        .card-header {
            background: linear-gradient(135deg, rgba(11, 31, 52, 0.9), rgba(96, 116, 138, 0.8));
            color: #fff;
            font-size: 1.05rem;
            letter-spacing: 0.5px;
        }

        .section-title {
            font-weight: 600;
            color: var(--color-primary);
        }

        .totals-wrapper {
            background: #0b1f34;
            color: #fff;
            border-radius: 0.75rem;
            padding: 1.5rem;
        }

        .totals-wrapper .label {
            font-size: 0.85rem;
            text-transform: uppercase;
            opacity: 0.75;
        }

        .totals-wrapper .value {
            font-size: 1.5rem;
            font-weight: 600;
        }

        .detail-form {
            background: rgba(206, 214, 222, 0.3);
            border-radius: 0.75rem;
            padding: 1.25rem;
        }

        .btn-icon {
            display: inline-flex;
            align-items: center;
            gap: 0.35rem;
        }

        .grid-empty {
            text-align: center;
            padding: 2rem !important;
            color: var(--color-secondary);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="upNuevaFactura" runat="server">
        <ContentTemplate>
            <div class="page-title">
                <i class="fas fa-file-circle-plus"></i> Nueva factura
            </div>

            <asp:ValidationSummary ID="vsNuevaFactura" runat="server" CssClass="alert alert-danger" ValidationGroup="NuevaFactura" ShowSummary="true" />

            <div class="card mb-4">
                <div class="card-header">
                    <i class="fas fa-receipt me-2"></i>Datos generales de la factura
                </div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label for="txtNumeroFactura" class="form-label section-title">Número de factura</label>
                            <asp:TextBox ID="txtNumeroFactura" runat="server" CssClass="form-control" Placeholder="F0001-000045"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNumeroFactura" runat="server" ControlToValidate="txtNumeroFactura" ValidationGroup="NuevaFactura" ErrorMessage="Ingrese el número de la factura" CssClass="text-danger" Display="Dynamic" />
                        </div>
                        <div class="col-md-3">
                            <label for="ddlProveedor" class="form-label section-title">Proveedor</label>
                            <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvProveedor" runat="server" ControlToValidate="ddlProveedor" InitialValue="" ValidationGroup="NuevaFactura" ErrorMessage="Seleccione un proveedor" CssClass="text-danger" Display="Dynamic" />
                        </div>
                        <div class="col-md-3">
                            <label for="ddlMoneda" class="form-label section-title">Moneda</label>
                            <asp:DropDownList ID="ddlMoneda" runat="server" CssClass="form-select"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvMoneda" runat="server" ControlToValidate="ddlMoneda" InitialValue="" ValidationGroup="NuevaFactura" ErrorMessage="Seleccione una moneda" CssClass="text-danger" Display="Dynamic" />
                        </div>
                        <div class="col-md-3">
                            <label for="ddlEstado" class="form-label section-title">Estado</label>
                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Pendiente" Value="Pendiente"></asp:ListItem>
                                <asp:ListItem Text="Pagado" Value="Pagado"></asp:ListItem>
                                <asp:ListItem Text="Vencido" Value="Vencido"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label for="txtFechaEmision" class="form-label section-title">Fecha de emisión</label>
                            <asp:TextBox ID="txtFechaEmision" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFechaEmision" runat="server" ControlToValidate="txtFechaEmision" ValidationGroup="NuevaFactura" ErrorMessage="Seleccione la fecha de emisión" CssClass="text-danger" Display="Dynamic" />
                        </div>
                        <div class="col-md-3">
                            <label for="txtFechaRecepcion" class="form-label section-title">Fecha de recepción</label>
                            <asp:TextBox ID="txtFechaRecepcion" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label for="txtFechaLimitePago" class="form-label section-title">Fecha límite de pago</label>
                            <asp:TextBox ID="txtFechaLimitePago" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFechaLimitePago" runat="server" ControlToValidate="txtFechaLimitePago" ValidationGroup="NuevaFactura" ErrorMessage="Seleccione la fecha límite" CssClass="text-danger" Display="Dynamic" />
                        </div>
                        <div class="col-md-3">
                            <label for="txtRegimenFiscal" class="form-label section-title">Régimen fiscal</label>
                            <asp:TextBox ID="txtRegimenFiscal" runat="server" CssClass="form-control" Placeholder="Régimen General"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label for="txtTasaIva" class="form-label section-title">Tasa de IVA (%)</label>
                            <asp:TextBox ID="txtTasaIva" runat="server" CssClass="form-control" TextMode="Number" step="0.01" AutoPostBack="true" OnTextChanged="txtTasaIva_TextChanged"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label for="txtOtrosTributos" class="form-label section-title">Otros tributos</label>
                            <asp:TextBox ID="txtOtrosTributos" runat="server" CssClass="form-control" TextMode="Number" step="0.01" AutoPostBack="true" OnTextChanged="txtOtrosTributos_TextChanged"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card mb-4">
                <div class="card-header">
                    <i class="fas fa-list-check me-2"></i>Detalle de la factura
                </div>
                <div class="card-body">
                    <div class="detail-form mb-3">
                        <div class="row g-3 align-items-end">
                            <div class="col-md-7">
                                <label for="txtDescripcionDetalle" class="form-label section-title">Descripción del servicio o producto</label>
                                <asp:TextBox ID="txtDescripcionDetalle" runat="server" CssClass="form-control" Placeholder="Detalle de la línea"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDescripcionDetalle" runat="server" ControlToValidate="txtDescripcionDetalle" ValidationGroup="Detalle" ErrorMessage="Ingrese la descripción" CssClass="text-danger" Display="Dynamic" />
                            </div>
                            <div class="col-md-3">
                                <label for="txtSubtotalDetalle" class="form-label section-title">Subtotal</label>
                                <asp:TextBox ID="txtSubtotalDetalle" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvSubtotalDetalle" runat="server" ControlToValidate="txtSubtotalDetalle" ValidationGroup="Detalle" ErrorMessage="Ingrese el subtotal" CssClass="text-danger" Display="Dynamic" />
                            </div>
                            <div class="col-md-2 d-grid">
                                <asp:Button ID="btnAgregarDetalle" runat="server" CssClass="btn btn-secondary btn-icon" Text="<i class='fas fa-plus'></i> Agregar" OnClick="btnAgregarDetalle_Click" ValidationGroup="Detalle" UseSubmitBehavior="false" />
                            </div>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="gvDetalles" runat="server" CssClass="table table-hover align-middle" AutoGenerateColumns="False" DataKeyNames="DetalleFacturaId" EmptyDataText="Sin detalles registrados" OnRowCommand="gvDetalles_RowCommand" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="DetalleFacturaId" HeaderText="#" />
                                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:N2}" />
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEliminarDetalle" runat="server" CommandName="EliminarDetalle" CommandArgument='<%# Eval("DetalleFacturaId") %>' CssClass="btn btn-sm btn-danger btn-icon" ToolTip="Eliminar detalle" OnClientClick="return confirm('¿Desea eliminar este detalle?');">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataRowStyle CssClass="grid-empty" />
                        </asp:GridView>
                    </div>

                    <div class="row g-3 mt-4 justify-content-end">
                        <div class="col-md-4">
                            <div class="totals-wrapper">
                                <div class="d-flex justify-content-between mb-2">
                                    <div class="label">Subtotal</div>
                                    <div class="value"><asp:Label ID="lblSubtotal" runat="server" Text="0.00"></asp:Label></div>
                                </div>
                                <div class="d-flex justify-content-between mb-2">
                                    <div class="label">Impuestos (IVA)</div>
                                    <div class="value"><asp:Label ID="lblIgv" runat="server" Text="0.00"></asp:Label></div>
                                </div>
                                <div class="d-flex justify-content-between">
                                    <div class="label">Total a pagar</div>
                                    <div class="value"><asp:Label ID="lblTotal" runat="server" Text="0.00"></asp:Label></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-between align-items-center">
                <div>
                    <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-icon" Text="<i class='fas fa-save'></i> Guardar factura" OnClick="btnGuardar_Click" ValidationGroup="NuevaFactura" />
                    <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-outline-secondary ms-2" Text="Cancelar" OnClick="btnCancelar_Click" CausesValidation="false" />
                </div>
                <small class="text-muted">Los importes se calcularán automáticamente a partir de los detalles y tributos registrados.</small>
            </div>

            <asp:HiddenField ID="hfFacturaId" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
