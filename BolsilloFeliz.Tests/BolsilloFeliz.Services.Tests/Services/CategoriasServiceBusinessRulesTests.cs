using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using BolsilloFeliz.Services.Tests.Infrastructure;
using Xunit;

namespace BolsilloFeliz.Services.Tests.Services;

public sealed class CategoriasServiceBusinessRulesTests : ServiceTestBase
{
    [Fact]
    public async Task Guardar_MismoNombreTipoYUsuario_RechazaDuplicado()
    {
        await SeedUsuario();
        var service = new CategoriasService(Factory);
        var dto = new CategoriaDto { IdUsuario = 1, Nombre = "Alimentación", Tipo = false };
        var first = await service.Guardar(dto);
        Assert.True(first.IsSuccess);
        var second = await service.Guardar(dto);
        Assert.False(second.IsSuccess);
    }
}
