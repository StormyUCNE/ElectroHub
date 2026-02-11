using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ElectroHub.Models;

public class Productos
{
    [Key]
    public int ProductoId { get; set; }
    public bool Eliminado { get; set; } = false;

    [Required(ErrorMessage ="Debe de llenar este campo.")]
    [Range(1, Double.MaxValue, ErrorMessage = "Código del Producto debe de ser mayor a Cero.")]
    public int CodigoProducto { get; set; }

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "El Nombre debe tener entre 5 y 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [StringLength(75, MinimumLength = 5, ErrorMessage = "La Descripción debe tener entre 5 y 50 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [ForeignKey(nameof(CategoriaId))]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [Range(1, Double.MaxValue, ErrorMessage = "Precio Compra del Producto debe de ser mayor a Cero.")]
    public decimal? PrecioCompra { get; set; }

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [Range(1, Double.MaxValue, ErrorMessage = "Precio Venta del Producto debe de ser mayor a Cero.")]
    public decimal? PrecioVenta { get; set; }

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [Range(1, int.MaxValue, ErrorMessage = "Cantidad de Inventario del Producto debe de ser mayor a Cero.")]
    public int? CantidadInventario { get; set; }

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [Range(1, int.MaxValue, ErrorMessage = "Stock Minimo del Producto debe de ser mayor a Cero.")]
    public int? StockMinimo { get; set; }

    [ForeignKey(nameof(ProveedorId))]
    public int ProveedorId { get; set; }

    [ForeignKey(nameof(EstadoProductoId))]
    public int EstadoProductoId { get; set; }

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    Categorias? Categorias { get; set; }
    Proveedores? Proveedores { get; set; }
    EstadosProductos? EstadosProductos { get; set; }
}