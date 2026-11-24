using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BolsilloFeliz.Application.DTOs;

public class UsuarioDto
{
    public int IdUsuario { get; set; }

    // Solo lectura: al crear siempre se asigna el rol de usuario
    public int IdRol { get; set; }

    [Required, StringLength(100)]
    public string Nombres { get; set; } = null!;

    [Required, StringLength(100)]
    public string Apellidos { get; set; } = null!;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = null!;

    // Solo se usa al crear (obligatoria). Nunca se devuelve en las respuestas.
    [StringLength(100)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }

    // Solo lectura
    public DateTime FechaRegistro { get; set; }

    public bool Estado { get; set; } = true;
}

public class CambiarPasswordDto
{
    [Required]
    public string PasswordActual { get; set; } = null!;

    [Required, StringLength(100, MinimumLength = 8)]
    public string PasswordNueva { get; set; } = null!;
}

public class LoginDto
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = "";
}