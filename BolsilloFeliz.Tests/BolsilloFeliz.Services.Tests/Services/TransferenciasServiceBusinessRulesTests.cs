using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Application.Services;
using BolsilloFeliz.Services.Tests.Infrastructure;
using Xunit;

namespace BolsilloFeliz.Services.Tests.Services;

public sealed class TransferenciasServiceBusinessRulesTests : ServiceTestBase
{
    [Fact]
    public async Task Guardar_MismaCuentaOrigenYDestino_RechazaTransferencia()
    {
        var result = await new TransferenciasService(Factory).Guardar(new TransferenciaDto
        {
            IdCuentaOrigen = 5,
            IdCuentaDestino = 5
        });
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GuardarCompleta_MismaCuentaOrigenYDestino_RechazaTransferencia()
    {
        var result = await new TransferenciasService(Factory).GuardarCompleta(new TransferenciaCompletaDto
        {
            IdCuentaOrigen = 7,
            IdCuentaDestino = 7,
            Monto = 50
        });
        Assert.False(result.IsSuccess);
    }
}
