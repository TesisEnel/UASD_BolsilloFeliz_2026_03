using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(RolesService service) : ControllerBase
{
    // GET api/Roles
    [HttpGet]
    public async Task<ActionResult<List<RolDto>>> Get()
    {
        var result = await service.GetList(x => true);

        List<RolDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de roles.")
            : Ok(lista);
    }

    // GET api/Roles/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RolDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe rol con id {id}.")
            : Ok(dto);
    }

    // POST api/Roles
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RolDto dto)
    {
        dto.IdRol = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/Roles/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] RolDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe rol con id {id}.");

        dto.IdRol = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/Roles/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe rol con id {id}.");

        try
        {
            var result = await service.Eliminar(id);

            if (result.IsFailure)
                return Falla(result.Error);

            var eliminado = false;
            result.OnSuccess(() => eliminado = true);

            return eliminado ? NoContent() : Problem("No se pudo eliminar el registro.");
        }
        catch (DbUpdateException)
        {
            return Conflict("No se puede eliminar: el registro tiene datos relacionados.");
        }
    }

    private async Task<RolDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        RolDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(RolDto dto, Func<IActionResult> alExito)
    {
        try
        {
            var result = await service.Guardar(dto);

            if (result.IsFailure)
                return Falla(result.Error);

            var guardado = false;
            result.OnSuccess(() => guardado = true);

            return guardado ? alExito() : Problem("No se pudo guardar el registro.");
        }
        catch (DbUpdateException)
        {
            return BadRequest("No se pudo guardar: revise las llaves foráneas, los valores permitidos o que no haya duplicados.");
        }
    }

    // Convierte el Error del Result en la respuesta HTTP adecuada
    private IActionResult Falla(Error error)
    {
        var cuerpo = new { codigo = error.Code, mensaje = error.Description };

        return error.Code switch
        {
            "ROL_DUPLICADO" or "TIENE_DATOS_RELACIONADOS" => Conflict(cuerpo),
            _ => BadRequest(cuerpo)
        };
    }
}