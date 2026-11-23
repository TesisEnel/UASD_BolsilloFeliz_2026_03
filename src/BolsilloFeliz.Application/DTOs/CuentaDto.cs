using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class CuentaDto
{
    public int IdCuenta { get; set; }

    public int IdUsuario { get; set; }

    public int IdMoneda { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(30)]
    [AllowedValues("Efectivo", "Banco", "Ahorro", "Tarjeta de credito", "Inversion", "Otro")]
    public string Tipo { get; set; } = "Efectivo";

    public decimal SaldoInicial { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }
}
