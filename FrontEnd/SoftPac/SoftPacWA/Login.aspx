<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SoftPacWA.Login" %>

<!DOCTYPE html>

<html lang="es">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Iniciar Sesión - SoftPac</title>

    <link href="Content/Fonts/css/all.min.css" rel="stylesheet" />
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <script src="Scripts/bootstrap.bundle.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <script src="Scripts/jquery-3.7.1.min.js"></script>

    <style>
        :root {
            --color-primary: #0B1F34;
            --color-secondary: #60748A;
            --color-accent: #5E4923;
            --color-light-1: #CED6DE;
            --color-light-2: #DED8CE;
            --color-neutral: #A3957D;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f8f9fa;
            color: var(--color-primary);
        }

        .login-container {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .card {
            border: none;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 8px 20px rgba(0,0,0,0.12);
        }

        .card-header {
            background-color: var(--color-primary);
            color: #fff;
            text-align: center;
            padding: 1.5rem;
        }

            .card-header h3 {
                font-weight: 600;
                margin: 0;
            }

        .card-body {
            background-color: #fff;
        }

        .input-group-text {
            background-color: var(--color-light-1);
            border: none;
            color: var(--color-primary);
        }

        .form-control {
            border: 1px solid var(--color-light-1);
            border-left: none;
        }

        .btn-primary {
            background-color: var(--color-primary);
            border: none;
        }

            .btn-primary:hover {
                background-color: var(--color-secondary);
            }

        .text-danger {
            font-size: 0.9rem;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container login-container">
            <div class="col-md-5 col-lg-4">
                <div class="card">
                    <div class="card-header">
                        <h3><i class="fas fa-file-invoice-dollar me-2"></i>SoftPac S.A.C</h3>
                    </div>
                    <div class="card-body p-4">
                        <p class="text-center text-muted mb-4">Ingrese sus credenciales para continuar</p>

                        <div class="input-group mb-3">
                            <span class="input-group-text"><i class="fas fa-user"></i></span>
                            <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Nombre de Usuario"></asp:TextBox>
                        </div>

                        <div class="input-group mb-4">
                            <span class="input-group-text"><i class="fas fa-lock"></i></span>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Contraseña" TextMode="Password"></asp:TextBox>
                        </div>

                        <div class="d-grid">
                            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btn btn-primary" OnClick="btnLogin_Click" />
                        </div>

                        <div class="mt-3 text-center">
                            <asp:Label ID="lblMensajeError" runat="server" CssClass="text-danger" Visible="false"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <footer class="text-center mt-4 text-muted" style="font-size: 0.85rem;">
            © 2025 SoftwarePagoAcreedores - Todos los derechos reservados
        </footer>

    </form>

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
