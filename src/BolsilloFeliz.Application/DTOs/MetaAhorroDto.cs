using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class MetaAhorroDto
{
    public int IdMetaAhorro { get; set; }

    public int IdUsuario { get; set; }

    public int IdMoneda { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = null!;

    [Range(0.01, 922337203685477)]
    public decimal MontoObjetivo { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaObjetivo { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public bool Estado { get; set; } = true;
}
