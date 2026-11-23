using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.MetaAhorroDto
public class MetaAhorroDto
{
    public int IdMetaAhorro { get; set; }
    public int IdUsuario { get; set; }
    public int IdMoneda { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120)]
    public string Nombre { get; set; } = "";

    [Range(0.01, 922337203685477, ErrorMessage = "El monto objetivo debe ser mayor que cero.")]
    public decimal MontoObjetivo { get; set; }

    public DateOnly FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? FechaObjetivo { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public bool Estado { get; set; } = true;
}

// Copia de BolsilloFeliz.Application.DTOs.AporteAhorroDto
public class AporteAhorroDto
{
    public int IdAporteAhorro { get; set; }
    public int IdUsuario { get; set; }
    public int IdMetaAhorro { get; set; }
    public int? IdCuenta { get; set; }

    [Range(0.01, 922337203685477, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Monto { get; set; }

    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [StringLength(250)]
    public string? Nota { get; set; }
}
