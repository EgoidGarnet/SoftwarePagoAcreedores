<%@ Page Title="Gestión de Cuenta de Acreedor" Language="C#" MasterPageFile="~/SoftPac.Master" AutoEventWireup="true" CodeBehind="GestionDetalleAcreedor.aspx.cs" Inherits="SoftPacWA.GestionDetalleAcreedor" %>

<asp:Content ID="Head1" ContentPlaceHolderID="head" runat="server">
    <style>
        .box{background:#fff;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,.08)}
        .error-message{font-size:0.875rem;margin-top:0.25rem;display:block;}
        .form-control.is-invalid, .form-select.is-invalid{border-color:#dc3545;}
    </style>
</asp:Content>

<asp:Content ID="Body1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="pb-1"><i class="fa-solid fa-building-columns"></i> <asp:Label ID="lblTitulo" runat="server" Text="Nueva cuenta" /></h3>
    </div>

    <asp:HiddenField ID="hfAcreedorId" runat="server" />
    <asp:HiddenField ID="hfCuentaId" runat="server" />

    <div class="box p-4">
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Entidad bancaria <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlEntidad" runat="server" CssClass="form-select" />
                <asp:Label ID="lblEntidadError" runat="server" CssClass="text-danger error-message"></asp:Label>
            </div>
            <div class="col-md-6">
                <label class="form-label">Número de cuenta <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtNumero" runat="server" CssClass="form-control" MaxLength="20" placeholder="Ej: 00123456789012345678" />
                <asp:Label ID="lblNumeroError" runat="server" CssClass="text-danger error-message"></asp:Label>
            </div>
            <div class="col-md-6">
                <label class="form-label">CCI / IBAN <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtCCI" runat="server" CssClass="form-control" MaxLength="20" placeholder="Ej: 00123456789012345678" />
                <asp:Label ID="lblCCIError" runat="server" CssClass="text-danger error-message"></asp:Label>
            </div>
            <div class="col-md-3">
                <label class="form-label">Tipo de cuenta <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">--Seleccione--</asp:ListItem>
                    <asp:ListItem Value="Ahorro">Ahorro</asp:ListItem>
                    <asp:ListItem Value="Corriente">Corriente</asp:ListItem>
                    <asp:ListItem Value="Cuenta Maestra">Cuenta Maestra</asp:ListItem>
                    <asp:ListItem Value="Interbancaria">Interbancaria</asp:ListItem>
                    <asp:ListItem Value="Detracción">Detracción</asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="lblTipoError" runat="server" CssClass="text-danger error-message"></asp:Label>
            </div>
            <div class="col-md-3">
                <label class="form-label">Divisa <span class="text-danger">*</span></label>
                <asp:DropDownList ID="ddlDivisa" runat="server" CssClass="form-select" />
                <asp:Label ID="lblDivisaError" runat="server" CssClass="text-danger error-message"></asp:Label>
            </div>
        </div>
        <div class="mt-4">
            <small class="text-muted"><span class="text-danger">*</span> Campos obligatorios</small>
        </div>
        <div class="mt-3 d-flex gap-2">
            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primary" Text="Guardar" OnClick="btnGuardar_Click" />
        </div>
    </div>

</asp:Content>