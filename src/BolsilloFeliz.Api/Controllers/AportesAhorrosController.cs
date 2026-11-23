using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AportesAhorrosController(AportesAhorrosService service) : ControllerBase
{
    // GET api/AportesAhorros  (opcional: ?idUsuario=1)
    [HttpGet]
    public async Task<ActionResult<List<AporteAhorroDto>>> Get([FromQuery] int? idUsuario = null)
    {
        var result = await service.GetList(x => idUsuario == null || x.IdUsuario == idUsuario);

        List<AporteAhorroDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de aporte de ahorros.")
            : Ok(lista);
    }

    // GET api/AportesAhorros/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AporteAhorroDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe aporte de ahorro con id {id}.")
            : Ok(dto);
    }

    // POST api/AportesAhorros
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AporteAhorroDto dto)
    {
        dto.IdAporteAhorro = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/AportesAhorros/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] AporteAhorroDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe aporte de ahorro con id {id}.");

        dto.IdAporteAhorro = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/AportesAhorros/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe aporte de ahorro con id {id}.");

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

    private async Task<AporteAhorroDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        AporteAhorroDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(AporteAhorroDto dto, Func<IActionResult> alExito)
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
