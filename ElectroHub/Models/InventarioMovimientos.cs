using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElectroHub.Models;

public class InventarioMovimientos
{
    [Key]
    public int MovimientoId { get; set; }
    public int ProductoId { get; set; }

    [ForeignKey("ProductoId")]
    public Productos? Producto { get; set; }

    public DateTime FechaMovimiento { get; set; }
    public string TipoMovimiento { get; set; } // Entrada, Salida, Ajuste
    public int Cantidad { get; set; }
    public int StockResultante { get; set; }
    public string Usuario { get; set; }
}