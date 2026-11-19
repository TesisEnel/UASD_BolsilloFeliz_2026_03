using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class RolDto
{
    public int IdRol { get; set; }

    [Required, StringLength(50)]
    public string Nombre { get; set; } = null!;
}