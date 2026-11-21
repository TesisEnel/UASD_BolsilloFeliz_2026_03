using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class PresupuestoDto
{
    public int IdPresupuesto { get; set; }

    public int IdUsuario { get; set; }

    public int IdCategoria { get; set; }

    public int? IdCuenta { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = null!;

    [Range(0.01, 922337203685477)]
    public decimal Monto { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public bool Estado { get; set; } = true;
}