using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.DeudaDto
public class DeudaDto
{
    public int IdDeuda { get; set; }
    public int IdUsuario { get; set; }
    public int IdEntidad { get; set; }
    public int? IdTransaccionDesembolso { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120)]
    public string Nombre { get; set; } = "";

    [Range(0.01, 922337203685477, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal MontoOriginal { get; set; }

    public DateOnly FechaDeuda { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? FechaVencimiento { get; set; }
    public decimal? CuotaMensual { get; set; }

    [Required, StringLength(20)]
    public string Estado { get; set; } = "Activa";

    [StringLength(250)]
    public string? Descripcion { get; set; }
}

// Copia de BolsilloFeliz.Application.DTOs.DetallesDeudaDto
public class DetallesDeudaDto
{
    public int IdDetalleDeuda { get; set; }
    public int IdUsuario { get; set; }
    public int IdDeuda { get; set; }
    public int IdTransaccion { get; set; }

    public decimal MontoPagado { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.Today;

    [StringLength(250)]
    public string? Descripcion { get; set; }
}
