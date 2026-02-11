using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroHub.Models;

public class Proveedores
{
    [Key]
    public int ProveedorId { get; set; }

    public bool Eliminado { get; set; } = false;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [MinLength(4, ErrorMessage = "Nombre de Proveedor muy corto, debe de contener más de 3 Carácteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}", ErrorMessage = "Debe de completar el número telefónico, (Ej:000-000-0000")]
    public string Contacto { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El Correo Electrónico debe de estar en el formato correcto (Ej:minombre@gmail.com)")]
    public string CorreoElectronico { get; set; } = string.Empty;

    [ForeignKey(nameof(TipoProveedorId))]
    public int TipoProveedorId { get; set; }

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [MinLength(10, ErrorMessage = "Dirección de Proveedor muy corta, debe de contener más de 9 Carácteres")]
    public string Direccion { get; set; } = string.Empty;

    [ForeignKey(nameof(EstadoProveedorId))]
    public int EstadoProveedorId { get; set; }
}