using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using SoftPac.Model;
using SoftPac.Persistance.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoftPac.Business
{
    public class ReporteFacturasBO
    {
        private FacturasDAO facturasDAO = new FacturasDAOImpl();

        public class FacturaReporte
        {
            public string NumeroFactura { get; set; }
            public string Acreedor { get; set; }
            public string Pais { get; set; }
            public DateTime FechaVencimiento { get; set; }
            public int DiasVencimiento { get; set; }
            public string Moneda { get; set; }
            public decimal Monto { get; set; }
        }

        public class RangoVencimiento
        {
            public string Rango { get; set; }
            public List<FacturaReporte> Facturas { get; set; }
            public int CantidadFacturas => Facturas.Count;
        }

        public List<RangoVencimiento> GenerarReportePorVencimiento(
            int? acreedorId,
            int? paisId,
            int? monedaId,
            string rangoFiltro)
        {
            var facturas = facturasDAO.ListarTodos()
                .Where(f => f.Estado == "Pendiente" && f.MontoRestante > 0)
                .ToList();

            // Aplicar filtros
            if (acreedorId.HasValue)
                facturas = facturas.Where(f => f.Acreedor?.AcreedorId == acreedorId.Value).ToList();

            if (paisId.HasValue)
                facturas = facturas.Where(f => f.Acreedor?.Pais?.PaisId == paisId.Value).ToList();

            if (monedaId.HasValue)
                facturas = facturas.Where(f => f.Moneda?.MonedaId == monedaId.Value).ToList();

            // Convertir a modelo de reporte
            var facturasReporte = facturas.Select(f => new FacturaReporte
            {
                NumeroFactura = f.NumeroFactura ?? "-",
                Acreedor = f.Acreedor?.RazonSocial ?? "-",
                Pais = f.Acreedor?.Pais?.Nombre ?? "-",
                FechaVencimiento = f.FechaLimitePago ?? DateTime.Now,
                DiasVencimiento = f.FechaLimitePago.HasValue ?
                    (f.FechaLimitePago.Value - DateTime.Now).Days : 0,
                Moneda = f.Moneda?.CodigoIso ?? "-",
                Monto = f.MontoRestante
            }).ToList();

            // Agrupar por rangos
            var rangos = new List<RangoVencimiento>();

            if (string.IsNullOrEmpty(rangoFiltro) || rangoFiltro == "0-30")
            {
                var facturas030 = facturasReporte
                    .Where(f => f.DiasVencimiento >= 0 && f.DiasVencimiento <= 30)
                    .OrderBy(f => f.DiasVencimiento)
                    .ToList();

                if (facturas030.Any())
                    rangos.Add(new RangoVencimiento
                    {
                        Rango = "0-30 días",
                        Facturas = facturas030
                    });
            }

            if (string.IsNullOrEmpty(rangoFiltro) || rangoFiltro == "31-60")
            {
                var facturas3160 = facturasReporte
                    .Where(f => f.DiasVencimiento >= 31 && f.DiasVencimiento <= 60)
                    .OrderBy(f => f.DiasVencimiento)
                    .ToList();

                if (facturas3160.Any())
                    rangos.Add(new RangoVencimiento
                    {
                        Rango = "31-60 días",
                        Facturas = facturas3160
                    });
            }

            if (string.IsNullOrEmpty(rangoFiltro) || rangoFiltro == "61-90")
            {
                var facturas6190 = facturasReporte
                    .Where(f => f.DiasVencimiento >= 61 && f.DiasVencimiento <= 90)
                    .OrderBy(f => f.DiasVencimiento)
                    .ToList();

                if (facturas6190.Any())
                    rangos.Add(new RangoVencimiento
                    {
                        Rango = "61-90 días",
                        Facturas = facturas6190
                    });
            }

            if (string.IsNullOrEmpty(rangoFiltro) || rangoFiltro == "90+")
            {
                var facturas90plus = facturasReporte
                    .Where(f => f.DiasVencimiento > 90)
                    .OrderBy(f => f.DiasVencimiento)
                    .ToList();

                if (facturas90plus.Any())
                    rangos.Add(new RangoVencimiento
                    {
                        Rango = "+90 días",
                        Facturas = facturas90plus
                    });
            }

            return rangos;
        }

        public byte[] GenerarPDF(
            List<RangoVencimiento> datos,
            string usuario,
            Dictionary<string, string> filtros)
        {

            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Fuentes
                PdfFont fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont fontNormal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Colores del sistema
                var colorPrimario = new DeviceRgb(11, 31, 52);
                var colorSecundario = new DeviceRgb(96, 116, 138);

                // Cabecera
                var titulo = new Paragraph("REPORTE DE FACTURAS PENDIENTES")
                    .SetFont(fontBold)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(colorPrimario);
                document.Add(titulo);

                var subtitulo = new Paragraph("Agrupadas por Rangos de Vencimiento")
                    .SetFont(fontNormal)
                    .SetFontSize(12)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(colorSecundario);
                document.Add(subtitulo);

                document.Add(new Paragraph("\n"));

                // Información de auditoría
                var tablaAudit = new Table(2).UseAllAvailableWidth();
                tablaAudit.AddCell(new Cell().Add(new Paragraph("Fecha de Generación:").SetFont(fontBold)));
                tablaAudit.AddCell(new Cell().Add(new Paragraph(DateTime.Now.ToString("dd/MM/yyyy")).SetFont(fontNormal)));
                tablaAudit.AddCell(new Cell().Add(new Paragraph("Hora de Generación:").SetFont(fontBold)));
                tablaAudit.AddCell(new Cell().Add(new Paragraph(DateTime.Now.ToString("HH:mm:ss")).SetFont(fontNormal)));
                tablaAudit.AddCell(new Cell().Add(new Paragraph("Usuario:").SetFont(fontBold)));
                tablaAudit.AddCell(new Cell().Add(new Paragraph(usuario).SetFont(fontNormal)));
                document.Add(tablaAudit);

                document.Add(new Paragraph("\n"));

                // Filtros aplicados
                if (filtros != null && filtros.Count > 0)
                {
                    var paragrafoFiltros = new Paragraph()
                        .SetFont(fontNormal)
                        .SetFontColor(colorPrimario);

                    paragrafoFiltros.Add(new Text("Filtros Aplicados: ").SetFont(fontBold));

                    foreach (var filtro in filtros)
                    {
                        paragrafoFiltros.Add(new Text($"{filtro.Key}: {filtro.Value} | ").SetFont(fontNormal));
                    }

                    document.Add(paragrafoFiltros);
                    document.Add(new Paragraph("\n"));
                }

                // Datos por rango
                decimal totalGeneral = 0;

                foreach (var rango in datos)
                {
                    // Título del rango
                    var tituloRango = new Paragraph($"{rango.Rango} ({rango.CantidadFacturas} facturas)")
                        .SetFont(fontBold)
                        .SetFontSize(14)
                        .SetBackgroundColor(colorPrimario)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetPadding(5);
                    document.Add(tituloRango);

                    // Tabla de facturas
                    var tabla = new Table(new float[] { 2, 3, 2, 2, 2, 1, 2 }).UseAllAvailableWidth();
                    tabla.SetMarginTop(10);

                    // Encabezados
                    tabla.AddHeaderCell(new Cell().Add(new Paragraph("N° Factura").SetFont(fontBold)));
                    tabla.AddHeaderCell(new Cell().Add(new Paragraph("Proveedor").SetFont(fontBold)));
                    tabla.AddHeaderCell(new Cell().Add(new Paragraph("País").SetFont(fontBold)));
                    tabla.AddHeaderCell(new Cell().Add(new Paragraph("F. Vencimiento").SetFont(fontBold)));
                    tabla.AddHeaderCell(new Cell().Add(new Paragraph("Días").SetFont(fontBold)));
                    tabla.AddHeaderCell(new Cell().Add(new Paragraph("Mon.").SetFont(fontBold)));
                    tabla.AddHeaderCell(new Cell().Add(new Paragraph("Monto").SetFont(fontBold)).SetTextAlignment(TextAlignment.RIGHT));

                    decimal subtotal = 0;

                    // Filas
                    foreach (var factura in rango.Facturas)
                    {
                        tabla.AddCell(new Cell().Add(new Paragraph(factura.NumeroFactura).SetFont(fontNormal)));
                        tabla.AddCell(new Cell().Add(new Paragraph(factura.Acreedor).SetFont(fontNormal)));
                        tabla.AddCell(new Cell().Add(new Paragraph(factura.Pais).SetFont(fontNormal)));
                        tabla.AddCell(new Cell().Add(new Paragraph(factura.FechaVencimiento.ToString("dd/MM/yyyy")).SetFont(fontNormal)));
                        tabla.AddCell(new Cell().Add(new Paragraph(factura.DiasVencimiento.ToString()).SetFont(fontNormal)));
                        tabla.AddCell(new Cell().Add(new Paragraph(factura.Moneda).SetFont(fontNormal)));
                        tabla.AddCell(new Cell().Add(new Paragraph(factura.Monto.ToString("N2")).SetFont(fontNormal)).SetTextAlignment(TextAlignment.RIGHT));

                        subtotal += factura.Monto;
                    }

                    document.Add(tabla);

                    // Subtotal
                    var paragrafoSubtotal = new Paragraph($"Subtotal: {subtotal:N2}")
                        .SetFont(fontBold)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetMarginTop(5)
                        .SetMarginBottom(15);
                    document.Add(paragrafoSubtotal);

                    totalGeneral += subtotal;
                }

                // Total general
                var totalParagraph = new Paragraph($"TOTAL GENERAL: {totalGeneral:N2}")
                    .SetFont(fontBold)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBackgroundColor(colorPrimario)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetPadding(10)
                    .SetMarginTop(20);
                document.Add(totalParagraph);

                // Pie de página con número de página
                int numberOfPages = pdf.GetNumberOfPages();
                for (int i = 1; i <= numberOfPages; i++)
                {
                    document.ShowTextAligned(
                        new Paragraph($"Página {i} de {numberOfPages}").SetFont(fontNormal),
                        297.5f, 30, i, TextAlignment.CENTER, VerticalAlignment.BOTTOM, 0);
                }

                document.Close();
                return ms.ToArray();
            }
        }
    }
}