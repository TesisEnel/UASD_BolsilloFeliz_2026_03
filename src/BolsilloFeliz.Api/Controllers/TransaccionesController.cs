using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController(TransaccionesService service) : ControllerBase
{
    // GET api/Transacciones  (opcional: ?idUsuario=1&idCuenta=1&idCategoria=1)
    [HttpGet]
    public async Task<ActionResult<List<TransaccionDto>>> Get(
        [FromQuery] int? idUsuario = null,
        [FromQuery] int? idCuenta = null,
        [FromQuery] int? idCategoria = null)
    {
        var result = await service.GetList(x =>
            (idUsuario == null || x.IdUsuario == idUsuario)
            && (idCuenta == null || x.IdCuenta == idCuenta)
            && (idCategoria == null || x.IdCategoria == idCategoria));

        List<TransaccionDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de transacciones.")
            : Ok(lista);
    }

    // GET api/Transacciones/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransaccionDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe transacción con id {id}.")
            : Ok(dto);
    }

    // POST api/Transacciones
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TransaccionDto dto)
    {
        dto.IdTransaccion = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/Transacciones/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] TransaccionDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe transacción con id {id}.");

        dto.IdTransaccion = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/Transacciones/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe transacción con id {id}.");

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

    private async Task<TransaccionDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        TransaccionDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(TransaccionDto dto, Func<IActionResult> alExito)
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
            "TIENE_DATOS_RELACIONADOS" => Conflict(cuerpo),
            _ => BadRequest(cuerpo)
        };
    }
}
