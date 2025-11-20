<%@ Page Title="Gestión de Factura" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="GestionFactura.aspx.cs" Inherits="SoftPacWA.GestionFactura" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-section {
            background-color: white;
            padding: 2rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
        }
        .section-title {
            font-size: 1.5rem;
            font-weight: 600;
            color: var(--color-primary);
            margin-bottom: 1.5rem;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid var(--color-light-1);
        }
        .form-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 1rem;
        }
        .grid-span-2 {
            grid-column: span 2;
        }
        .footer-buttons {
            display: flex;
            justify-content: flex-end;
            gap: 1rem;
            margin-top: 2rem;
            padding-top: 1rem;
            border-top: 1px solid #dee2e6;
        }
        .details-grid-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1rem;
        }
        .btn-primary:disabled,
        .btn-primary.disabled {
            background-color: var(--color-secondary);
            border-color: var(--color-secondary);
            opacity: 0.65; /* Mantenemos una ligera opacidad para indicar que no es clickeable */
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <asp:UpdatePanel ID="updPanelPrincipal" runat="server">
            <ContentTemplate>
                <div class="form-section">
                    <div class="d-flex justify-content-between align-items-center">
                        <h2 class="section-title">
                            <asp:Label ID="lblTitulo" runat="server" Text="Gestión de Factura"></asp:Label>
                        </h2>
                        <div class="d-flex justify-content-end gap-2">
                            <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn btn-primary" Visible="false" OnClick="btnEditar_Click" />
                            <asp:Button ID="btnGuardar" runat="server" Text="Guardar Cambios" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" Visible="false" OnClick="btnCancelar_Click" />
                            <asp:Button ID="btnEliminarFactura" runat="server" Text="Eliminar" 
                                CssClass="btn btn-danger" 
                                Visible="false" 
                                ToolTip="Eliminar esta factura permanentemente"
                                OnClientClick="return mostrarModalEliminarFactura();" />
                            <asp:Button ID="btnRegresar" runat="server" Text="Regresar" CssClass="btn btn-secondary" OnClick="btnRegresar_Click" />
                        </div>
                    </div>

                    <h3 class="h5 mb-3">Datos Generales</h3>
                    <div class="form-grid">
                        <div>
                            <label class="form-label">Número de Factura</label>
                            <asp:TextBox ID="txtNumeroFactura" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div>
                            <label class="form-label">Estado</label>
                            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Pendiente" Value="Pendiente"></asp:ListItem>
                                <asp:ListItem Text="Pagada" Value="Pagada"></asp:ListItem>
                                <asp:ListItem Text="Vencida" Value="Vencida"></asp:ListItem>
                                <asp:ListItem Text="Eliminado" Value="Eliminado"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <h3 class="h5 mt-4 mb-3">Acreedor</h3>
                    <div class="form-grid">
                        <div>
                            <label class="form-label">País</label>
                            <asp:DropDownList ID="ddlPais" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPais_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div>
                            <label class="form-label">Acreedor</label>
                            <asp:DropDownList ID="ddlAcreedor" runat="server" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <h3 class="h5 mt-4 mb-3">Fechas</h3>
                    <div class="form-grid">
                        <div>
                            <label class="form-label">Fecha de Emisión</label>
                            <asp:TextBox ID="txtFechaEmision" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div>
                            <label class="form-label">Fecha de Recepción</label>
                            <asp:TextBox ID="txtFechaRecepcion" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div>
                            <label class="form-label">Fecha Límite de Pago</label>
                            <asp:TextBox ID="txtFechaLimitePago" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>

                    <div id="divMontos" runat="server" visible="false">
                        <h3 class="h5 mt-4 mb-3">Montos y Moneda</h3>
                        <div class="form-grid">
                            <div>
                                <label class="form-label">Moneda</label>
                                <asp:DropDownList ID="ddlMoneda" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div>
                                <label class="form-label">Monto Total</label>
                                <asp:TextBox ID="txtMontoTotal" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                            </div>
                            <div>
                                <asp:Label ID="lblMontoIgv" runat="server" Text="IVA/IGV" CssClass="form-label"></asp:Label>
                                <asp:TextBox ID="txtMontoIgv" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                                <asp:Button ID="btnCalcularImpuesto" runat="server" 
                                    Text="Calcular automáticamente" 
                                    CssClass="btn btn-sm btn-outline-secondary mt-2" 
                                    OnClick="btnCalcularImpuesto_Click"
                                    Visible="false" />
                            </div>
                            <div>
                                <label class="form-label">Monto Restante</label>
                                <asp:TextBox ID="txtMontoRestante" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                            </div>
                        </div>
                    </div>


                    <div id="divFiscal" runat="server" visible="false">
                        <h3 class="h5 mt-4 mb-3">Detalles Fiscales</h3>
                        <div class="form-grid">
                            <div>
                                <asp:Label ID="lblRegimenFiscal" runat="server" Text="Régimen Fiscal" CssClass="form-label"></asp:Label>
                                <asp:TextBox ID="txtRegimenFiscal" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div id="divTasaIva" runat="server" visible="false">
                                <label class="form-label">Tasa IVA (%)</label>
                                <asp:TextBox ID="txtTasaIva" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                            </div>
                            <div id="divOtrosTributos" runat="server" visible="false">
                                <label class="form-label">Otros Impuestos</label>
                                <asp:TextBox ID="txtOtrosTributos" runat="server" CssClass="form-control" TextMode="Number" step="0.01"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-section">
                     <div class="details-grid-header">
                        <h3 class="section-title" style="margin-bottom: 0;">Detalles de la Factura</h3>
                        <asp:Button ID="btnNuevoDetalle" runat="server" Text="Añadir Detalle" CssClass="btn btn-success" OnClick="btnNuevoDetalle_Click" />
                    </div>
                    <asp:GridView ID="gvDetallesFactura" runat="server" AutoGenerateColumns="False" CssClass="table table-hover" GridLines="None" Width="100%">
                        <Columns>
                            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                            <asp:TemplateField HeaderText="Subtotal">
                                <ItemTemplate>
                                    <%# string.Format("{0} {1:N2}", Eval("factura.moneda.codigo_iso"), Eval("subtotal")) %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Acciones">
                                <ItemTemplate>
                                    <div class="action-buttons">
                                        <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-sm btn-warning btn-icon" 
                                            CommandName="Editar" CommandArgument='<%# Eval("detalle_factura_id") %>' 
                                            OnClick="btnAccion_Click" ToolTip="Editar">
                                            <i class="fas fa-edit"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-sm btn-danger btn-icon" 
                                            CommandName="Eliminar" CommandArgument='<%# Eval("detalle_factura_id") %>' 
                                            OnClick="btnAccion_Click" ToolTip="Eliminar">
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate> 
                            </asp:TemplateField>
                        </Columns>
                         <EmptyDataTemplate>
                            <div class="text-center p-4">
                                No hay detalles para mostrar.
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <!-- Modal de Confirmación de Eliminación de Factura -->
    <div class="modal fade" id="modalEliminarFactura" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-exclamation-triangle"></i>
                        Eliminar Factura
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p><strong>¿Está completamente seguro de eliminar esta factura?</strong></p>
                    <p class="text-danger mb-0">Esta acción no se puede deshacer.</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnConfirmarEliminarFactura" runat="server" 
                        Text="Eliminar Factura" 
                        CssClass="btn btn-danger"
                        OnClick="btnEliminarFactura_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Modal de Confirmación de Eliminación de Detalle -->
    <div class="modal fade" id="modalEliminarDetalle" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-exclamation-triangle"></i>
                        Eliminar Detalle
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p>¿Está seguro de eliminar este detalle de factura?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnConfirmarEliminarDetalle" runat="server" 
                        Text="Eliminar" 
                        CssClass="btn btn-danger"
                        OnClick="btnConfirmarEliminarDetalle_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfDetalleIdEliminar" runat="server" />

    <script type="text/javascript">
        function mostrarModalEliminarFactura() {
            $('#modalEliminarFactura').modal('show');
            return false;
        }

        function mostrarModalEliminarDetalle(detalleId) {
            document.getElementById('<%= hfDetalleIdEliminar.ClientID %>').value = detalleId;
            $('#modalEliminarDetalle').modal('show');
            return false;
        }
    </script>
</asp:Content>