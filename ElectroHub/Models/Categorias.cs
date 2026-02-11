using System.ComponentModel.DataAnnotations;
namespace ElectroHub.Models;

public class Categorias
{
    [Key]
    public int CategoriaId { get; set; }
    public bool Eliminado { get; set; } = false;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [MinLength(4, ErrorMessage ="Nombre de Categoría muy corto, debe de contener más de 3 Carácteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    [MinLength(4, ErrorMessage = "Descripción de Categoría muy corto, debe de contener más de 3 Carácteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe de llenar este campo.")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}