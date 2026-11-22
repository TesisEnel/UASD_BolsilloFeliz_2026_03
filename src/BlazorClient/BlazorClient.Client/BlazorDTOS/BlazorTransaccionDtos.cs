using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.TransaccionDto
public class TransaccionDto
{
    public int IdTransaccion { get; set; }
    public int IdUsuario { get; set; }
    public int? IdCuenta { get; set; }
    public int? IdCategoria { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = "";

    public decimal Monto { get; set; }

    public string Tipo { get; set; } = "Gasto";

    public DateTime Fecha { get; set; } = DateTime.Today;

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public string Estado { get; set; } = "Completada";
}
