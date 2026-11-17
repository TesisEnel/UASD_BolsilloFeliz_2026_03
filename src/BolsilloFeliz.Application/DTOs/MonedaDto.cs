using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class MonedaDto
{
    public int IdMoneda { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string Codigo { get; set; } = null!;

    [Required, StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(5)]
    public string Simbolo { get; set; } = null!;

    public bool Activa { get; set; } = true;
}