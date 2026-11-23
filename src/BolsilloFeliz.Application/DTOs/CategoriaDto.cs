using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class CategoriaDto
{
    public int IdCategoria { get; set; }

    public int IdUsuario { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

    // bit en la BD (ej.: true = Ingreso, false = Gasto)
    public bool Tipo { get; set; }

    [StringLength(250)]
    public string? Descripcion { get; set; }
}