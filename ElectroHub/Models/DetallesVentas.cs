using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroHub.Models;

public class DetallesVentas
{
    [Key]
    public int IdDetalle { get; set; }

    public bool Eliminado { get; set; } = false;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Venta.")]
    public int VentaId { get; set; }

    [ForeignKey(nameof(VentaId))]
    public Ventas? Ventas { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un Producto.")]
    public int ProductoId { get; set; }

    [ForeignKey(nameof(ProductoId))]
    public Productos? Productos { get; set; }

    [StringLength(50, MinimumLength = 5, ErrorMessage = "La descripcion debe tener entre 5 y 50 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    public Categorias? Categoria { get; set; }

    [Required(ErrorMessage = "La cantidad del producto es obligatorio")]
    public int Cantidad { get; set; } = 1;

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser positivo")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El ITBIS es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El ITBIS debe ser positivo")]
    public decimal Itbis { get; set; }

    [Required(ErrorMessage = "El subtotal es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser positivo")]
    public decimal Subtotal { get; set; }
}