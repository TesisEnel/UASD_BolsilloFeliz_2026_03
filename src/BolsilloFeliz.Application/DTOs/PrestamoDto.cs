using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class PrestamoDto
{
    public int IdPrestamo { get; set; }

    public int IdUsuario { get; set; }

    public int IdEntidad { get; set; }

    public int? IdTransaccionDesembolso { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = null!;

    public decimal MontoOriginal { get; set; }

    public DateOnly FechaPrestamo { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public decimal? CuotaMensual { get; set; }

    [Required, StringLength(20)]
    public string Estado { get; set; } = "Activo";

    [StringLength(250)]
    public string? Descripcion { get; set; }
}
