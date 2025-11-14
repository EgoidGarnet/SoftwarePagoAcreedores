-- =============================================
-- SQL Server DML - Data Insert Script (CORRECTED)
-- Database: softpacbd
-- =============================================

USE softpacbd;
GO

SET NOCOUNT ON;
GO

-- =============================================
-- Table: PA_PAISES
-- Inserting 3 countries: Peru, Mexico, Colombia
-- =============================================
PRINT 'Inserting PA_PAISES...';

SET IDENTITY_INSERT [dbo].[PA_PAISES] ON;

INSERT INTO [dbo].[PA_PAISES] ([PAIS_ID], [NOMBRE], [CODIGO_ISO], [CODIGO_TELEFONICO])
VALUES
    (1, 'Peru', 'PE', '+51'),
    (2, 'Mexico', 'MX', '+52'),
    (3, 'Colombia', 'CO', '+57');

SET IDENTITY_INSERT [dbo].[PA_PAISES] OFF;

PRINT 'PA_PAISES inserted successfully: 3 records';
GO

-- =============================================
-- Table: PA_MONEDAS
-- Inserting 5 currencies
-- =============================================
PRINT 'Inserting PA_MONEDAS...';

SET IDENTITY_INSERT [dbo].[PA_MONEDAS] ON;

INSERT INTO [dbo].[PA_MONEDAS] ([MONEDA_ID], [NOMBRE], [CODIGO_ISO], [SIMBOLO])
VALUES
    (1, 'Sol Peruano', 'PEN', 'S/'),
    (2, 'Dólar Estadounidense', 'USD', '$'),
    (3, 'Peso Mexicano', 'MXN', 'MX$'),
    (4, 'Peso Colombiano', 'COP', 'CO$'),
    (5, 'Euro', 'EUR', '€');

SET IDENTITY_INSERT [dbo].[PA_MONEDAS] OFF;

PRINT 'PA_MONEDAS inserted successfully: 5 records';
GO

-- =============================================
-- Table: PA_ENTIDADES_BANCARIAS
-- Inserting 10 banking entities
-- =============================================
PRINT 'Inserting PA_ENTIDADES_BANCARIAS...';

SET IDENTITY_INSERT [dbo].[PA_ENTIDADES_BANCARIAS] ON;

INSERT INTO [dbo].[PA_ENTIDADES_BANCARIAS] 
    ([ENTIDAD_BANCARIA_ID], [NOMBRE], [FORMATO_ACEPTADO], [CODIGO_SWIFT], [PAIS_ID])
VALUES
    -- Peru (4 banks)
    (1, 'Banco de Crédito del Perú', 'EXCEL, CSV, TXT', 'BCPLPEPL', 1),
    (2, 'Interbank', 'EXCEL, CSV', 'BINPPEPL', 1),
    (3, 'BBVA Perú', 'CSV, TXT', 'BCONPEPL', 1),
    (4, 'Scotiabank Perú', 'EXCEL, CSV', 'BSUDPEPL', 1),
    -- Mexico (3 banks)
    (5, 'BBVA Bancomer', 'EXCEL, CSV, TXT', 'BCMRMXMM', 2),
    (6, 'Santander México', 'CSV, TXT', 'BMSXMXMM', 2),
    (7, 'Banorte', 'EXCEL, CSV', 'MENOMXMT', 2),
    -- Colombia (3 banks)
    (8, 'Bancolombia', 'EXCEL, CSV, TXT', 'COLOCOBM', 3),
    (9, 'Banco de Bogotá', 'CSV, TXT', 'BBOPCOBM', 3),
    (10, 'Davivienda', 'EXCEL, CSV', 'CAFECOBM', 3);

SET IDENTITY_INSERT [dbo].[PA_ENTIDADES_BANCARIAS] OFF;

PRINT 'PA_ENTIDADES_BANCARIAS inserted successfully: 10 records';
GO

-- =============================================
-- Table: PA_ACREEDORES
-- Inserting 10 creditors (CORRECTED RUC for Mexico)
-- =============================================
PRINT 'Inserting PA_ACREEDORES...';

SET IDENTITY_INSERT [dbo].[PA_ACREEDORES] ON;

INSERT INTO [dbo].[PA_ACREEDORES] 
    ([ACREEDOR_ID], [RAZON_SOCIAL], [RUC], [DIRECCION_FISCAL], [CONDICION], [PLAZO_DE_PAGO], [ACTIVO], [PAIS_ID], [FECHA_ELIMINACION], [USUARIO_ELIMINACION])
VALUES
    -- Peru (4 creditors) - RUC 11 characters
    (1, 'Distribuidora Comercial Los Andes S.A.C.', '20512345678', 'Av. Arequipa 1234, Lima', 'Habido', 30, 'S', 1, NULL, NULL),
    (2, 'Importaciones y Exportaciones del Sur S.A.', '20587654321', 'Jr. Cusco 567, Arequipa', 'Habido', 45, 'S', 1, NULL, NULL),
    (3, 'Servicios Logísticos Pacífico E.I.R.L.', '20534567890', 'Av. La Marina 890, Callao', 'Habido', 60, 'S', 1, NULL, NULL),
    (4, 'Tecnología y Equipos Industriales S.A.C.', '20598765432', 'Calle Los Talleres 456, Lima', 'Habido', 30, 'S', 1, NULL, NULL),
    -- Mexico (3 creditors) - RFC 11 characters (adjusted)
    (5, 'Comercializadora Azteca S.A. de C.V.', 'AAM95010141', 'Av. Reforma 2345, Ciudad de México', 'Activo', 30, 'S', 2, NULL, NULL),
    (6, 'Proveedora Industrial del Norte S.A. de C.V.', 'PIN95031542', 'Calle Industria 123, Monterrey', 'Activo', 45, 'S', 2, NULL, NULL),
    (7, 'Distribuidora Maya S. de R.L. de C.V.', 'DMA96042043', 'Av. Universidad 789, Guadalajara', 'Activo', 60, 'S', 2, NULL, NULL),
    -- Colombia (3 creditors) - NIT 11 characters
    (8, 'Distribuciones Andinas S.A.S.', '90012345678', 'Carrera 7 #45-67, Bogotá', 'Activo', 30, 'S', 3, NULL, NULL),
    (9, 'Comercial Caribe Ltda.', '90087654321', 'Calle 100 #25-30, Barranquilla', 'Activo', 45, 'S', 3, NULL, NULL),
    (10, 'Suministros Empresariales Colombia S.A.S.', '90034567890', 'Av. El Poblado #12-34, Medellín', 'Activo', 60, 'S', 3, NULL, NULL);

SET IDENTITY_INSERT [dbo].[PA_ACREEDORES] OFF;

PRINT 'PA_ACREEDORES inserted successfully: 10 records';
GO

-- =============================================
-- Table: PA_CUENTAS_ACREEDOR
-- Inserting 20 creditor accounts (2 per creditor)
-- =============================================
PRINT 'Inserting PA_CUENTAS_ACREEDOR...';

SET IDENTITY_INSERT [dbo].[PA_CUENTAS_ACREEDOR] ON;

INSERT INTO [dbo].[PA_CUENTAS_ACREEDOR] 
    ([CUENTA_ACREEDOR_ID], [TIPO_CUENTA], [NUMERO_CUENTA], [CCI], [ACTIVO], [ACREEDOR_ID], [ENTIDAD_BANCARIA_ID], [MONEDA_ID], [FECHA_ELIMINACION], [USUARIO_ELIMINACION])
VALUES
    -- Acreedor 1 (Peru)
    (1, 'Corriente', '19100234567801', '00219110023456780145', 'S', 1, 1, 1, NULL, NULL),
    (2, 'Ahorros', '19100234567802', '00219110023456780246', 'S', 1, 1, 2, NULL, NULL),
    -- Acreedor 2 (Peru)
    (3, 'Corriente', '30345678901234', '00330334567890123478', 'S', 2, 2, 1, NULL, NULL),
    (4, 'Ahorros', '30345678901235', '00330334567890123579', 'S', 2, 2, 2, NULL, NULL),
    -- Acreedor 3 (Peru)
    (5, 'Corriente', '01145678901234', '00201114567890123456', 'S', 3, 3, 1, NULL, NULL),
    (6, 'Ahorros', '01145678901235', '00201114567890123557', 'S', 3, 3, 2, NULL, NULL),
    -- Acreedor 4 (Peru)
    (7, 'Corriente', '00978912345678', '00900997891234567823', 'S', 4, 4, 1, NULL, NULL),
    (8, 'Ahorros', '00978912345679', '00900997891234567924', 'S', 4, 4, 2, NULL, NULL),
    -- Acreedor 5 (Mexico)
    (9, 'Corriente', '01234567890123456789', '01401234567890123456789012345678', 'S', 5, 5, 3, NULL, NULL),
    (10, 'Ahorros', '01234567890123456790', '01401234567890123456790123456789', 'S', 5, 5, 2, NULL, NULL),
    -- Acreedor 6 (Mexico)
    (11, 'Corriente', '65432109876543210987', '01465432109876543210987654321098', 'S', 6, 6, 3, NULL, NULL),
    (12, 'Ahorros', '65432109876543210988', '01465432109876543210988765432109', 'S', 6, 6, 2, NULL, NULL),
    -- Acreedor 7 (Mexico)
    (13, 'Corriente', '11223344556677889900', '07211223344556677889900112233445', 'S', 7, 7, 3, NULL, NULL),
    (14, 'Ahorros', '11223344556677889901', '07211223344556677889901223344556', 'S', 7, 7, 2, NULL, NULL),
    -- Acreedor 8 (Colombia)
    (15, 'Corriente', '12345678901234567890', '00712345678901234567890123456789', 'S', 8, 8, 4, NULL, NULL),
    (16, 'Ahorros', '12345678901234567891', '00712345678901234567891234567890', 'S', 8, 8, 2, NULL, NULL),
    -- Acreedor 9 (Colombia)
    (17, 'Corriente', '98765432109876543210', '00998765432109876543210987654321', 'S', 9, 9, 4, NULL, NULL),
    (18, 'Ahorros', '98765432109876543211', '00998765432109876543211098765432', 'S', 9, 9, 2, NULL, NULL),
    -- Acreedor 10 (Colombia)
    (19, 'Corriente', '55667788990011223344', '00755667788990011223344556677889', 'S', 10, 10, 4, NULL, NULL),
    (20, 'Ahorros', '55667788990011223345', '00755667788990011223345667788990', 'S', 10, 10, 2, NULL, NULL);

SET IDENTITY_INSERT [dbo].[PA_CUENTAS_ACREEDOR] OFF;

PRINT 'PA_CUENTAS_ACREEDOR inserted successfully: 20 records';
GO

-- =============================================
-- Table: PA_CUENTAS_PROPIAS
-- Inserting 10 own accounts
-- =============================================
PRINT 'Inserting PA_CUENTAS_PROPIAS...';

SET IDENTITY_INSERT [dbo].[PA_CUENTAS_PROPIAS] ON;

INSERT INTO [dbo].[PA_CUENTAS_PROPIAS] 
    ([CUENTA_PROPIA_ID], [SALDO_DISPONIBLE], [TIPO_CUENTA], [NUMERO_CUENTA], [CCI], [ACTIVO], [ENTIDAD_BANCARIA_ID], [MONEDA_ID], [FECHA_ELIMINACION], [USUARIO_ELIMINACION])
VALUES
    -- Peru accounts
    (1, 250000.00, 'Corriente', '19199887766554433', '00219119988776655443312', 'S', 1, 1, NULL, NULL),
    (2, 180000.00, 'Corriente', '19199887766554434', '00219119988776655443413', 'S', 1, 2, NULL, NULL),
    (3, 320000.00, 'Ahorros', '30312345678909876', '00330331234567890987654', 'S', 2, 1, NULL, NULL),
    -- Mexico accounts
    (4, 450000.00, 'Corriente', '99887766554433221100', '01499887766554433221100998877665', 'S', 5, 3, NULL, NULL),
    (5, 280000.00, 'Corriente', '99887766554433221101', '01499887766554433221101098877666', 'S', 5, 2, NULL, NULL),
    (6, 510000.00, 'Ahorros', '11223344556677889922', '01411223344556677889922112233445', 'S', 6, 3, NULL, NULL),
    -- Colombia accounts
    (7, 850000000.00, 'Corriente', '33445566778899001122', '00733445566778899001122334455667', 'S', 8, 4, NULL, NULL),
    (8, 620000.00, 'Corriente', '33445566778899001123', '00733445566778899001123445566778', 'S', 8, 2, NULL, NULL),
    (9, 920000000.00, 'Ahorros', '77889900112233445566', '00977889900112233445566778899001', 'S', 9, 4, NULL, NULL),
    -- Euro account
    (10, 85000.00, 'Ahorros', '01198765432109876543', '00201119876543210987654321098765', 'S', 3, 5, NULL, NULL);

SET IDENTITY_INSERT [dbo].[PA_CUENTAS_PROPIAS] OFF;

PRINT 'PA_CUENTAS_PROPIAS inserted successfully: 10 records';
GO

-- =============================================
-- Table: PA_FACTURAS
-- Inserting 10 invoices
-- =============================================
PRINT 'Inserting PA_FACTURAS...';

SET IDENTITY_INSERT [dbo].[PA_FACTURAS] ON;

INSERT INTO [dbo].[PA_FACTURAS] 
    ([FACTURA_ID], [NUMERO_FACTURA], [FECHA_EMISION], [FECHA_RECEPCION], [FECHA_LIMITE_PAGO], [ESTADO], 
     [MONTO_TOTAL], [MONTO_IGV], [MONTO_RESTANTE], [REGIMEN_FISCAL], [TASA_IVA], [OTROS_TRIBUTOS], 
     [MONEDA_ID], [ACREEDOR_ID], [FECHA_ELIMINACION], [USUARIO_ELIMINACION])
VALUES
    -- Facturas Peru
    (1, 'F001-00001234', '2025-01-15 10:30:00', '2025-01-16 14:20:00', '2025-02-14 23:59:59', 'PENDIENTE', 
     11800.00, 1800.00, 11800.00, 'Régimen General', 18.00, 0.00, 1, 1, NULL, NULL),
    
    (2, 'F001-00001235', '2025-01-20 09:15:00', '2025-01-21 11:30:00', '2025-03-06 23:59:59', 'PENDIENTE', 
     23600.00, 3600.00, 23600.00, 'Régimen General', 18.00, 0.00, 1, 2, NULL, NULL),
    
    (3, 'F001-00001236', '2025-02-01 15:45:00', '2025-02-02 10:20:00', '2025-04-03 23:59:59', 'PENDIENTE', 
     5900.00, 900.00, 5900.00, 'Régimen General', 18.00, 0.00, 2, 3, NULL, NULL),
    
    (4, 'F002-00002345', '2025-02-10 11:00:00', '2025-02-11 16:40:00', '2025-03-12 23:59:59', 'PENDIENTE', 
     17700.00, 2700.00, 17700.00, 'Régimen General', 18.00, 0.00, 1, 4, NULL, NULL),
    
    -- Facturas Mexico
    (5, 'FAC-MX-2025-001', '2025-01-18 12:30:00', '2025-01-19 09:15:00', '2025-02-17 23:59:59', 'PENDIENTE', 
     58000.00, 9280.00, 58000.00, 'Régimen General', 16.00, 0.00, 3, 5, NULL, NULL),
    
    (6, 'FAC-MX-2025-002', '2025-01-25 14:20:00', '2025-01-26 10:30:00', '2025-03-11 23:59:59', 'PENDIENTE', 
     34800.00, 5568.00, 34800.00, 'Régimen General', 16.00, 0.00, 3, 6, NULL, NULL),
    
    (7, 'FAC-MX-2025-003', '2025-02-05 10:00:00', '2025-02-06 15:45:00', '2025-04-07 23:59:59', 'PENDIENTE', 
     87000.00, 13920.00, 87000.00, 'Régimen General', 16.00, 0.00, 2, 7, NULL, NULL),
    
    -- Facturas Colombia
    (8, 'FCOL-2025-0001', '2025-01-12 08:45:00', '2025-01-13 13:20:00', '2025-02-11 23:59:59', 'PENDIENTE', 
     9500000.00, 1805000.00, 9500000.00, 'Régimen Común', 19.00, 0.00, 4, 8, NULL, NULL),
    
    (9, 'FCOL-2025-0002', '2025-01-28 13:15:00', '2025-01-29 11:40:00', '2025-03-14 23:59:59', 'PENDIENTE', 
     15960000.00, 3032400.00, 15960000.00, 'Régimen Común', 19.00, 0.00, 4, 9, NULL, NULL),
    
    (10, 'FCOL-2025-0003', '2025-02-08 16:30:00', '2025-02-09 09:50:00', '2025-04-10 23:59:59', 'PENDIENTE', 
     23800.00, 4522.00, 23800.00, 'Régimen Común', 19.00, 0.00, 2, 10, NULL, NULL);

SET IDENTITY_INSERT [dbo].[PA_FACTURAS] OFF;

PRINT 'PA_FACTURAS inserted successfully: 10 records';
GO

-- =============================================
-- Table: PA_DETALLES_FACTURA
-- Inserting 20 invoice details (2 per invoice)
-- =============================================
PRINT 'Inserting PA_DETALLES_FACTURA...';

SET IDENTITY_INSERT [dbo].[PA_DETALLES_FACTURA] ON;

INSERT INTO [dbo].[PA_DETALLES_FACTURA] 
    ([DETALLE_FACTURA_ID], [SUBTOTAL], [DESCRIPCION], [FACTURA_ID], [FECHA_ELIMINACION], [USUARIO_ELIMINACION])
VALUES
    -- Detalles Factura 1
    (1, 6000.00, 'Suministro de equipos de cómputo - 10 laptops HP ProBook 450', 1, NULL, NULL),
    (2, 4000.00, 'Servicio de instalación y configuración de red empresarial', 1, NULL, NULL),
    
    -- Detalles Factura 2
    (3, 12000.00, 'Importación de maquinaria industrial - Cortadora CNC modelo X500', 2, NULL, NULL),
    (4, 8000.00, 'Servicio de transporte y logística internacional', 2, NULL, NULL),
    
    -- Detalles Factura 3
    (5, 3000.00, 'Servicio de almacenamiento por 3 meses', 3, NULL, NULL),
    (6, 2000.00, 'Distribución y entrega a sucursales nacionales', 3, NULL, NULL),
    
    -- Detalles Factura 4
    (7, 9000.00, 'Equipos industriales - Compresor de aire 50HP', 4, NULL, NULL),
    (8, 6000.00, 'Repuestos y accesorios para maquinaria', 4, NULL, NULL),
    
    -- Detalles Factura 5
    (9, 30000.00, 'Comercialización de productos electrónicos - Smart TVs 55" (20 unidades)', 5, NULL, NULL),
    (10, 20000.00, 'Accesorios y garantía extendida para productos', 5, NULL, NULL),
    
    -- Detalles Factura 6
    (11, 18000.00, 'Suministro industrial - Materiales de construcción', 6, NULL, NULL),
    (12, 12000.00, 'Herramientas especializadas para construcción', 6, NULL, NULL),
    
    -- Detalles Factura 7
    (13, 50000.00, 'Distribución de productos farmacéuticos', 7, NULL, NULL),
    (14, 25000.00, 'Servicio de cadena de frío y transporte especializado', 7, NULL, NULL),
    
    -- Detalles Factura 8
    (15, 5000000.00, 'Distribución de productos de consumo masivo - 500 cajas mixtas', 8, NULL, NULL),
    (16, 2927966.10, 'Servicios de merchandising y exhibición en puntos de venta', 8, NULL, NULL),
    
    -- Detalles Factura 9
    (17, 8000000.00, 'Suministro de materiales de oficina para empresas - Pedido anual', 9, NULL, NULL),
    (18, 5411764.71, 'Servicio de logística y distribución trimestral', 9, NULL, NULL),
    
    -- Detalles Factura 10
    (19, 12000.00, 'Equipamiento tecnológico - Servidores Dell PowerEdge', 10, NULL, NULL),
    (20, 8000.00, 'Licencias de software empresarial y soporte técnico anual', 10, NULL, NULL);

SET IDENTITY_INSERT [dbo].[PA_DETALLES_FACTURA] OFF;

PRINT 'PA_DETALLES_FACTURA inserted successfully: 20 records';
GO

-- =============================================
-- Summary Report
-- =============================================
PRINT '';
PRINT '=============================================';
PRINT 'DATA INSERTION COMPLETED SUCCESSFULLY';
PRINT '=============================================';
PRINT '';
PRINT 'Summary of inserted records:';
PRINT '- PA_PAISES: 3 records (Peru, Mexico, Colombia)';
PRINT '- PA_MONEDAS: 5 records (PEN, USD, MXN, COP, EUR)';
PRINT '- PA_ENTIDADES_BANCARIAS: 10 records';
PRINT '- PA_ACREEDORES: 10 records';
PRINT '- PA_CUENTAS_ACREEDOR: 20 records (2 per creditor)';
PRINT '- PA_CUENTAS_PROPIAS: 10 records';
PRINT '- PA_FACTURAS: 10 records';
PRINT '- PA_DETALLES_FACTURA: 20 records (2 per invoice)';
PRINT '';
PRINT 'Database is ready for use!';
PRINT '=============================================';
GO

-- =============================================
-- Verification Queries (Optional)
-- =============================================
/*
-- Uncomment to verify data insertion

SELECT 'PA_PAISES' AS Tabla, COUNT(*) AS Total FROM PA_PAISES
UNION ALL
SELECT 'PA_MONEDAS', COUNT(*) FROM PA_MONEDAS
UNION ALL
SELECT 'PA_ENTIDADES_BANCARIAS', COUNT(*) FROM PA_ENTIDADES_BANCARIAS
UNION ALL
SELECT 'PA_ACREEDORES', COUNT(*) FROM PA_ACREEDORES
UNION ALL
SELECT 'PA_CUENTAS_ACREEDOR', COUNT(*) FROM PA_CUENTAS_ACREEDOR
UNION ALL
SELECT 'PA_CUENTAS_PROPIAS', COUNT(*) FROM PA_CUENTAS_PROPIAS
UNION ALL
SELECT 'PA_FACTURAS', COUNT(*) FROM PA_FACTURAS
UNION ALL
SELECT 'PA_DETALLES_FACTURA', COUNT(*) FROM PA_DETALLES_FACTURA;
*/
GO