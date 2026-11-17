using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorClient.Client.BlazorDTOS;

// Copia de BolsilloFeliz.Application.DTOs.UsuarioDto

public class UsuarioDto
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = "";

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100)]
    public string Apellidos { get; set; } = "";

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    [StringLength(254)]
    public string Email { get; set; } = "";

    [StringLength(100)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Estado { get; set; } = true;
}

public class CambiarPasswordDto
{
    [Required]
    public string PasswordActual { get; set; } = "";

    [Required, StringLength(100, MinimumLength = 8)]
    public string PasswordNueva { get; set; } = "";
}

// Forma del error que devuelve la API: { "codigo": "...", "mensaje": "..." }
public class ErrorApiDto
{
    public string? Codigo { get; set; }
    public string? Mensaje { get; set; }
}