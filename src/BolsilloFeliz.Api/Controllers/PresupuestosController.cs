using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresupuestosController(PresupuestosService service) : ControllerBase
{
    // GET api/Presupuestos  (opcional: ?idUsuario=1)
    [HttpGet]
    public async Task<ActionResult<List<PresupuestoDto>>> Get([FromQuery] int? idUsuario = null)
    {
        var result = await service.GetList(x => idUsuario == null || x.IdUsuario == idUsuario);

        List<PresupuestoDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de presupuestos.")
            : Ok(lista);
    }

    // GET api/Presupuestos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PresupuestoDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe presupuesto con id {id}.")
            : Ok(dto);
    }

    // POST api/Presupuestos
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PresupuestoDto dto)
    {
        dto.IdPresupuesto = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/Presupuestos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] PresupuestoDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe presupuesto con id {id}.");

        dto.IdPresupuesto = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/Presupuestos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe presupuesto con id {id}.");

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

    private async Task<PresupuestoDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        PresupuestoDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(PresupuestoDto dto, Func<IActionResult> alExito)
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

        return BadRequest(cuerpo);
    }
}