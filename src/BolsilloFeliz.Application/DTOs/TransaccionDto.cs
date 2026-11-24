using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class TransaccionDto
{
    public int IdTransaccion { get; set; }

    public int IdUsuario { get; set; }

    public int? IdCuenta { get; set; }

    public int? IdCategoria { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = null!;

    [Range(0.01, 922337203685477)]
    public decimal Monto { get; set; }

    [Required, StringLength(30)]
    [AllowedValues("Pago de deuda", "Abono de prestamo", "Deuda recibida",
                   "Prestamo otorgado", "Transferencia", "Gasto", "Ingreso")]
    public string Tipo { get; set; } = "Gasto";

    public DateTime Fecha { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Required, StringLength(20)]
    [AllowedValues("Anulada", "Pendiente", "Completada")]
    public string Estado { get; set; } = "Completada";
}