using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroHub.Models;

public class Proveedores
{
    [Key]
    public int ProveedorId { get; set; }
    public bool Eliminado { get; set; } = false;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "El Nombre debe tener entre 5 y 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}", ErrorMessage = "Debe de completar el número telefónico, (Ej:000-000-0000")]
    public string Contacto { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El Correo Electrónico debe de estar en el formato correcto (Ej:minombre@gmail.com)")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [ForeignKey(nameof(TipoProveedorId))]
    public int TipoProveedorId { get; set; }

    [Required(ErrorMessage = "Campo es obligatorio.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "La Dirección debe tener entre 10 y 100 caracteres")]
    public string Direccion { get; set; } = string.Empty;

    [ForeignKey(nameof(EstadoProveedorId))]
    public int EstadoProveedorId { get; set; }

    EstadosProveedores? EstadosProveedores { get; set; }
    TiposProveedores? TiposProveedores { get; set; }
}