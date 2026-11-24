using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using BolsilloFeliz.Services.Tests.Infrastructure;
using Xunit;

namespace BolsilloFeliz.Services.Tests.Services;

public sealed class PresupuestosServiceBusinessRulesTests : ServiceTestBase
{
    [Fact]
    public async Task Guardar_FechaFinAnteriorAInicio_RechazaPresupuesto()
    {
        var service = new PresupuestosService(Factory);
        var result = await service.Guardar(new PresupuestoDto
        {
            FechaInicio = new DateOnly(2026, 12, 10),
            FechaFin = new DateOnly(2026, 12, 1),
            Nombre = "Presupuesto inválido",
            Monto = 100
        });
        Assert.False(result.IsSuccess);
    }
}
