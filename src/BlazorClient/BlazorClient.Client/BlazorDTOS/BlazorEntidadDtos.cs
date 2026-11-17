using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.EntidadDto
public class EntidadDto
{
    public int IdEntidad { get; set; }

    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [StringLength(30)]
    public string TipoEntidad { get; set; } = "Persona";

    [StringLength(30)]
    public string? Identificacion { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El email no es válido.")]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Direccion { get; set; }
}
