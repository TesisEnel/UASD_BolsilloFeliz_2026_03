using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

// Transferencia + su transacción asociada, para crearlas/editarlas de forma atómica.
public class TransferenciaCompletaDto
{
    public int IdTransferencia { get; set; }

    public int IdTransaccion { get; set; }

    public int IdUsuario { get; set; }

    public int IdCuentaOrigen { get; set; }

    public int IdCuentaDestino { get; set; }

    [Range(0.01, 922337203685477)]
    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Required, StringLength(20)]
    [AllowedValues("Anulada", "Pendiente", "Completada")]
    public string Estado { get; set; } = "Completada";
}