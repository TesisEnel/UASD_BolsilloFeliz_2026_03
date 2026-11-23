using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallesPrestamoController(DetallesPrestamoService service) : ControllerBase
{
    // GET api/DetallesPrestamo  (opcional: ?idUsuario=1)
    [HttpGet]
    public async Task<ActionResult<List<DetallesPrestamoDto>>> Get([FromQuery] int? idUsuario = null)
    {
        var result = await service.GetList(x => idUsuario == null || x.IdUsuario == idUsuario);

        List<DetallesPrestamoDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de detalle de préstamos.")
            : Ok(lista);
    }

    // GET api/DetallesPrestamo/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DetallesPrestamoDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe detalle de préstamo con id {id}.")
            : Ok(dto);
    }

    // POST api/DetallesPrestamo
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DetallesPrestamoDto dto)
    {
        dto.IdDetallePrestamo = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/DetallesPrestamo/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] DetallesPrestamoDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe detalle de préstamo con id {id}.");

        dto.IdDetallePrestamo = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/DetallesPrestamo/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe detalle de préstamo con id {id}.");

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

    private async Task<DetallesPrestamoDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        DetallesPrestamoDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(DetallesPrestamoDto dto, Func<IActionResult> alExito)
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
