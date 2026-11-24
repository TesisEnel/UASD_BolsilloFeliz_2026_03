using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.CuentaDto
public class CuentaDto
{
    public int IdCuenta { get; set; }
    public int IdUsuario { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una moneda.")]
    public int IdMoneda { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = "";

    [Required, StringLength(30)]
    public string Tipo { get; set; } = "Efectivo";

    public decimal SaldoInicial { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }
}
