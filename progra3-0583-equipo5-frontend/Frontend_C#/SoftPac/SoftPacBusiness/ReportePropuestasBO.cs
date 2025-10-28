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
    public class ReportePropuestasBO
    {
        private PropuestasPagoDAO propuestasDAO = new PropuestasPagoDAOImpl();

        public class DetallePago
        {
            public string NumeroFactura { get; set; }
            public string Acreedor { get; set; }
            public string Moneda { get; set; }
            public decimal Monto { get; set; }
            public string CuentaOrigen { get; set; }
            public string BancoOrigen { get; set; }
            public string CuentaDestino { get; set; }
            public string BancoDestino { get; set; }
            public string FormaPago { get; set; }
        }

        public class PropuestaDetalle
        {
            public int PropuestaId { get; set; }
            public DateTime FechaCreacion { get; set; }
            public string UsuarioCreador { get; set; }
            public string Pais { get; set; }
            public string EntidadBancaria { get; set; }
            public string Estado { get; set; }
            public int TotalPagos { get; set; }
            public List<DetallePago> Detalles { get; set; }
        }

        public List<PropuestaDetalle> GenerarReporteDetallado(
            int? paisId,
            int? bancoId,
            string estado)
        {
            var propuestas = propuestasDAO.ListarTodos().ToList();

            // Aplicar filtros
            if (paisId.HasValue)
                propuestas = propuestas
                    .Where(p => p.EntidadBancaria?.Pais?.PaisId == paisId.Value)
                    .ToList();

            if (bancoId.HasValue)
                propuestas = propuestas
                    .Where(p => p.EntidadBancaria?.EntidadBancariaId == bancoId.Value)
                    .ToList();

            if (!string.IsNullOrEmpty(estado))
                propuestas = propuestas
                    .Where(p => p.Estado == estado)
                    .ToList();

            // Convertir a modelo de reporte
            var resultado = propuestas
                .OrderByDescending(p => p.FechaHoraCreacion)
                .Select(p => new PropuestaDetalle
                {
                    PropuestaId = p.PropuestaId ?? 0,
                    FechaCreacion = p.FechaHoraCreacion ?? DateTime.Now,
                    UsuarioCreador = p.UsuarioCreacion != null ?
                        $"{p.UsuarioCreacion.Nombre} {p.UsuarioCreacion.Apellidos}" : "-",
                    Pais = p.EntidadBancaria?.Pais?.Nombre ?? "-",
                    EntidadBancaria = p.EntidadBancaria?.Nombre ?? "-",
                    Estado = p.Estado ?? "-",
                    TotalPagos = p.DetallesPropuesta?.Count ?? 0,
                    Detalles = p.DetallesPropuesta?.Select(d => new DetallePago
                    {
                        NumeroFactura = d.Factura?.NumeroFactura ?? "-",
                        Acreedor = d.Factura?.Acreedor?.RazonSocial ?? "-",
                        Moneda = d.Factura?.Moneda?.CodigoIso ?? "-",
                        Monto = d.MontoPago,
                        CuentaOrigen = d.CuentaPropia?.NumeroCuenta ?? "-",
                        BancoOrigen = d.CuentaPropia?.EntidadBancaria?.Nombre ?? "-",
                        CuentaDestino = d.CuentaAcreedor?.NumeroCuenta ?? "-",
                        BancoDestino = d.CuentaAcreedor?.EntidadBancaria?.Nombre ?? "-",
                        FormaPago = MapearFormaPago(d.FormaPago)
                    }).ToList() ?? new List<DetallePago>()
                })
                .ToList();

            return resultado;
        }

        private string MapearFormaPago(char? formaPago)
        {
            if (!formaPago.HasValue)
                return "-";

            switch (char.ToUpper(formaPago.Value))
            {
                case 'T':
                    return "Transferencia";
                case 'C':
                    return "Cheque";
                case 'E':
                    return "Efectivo";
                default:
                    return formaPago.ToString();
            }
        }

        public byte[] GenerarPDF(
            List<PropuestaDetalle> datos,
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
                var titulo = new Paragraph("REPORTE DE PROPUESTAS DE PAGO")
                    .SetFont(fontBold)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(colorPrimario);
                document.Add(titulo);

                var subtitulo = new Paragraph("Detalle de Pagos Individuales por Propuesta")
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

                // Datos de propuestas
                int totalPropuestas = 0;
                int totalPagos = 0;

                foreach (var propuesta in datos)
                {
                    totalPropuestas++;
                    totalPagos += propuesta.TotalPagos;

                    // Encabezado de propuesta
                    var tituloPropuesta = new Paragraph($"Propuesta #{propuesta.PropuestaId} - {propuesta.EntidadBancaria}")
                        .SetFont(fontBold)
                        .SetFontSize(14)
                        .SetBackgroundColor(colorPrimario)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetPadding(5);
                    document.Add(tituloPropuesta);

                    // Información de la propuesta
                    var tablaInfo = new Table(4).UseAllAvailableWidth();
                    tablaInfo.SetMarginTop(5);
                    tablaInfo.SetMarginBottom(10);

                    tablaInfo.AddCell(new Cell().Add(new Paragraph("Fecha Creación:").SetFont(fontBold)));
                    tablaInfo.AddCell(new Cell().Add(new Paragraph(propuesta.FechaCreacion.ToString("dd/MM/yyyy HH:mm")).SetFont(fontNormal)));
                    tablaInfo.AddCell(new Cell().Add(new Paragraph("Usuario:").SetFont(fontBold)));
                    tablaInfo.AddCell(new Cell().Add(new Paragraph(propuesta.UsuarioCreador).SetFont(fontNormal)));
                    tablaInfo.AddCell(new Cell().Add(new Paragraph("País:").SetFont(fontBold)));
                    tablaInfo.AddCell(new Cell().Add(new Paragraph(propuesta.Pais).SetFont(fontNormal)));
                    tablaInfo.AddCell(new Cell().Add(new Paragraph("Estado:").SetFont(fontBold)));
                    tablaInfo.AddCell(new Cell().Add(new Paragraph(propuesta.Estado).SetFont(fontNormal)));

                    document.Add(tablaInfo);

                    // Tabla de detalles
                    var tablaDetalles = new Table(new float[] { 2, 3, 1, 2, 2, 2, 2, 2, 2 })
                        .UseAllAvailableWidth();

                    // Encabezados
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("N° Factura").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Proveedor").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Mon.").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Monto").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Cta. Origen").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Banco Orig.").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Cta. Dest.").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Banco Dest.").SetFont(fontBold).SetFontSize(9)));
                    tablaDetalles.AddHeaderCell(new Cell().Add(new Paragraph("Forma Pago").SetFont(fontBold).SetFontSize(9)));

                    // Filas
                    foreach (var detalle in propuesta.Detalles)
                    {
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.NumeroFactura).SetFont(fontNormal).SetFontSize(8)));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.Acreedor).SetFont(fontNormal).SetFontSize(8)));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.Moneda).SetFont(fontNormal).SetFontSize(8)));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.Monto.ToString("N2")).SetFont(fontNormal).SetFontSize(8)).SetTextAlignment(TextAlignment.RIGHT));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.CuentaOrigen).SetFont(fontNormal).SetFontSize(8)));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.BancoOrigen).SetFont(fontNormal).SetFontSize(8)));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.CuentaDestino).SetFont(fontNormal).SetFontSize(8)));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.BancoDestino).SetFont(fontNormal).SetFontSize(8)));
                        tablaDetalles.AddCell(new Cell().Add(new Paragraph(detalle.FormaPago).SetFont(fontNormal).SetFontSize(8)));
                    }

                    document.Add(tablaDetalles);

                    // Totales por moneda
                    var totalesPorMoneda = propuesta.Detalles
                        .GroupBy(d => d.Moneda)
                        .Select(g => new { Moneda = g.Key, Total = g.Sum(x => x.Monto) })
                        .ToList();

                    var parrafoTotales = new Paragraph()
                        .SetFont(fontNormal)
                        .SetMarginTop(5)
                        .SetMarginBottom(15);

                    parrafoTotales.Add(new Text("Totales: ").SetFont(fontBold));

                    foreach (var total in totalesPorMoneda)
                    {
                        parrafoTotales.Add(new Text($"{total.Moneda}: {total.Total:N2}   ").SetFont(fontNormal));
                    }

                    document.Add(parrafoTotales);
                    document.Add(new Paragraph("\n"));
                }

                // Resumen final
                var resumenFinal = new Paragraph($"RESUMEN TOTAL: {totalPropuestas} Propuestas | {totalPagos} Pagos")
                    .SetFont(fontBold)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBackgroundColor(colorPrimario)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetPadding(10)
                    .SetMarginTop(20);
                document.Add(resumenFinal);

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