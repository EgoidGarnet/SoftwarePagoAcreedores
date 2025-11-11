using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SoftPacBusiness.PropuestaPagoWS;
using SoftPacBusiness.ReportePropPagoWS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Helpers;

namespace SoftPac.Business
{
    public class ReportePropuestasBO
    {
        private ReportePropPagoWSClient reportePropuestaClienteSOAP = new ReportePropPagoWSClient();

        public ReportePropuestasBO()
        {
            // Configurar QuestPDF (solo necesario una vez en la aplicación)
            QuestPDF.Settings.License = LicenseType.Community;
        }

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
            int? diasDesde,
            string estado)
        {
            // Si diasDesde es null, usar 90 días por defecto
            int dias = diasDesde ?? 90;

            // Llamar al webservice con los nuevos parámetros
            var propuestasDTO = reportePropuestaClienteSOAP.listarReportePropuesta(
                paisId.HasValue ? (int)paisId : 0,
                bancoId.HasValue ? (int)bancoId : 0,
                dias
            );

            if (propuestasDTO == null || propuestasDTO.Length == 0)
                return new List<PropuestaDetalle>();

            // Convertir de ReportePropPagoDTO[] a List<PropuestaDetalle>
            var resultado = propuestasDTO.Select(dto => new PropuestaDetalle
            {
                PropuestaId = dto.idPropuesta,
                FechaCreacion = dto.fechaCreacion,
                UsuarioCreador = !string.IsNullOrEmpty(dto.correoUsuarioCreador)
                    ? $"{dto.usuarioCreador} ({dto.correoUsuarioCreador})"
                    : dto.usuarioCreador ?? "-",
                Pais = dto.pais ?? "-",
                EntidadBancaria = dto.bancoPropuesta ?? "-",
                Estado = dto.estado ?? "-",
                TotalPagos = dto.totalPagos,
                Detalles = dto.pagos?.Select(pago => new DetallePago
                {
                    NumeroFactura = pago.numeroFactura ?? "-",
                    Acreedor = pago.acreedor ?? "-",
                    Moneda = pago.moneda ?? "-",
                    Monto = pago.monto,
                    CuentaOrigen = pago.cuentaOrigen ?? "-",
                    BancoOrigen = pago.bancoOrigen ?? "-",
                    CuentaDestino = pago.cuentaDestino ?? "-",
                    BancoDestino = pago.bancoDestino ?? "-",
                    FormaPago = pago.formaPago ?? "-"
                }).ToList() ?? new List<DetallePago>()
            }).ToList();

            // Filtrar por estado en memoria si se especificó
            if (!string.IsNullOrEmpty(estado))
            {
                resultado = resultado
                    .Where(p => p.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Ordenar por fecha de creación descendente
            resultado = resultado
                .OrderByDescending(p => p.FechaCreacion)
                .ToList();

            return resultado;
        }

        public byte[] GenerarPDF(
       List<PropuestaDetalle> datos,
       string usuario,
       Dictionary<string, string> filtros)
        {
            try
            {

                return Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                        // Cabecera
                        page.Header().Column(column =>
                        {
                            column.Item().AlignCenter().Text("REPORTE DE PROPUESTAS DE PAGO")
                                .FontSize(18)
                                .Bold()
                                .FontColor(Colors.Blue.Darken4);

                            column.Item().AlignCenter().Text("Detalle de Pagos Individuales por Propuesta")
                                .FontSize(12)
                                .FontColor(Colors.Grey.Darken1);

                            column.Item().PaddingTop(10);

                            // Información de auditoría
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Text("Fecha de Generación:").Bold();
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Text(DateTime.Now.ToString("dd/MM/yyyy"));

                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Text("Hora de Generación:").Bold();
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Text(DateTime.Now.ToString("HH:mm:ss"));

                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Text("Usuario:").Bold();
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5)
                                    .Text(usuario ?? "-");
                            });

                            // Filtros aplicados
                            if (filtros != null && filtros.Count > 0)
                            {
                                column.Item().PaddingTop(10).Text(text =>
                                {
                                    text.Span("Filtros Aplicados: ").Bold().FontColor(Colors.Blue.Darken4);

                                    foreach (var filtro in filtros)
                                    {
                                        text.Span($"{filtro.Key}: {filtro.Value} | ");
                                    }
                                });
                            }
                        });

                        // Contenido principal
                        page.Content().Column(column =>
                        {
                            if (datos == null || !datos.Any())
                            {
                                column.Item().PaddingTop(20).AlignCenter()
                                    .Text("No hay datos para mostrar")
                                    .FontSize(14)
                                    .Italic();
                                return;
                            }

                            int totalPropuestas = 0;
                            int totalPagos = 0;

                            foreach (var propuesta in datos)
                            {
                                totalPropuestas++;
                                totalPagos += propuesta.TotalPagos;

                                // Encabezado de propuesta
                                column.Item().PaddingTop(10).Background(Colors.Blue.Darken4)
                                    .Padding(8).Text($"Propuesta #{propuesta.PropuestaId} - {propuesta.EntidadBancaria ?? "N/A"}")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.White);

                                // Información de la propuesta
                                column.Item().BorderLeft(1).BorderRight(1).BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(8)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(100);
                                            columns.RelativeColumn();
                                            columns.ConstantColumn(100);
                                            columns.RelativeColumn();
                                        });

                                        table.Cell().Text("Fecha Creación:").Bold().FontSize(9);
                                        table.Cell().Text(propuesta.FechaCreacion.ToString("dd/MM/yyyy HH:mm")).FontSize(9);
                                        table.Cell().Text("Usuario:").Bold().FontSize(9);
                                        table.Cell().Text(propuesta.UsuarioCreador ?? "-").FontSize(9);

                                        table.Cell().Text("País:").Bold().FontSize(9);
                                        table.Cell().Text(propuesta.Pais ?? "-").FontSize(9);
                                        table.Cell().Text("Estado:").Bold().FontSize(9);
                                        table.Cell().Text(propuesta.Estado ?? "-").FontSize(9);
                                    });

                                // Tabla de detalles
                                if (propuesta.Detalles != null && propuesta.Detalles.Any())
                                {
                                    column.Item().PaddingTop(5).Table(detallesTable =>
                                    {
                                        // Definir columnas con tamaños ajustados
                                        detallesTable.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(45);   // N° Factura
                                            columns.RelativeColumn(2);    // Proveedor
                                            columns.ConstantColumn(35);   // Moneda
                                            columns.ConstantColumn(55);   // Monto
                                            columns.ConstantColumn(55);   // Cta. Origen
                                            columns.RelativeColumn(1.5f); // Banco Orig.
                                            columns.ConstantColumn(55);   // Cta. Dest.
                                            columns.RelativeColumn(1.5f); // Banco Dest.
                                            columns.ConstantColumn(50);   // Forma Pago
                                        });

                                        // Encabezados
                                        detallesTable.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Factura").FontSize(8).Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Proveedor").FontSize(8).Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Mon").FontSize(8).Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Monto").FontSize(8).Bold().AlignRight();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Cta Orig").FontSize(8).Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Banco Orig").FontSize(8).Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Cta Dest").FontSize(8).Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("Banco Dest").FontSize(8).Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3).Padding(3)
                                                .Text("F. Pago").FontSize(8).Bold();
                                        });

                                        // Filas de datos
                                        foreach (var detalle in propuesta.Detalles)
                                        {
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.NumeroFactura ?? "-").FontSize(7);
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.Acreedor ?? "-").FontSize(7);
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.Moneda ?? "-").FontSize(7);
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.Monto.ToString("N2")).FontSize(7).AlignRight();
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.CuentaOrigen ?? "-").FontSize(7);
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.BancoOrigen ?? "-").FontSize(7);
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.CuentaDestino ?? "-").FontSize(7);
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.BancoDestino ?? "-").FontSize(7);
                                            detallesTable.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                                .Padding(2).Text(detalle.FormaPago ?? "-").FontSize(7);
                                        }
                                    });

                                    // Totales por moneda
                                    var totalesPorMoneda = propuesta.Detalles
                                        .GroupBy(d => d.Moneda)
                                        .Select(g => new { Moneda = g.Key, Total = g.Sum(x => x.Monto) })
                                        .ToList();

                                    column.Item().PaddingTop(5).PaddingLeft(5).Text(text =>
                                    {
                                        text.Span("Totales: ").Bold().FontSize(9);

                                        foreach (var total in totalesPorMoneda)
                                        {
                                            text.Span($"{total.Moneda}: {total.Total:N2}   ").FontSize(9);
                                        }
                                    });
                                }

                                column.Item().PaddingBottom(10);
                            }

                            // Resumen final
                            column.Item().PaddingTop(10).Background(Colors.Blue.Darken4)
                                .Padding(10).AlignCenter()
                                .Text($"RESUMEN TOTAL: {totalPropuestas} Propuestas | {totalPagos} Pagos")
                                .FontSize(12)
                                .Bold()
                                .FontColor(Colors.White);
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
                }).GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF: {ex.Message}", ex);
            }
        }
    }
}