namespace BolsilloFeliz.Application.DTOs;

public class TransferenciaDto
{
    public int IdTransferencia { get; set; }

    public int IdUsuario { get; set; }

    public int IdTransaccion { get; set; }

    public int IdCuentaOrigen { get; set; }

    public int IdCuentaDestino { get; set; }
}