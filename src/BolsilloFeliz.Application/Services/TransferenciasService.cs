using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class TransferenciasService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<TransferenciaDto, int>
{
    public async Task<Result> Guardar(TransferenciaDto dto)
    {
        // CK_Transferencia_Cuentas
        if (dto.IdCuentaOrigen == dto.IdCuentaDestino)
            return Result.Failure(new Error("CUENTAS_IGUALES",
                "La cuenta de origen y la de destino deben ser distintas."));

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var transferencia = await contexto.Transferencias.FirstOrDefaultAsync(transferencia => transferencia.IdTransferencia == dto.IdTransferencia);

        if (transferencia is null)
        {
            transferencia = new Transferencia
            {
                IdUsuario = dto.IdUsuario,
                IdTransaccion = dto.IdTransaccion,
                IdCuentaOrigen = dto.IdCuentaOrigen,
                IdCuentaDestino = dto.IdCuentaDestino
            };

            contexto.Transferencias.Add(transferencia);
        }
        else
        {
            transferencia.IdTransaccion = dto.IdTransaccion;
            transferencia.IdCuentaOrigen = dto.IdCuentaOrigen;
            transferencia.IdCuentaDestino = dto.IdCuentaDestino;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<TransferenciaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var transferencia = await contexto.Transferencias
            .AsNoTracking()
            .FirstOrDefaultAsync(transferencia => transferencia.IdTransferencia == id);

        if (transferencia is null)
            return (TransferenciaDto?)null;

        return new TransferenciaDto
        {
            IdTransferencia = transferencia.IdTransferencia,
            IdUsuario = transferencia.IdUsuario,
            IdTransaccion = transferencia.IdTransaccion,
            IdCuentaOrigen = transferencia.IdCuentaOrigen,
            IdCuentaDestino = transferencia.IdCuentaDestino
        };
    }

    public async Task<Result<List<TransferenciaDto>>> GetList(Expression<Func<TransferenciaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Transferencias
            .AsNoTracking()
            .Select(transferencia => new TransferenciaDto
            {
                IdTransferencia = transferencia.IdTransferencia,
                IdUsuario = transferencia.IdUsuario,
                IdTransaccion = transferencia.IdTransaccion,
                IdCuentaOrigen = transferencia.IdCuentaOrigen,
                IdCuentaDestino = transferencia.IdCuentaDestino
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Transferencias
            .Where(transferencia => transferencia.IdTransferencia == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}