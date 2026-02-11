using System.ComponentModel.DataAnnotations;
namespace ElectroHub.Models;
public class Categorias
{
    [Key]
    public int CategoriaId { get; set; }
    public bool Eliminado { get; set; } = false;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Nombre de Categoría muy corto, debe de contener más de 5 Carácteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [StringLength(75, MinimumLength = 5, ErrorMessage = "Descripción de Categoría muy corto, debe de contener mínimo 5 y máximo 75 Carácteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}