using ElectroHub.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ElectroHub.Models;

public class Proveedores
{

    public bool Eliminado { get; set; } = false;

    [Key]
    public int ProveedorId { get; set; }

    [Required(ErrorMessage = "El nombre de la Empresa es Obligatorio.")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "El Nombre debe tener entre 5 y 50 caracteres")]
    public string NombreEmpresa { get; set; } = string.Empty;


    [Required(ErrorMessage = "El número del Contacto es Obligatorio.")]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}", ErrorMessage = "Debe de completar el número telefónico, (Ej:000-000-0000")]
    public string Contacto { get; set; } = string.Empty;


    [Required(ErrorMessage = "El número telefónico de la Empresa es Obligatorio.")]
    [RegularExpression(@"^\d{3}-\d{3}-\d{4}", ErrorMessage = "Debe de completar el número telefónico, (Ej:000-000-0000")]
    public string Telefono { get; set; } = string.Empty;


    [EmailAddress(ErrorMessage = "El Correo Electrónico debe de estar en el formato correcto (Ej:minombre@gmail.com)")]
    public string CorreoElectronico { get; set; } = string.Empty;



    [Required(ErrorMessage = "El Tipo de proveedor es obligatorio")]
    public TiposProveedores TipoProveedor { get; set; }



    [Required(ErrorMessage = "Campo es obligatorio.")]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "La Dirección debe tener entre 10 y 100 caracteres")]
    public string Direccion { get; set; } = string.Empty;


    public EstadosProveedores EstadoProveedor { get; set; } = EstadosProveedores.Activo;



}