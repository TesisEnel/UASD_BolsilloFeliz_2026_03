using System.ComponentModel.DataAnnotations;

namespace BolsilloFeliz.Application.DTOs;

public class NotificacionDto
{
    public int IdNotificacion { get; set; }

    public int IdUsuario { get; set; }

    [Required, StringLength(120)]
    public string Titulo { get; set; } = null!;

    [Required, StringLength(500)]
    public string Mensaje { get; set; } = null!;

    [Required, StringLength(30)]
    public string Tipo { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public bool Leida { get; set; }
}
