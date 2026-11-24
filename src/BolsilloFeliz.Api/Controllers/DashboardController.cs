using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(DashboardService service) : ControllerBase
{
    // GET api/Dashboard?idUsuario=1&semanas=8
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get([FromQuery] int idUsuario, [FromQuery] int semanas = 8)
    {
        if (idUsuario <= 0)
            return BadRequest(new { codigo = "USUARIO_REQUERIDO", mensaje = "Indique idUsuario." });

        return Ok(await service.Obtener(idUsuario, Math.Clamp(semanas, 1, 26)));
    }
}