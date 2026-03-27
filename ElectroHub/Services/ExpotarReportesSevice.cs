using System.Text;
using ClosedXML.Excel;
using ElectroHub.Models;

using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ElectroHub.Services;

public class ExportarReportesService
{

    public byte[] ExportarCSV(List<VentaReporteDTO> ventas)
    {
        var builder = new StringBuilder();

        builder.AppendLine("Producto,Categoria,Fecha,Ingreso");

        foreach (var venta in ventas)
        {
            var producto = venta.Producto.Contains(",") ? $"\"{venta.Producto}\"" : venta.Producto;
            builder.AppendLine($"{producto},{venta.Categoria},{venta.Fecha:yyyy-MM-dd},{venta.Ingreso}");
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }
    public byte[] ExportarExcel(List<VentaReporteDTO> ventas)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Ventas");
            worksheet.Cell(1, 1).Value = "Producto";
            worksheet.Cell(1, 2).Value = "Categoría";
            worksheet.Cell(1, 3).Value = "Fecha";
            worksheet.Cell(1, 4).Value = "Ingreso";

            for (int i = 0; i < ventas.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = ventas[i].Producto;
                worksheet.Cell(i + 2, 2).Value = ventas[i].Categoria;
                worksheet.Cell(i + 2, 3).Value = ventas[i].Fecha.ToShortDateString();
                worksheet.Cell(i + 2, 4).Value = ventas[i].Ingreso;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return content;
            }
        }
    }

    public byte[] ExportarPDF(List<VentaReporteDTO> ventas)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignCenter().Text("ElectroHub").FontSize(30).Medium().FontColor(Colors.Black);
                        col.Item().Text("Reporte de Ventas").FontSize(15).SemiBold();      
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text("Generado el:").FontSize(10).FontColor(Colors.Grey.Medium);
                        col.Item().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(10);
                    });
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Producto");
                        header.Cell().Text("Categoría");
                        header.Cell().Text("Fecha");
                        header.Cell().Text("Ingreso");
                    });

                    foreach (var venta in ventas)
                    {
                        table.Cell().Text(venta.Producto);
                        table.Cell().Text(venta.Categoria);
                        table.Cell().Text(venta.Fecha.ToShortDateString());
                        table.Cell().Text(venta.Ingreso.ToString("C"));
                    }
                });
            });
        }).GeneratePdf();
    }
}