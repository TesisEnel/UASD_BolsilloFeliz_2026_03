using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using BolsilloFeliz.Services.Tests.Infrastructure;
using Xunit;

namespace BolsilloFeliz.Services.Tests.Services;

public sealed class UsuariosServiceBusinessRulesTests : ServiceTestBase
{
    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    public async Task Guardar_UsuarioNuevoConPasswordCorta_RechazaRegistro(string password)
    {
        var result = await new UsuariosService(Factory).Guardar(new UsuarioDto
        {
            Nombres = "Usuario",
            Apellidos = "Prueba",
            Email = "nuevo@test.com",
            Password = password
        });
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task IniciarSesion_UsuarioInexistente_NoAutentica()
    {
        var result = await new UsuariosService(Factory).IniciarSesion(new LoginDto
        {
            Email = "noexiste@test.com",
            Password = "Password123"
        });
        Assert.True(result.IsSuccess);
    }
}
