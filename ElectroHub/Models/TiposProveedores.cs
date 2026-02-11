using System.ComponentModel.DataAnnotations;

namespace ElectroHub.Models;

public class TiposProveedores{
    [Key]
    public int TipoProveedorId { get; set; }

    [Required(ErrorMessage="El nombre del tipo de proveedor es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del proveedor debe tener entre 3 y 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;
}
