using BolsilloFeliz.Application.Services;
using BolsilloFeliz.Services.Tests.Infrastructure;
using Xunit;

namespace BolsilloFeliz.Services.Tests.Services;

public sealed class DashboardServiceTests : ServiceTestBase
{
    [Fact]
    public async Task Obtener_SinDatos_DevuelveColeccionesVaciasYSemanasSolicitadas()
    {
        var result = await new DashboardService(Factory).Obtener(12345, 4);
        Assert.Empty(result.Saldos);
        Assert.Empty(result.Proximas);
        Assert.Empty(result.Ultimas);
        Assert.Equal(4, result.Semanas.Count);
    }
}
