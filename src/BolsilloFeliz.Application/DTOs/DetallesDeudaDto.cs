using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class DetallesDeudaDto
{
    public int IdDetalleDeuda { get; set; }

    public int IdUsuario { get; set; }

    public int IdDeuda { get; set; }

    public int IdTransaccion { get; set; }

    public decimal MontoPagado { get; set; }

    public DateTime FechaPago { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }
}
