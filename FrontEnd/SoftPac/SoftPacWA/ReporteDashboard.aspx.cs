using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using SoftPac.Business;
using SoftPacBusiness.UsuariosWS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Font = iTextSharp.text.Font;
using ListItem = iTextSharp.text.ListItem;
using Rectangle = iTextSharp.text.Rectangle;

namespace SoftPacWA
{
    public partial class ReporteDashboard : System.Web.UI.Page
    {
        private FacturasBO facturasBO = new FacturasBO();
        private CuentasPropiasBO cuentasBO = new CuentasPropiasBO();
        private PropuestasPagoBO propuestasBO = new PropuestasPagoBO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            try
            {
                GenerarReportePDF();
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Error al generar el reporte: {ex.Message}'); window.close();</script>");
            }
        }

        private void GenerarReportePDF()
        {
            // Obtener usuario logueado
            usuariosDTO usuario = (usuariosDTO)Session["UsuarioLogueado"];

            // Obtener países del usuario
            var paisesUsuario = usuario.usuario_pais
                .Where(up => up.acceso == true)
                .Select(up => up.pais.pais_id)
                .ToList();

            // Obtener datos
            var todasFacturas = facturasBO.ListarTodos()
                .Where(f => f.acreedor != null &&
                            f.acreedor.pais != null &&
                            paisesUsuario.Contains(f.acreedor.pais.pais_id))
                .ToList();

            var todasCuentas = cuentasBO.ListarTodos().ToList();
            var todasPropuestas = propuestasBO.ListarUltimasPorUsuario(usuario.usuario_id, 100);

            // Crear documento PDF
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            MemoryStream memoryStream = new MemoryStream();

            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                AgregarEncabezado(document, usuario);

                AgregarResumenEjecutivo(document, todasFacturas, todasCuentas, todasPropuestas);

                document.NewPage();
                AgregarAnalisisFacturas(document, todasFacturas);

                AgregarAnalisisCuentas(document, todasCuentas);

                document.NewPage();
                AgregarAnalisisPropuestas(document, todasPropuestas);

                AgregarTendencias(document, todasFacturas);

                AgregarPiePagina(document);

                document.Close();
                writer.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition",
                    $"attachment; filename=Reporte_Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                Response.Buffer = true;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception ex)
            {
                document.Close();
                throw new ApplicationException("Error al generar el PDF", ex);
            }
        }

        private void AgregarEncabezado(Document document, usuariosDTO usuario)
        {
            // Título principal
            Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, BaseColor.DARK_GRAY);
            Paragraph titulo = new Paragraph("REPORTE DE ANÁLISIS ESTADÍSTICO", tituloFont);
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 10;
            document.Add(titulo);

            // Subtítulo
            Font subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA, 14, BaseColor.GRAY);
            Paragraph subtitulo = new Paragraph("Sistema de Pago a Acreedores", subtituloFont);
            subtitulo.Alignment = Element.ALIGN_CENTER;
            subtitulo.SpacingAfter = 20;
            document.Add(subtitulo);

            // Línea separadora
            LineSeparator linea = new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
            document.Add(linea);
            document.Add(new Paragraph(" "));

            // Información del reporte
            Font infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.DARK_GRAY);
            PdfPTable infoTable = new PdfPTable(2);
            infoTable.WidthPercentage = 100;
            infoTable.SpacingAfter = 20;

            AgregarCeldaInfo(infoTable, "Generado por:", $"{usuario.nombre} {usuario.apellidos}");
            AgregarCeldaInfo(infoTable, "Fecha de generación:", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            AgregarCeldaInfo(infoTable, "Período analizado:", "Últimos 6 meses");

            document.Add(infoTable);
        }

        private void AgregarResumenEjecutivo(Document document, List<SoftPacBusiness.FacturasWS.facturasDTO> facturas,
            List<SoftPacBusiness.CuentasPropiasWS.cuentasPropiasDTO> cuentas,
            IList<SoftPacBusiness.PropuestaPagoWS.propuestasPagoDTO> propuestas)
        {
            // Título sección
            AgregarTituloSeccion(document, "1. RESUMEN EJECUTIVO");

            // Crear tabla de métricas
            PdfPTable tabla = new PdfPTable(4);
            tabla.WidthPercentage = 100;
            tabla.SpacingAfter = 20;

            // Encabezados
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
            PdfPCell[] headers = {
                new PdfPCell(new Phrase("Métrica", headerFont)),
                new PdfPCell(new Phrase("Total", headerFont)),
                new PdfPCell(new Phrase("Activas/Pendientes", headerFont)),
                new PdfPCell(new Phrase("Estado", headerFont))
            };

            foreach (var header in headers)
            {
                header.BackgroundColor = new BaseColor(102, 126, 234);
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                header.Padding = 8;
                tabla.AddCell(header);
            }

            // Datos
            Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            // Facturas
            var facturasPendientes = facturas.Count(f => f.estado != null &&
                f.estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase));
            AgregarFilaMetrica(tabla, "Facturas", facturas.Count.ToString(),
                facturasPendientes.ToString(), facturasPendientes > 0 ? "⚠ Atención" : "✓ Normal");

            // Cuentas
            var cuentasActivas = cuentas.Count(c => c.activa);
            AgregarFilaMetrica(tabla, "Cuentas Bancarias", cuentas.Count.ToString(),
                cuentasActivas.ToString(), cuentasActivas > 0 ? "✓ Activo" : "✗ Inactivo");

            // Propuestas
            var propuestasPendientes = propuestas?.Count(p => p.estado != null &&
                p.estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase)) ?? 0;
            AgregarFilaMetrica(tabla, "Propuestas", propuestas?.Count.ToString() ?? "0",
                propuestasPendientes.ToString(), "📊 Análisis");

            // Facturas Vencidas
            var facturasVencidas = facturas.Count(f => f.estado != null &&
                f.estado.Equals("Vencida", StringComparison.OrdinalIgnoreCase));
            AgregarFilaMetrica(tabla, "Facturas Vencidas", facturasVencidas.ToString(),
                "-", facturasVencidas > 0 ? "🔴 Crítico" : "✓ OK");

            document.Add(tabla);

            // Indicadores clave
            AgregarIndicadoresClave(document, facturas, cuentas, propuestas);
        }

        private void AgregarIndicadoresClave(Document document,
            List<SoftPacBusiness.FacturasWS.facturasDTO> facturas,
            List<SoftPacBusiness.CuentasPropiasWS.cuentasPropiasDTO> cuentas,
            IList<SoftPacBusiness.PropuestaPagoWS.propuestasPagoDTO> propuestas)
        {
            Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            Paragraph indicadores = new Paragraph("Indicadores Clave de Rendimiento (KPIs)", tituloFont);
            indicadores.SpacingBefore = 10;
            indicadores.SpacingAfter = 10;
            document.Add(indicadores);

            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            List list = new List(List.UNORDERED);
            list.SetListSymbol("•");

            // Calcular KPIs
            decimal montoTotal = facturas.Sum(f => f.monto_total);
            decimal montoPendiente = facturas.Where(f => f.estado != null &&
                f.estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))
                .Sum(f => f.monto_restante);

            var facturasPagadas = facturas.Count(f => f.estado != null &&
                f.estado.Equals("Pagada", StringComparison.OrdinalIgnoreCase));
            double tasaPago = facturas.Count > 0 ?
                (facturasPagadas * 100.0 / facturas.Count) : 0;

            list.Add(new ListItem($"Monto Total en Facturas: S/ {montoTotal:N2}", normalFont));
            list.Add(new ListItem($"Monto Pendiente de Pago: S/ {montoPendiente:N2}", normalFont));
            list.Add(new ListItem($"Tasa de Pago: {tasaPago:F1}%", normalFont));
            list.Add(new ListItem($"Promedio por Factura: S/ {(facturas.Count > 0 ? montoTotal / facturas.Count : 0):N2}", normalFont));
            list.Add(new ListItem($"Cuentas con Saldo Disponible: {cuentas.Count(c => c.saldo_disponible > 0)}", normalFont));

            document.Add(list);
        }

        private void AgregarAnalisisFacturas(Document document,
            List<SoftPacBusiness.FacturasWS.facturasDTO> facturas)
        {
            AgregarTituloSeccion(document, "2. ANÁLISIS DETALLADO DE FACTURAS");

            // 2.1 Distribución por Estado
            Font subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            Paragraph sub1 = new Paragraph("2.1 Distribución por Estado", subtituloFont);
            sub1.SpacingBefore = 10;
            sub1.SpacingAfter = 10;
            document.Add(sub1);

            PdfPTable tablaEstados = new PdfPTable(3);
            tablaEstados.WidthPercentage = 70;
            tablaEstados.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaEstados.SpacingAfter = 15;

            // Headers
            AgregarCeldaHeader(tablaEstados, "Estado");
            AgregarCeldaHeader(tablaEstados, "Cantidad");
            AgregarCeldaHeader(tablaEstados, "Porcentaje");

            // Datos por estado
            var estados = new[] { "Pendiente", "Pagada", "Vencida", "Anulada" };
            foreach (var estado in estados)
            {
                int cantidad = facturas.Count(f => f.estado != null &&
                    f.estado.Equals(estado, StringComparison.OrdinalIgnoreCase));
                double porcentaje = facturas.Count > 0 ?
                    (cantidad * 100.0 / facturas.Count) : 0;

                AgregarCeldaDato(tablaEstados, estado);
                AgregarCeldaDato(tablaEstados, cantidad.ToString());
                AgregarCeldaDato(tablaEstados, $"{porcentaje:F1}%");
            }

            document.Add(tablaEstados);

            // 2.2 Tendencia Mensual
            Paragraph sub2 = new Paragraph("2.2 Tendencia de Facturas (Últimos 6 Meses)", subtituloFont);
            sub2.SpacingBefore = 10;
            sub2.SpacingAfter = 10;
            document.Add(sub2);

            AgregarGraficoTendencia(document, facturas);

            // 2.3 Análisis de Montos
            Paragraph sub3 = new Paragraph("2.3 Análisis de Montos", subtituloFont);
            sub3.SpacingBefore = 15;
            sub3.SpacingAfter = 10;
            document.Add(sub3);

            var montosPorEstado = estados.Select(estado => new
            {
                Estado = estado,
                Monto = facturas.Where(f => f.estado != null &&
                    f.estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
                    .Sum(f => f.monto_total)
            }).ToList();

            PdfPTable tablaMontos = new PdfPTable(2);
            tablaMontos.WidthPercentage = 70;
            tablaMontos.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaMontos.SpacingAfter = 15;

            AgregarCeldaHeader(tablaMontos, "Estado");
            AgregarCeldaHeader(tablaMontos, "Monto Total (S/)");

            foreach (var item in montosPorEstado)
            {
                AgregarCeldaDato(tablaMontos, item.Estado);
                AgregarCeldaDato(tablaMontos, $"S/ {item.Monto:N2}");
            }

            document.Add(tablaMontos);
        }

        private void AgregarGraficoTendencia(Document document,
            List<SoftPacBusiness.FacturasWS.facturasDTO> facturas)
        {
            PdfPTable tablaTendencia = new PdfPTable(2);
            tablaTendencia.WidthPercentage = 80;
            tablaTendencia.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaTendencia.SpacingAfter = 15;

            AgregarCeldaHeader(tablaTendencia, "Mes");
            AgregarCeldaHeader(tablaTendencia, "Facturas");

            var fechaInicio = DateTime.Now.AddMonths(-6);
            for (int i = 5; i >= 0; i--)
            {
                var mes = DateTime.Now.AddMonths(-i);
                var cantidad = facturas.Count(f =>
                    f.fecha_emision.Year == mes.Year &&
                    f.fecha_emision.Month == mes.Month);

                AgregarCeldaDato(tablaTendencia,
                    $"{ObtenerNombreMes(mes.Month)} {mes.Year}");

                // Barra visual simple
                string barra = new string('█', Math.Min(cantidad, 50));
                AgregarCeldaDato(tablaTendencia, $"{cantidad} {barra}");
            }

            document.Add(tablaTendencia);
        }

        private void AgregarAnalisisCuentas(Document document,
            List<SoftPacBusiness.CuentasPropiasWS.cuentasPropiasDTO> cuentas)
        {
            AgregarTituloSeccion(document, "3. ANÁLISIS DE CUENTAS BANCARIAS");

            // 3.1 Distribución por Banco
            Font subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            Paragraph sub1 = new Paragraph("3.1 Distribución por Entidad Bancaria", subtituloFont);
            sub1.SpacingBefore = 10;
            sub1.SpacingAfter = 10;
            document.Add(sub1);

            var bancos = cuentas
                .Where(c => c.entidad_bancaria != null && c.entidad_bancaria.nombre != null)
                .GroupBy(c => c.entidad_bancaria.nombre)
                .Select(g => new
                {
                    Banco = g.Key,
                    Cantidad = g.Count(),
                    CuentasActivas = g.Count(c => c.activa),
                    SaldoTotal = g.Sum(c => c.saldo_disponible)
                })
                .OrderByDescending(b => b.Cantidad)
                .ToList();

            PdfPTable tablaBancos = new PdfPTable(4);
            tablaBancos.WidthPercentage = 100;
            tablaBancos.SpacingAfter = 15;

            AgregarCeldaHeader(tablaBancos, "Entidad Bancaria");
            AgregarCeldaHeader(tablaBancos, "Total Cuentas");
            AgregarCeldaHeader(tablaBancos, "Activas");
            AgregarCeldaHeader(tablaBancos, "Saldo Total (S/)");

            foreach (var banco in bancos)
            {
                AgregarCeldaDato(tablaBancos, banco.Banco);
                AgregarCeldaDato(tablaBancos, banco.Cantidad.ToString());
                AgregarCeldaDato(tablaBancos, banco.CuentasActivas.ToString());
                AgregarCeldaDato(tablaBancos, $"S/ {banco.SaldoTotal:N2}");
            }

            document.Add(tablaBancos);

            // 3.2 Estado de Cuentas
            Paragraph sub2 = new Paragraph("3.2 Estado General de Cuentas", subtituloFont);
            sub2.SpacingBefore = 15;
            sub2.SpacingAfter = 10;
            document.Add(sub2);

            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            List list = new List(List.UNORDERED);
            list.SetListSymbol("•");

            decimal saldoTotalDisponible = cuentas.Sum(c => c.saldo_disponible);
            var cuentasConSaldo = cuentas.Count(c => c.saldo_disponible > 0);

            list.Add(new ListItem($"Saldo Total Disponible: S/ {saldoTotalDisponible:N2}", normalFont));
            list.Add(new ListItem($"Cuentas con Saldo: {cuentasConSaldo} de {cuentas.Count}", normalFont));
            list.Add(new ListItem($"Cuentas Activas: {cuentas.Count(c => c.activa)}", normalFont));
            list.Add(new ListItem($"Cuentas Inactivas: {cuentas.Count(c => !c.activa)}", normalFont));
            list.Add(new ListItem($"Promedio de Saldo: S/ {(cuentas.Count > 0 ? saldoTotalDisponible / cuentas.Count : 0):N2}", normalFont));

            document.Add(list);
        }

        private void AgregarAnalisisPropuestas(Document document,
            IList<SoftPacBusiness.PropuestaPagoWS.propuestasPagoDTO> propuestas)
        {
            AgregarTituloSeccion(document, "4. ANÁLISIS DE PROPUESTAS DE PAGO");

            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            if (propuestas == null || propuestas.Count == 0)
            {
                Paragraph noData = new Paragraph("No hay propuestas de pago registradas en el período analizado.", normalFont);
                noData.SpacingAfter = 20;
                document.Add(noData);
                return;
            }

            // 4.1 Distribución por Estado
            Font subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            Paragraph sub1 = new Paragraph("4.1 Distribución por Estado", subtituloFont);
            sub1.SpacingBefore = 10;
            sub1.SpacingAfter = 10;
            document.Add(sub1);

            var estadosPropuestas = propuestas
                .GroupBy(p => p.estado ?? "Sin Estado")
                .Select(g => new
                {
                    Estado = g.Key,
                    Cantidad = g.Count(),
                    Porcentaje = (g.Count() * 100.0 / propuestas.Count)
                })
                .OrderByDescending(e => e.Cantidad)
                .ToList();

            PdfPTable tablaEstados = new PdfPTable(3);
            tablaEstados.WidthPercentage = 70;
            tablaEstados.HorizontalAlignment = Element.ALIGN_CENTER;
            tablaEstados.SpacingAfter = 15;

            AgregarCeldaHeader(tablaEstados, "Estado");
            AgregarCeldaHeader(tablaEstados, "Cantidad");
            AgregarCeldaHeader(tablaEstados, "Porcentaje");

            foreach (var estado in estadosPropuestas)
            {
                AgregarCeldaDato(tablaEstados, estado.Estado);
                AgregarCeldaDato(tablaEstados, estado.Cantidad.ToString());
                AgregarCeldaDato(tablaEstados, $"{estado.Porcentaje:F1}%");
            }

            document.Add(tablaEstados);

            // 4.2 Actividad Reciente
            Paragraph sub2 = new Paragraph("4.2 Actividad de Propuestas", subtituloFont);
            sub2.SpacingBefore = 15;
            sub2.SpacingAfter = 10;
            document.Add(sub2);

            List list = new List(List.UNORDERED);
            list.SetListSymbol("•");

            var propuestasEsteMes = propuestas.Count(p =>
                p.fecha_hora_creacion.Month == DateTime.Now.Month &&
                p.fecha_hora_creacion.Year == DateTime.Now.Year);

            var propuestasMesAnterior = propuestas.Count(p =>
                p.fecha_hora_creacion.Month == DateTime.Now.AddMonths(-1).Month &&
                p.fecha_hora_creacion.Year == DateTime.Now.AddMonths(-1).Year);

            list.Add(new ListItem($"Total de Propuestas: {propuestas.Count}", normalFont));
            list.Add(new ListItem($"Propuestas Este Mes: {propuestasEsteMes}", normalFont));
            list.Add(new ListItem($"Propuestas Mes Anterior: {propuestasMesAnterior}", normalFont));
            list.Add(new ListItem($"Propuestas Pendientes: {propuestas.Count(p => p.estado == "Pendiente")}", normalFont));
            list.Add(new ListItem($"Propuestas Enviadas: {propuestas.Count(p => p.estado == "Enviada")}", normalFont));

            document.Add(list);
        }

        private void AgregarTendencias(Document document,
            List<SoftPacBusiness.FacturasWS.facturasDTO> facturas)
        {
            AgregarTituloSeccion(document, "5. TENDENCIAS Y CONCLUSIONES");

            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

            // Análisis de crecimiento
            var facturasMesActual = facturas.Count(f =>
                f.fecha_emision.Month == DateTime.Now.Month &&
                f.fecha_emision.Year == DateTime.Now.Year);

            var facturasMesAnterior = facturas.Count(f =>
                f.fecha_emision.Month == DateTime.Now.AddMonths(-1).Month &&
                f.fecha_emision.Year == DateTime.Now.AddMonths(-1).Year);

            Paragraph tendencia1 = new Paragraph();
            tendencia1.Add(new Chunk("Crecimiento Mensual: ", boldFont));

            if (facturasMesAnterior > 0)
            {
                double cambio = ((facturasMesActual - facturasMesAnterior) * 100.0 / facturasMesAnterior);
                tendencia1.Add(new Chunk($"{(cambio >= 0 ? "+" : "")}{cambio:F1}% respecto al mes anterior", normalFont));
            }
            else
            {
                tendencia1.Add(new Chunk("Sin datos del mes anterior para comparar", normalFont));
            }
            tendencia1.SpacingAfter = 10;
            document.Add(tendencia1);

            // Conclusiones
            Font subtituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
            Paragraph conclusiones = new Paragraph("Conclusiones", subtituloFont);
            conclusiones.SpacingBefore = 15;
            conclusiones.SpacingAfter = 10;
            document.Add(conclusiones);

            List list = new List(List.UNORDERED);
            list.SetListSymbol("✓");

            var facturasVencidas = facturas.Count(f => f.estado == "Vencida");
            if (facturasVencidas > 0)
            {
                list.Add(new ListItem($"Atención: Existen {facturasVencidas} facturas vencidas que requieren seguimiento inmediato.", normalFont));
            }

            var facturasPendientes = facturas.Count(f => f.estado == "Pendiente");
            list.Add(new ListItem($"Se tienen {facturasPendientes} facturas pendientes de pago.", normalFont));

            var tasaPago = facturas.Count > 0 ?
                (facturas.Count(f => f.estado == "Pagada") * 100.0 / facturas.Count) : 0;
            list.Add(new ListItem($"La tasa de cumplimiento de pagos es del {tasaPago:F1}%.", normalFont));

            if (facturasMesActual > facturasMesAnterior)
            {
                list.Add(new ListItem("Se observa un incremento en la actividad de facturas respecto al mes anterior.", normalFont));
            }

            document.Add(list);
        }

        private void AgregarPiePagina(Document document)
        {
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            LineSeparator linea = new LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -2);
            document.Add(linea);

            Font pieFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);
            Paragraph pie = new Paragraph(
                $"Reporte generado automáticamente por SoftPac - Sistema de Pago a Acreedores\n" +
                $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n" +
                "Este documento contiene información confidencial y está destinado únicamente para uso interno.",
                pieFont
            );
            pie.Alignment = Element.ALIGN_CENTER;
            pie.SpacingBefore = 10;
            document.Add(pie);
        }

        // ========== MÉTODOS AUXILIARES ==========

        private void AgregarTituloSeccion(Document document, string titulo)
        {
            Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, new BaseColor(102, 126, 234));
            Paragraph p = new Paragraph(titulo, tituloFont);
            p.SpacingBefore = 20;
            p.SpacingAfter = 15;
            document.Add(p);
        }

        private void AgregarCeldaInfo(PdfPTable tabla, string label, string valor)
        {
            Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.DARK_GRAY);
            Font valorFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            PdfPCell celdaLabel = new PdfPCell(new Phrase(label, labelFont));
            celdaLabel.Border = Rectangle.NO_BORDER;
            celdaLabel.PaddingBottom = 5;

            PdfPCell celdaValor = new PdfPCell(new Phrase(valor, valorFont));
            celdaValor.Border = Rectangle.NO_BORDER;
            celdaValor.PaddingBottom = 5;

            tabla.AddCell(celdaLabel);
            tabla.AddCell(celdaValor);
        }

        private void AgregarFilaMetrica(PdfPTable tabla, string metrica, string total, string activas, string estado)
        {
            Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            PdfPCell[] celdas = {
                new PdfPCell(new Phrase(metrica, dataFont)),
                new PdfPCell(new Phrase(total, dataFont)),
                new PdfPCell(new Phrase(activas, dataFont)),
                new PdfPCell(new Phrase(estado, dataFont))
            };

            foreach (var celda in celdas)
            {
                celda.HorizontalAlignment = Element.ALIGN_CENTER;
                celda.Padding = 6;
                celda.BackgroundColor = BaseColor.WHITE;
                tabla.AddCell(celda);
            }
        }

        private void AgregarCeldaHeader(PdfPTable tabla, string texto)
        {
            Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
            PdfPCell celda = new PdfPCell(new Phrase(texto, headerFont));
            celda.BackgroundColor = new BaseColor(102, 126, 234);
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Padding = 8;
            tabla.AddCell(celda);
        }

        private void AgregarCeldaDato(PdfPTable tabla, string texto)
        {
            Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            PdfPCell celda = new PdfPCell(new Phrase(texto, dataFont));
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Padding = 6;
            tabla.AddCell(celda);
        }

        private string ObtenerNombreMes(int mes)
        {
            string[] meses = {
                "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };
            return meses[mes];
        }
    }
}