using System.ComponentModel.DataAnnotations;


namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.TransferenciaDto
public class TransferenciaDto
{
    public int IdTransferencia { get; set; }
    public int IdUsuario { get; set; }
    public int IdTransaccion { get; set; }
    public int IdCuentaOrigen { get; set; }
    public int IdCuentaDestino { get; set; }
}

// Copia de BolsilloFeliz.Application.DTOs.TransferenciaCompletaDto
public class TransferenciaCompletaDto
{
    public int IdTransferencia { get; set; }
    public int IdTransaccion { get; set; }
    public int IdUsuario { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la cuenta de origen.")]
    public int IdCuentaOrigen { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la cuenta de destino.")]
    public int IdCuentaDestino { get; set; }

    [Range(0.01, 922337203685477, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Today;

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public string Estado { get; set; } = "Completada";
}

