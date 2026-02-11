using System.ComponentModel.DataAnnotations;
namespace ElectroHub.Models;
public class EstadosProveedores
{
    [Key]
    public int EstadoProveedorId { get; set; }

    [Required(ErrorMessage = "El nombre del estado es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del estado debe tener entre 3 y 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;
}

