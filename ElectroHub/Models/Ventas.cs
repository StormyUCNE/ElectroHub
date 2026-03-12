using ElectroHub.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElectroHub.Models;

public class Ventas
{
    [Key]
    public int VentaId { get; set; }

    public bool Eliminado { get; set; } = false;

    public ICollection<DetallesVentas> DetallesVentas { get; set; } = new List<DetallesVentas>();

    [Required(ErrorMessage = "La fecha de registro de venta es obligatorio")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El vendedor es obligatorio")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "El Vendedor debe tener entre 2 y 50 caracteres")]
    public string Vendedor { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0% y 100%")]
    public decimal Descuento { get; set; }

    [Required(ErrorMessage = "El metodo de pago es obligatorio")]
    public MetodosPago MetodoPago { get; set; }

    [Required(ErrorMessage = "El monto recibido es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El monto recibido debe ser positivo")]
    public decimal MontoRecibido { get; set; }

    [Required(ErrorMessage = "El subtotal es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser positivo")]
    public decimal Subtotal { get; set; }

    [Required(ErrorMessage = "El descuento aplicado es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El descuento aplicado debe ser positivo")]
    public decimal DescuentoAplicado { get; set; }

    [Required(ErrorMessage = "El ITBIS es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El ITBIS debe ser positivo")]
    public decimal Itbis { get; set; }

    [Required(ErrorMessage = "El total es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El total debe ser positivo")]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "El Vuelto es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El vuelto debe ser positivo")]
    public decimal Vuelto { get; set; }
}