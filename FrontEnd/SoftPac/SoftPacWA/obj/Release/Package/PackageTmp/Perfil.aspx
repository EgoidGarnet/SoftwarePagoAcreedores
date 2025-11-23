<%@ Page Title="Mi Perfil" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="SoftPacWA.Perfil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .profile-header {
            display: flex;
            align-items: center;
            gap: 1.5rem;
            margin-bottom: 2rem;
            flex-wrap: wrap;
        }

        .profile-avatar {
            font-size: 5rem;
            color: var(--color-secondary);
        }

        .profile-details h2 {
            margin-bottom: 0.25rem;
            color: var(--color-primary);
        }

        .profile-details .text-muted {
            font-size: 1.1rem;
        }

        .card-header-icon {
            font-size: 1.2rem;
            margin-right: 0.75rem;
            color: var(--color-primary);
        }

        .card {
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            border-radius: 12px;
        }

        .stat-card {
            text-align: center;
            padding: 1rem;
        }

        .stat-icon {
            font-size: 2rem;
            color: var(--color-secondary);
            margin-bottom: 0.5rem;
        }

        .stat-value {
            font-size: 1.75rem;
            font-weight: 700;
            color: var(--color-primary);
        }

        .stat-label {
            font-size: 0.9rem;
            color: #6c757d;
        }

        .bg-primary {
            background-color: cornflowerblue !important;
        }

        @media (max-width: 768px) {
            .profile-avatar {
                font-size: 3.5rem;
            }

            .profile-details h2 {
                font-size: 1.5rem;
            }

            .stat-value {
                font-size: 1.5rem;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-container">
        <div class="profile-header">
            <i class="fas fa-user-circle profile-avatar"></i>
            <div class="profile-details">
                <h2><asp:Literal ID="litNombreCompleto" runat="server"></asp:Literal></h2>
                <p class="text-muted mb-0"><asp:Literal ID="litCorreo" runat="server"></asp:Literal></p>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-4 mb-4">
                <div class="card shadow-sm h-100">
                    <div class="card-header d-flex align-items-center">
                        <i class="fas fa-shield-alt card-header-icon"></i>
                        <h5 class="mb-0">Permisos</h5>
                    </div>
                    <div class="card-body">
                        <strong>Países con Acceso Permitido:</strong>
                        <div class="mt-2">
                            <asp:Repeater ID="rptPaisesAcceso" runat="server">
                                <ItemTemplate>
                                    <span class="badge bg-primary fs-6 me-2 mb-2"><%# Eval("Nombre") %></span>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-8 mb-4">
                <div class="card shadow-sm">
                    <div class="card-header d-flex align-items-center">
                        <i class="fas fa-chart-line card-header-icon"></i>
                        <h5 class="mb-0">Actividad Reciente del Usuario</h5>
                    </div>
                    <div class="card-body">
                        <div class="row mb-4">
                            <div class="col-md-6 mb-3 mb-md-0">
                                <div class="card stat-card h-100">
                                    <div class="stat-icon"><i class="fas fa-file-signature"></i></div>
                                    <div class="stat-value"><asp:Literal ID="litPropuestasMes" runat="server" Text="0"></asp:Literal></div>
                                    <div class="stat-label">Propuestas creadas este mes</div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="card stat-card h-100">
                                    <div class="stat-icon"><i class="fas fa-map-marker-alt"></i></div>
                                    <div class="stat-value"><asp:Literal ID="litUltimoPais" runat="server" Text="-"></asp:Literal></div>
                                    <div class="stat-label">Último país de operación</div>
                                </div>
                            </div>
                        </div>

                        <h6 class="text-muted">ÚLTIMAS ACCIONES REALIZADAS</h6>
                        <hr />
                        <div class="table-responsive" style="overflow-x: auto; white-space: nowrap;">
                            <asp:GridView ID="gvUltimasAcciones" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-hover" GridLines="None"
                                EmptyDataText="No se han registrado acciones recientes en el sistema.">
                                <Columns>
                                    <asp:BoundField DataField="PropuestaId" HeaderText="ID Propuesta" />
                                    <asp:BoundField DataField="FechaAccion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                    <asp:TemplateField HeaderText="Tipo de Acción">
                                        <ItemTemplate>
                                            <span class='badge <%# GetActionClass(Eval("TipoAccion").ToString()) %>'><%# Eval("TipoAccion") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Estado Actual">
                                        <ItemTemplate>
                                            <span class='badge <%# GetEstadoClass(Eval("Estado").ToString()) %>'><%# Eval("Estado") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="NumFacturas" HeaderText="N° de Facturas" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="text-center p-3 text-muted">
                                        <i class="fas fa-inbox fa-2x mb-3 d-block"></i>
                                        No se han registrado acciones recientes en el sistema.
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>