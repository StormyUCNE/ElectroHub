using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroHub.Models;

public class Productos
{
    [Key]
    public int ProductoId { get; set; }
    public bool Eliminado { get; set; } = false;

    [Required(ErrorMessage = "El Código es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El Código debe ser mayor a 0.")]
    public int? CodigoProducto { get; set; }

    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "El Nombre debe tener entre 5 y 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La Descripción es obligatoria.")]
    [StringLength(75, MinimumLength = 5, ErrorMessage = "La Descripción debe tener entre 5 y 75 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [ForeignKey(nameof(CategoriaId))]
    public Categorias? Categorias { get; set; } // <--- ¡SIN [Required]!

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una Categoría.")]
    public int CategoriaId { get; set; } // Validamos el ID aquí

    [Required(ErrorMessage = "El Precio de Compra es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal? PrecioCompra { get; set; }

    [Required(ErrorMessage = "El Precio de Venta es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal? PrecioVenta { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int? CantidadInventario { get; set; }

    [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor a 0.")]
    public int? StockMinimo { get; set; }

    [ForeignKey(nameof(ProveedorId))]
    public Proveedores? Proveedores { get; set; } // <--- ¡SIN [Required]!

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Proveedor.")]
    public int ProveedorId { get; set; }

    [ForeignKey(nameof(EstadoProductoId))]
    public EstadosProductos? EstadosProductos { get; set; } // <--- ¡SIN [Required]!

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Estado.")]
    public int EstadoProductoId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}