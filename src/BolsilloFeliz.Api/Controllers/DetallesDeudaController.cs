using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallesDeudaController(DetallesDeudaService service) : ControllerBase
{
    // GET api/DetallesDeuda  (opcional: ?idUsuario=1)
    [HttpGet]
    public async Task<ActionResult<List<DetallesDeudaDto>>> Get([FromQuery] int? idUsuario = null)
    {
        var result = await service.GetList(x => idUsuario == null || x.IdUsuario == idUsuario);

        List<DetallesDeudaDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de detalle de deudas.")
            : Ok(lista);
    }

    // GET api/DetallesDeuda/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DetallesDeudaDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe detalle de deuda con id {id}.")
            : Ok(dto);
    }

    // POST api/DetallesDeuda
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DetallesDeudaDto dto)
    {
        dto.IdDetalleDeuda = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/DetallesDeuda/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] DetallesDeudaDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe detalle de deuda con id {id}.");

        dto.IdDetalleDeuda = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/DetallesDeuda/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe detalle de deuda con id {id}.");

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

    private async Task<DetallesDeudaDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        DetallesDeudaDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(DetallesDeudaDto dto, Func<IActionResult> alExito)
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
