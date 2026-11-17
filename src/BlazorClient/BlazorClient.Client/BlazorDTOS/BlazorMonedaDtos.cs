using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.MonedaDto
public class MonedaDto
{
    public int IdMoneda { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "El código debe tener exactamente 3 letras.")]
    public string Codigo { get; set; } = "";

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El símbolo es obligatorio.")]
    [StringLength(5)]
    public string Simbolo { get; set; } = "";

    public bool Activa { get; set; } = true;
}
