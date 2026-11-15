using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonedasController(BolsilloFelizDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await db.Monedas.ToListAsync());
}
