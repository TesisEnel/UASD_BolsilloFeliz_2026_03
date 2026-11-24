using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using BolsilloFeliz.Services.Tests.Infrastructure;
using Xunit;

namespace BolsilloFeliz.Services.Tests.Services;

public sealed class RolesServiceBusinessRulesTests : ServiceTestBase
{
    [Fact]
    public async Task Guardar_NombreDeRolDuplicado_RechazaRegistro()
    {
        var service = new RolesService(Factory);
        var dto = new RolDto { Nombre = "Usuario" };
        Assert.True((await service.Guardar(dto)).IsSuccess);
        Assert.False((await service.Guardar(dto)).IsSuccess);
    }
}
