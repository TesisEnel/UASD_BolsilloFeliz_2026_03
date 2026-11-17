using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntidadesController(EntidadesService service) : ControllerBase
{
    // GET api/Entidades  (opcional: ?idUsuario=1)
    [HttpGet]
    public async Task<ActionResult<List<EntidadDto>>> Get([FromQuery] int? idUsuario = null)
    {
        var result = await service.GetList(x => idUsuario == null || x.IdUsuario == idUsuario);

        List<EntidadDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de entidads.")
            : Ok(lista);
    }

    // GET api/Entidades/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EntidadDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe entidad con id {id}.")
            : Ok(dto);
    }

    // POST api/Entidades
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EntidadDto dto)
    {
        dto.IdEntidad = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/Entidades/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] EntidadDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe entidad con id {id}.");

        dto.IdEntidad = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/Entidades/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe entidad con id {id}.");

        try
        {
            var result = await service.Eliminar(id);

            var eliminado = false;
            result.OnSuccess(() => eliminado = true);

            return eliminado ? NoContent() : Problem("No se pudo eliminar el registro.");
        }
        catch (DbUpdateException)
        {
            return Conflict("No se puede eliminar: el registro tiene datos relacionados.");
        }
    }

    private async Task<EntidadDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        EntidadDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(EntidadDto dto, Func<IActionResult> alExito)
    {
        try
        {
            var result = await service.Guardar(dto);

            var guardado = false;
            result.OnSuccess(() => guardado = true);

            return guardado ? alExito() : Problem("No se pudo guardar el registro.");
        }
        catch (DbUpdateException)
        {
            return BadRequest("No se pudo guardar: revise las llaves foráneas, los valores permitidos o que no haya duplicados.");
        }
    }
}
