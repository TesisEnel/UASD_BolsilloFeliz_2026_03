using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(UsuariosService service) : ControllerBase
{
    // GET api/usuarios
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await service.GetList(u => true);
        return resultado.IsFailure ? Falla(resultado.Error) : Ok(resultado.Value);
    }

    // GET api/usuarios/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await service.Buscar(id);

        if (resultado.IsFailure) return Falla(resultado.Error);
        return resultado.Value is null ? NotFound() : Ok(resultado.Value);
    }

    // POST api/usuarios
    [HttpPost]
    public async Task<IActionResult> Crear(UsuarioDto dto)
    {
        dto.IdUsuario = 0; // siempre es un registro nuevo

        var resultado = await service.Guardar(dto);
        if (resultado.IsFailure) return Falla(resultado.Error);

        // Se busca por email para devolver el usuario creado con su Id
        var email = dto.Email.Trim().ToLowerInvariant();
        var creado = await service.GetList(u => u.Email == email);

        var usuario = creado.Value?.FirstOrDefault();
        return usuario is null
            ? StatusCode(500, new { mensaje = "No se pudo recuperar el usuario creado." })
            : CreatedAtAction(nameof(ObtenerPorId), new { id = usuario.IdUsuario }, usuario);
    }

    // PUT api/usuarios/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, UsuarioDto dto)
    {
        var existente = await service.Buscar(id);
        if (existente.IsFailure) return Falla(existente.Error);
        if (existente.Value is null) return NotFound();

        dto.IdUsuario = id;

        var resultado = await service.Guardar(dto);
        if (resultado.IsFailure) return Falla(resultado.Error);

        return Ok((await service.Buscar(id)).Value);
    }

    // PUT api/usuarios/5/password
    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> CambiarPassword(int id, CambiarPasswordDto dto)
    {
        var resultado = await service.CambiarPassword(id, dto);
        return resultado.IsFailure ? Falla(resultado.Error) : NoContent();
    }

    // DELETE api/usuarios/5  (desactivación lógica)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var existente = await service.Buscar(id);
        if (existente.IsFailure) return Falla(existente.Error);
        if (existente.Value is null) return NotFound();

        var resultado = await service.Eliminar(id);
        return resultado.IsFailure ? Falla(resultado.Error) : NoContent();
    }

    // Convierte el Error del Result en la respuesta HTTP adecuada
    private IActionResult Falla(Error error)
    {
        var cuerpo = new { codigo = error.Code, mensaje = error.Description };

        return error.Code switch
        {
            "EMAIL_DUPLICADO" => Conflict(cuerpo),
            "TIENE_DATOS_RELACIONADOS" => Conflict(cuerpo),
            "NO_ENCONTRADO" => NotFound(cuerpo),
            _ => BadRequest(cuerpo)
        };
    }

    // POST api/usuarios/login
    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion(LoginDto dto)
    {
        var resultado = await service.IniciarSesion(dto);
        if (resultado.IsFailure) return Falla(resultado.Error);

        return resultado.Value is null
            ? Unauthorized(new { codigo = "CREDENCIALES_INVALIDAS", mensaje = "Email o contraseña incorrectos." })
            : Ok(resultado.Value);
    }
}