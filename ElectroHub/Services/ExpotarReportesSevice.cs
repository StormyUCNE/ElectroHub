using System.Text;
using ClosedXML.Excel;
using ElectroHub.Models; 
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace ElectroHub.Services
{
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
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte de Ventas");

            worksheet.Cell(1, 1).Value = "Producto";
            worksheet.Cell(1, 2).Value = "Categoría";
            worksheet.Cell(1, 3).Value = "Fecha";
            worksheet.Cell(1, 4).Value = "Ingreso";

            var headerRange = worksheet.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1f66d8"); 
            headerRange.Style.Font.FontColor = XLColor.White;

            int row = 2;
            foreach (var venta in ventas)
            {
                worksheet.Cell(row, 1).Value = venta.Producto;
                worksheet.Cell(row, 2).Value = venta.Categoria;
                worksheet.Cell(row, 3).Value = venta.Fecha.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 4).Value = venta.Ingreso;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00"; 
                row++;
            }

            worksheet.Columns().AdjustToContents(); 

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportarPDF(List<VentaReporteDTO> ventas)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            PdfFont fontNegrita = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            document.Add(new Paragraph("Reporte de Ventas - ElectroHub")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20)
                .SetFont(fontNegrita));

            document.Add(new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            Table table = new Table(4).UseAllAvailableWidth();

            table.AddHeaderCell(new Cell().Add(new Paragraph("Producto").SetFont(fontNegrita)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Categoría").SetFont(fontNegrita)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha").SetFont(fontNegrita)));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ingreso").SetFont(fontNegrita)));

            decimal totalIngresos = 0;

            foreach (var venta in ventas)
            {
                table.AddCell(new Cell().Add(new Paragraph(venta.Producto ?? "")));
                table.AddCell(new Cell().Add(new Paragraph(venta.Categoria ?? "")));
                table.AddCell(new Cell().Add(new Paragraph(venta.Fecha.ToString("dd MMM yyyy"))));
                table.AddCell(new Cell().Add(new Paragraph(venta.Ingreso.ToString("C"))));

                totalIngresos += venta.Ingreso;
            }

            document.Add(table);

            document.Add(new Paragraph($"Total Ingresos: {totalIngresos:C}")
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetMarginTop(20)
                .SetFont(fontNegrita)
                .SetFontSize(14));

            document.Close();
            return stream.ToArray();
        }
    }
}