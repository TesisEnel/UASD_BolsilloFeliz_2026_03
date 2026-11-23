using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.CategoriaDto
public class CategoriaDto
{
    public int IdCategoria { get; set; }
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = "";

    // true = Ingreso, false = Gasto
    public bool Tipo { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }
}