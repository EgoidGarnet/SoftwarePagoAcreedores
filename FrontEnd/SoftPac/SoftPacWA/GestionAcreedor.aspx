<%@ Page Title="Gestión de Acreedor" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="GestionAcreedor.aspx.cs" Inherits="SoftPacWA.GestionAcreedor" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
    <style>.box{background:#fff;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,.08)}</style>
</asp:Content>

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="pb-1"><i class="fa-solid fa-user-pen"></i> <asp:Label ID="lblTitulo" runat="server" Text="Nuevo acreedor" /></h3>
        <asp:HyperLink ID="lnkVolver" runat="server" CssClass="btn btn-secondary btn-sm" NavigateUrl="Acreedores.aspx">Volver</asp:HyperLink>
    </div>

    <asp:HiddenField ID="hfAccion" runat="server" />
    <asp:HiddenField ID="hfId" runat="server" />

    <div class="box p-4">
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Razón social</label>
                <asp:TextBox ID="txtRazon" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">RUC</label>
                <asp:TextBox ID="txtRuc" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">País</label>
                <asp:DropDownList ID="ddlPais" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Dirección fiscal</label>
                <asp:TextBox ID="txtDir" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Condición</label>
                <asp:TextBox ID="txtCondicion" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Plazo pago (días)</label>
                <asp:TextBox ID="txtPlazo" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Activo</label>
                <asp:DropDownList ID="ddlActivo" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Sí" Value="S"></asp:ListItem>
                    <asp:ListItem Text="No" Value="N"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="mt-3 d-flex gap-2">
            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primary" Text="Guardar" OnClick="btnGuardar_Click" />
            <asp:HyperLink ID="btnCancelar" runat="server" CssClass="btn btn-secondary" NavigateUrl="Acreedores.aspx">Cancelar</asp:HyperLink>
        </div>
    </div>
</asp:Content>