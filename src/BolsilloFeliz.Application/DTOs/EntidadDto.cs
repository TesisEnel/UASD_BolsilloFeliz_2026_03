using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class EntidadDto
{
    public int IdEntidad { get; set; }

    public int IdUsuario { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(30)]
    public string TipoEntidad { get; set; } = null!;

    [StringLength(30)]
    public string? Identificacion { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Direccion { get; set; }
}
