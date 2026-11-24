using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.TransaccionDto
public class TransaccionDto
{
    public int IdTransaccion { get; set; }
    public int IdUsuario { get; set; }
    public int? IdCuenta { get; set; }
    public int? IdCategoria { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(120)]
    public string Nombre { get; set; } = "";

    [Range(0.01, 922337203685477, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Monto { get; set; }

    public string Tipo { get; set; } = "Gasto";

    public DateTime Fecha { get; set; } = DateTime.Today;

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public string Estado { get; set; } = "Completada";
}
