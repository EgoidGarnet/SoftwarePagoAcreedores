<%@ Page Title="Gestión de Cuenta de Acreedor" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="GestionDetalleAcreedor.aspx.cs" Inherits="SoftPacWA.GestionDetalleAcreedor" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
    <style>.box{background:#fff;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,.08)}</style>
</asp:Content>

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="pb-1"><i class="fa-solid fa-building-columns"></i> <asp:Label ID="lblTitulo" runat="server" Text="Nueva cuenta" /></h3>
        <asp:HyperLink ID="lnkVolver" runat="server" CssClass="btn btn-secondary btn-sm" NavigateUrl="#">Volver</asp:HyperLink>
    </div>

    <asp:HiddenField ID="hfAcreedorId" runat="server" />
    <asp:HiddenField ID="hfCuentaId" runat="server" />

    <div class="box p-4">
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Entidad bancaria</label>
                <asp:TextBox ID="txtEntidad" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label class="form-label">Número de cuenta</label>
                <asp:TextBox ID="txtNumero" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label class="form-label">CCI / IBAN</label>
                <asp:TextBox ID="txtCCI" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Tipo</label>
                <asp:TextBox ID="txtTipo" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Divisa</label>
                <asp:TextBox ID="txtDivisa" runat="server" CssClass="form-control" />
            </div>
        </div>
        <div class="mt-3 d-flex gap-2">
            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primary" Text="Guardar" OnClick="btnGuardar_Click" />
            <asp:HyperLink ID="btnCancelar" runat="server" CssClass="btn btn-secondary" NavigateUrl="#">Cancelar</asp:HyperLink>
        </div>
    </div>
</asp:Content>
