<%@ Page Title="Mi Perfil" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="SoftPacWA.Perfil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .profile-header {
            display: flex;
            align-items: center;
            gap: 1.5rem;
            margin-bottom: 2rem;
        }
        .profile-avatar {
            font-size: 5rem;
            color: var(--color-secondary);
        }
        .profile-details h2 {
            margin-bottom: 0.25rem;
        }
        .profile-details .text-muted {
            font-size: 1.1rem;
        }
        .card-header-icon {
            font-size: 1.2rem;
            margin-right: 0.75rem;
            color: var(--color-primary);
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="profile-header">
            <i class="fas fa-user-circle profile-avatar"></i>
            <div class="profile-details">
                <h2><asp:Literal ID="litNombreCompleto" runat="server"></asp:Literal></h2>
                <p class="text-muted"><asp:Literal ID="litCorreo" runat="server"></asp:Literal></p>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-4">
                <div class="card shadow-sm mb-4">
                    <div class="card-header d-flex align-items-center">
                        <i class="fas fa-shield-alt card-header-icon"></i>
                        <h5 class="mb-0">Permisos</h5>
                    </div>
                    <div class="card-body">
                        <strong>Países con Acceso Permitido:</strong>
                        <div class="mt-2">
                             <asp:Repeater ID="rptPaisesAcceso" runat="server">
                                <ItemTemplate>
                                    <span class="badge bg-primary fs-6 me-2"><%# Eval("Nombre") %></span>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>