using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferenciasController(TransferenciasService service) : ControllerBase
{
    // GET api/Transferencias  (opcional: ?idUsuario=1)
    [HttpGet]
    public async Task<ActionResult<List<TransferenciaDto>>> Get([FromQuery] int? idUsuario = null)
    {
        var result = await service.GetList(x => idUsuario == null || x.IdUsuario == idUsuario);

        List<TransferenciaDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de transferencias.")
            : Ok(lista);
    }

    // ---- Transferencia + transacción (atómico) ----

    // GET api/Transferencias/detalle?idUsuario=1
    [HttpGet("detalle")]
    public async Task<ActionResult<List<TransferenciaCompletaDto>>> GetDetalle([FromQuery] int? idUsuario = null)
    {
        var result = await service.ListarCompletas(idUsuario);

        List<TransferenciaCompletaDto>? lista = null;
        result.OnSuccess(l => lista = l);

        return lista is null
            ? Problem("No se pudo obtener la lista de transferencias.")
            : Ok(lista);
    }

    // POST api/Transferencias/detalle
    [HttpPost("detalle")]
    public async Task<IActionResult> PostDetalle([FromBody] TransferenciaCompletaDto dto)
    {
        dto.IdTransferencia = 0;
        dto.IdTransaccion = 0;

        return await GuardarCompletaAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/Transferencias/detalle/5
    [HttpPut("detalle/{id:int}")]
    public async Task<IActionResult> PutDetalle(int id, [FromBody] TransferenciaCompletaDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe transferencia con id {id}.");

        dto.IdTransferencia = id;

        return await GuardarCompletaAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/Transferencias/detalle/5  (borra la transferencia y su transacción)
    [HttpDelete("detalle/{id:int}")]
    public async Task<IActionResult> DeleteDetalle(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe transferencia con id {id}.");

        var result = await service.EliminarCompleta(id);

        if (result.IsFailure)
            return Falla(result.Error);

        return NoContent();
    }

    private async Task<IActionResult> GuardarCompletaAsync(TransferenciaCompletaDto dto, Func<IActionResult> alExito)
    {
        try
        {
            var result = await service.GuardarCompleta(dto);

            if (result.IsFailure)
                return Falla(result.Error);

            var guardado = false;
            result.OnSuccess(() => guardado = true);

            return guardado ? alExito() : Problem("No se pudo guardar el registro.");
        }
        catch (DbUpdateException)
        {
            return BadRequest("No se pudo guardar: revise las cuentas seleccionadas.");
        }
    }

    // GET api/Transferencias/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransferenciaDto>> Get(int id)
    {
        var dto = await ObtenerAsync(id);

        return dto is null
            ? NotFound($"No existe transferencia con id {id}.")
            : Ok(dto);
    }

    // POST api/Transferencias
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TransferenciaDto dto)
    {
        dto.IdTransferencia = 0; // siempre es un registro nuevo

        return await GuardarAsync(dto,
            () => StatusCode(StatusCodes.Status201Created, new { mensaje = "Registro creado correctamente." }));
    }

    // PUT api/Transferencias/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] TransferenciaDto dto)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe transferencia con id {id}.");

        dto.IdTransferencia = id;

        return await GuardarAsync(dto,
            () => Ok(new { mensaje = "Registro actualizado correctamente." }));
    }

    // DELETE api/Transferencias/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await ObtenerAsync(id) is null)
            return NotFound($"No existe transferencia con id {id}.");

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

    private async Task<TransferenciaDto?> ObtenerAsync(int id)
    {
        var result = await service.Buscar(id);

        TransferenciaDto? dto = null;
        result.OnSuccess(d => dto = d);
        return dto;
    }

    private async Task<IActionResult> GuardarAsync(TransferenciaDto dto, Func<IActionResult> alExito)
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