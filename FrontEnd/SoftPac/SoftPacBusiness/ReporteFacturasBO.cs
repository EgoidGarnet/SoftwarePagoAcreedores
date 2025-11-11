using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SoftPacBusiness.ReporteFactPendWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static SoftPac.Business.ReporteFacturasBO;

namespace SoftPac.Business
{
    public class ReporteFacturasBO
    {
        private ReporteFactPendWSClient facturasClienteSOAP = new ReporteFactPendWSClient();

        public ReporteFacturasBO()
        {
            // Configurar QuestPDF (solo necesario una vez en la aplicación)
            QuestPDF.Settings.License = LicenseType.Community;
        }

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
            // Paso 1: Generar el reporte en BD
            facturasClienteSOAP.generarReporteFacturasPendientes(
                acreedorId.HasValue ? (int)acreedorId : 0,
                paisId.HasValue ? (int)paisId : 0,
                monedaId.HasValue ? (int)monedaId : 0
            );

            // Paso 2: Obtener los datos del reporte
            var reporteDTO = facturasClienteSOAP.listarPorFiltros(
                acreedorId.HasValue ? (int)acreedorId : 0,
                paisId.HasValue ? (int)paisId : 0,
                monedaId.HasValue ? (int)monedaId : 0
            );

            if (reporteDTO == null || reporteDTO.Length == 0)
                return new List<RangoVencimiento>();

            // Paso 3: Convertir de ReporteFactPendDTO[] a List<FacturaReporte>
            var facturasReporte = reporteDTO.Select(dto => new FacturaReporte
            {
                NumeroFactura = dto.factura?.numero_factura ?? "-",
                Acreedor = dto.acreedor?.razon_social ?? "-",
                Pais = dto.pais?.nombre ?? "-",
                FechaVencimiento = dto.factura != null && dto.factura.fecha_limite_pagoSpecified
                    ? dto.factura.fecha_limite_pago
                    : DateTime.Now,
                DiasVencimiento = dto.dias_vencimiento,
                Moneda = dto.moneda?.codigo_iso ?? "-",
                Monto = dto.factura?.monto_restante ?? 0
            }).ToList();

            // Paso 4: Filtrar por rango si se especificó
            if (!string.IsNullOrEmpty(rangoFiltro))
            {
                facturasReporte = FiltrarPorRango(facturasReporte, rangoFiltro);
            }

            // Paso 5: Agrupar por rangos
            var rangos = AgruparPorRangos(facturasReporte);

            return rangos;
        }

        private List<FacturaReporte> FiltrarPorRango(List<FacturaReporte> facturas, string rangoFiltro)
        {
            switch (rangoFiltro)
            {
                case "0-30":
                    return facturas.Where(f => f.DiasVencimiento >= 0 && f.DiasVencimiento <= 30).ToList();
                case "31-60":
                    return facturas.Where(f => f.DiasVencimiento >= 31 && f.DiasVencimiento <= 60).ToList();
                case "61-90":
                    return facturas.Where(f => f.DiasVencimiento >= 61 && f.DiasVencimiento <= 90).ToList();
                case "90+":
                    return facturas.Where(f => f.DiasVencimiento > 90).ToList();
                default:
                    return facturas;
            }
        }

        private List<RangoVencimiento> AgruparPorRangos(List<FacturaReporte> facturas)
        {
            var rangos = new List<RangoVencimiento>();

            // Rango 0-30 días
            var facturas030 = facturas
                .Where(f => f.DiasVencimiento >= 0 && f.DiasVencimiento <= 30)
                .OrderBy(f => f.DiasVencimiento)
                .ToList();

            if (facturas030.Any())
                rangos.Add(new RangoVencimiento
                {
                    Rango = "0-30 días",
                    Facturas = facturas030
                });

            // Rango 31-60 días
            var facturas3160 = facturas
                .Where(f => f.DiasVencimiento >= 31 && f.DiasVencimiento <= 60)
                .OrderBy(f => f.DiasVencimiento)
                .ToList();

            if (facturas3160.Any())
                rangos.Add(new RangoVencimiento
                {
                    Rango = "31-60 días",
                    Facturas = facturas3160
                });

            // Rango 61-90 días
            var facturas6190 = facturas
                .Where(f => f.DiasVencimiento >= 61 && f.DiasVencimiento <= 90)
                .OrderBy(f => f.DiasVencimiento)
                .ToList();

            if (facturas6190.Any())
                rangos.Add(new RangoVencimiento
                {
                    Rango = "61-90 días",
                    Facturas = facturas6190
                });

            // Rango +90 días
            var facturas90plus = facturas
                .Where(f => f.DiasVencimiento > 90)
                .OrderBy(f => f.DiasVencimiento)
                .ToList();

            if (facturas90plus.Any())
                rangos.Add(new RangoVencimiento
                {
                    Rango = "+90 días",
                    Facturas = facturas90plus
                });

            return rangos;
        }


        public byte[] GenerarPDF(
            List<RangoVencimiento> datos,
            string usuario,
            Dictionary<string, string> filtros)
        {

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);

                    // Cabecera
                    page.Header().Column(column =>
                    {
                        column.Item().AlignCenter().Text("REPORTE DE FACTURAS PENDIENTES")
                            .FontSize(18)
                            .Bold()
                            .FontColor("#0B1F34");

                        column.Item().AlignCenter().Text("Agrupadas por Rangos de Vencimiento")
                            .FontSize(12)
                            .FontColor("#60748A");

                        column.Item().PaddingVertical(10);

                        // Información de auditoría
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Fecha de Generación:").Bold();
                            table.Cell().Text(DateTime.Now.ToString("dd/MM/yyyy"));

                            table.Cell().Text("Hora de Generación:").Bold();
                            table.Cell().Text(DateTime.Now.ToString("HH:mm:ss"));

                            table.Cell().Text("Usuario:").Bold();
                            table.Cell().Text(usuario);
                        });

                        // Filtros aplicados
                        if (filtros != null && filtros.Count > 0)
                        {
                            column.Item().PaddingTop(10).Row(row =>
                            {
                                row.AutoItem().Text("Filtros Aplicados: ").Bold().FontColor("#0B1F34");
                                row.RelativeItem().Text(string.Join(" | ",
                                    filtros.Select(f => $"{f.Key}: {f.Value}")));
                            });
                        }
                    });

                    // Contenido
                    page.Content().Column(column =>
                    {
                        // Diccionario para acumular totales por moneda
                        var totalesPorMoneda = new Dictionary<string, decimal>();

                        foreach (var rango in datos)
                        {
                            // Título del rango
                            column.Item().PaddingTop(15).Background("#0B1F34").Padding(5)
                                .Text($"{rango.Rango} ({rango.CantidadFacturas} facturas)")
                                .FontSize(14)
                                .Bold()
                                .FontColor(Colors.White);

                            // Agrupar facturas por moneda dentro del rango
                            var facturasPorMoneda = rango.Facturas
                                .GroupBy(f => f.Moneda)
                                .OrderBy(g => g.Key);

                            foreach (var grupoMoneda in facturasPorMoneda)
                            {
                                // Subtítulo de moneda (opcional, puedes comentarlo si no lo quieres)
                                column.Item().PaddingTop(10).Text($"Moneda: {grupoMoneda.Key}")
                                    .FontSize(11)
                                    .SemiBold()
                                    .FontColor("#60748A");

                                decimal subtotal = 0;

                                // Tabla de facturas para esta moneda
                                column.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(2);
                                    });

                                    // Encabezados
                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                            .Text("N° Factura").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                            .Text("Proveedor").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                            .Text("País").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                            .Text("F. Vencimiento").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                            .Text("Días").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                            .Text("Mon.").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5)
                                            .AlignRight().Text("Monto").Bold();
                                    });


                                    // Filas de facturas de esta moneda
                                    foreach (var factura in grupoMoneda)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(factura.NumeroFactura);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(factura.Acreedor);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(factura.Pais);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(factura.FechaVencimiento.ToString("dd/MM/yyyy"));
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(factura.DiasVencimiento.ToString());
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(factura.Moneda);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).AlignRight().Text(factura.Monto.ToString("N2"));

                                        subtotal += factura.Monto;
                                    }
                                });

                                // Subtotal por moneda en este rango
                                column.Item().PaddingTop(5).PaddingBottom(10).AlignRight()
                                    .Text($"Subtotal {grupoMoneda.Key}: {subtotal:N2}")
                                    .Bold()
                                    .FontColor("#0B1F34");

                                // Acumular en el total general por moneda
                                if (!totalesPorMoneda.ContainsKey(grupoMoneda.Key))
                                    totalesPorMoneda[grupoMoneda.Key] = 0;

                                totalesPorMoneda[grupoMoneda.Key] += subtotal;
                            }
                        }

                        // Total general por moneda
                        column.Item().PaddingTop(20).Background("#0B1F34").Padding(10)
                            .Column(totalColumn =>
                            {
                                totalColumn.Item().AlignCenter()
                                    .Text("TOTALES GENERALES")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.White);

                                totalColumn.Item().PaddingTop(5);

                                foreach (var totalMoneda in totalesPorMoneda.OrderBy(m => m.Key))
                                {
                                    totalColumn.Item().AlignRight()
                                        .Text($"{totalMoneda.Key}: {totalMoneda.Value:N2}")
                                        .FontSize(12)
                                        .Bold()
                                        .FontColor(Colors.White);
                                }
                            });
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}