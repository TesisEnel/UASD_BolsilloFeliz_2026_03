using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.PresupuestoDto
public class PresupuestoDto
{
    public int IdPresupuesto { get; set; }
    public int IdUsuario { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
    public int IdCategoria { get; set; }

    public int? IdCuenta { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(120)]
    public string Nombre { get; set; } = "";

    [Range(0.01, 922337203685477, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Monto { get; set; }

    public DateOnly FechaInicio { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);

    public DateOnly FechaFin { get; set; } =
        new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public bool Estado { get; set; } = true;
}
