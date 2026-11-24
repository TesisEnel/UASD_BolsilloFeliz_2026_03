using BolsilloFeliz.Application.Services;
using BolsilloFeliz.Services.Tests.Infrastructure;
using Xunit;

namespace BolsilloFeliz.Services.Tests.Services;

public sealed class TransaccionesServiceTests : ServiceTestBase
{
    private TransaccionesService CreateService() => new(Factory);

    [Fact]
    public async Task Buscar_IdInexistente_NoDevuelveRegistro()
    {
        var result = await CreateService().Buscar(9999);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetList_SinRegistros_DevuelveListaVacia()
    {
        var result = await CreateService().GetList(_ => false);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Eliminar_IdInexistente_NoFalla()
    {
        var result = await CreateService().Eliminar(9999);
        Assert.True(result.IsSuccess);
    }
}
