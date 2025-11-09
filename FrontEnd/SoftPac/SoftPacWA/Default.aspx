<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SoftPacWA.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <h1 class="mt-4">Página Principal</h1>
        <p>Bienvenido al Sistema de Pago a Acreedores.</p>
        
        <div class="card">
            <div class="card-body">
                <h2>
                    ¡Hola, <asp:Literal ID="litNombreUsuario" runat="server" />!
                </h2>
                <p>Usa el menú de la izquierda para navegar por las opciones del sistema.</p>
            </div>
        </div>
    </div>
</asp:Content>