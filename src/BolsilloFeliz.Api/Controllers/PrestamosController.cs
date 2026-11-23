using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamosController(PrestamosService service) : ControllerBase
{
    // GET api/Prestamos  (opcional: ?idUsuario=1)
    [HttpGet]
    public async Task<ActionResult<List<PrestamoDto>>> Get([FromQuery] int? idUsuario = null)
    {
        var result = await service.GetList(x => idUsuario == null || x.IdUsuario == idUsuario);

        List<PrestamoDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de préstamos.")
            : Ok(lista);
    }

    // GET api/Prestamos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrestamoDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe préstamo con id {id}.")
            : Ok(dto);
    }

    // POST api/Prestamos
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PrestamoDto dto)
    {
        dto.IdPrestamo = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/Prestamos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] PrestamoDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe préstamo con id {id}.");

        dto.IdPrestamo = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/Prestamos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe préstamo con id {id}.");

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

    private async Task<PrestamoDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        PrestamoDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(PrestamoDto dto, Func<IActionResult> alExito)
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
