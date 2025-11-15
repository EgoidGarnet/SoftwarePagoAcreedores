<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReporteDashboard.aspx.cs" Inherits="SoftPacWA.ReporteDashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Generando Reporte PDF - SoftPac</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            overflow: hidden;
        }
        
        .loader-container {
            text-align: center;
            color: white;
            animation: fadeIn 0.5s ease-in;
        }
        
        .spinner {
            border: 6px solid rgba(255,255,255,0.2);
            border-radius: 50%;
            border-top: 6px solid white;
            border-right: 6px solid white;
            width: 80px;
            height: 80px;
            animation: spin 1s linear infinite;
            margin: 0 auto 30px;
            box-shadow: 0 0 20px rgba(255,255,255,0.3);
        }
        
        .loader-container h2 {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 15px;
            text-shadow: 0 2px 10px rgba(0,0,0,0.2);
        }
        
        .loader-container p {
            font-size: 16px;
            opacity: 0.9;
        }
        
        .icon {
            font-size: 48px;
            margin-bottom: 20px;
            animation: pulse 2s ease-in-out infinite;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-20px); }
            to { opacity: 1; transform: translateY(0); }
        }
        
        @keyframes pulse {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.1); }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="loader-container">
            <div class="icon">📊</div>
            <div class="spinner"></div>
            <h2>Generando Reporte PDF</h2>
            <p>Preparando análisis estadístico...</p>
            <p style="margin-top: 10px; font-size: 14px; opacity: 0.8;">
                Por favor espere, esto puede tomar unos segundos
            </p>
        </div>
    </form>
</body>
</html>