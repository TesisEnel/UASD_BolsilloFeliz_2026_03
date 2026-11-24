using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class AporteAhorroDto
{
    public int IdAporteAhorro { get; set; }

    public int IdUsuario { get; set; }

    public int IdMetaAhorro { get; set; }

    public int? IdCuenta { get; set; }

    [Range(0.01, 922337203685477)]
    public decimal Monto { get; set; }

    public DateOnly Fecha { get; set; }

    [StringLength(250)]
    public string? Nota { get; set; }
}
