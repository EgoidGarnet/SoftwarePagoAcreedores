<%@ Page Title="Carga Masiva de Facturas" Language="C#" MasterPageFile="~/SoftPac.Master" 
    AutoEventWireup="true" CodeBehind="CargaFacturas.aspx.cs" Inherits="SoftPacWA.CargaFacturas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .upload-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            margin-bottom: 1.5rem;
        }

        .upload-area {
            border: 3px dashed #667eea;
            border-radius: 12px;
            padding: 3rem;
            text-align: center;
            background: linear-gradient(135deg, #f8f9ff 0%, #f0f2ff 100%);
            transition: all 0.3s ease;
            cursor: pointer;
        }

        .upload-area:hover {
            border-color: #5568d3;
            background: linear-gradient(135deg, #f0f2ff 0%, #e8ebff 100%);
            transform: translateY(-2px);
        }

        .upload-icon {
            font-size: 4rem;
            color: #667eea;
            margin-bottom: 1rem;
        }

        .upload-text {
            font-size: 1.2rem;
            color: #2d3748;
            margin-bottom: 0.5rem;
            font-weight: 600;
        }

        .upload-hint {
            color: #718096;
            font-size: 0.9rem;
        }

        .stats-card {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.06);
            margin-bottom: 1rem;
        }

        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
        }

        .stat-item {
            text-align: center;
            padding: 1rem;
            border-radius: 8px;
            background: #f8f9fa;
        }

        .stat-value {
            font-size: 2rem;
            font-weight: 700;
            margin-bottom: 0.25rem;
        }

        .stat-label {
            font-size: 0.85rem;
            color: #718096;
            text-transform: uppercase;
        }

        .stat-item.success .stat-value { color: #28a745; }
        .stat-item.danger .stat-value { color: #dc3545; }
        .stat-item.info .stat-value { color: #17a2b8; }
        .stat-item.warning .stat-value { color: #ffc107; }

        .result-table {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.06);
        }

        .progress-section {
            margin: 1rem 0;
        }

        .progress {
            height: 30px;
            border-radius: 15px;
            background: #e9ecef;
        }

        .progress-bar {
            border-radius: 15px;
            font-size: 0.9rem;
            font-weight: 600;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .btn-process {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            color: white;
            padding: 0.75rem 2rem;
            border-radius: 10px;
            font-weight: 600;
            font-size: 1rem;
            transition: all 0.3s ease;
            box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
        }

        .btn-process:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
            color: white;
        }

        .file-info {
            background: #f8f9fa;
            border-radius: 8px;
            padding: 1rem;
            margin-top: 1rem;
            display: none;
        }

        .file-info.show {
            display: block;
        }

        .file-name {
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 0.5rem;
        }

        .file-details {
            font-size: 0.9rem;
            color: #718096;
        }

        .alert-custom {
            border-radius: 10px;
            padding: 1rem 1.5rem;
            margin-bottom: 1rem;
        }

        .acreedor-info {
            background: #e8f4fd;
            border-left: 4px solid #17a2b8;
            padding: 1rem;
            border-radius: 8px;
            margin-top: 1rem;
        }

        .file-list-item {
            padding: 0.25rem 0;
            border-bottom: 1px solid #e2e8f0;
        }

        .file-list-item:last-child {
            border-bottom: none;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(-10px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .animated {
            animation: fadeIn 0.3s ease-out;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="upCargaFacturas" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="page-title">
                <h3 class="pb-1">
                    <i class="fas fa-file-upload"></i> Carga Masiva de Facturas
                </h3>
                <p class="text-muted mb-0">Importa múltiples facturas desde archivos XML para acreedores existentes</p>
            </div>

            <!-- Sección de instrucciones -->
            <div class="alert alert-info alert-custom animated">
                <h5 class="alert-heading">
                    <i class="fas fa-info-circle"></i> Instrucciones
                </h5>
                <ul class="mb-0">
                    <li>Selecciona el país de origen de las facturas</li>
                    <li>Selecciona el acreedor para el cual cargarás facturas</li>
                    <li>Sube uno o varios archivos XML válidos (el RUC/RFC/NIT debe coincidir con el acreedor)</li>
                    <li>Puedes seleccionar múltiples archivos a la vez manteniendo presionada la tecla Ctrl o Cmd</li>
                    <li>El sistema validará y procesará todas las facturas automáticamente</li>
                </ul>
            </div>

            <!-- Selector de País y Acreedor -->
            <div class="upload-section animated">
                <h5 class="mb-3">
                    <i class="fas fa-filter"></i> Filtros de Carga
                </h5>
                <div class="row">
                    <div class="col-md-6">
                        <label class="form-label">
                            <i class="fas fa-globe-americas me-1"></i> País
                        </label>
                        <asp:DropDownList ID="ddlPais" runat="server" 
                            CssClass="form-select form-select-lg" 
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlPais_SelectedIndexChanged">
                            <asp:ListItem Value="">-- Seleccione un País --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">
                            <i class="fas fa-building me-1"></i> Acreedor
                        </label>
                        <asp:DropDownList ID="ddlAcreedor" runat="server" 
                            CssClass="form-select form-select-lg"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlAcreedor_SelectedIndexChanged">
                            <asp:ListItem Value="">-- Seleccione un Acreedor --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- Información del acreedor seleccionado -->
                <asp:Panel ID="pnlAcreedorInfo" runat="server" Visible="false" CssClass="acreedor-info">
                    <h6 class="mb-2">
                        <i class="fas fa-info-circle"></i> Información del Acreedor Seleccionado
                    </h6>
                    <div class="row">
                        <div class="col-md-6">
                            <strong>RUC/RFC/NIT:</strong> <asp:Label ID="lblAcreedorRuc" runat="server"></asp:Label>
                        </div>
                        <div class="col-md-6">
                            <strong>Razón Social:</strong> <asp:Label ID="lblAcreedorRazon" runat="server"></asp:Label>
                        </div>
                    </div>
                    <small class="text-muted mt-2 d-block">
                        <i class="fas fa-exclamation-triangle me-1"></i>
                        Los archivos XML deben contener facturas con este RUC/RFC/NIT
                    </small>
                </asp:Panel>
            </div>

            <!-- Área de carga -->
            <div class="upload-section animated">
                <h5 class="mb-3">
                    <i class="fas fa-cloud-upload-alt"></i> Subir Archivos XML
                </h5>
                
                <div class="upload-area" onclick="document.getElementById('<%= fuXmlFile.ClientID %>').click();">
                    <div class="upload-icon">
                        <i class="fas fa-file-code"></i>
                    </div>
                    <div class="upload-text">Haz clic aquí para seleccionar archivos</div>
                    <div class="upload-hint">o arrastra y suelta tus archivos XML (puedes seleccionar múltiples)</div>
                    <asp:FileUpload ID="fuXmlFile" runat="server" accept=".xml" 
                        AllowMultiple="true"
                        onchange="mostrarInfoArchivos(this);" style="display: none;" />
                </div>

                <div id="fileInfo" class="file-info">
                    <div class="file-name">
                        <i class="fas fa-file-alt me-2"></i>
                        <span id="fileCount"></span>
                    </div>
                    <div id="fileList" class="mt-2"></div>
                    <div class="file-details mt-2">
                        <i class="fas fa-weight me-1"></i>
                        <span id="fileSize"></span>
                    </div>
                </div>

                <div class="mt-3 text-center">
                    <asp:Button ID="btnProcesar" runat="server" Text="Procesar Facturas" 
                        CssClass="btn btn-process" OnClick="btnProcesar_Click" />
                </div>
            </div>

            <!-- Progreso -->
            <asp:Panel ID="pnlProgress" runat="server" Visible="false" CssClass="progress-section animated">
                <div class="upload-section">
                    <h5 class="mb-3">
                        <i class="fas fa-spinner fa-spin"></i> Procesando...
                    </h5>
                    <div class="progress">
                        <div class="progress-bar progress-bar-striped progress-bar-animated bg-primary" 
                            role="progressbar" style="width: 100%">
                            Analizando archivos XML y creando facturas...
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Resultados -->
            <asp:Panel ID="pnlResultados" runat="server" Visible="false" CssClass="animated">
                <div class="stats-card">
                    <h5 class="mb-3">
                        <i class="fas fa-chart-bar"></i> Resumen del Proceso
                    </h5>
                    <div class="stats-grid">
                        <div class="stat-item info">
                            <div class="stat-value">
                                <asp:Label ID="lblTotalProcesadas" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="stat-label">Total Procesadas</div>
                        </div>
                        <div class="stat-item success">
                            <div class="stat-value">
                                <asp:Label ID="lblExitosas" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="stat-label">Exitosas</div>
                        </div>
                        <div class="stat-item danger">
                            <div class="stat-value">
                                <asp:Label ID="lblFallidas" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="stat-label">Fallidas</div>
                        </div>
                        <div class="stat-item warning">
                            <div class="stat-value">
                                <asp:Label ID="lblDuplicadas" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="stat-label">Duplicadas</div>
                        </div>
                    </div>
                </div>

                <!-- Tabla de resultados detallados -->
                <div class="result-table">
                    <h5 class="mb-3">
                        <i class="fas fa-list-alt"></i> Detalle de Resultados
                    </h5>
                    <div class="table-responsive">
                        <asp:GridView ID="gvResultados" runat="server" CssClass="table table-hover" 
                            AutoGenerateColumns="False" GridLines="None">
                            <Columns>
                                <asp:TemplateField HeaderText="Archivo">
                                    <ItemTemplate>
                                        <small class="text-muted"><%# Eval("Archivo") %></small>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='badge bg-<%# GetEstadoBadge(Eval("Estado").ToString()) %>'>
                                            <%# Eval("Estado") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="NumeroFactura" HeaderText="Número Factura" />
                                <asp:BoundField DataField="RazonSocial" HeaderText="Acreedor" />
                                <asp:BoundField DataField="MontoTotal" HeaderText="Monto" DataFormatString="{0:N2}" />
                                <asp:BoundField DataField="Mensaje" HeaderText="Mensaje" />
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="text-center p-4">
                                    No hay resultados para mostrar
                                </div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </div>

                <div class="mt-3 text-center">
                    <asp:Button ID="btnNuevaCarga" runat="server" Text="Nueva Carga" 
                        CssClass="btn btn-primary" OnClick="btnNuevaCarga_Click" />
                    <asp:Button ID="btnVerFacturas" runat="server" Text="Ver Facturas" 
                        CssClass="btn btn-secondary" OnClick="btnVerFacturas_Click" />
                </div>
            </asp:Panel>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnProcesar" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function mostrarInfoArchivos(input) {
            var fileInfo = document.getElementById('fileInfo');
            var fileCount = document.getElementById('fileCount');
            var fileList = document.getElementById('fileList');
            var fileSize = document.getElementById('fileSize');

            if (input.files && input.files.length > 0) {
                var totalSize = 0;
                var filesHtml = '';

                fileCount.textContent = input.files.length + ' archivo(s) seleccionado(s)';

                for (var i = 0; i < input.files.length; i++) {
                    var file = input.files[i];
                    totalSize += file.size;

                    var fileItemSize = '';
                    if (file.size < 1024) {
                        fileItemSize = file.size + ' bytes';
                    } else if (file.size < 1024 * 1024) {
                        fileItemSize = (file.size / 1024).toFixed(2) + ' KB';
                    } else {
                        fileItemSize = (file.size / (1024 * 1024)).toFixed(2) + ' MB';
                    }

                    filesHtml += '<div class="file-list-item">' +
                        '<i class="fas fa-file-code text-primary me-2"></i>' +
                        '<strong>' + file.name + '</strong>' +
                        ' <span class="text-muted">(' + fileItemSize + ')</span>' +
                        '</div>';
                }

                fileList.innerHTML = filesHtml;

                var totalSizeText = '';
                if (totalSize < 1024) {
                    totalSizeText = totalSize + ' bytes';
                } else if (totalSize < 1024 * 1024) {
                    totalSizeText = (totalSize / 1024).toFixed(2) + ' KB';
                } else {
                    totalSizeText = (totalSize / (1024 * 1024)).toFixed(2) + ' MB';
                }
                fileSize.textContent = 'Tamaño total: ' + totalSizeText;

                fileInfo.classList.add('show');
            }
        }

        // Drag and drop functionality
        var uploadArea = document.querySelector('.upload-area');
        if (uploadArea) {
            uploadArea.addEventListener('dragover', function (e) {
                e.preventDefault();
                this.style.borderColor = '#5568d3';
                this.style.background = 'linear-gradient(135deg, #e8ebff 0%, #dde0ff 100%)';
            });

            uploadArea.addEventListener('dragleave', function (e) {
                e.preventDefault();
                this.style.borderColor = '#667eea';
                this.style.background = 'linear-gradient(135deg, #f8f9ff 0%, #f0f2ff 100%)';
            });

            uploadArea.addEventListener('drop', function (e) {
                e.preventDefault();
                this.style.borderColor = '#667eea';
                this.style.background = 'linear-gradient(135deg, #f8f9ff 0%, #f0f2ff 100%)';

                var fileInput = document.getElementById('<%= fuXmlFile.ClientID %>');
                fileInput.files = e.dataTransfer.files;
                mostrarInfoArchivos(fileInput);
            });
        }
    </script>
</asp:Content>
