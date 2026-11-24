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

    // ---------- Transferencia + transacción de forma atómica ----------

    public async Task<Result<List<TransferenciaCompletaDto>>> ListarCompletas(int? idUsuario)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Transferencias
            .AsNoTracking()
            .Where(x => idUsuario == null || x.IdUsuario == idUsuario)
            .Select(x => new TransferenciaCompletaDto
            {
                IdTransferencia = x.IdTransferencia,
                IdTransaccion = x.IdTransaccion,
                IdUsuario = x.IdUsuario,
                IdCuentaOrigen = x.IdCuentaOrigen,
                IdCuentaDestino = x.IdCuentaDestino,
                Monto = x.Transaccione.Monto,
                Fecha = x.Transaccione.Fecha,
                Descripcion = x.Transaccione.Descripcion,
                Estado = x.Transaccione.Estado
            })
            .OrderByDescending(x => x.Fecha)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> GuardarCompleta(TransferenciaCompletaDto dto)
    {
        if (dto.IdCuentaOrigen == dto.IdCuentaDestino)
            return Result.Failure(new Error("CUENTAS_IGUALES",
                "La cuenta de origen y la de destino deben ser distintas."));

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var cuentas = await contexto.Cuentas
            .Where(c => c.IdUsuario == dto.IdUsuario
                        && (c.IdCuenta == dto.IdCuentaOrigen || c.IdCuenta == dto.IdCuentaDestino))
            .ToListAsync();

        var origen = cuentas.FirstOrDefault(c => c.IdCuenta == dto.IdCuentaOrigen);
        var destino = cuentas.FirstOrDefault(c => c.IdCuenta == dto.IdCuentaDestino);

        if (origen is null || destino is null)
            return Result.Failure(new Error("CUENTA_NO_EXISTE",
                "La cuenta de origen o destino no existe para este usuario."));

        if (origen.IdMoneda != destino.IdMoneda)
            return Result.Failure(new Error("MONEDAS_DISTINTAS",
                "Las dos cuentas deben tener la misma moneda."));

        var transferencia = dto.IdTransferencia == 0
            ? null
            : await contexto.Transferencias
                .Include(x => x.Transaccione)
                .FirstOrDefaultAsync(x => x.IdTransferencia == dto.IdTransferencia);

        var nombre = $"Transferencia: {origen.Nombre} → {destino.Nombre}";
        if (nombre.Length > 120) nombre = nombre[..120];

        if (transferencia is null)
        {
            var transaccion = new Transaccion
            {
                IdUsuario = dto.IdUsuario,
                IdCuenta = dto.IdCuentaOrigen,
                Nombre = nombre,
                Monto = dto.Monto,
                Tipo = "Transferencia",
                Fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado
            };

            contexto.Transferencias.Add(new Transferencia
            {
                IdUsuario = dto.IdUsuario,
                IdCuentaOrigen = dto.IdCuentaOrigen,
                IdCuentaDestino = dto.IdCuentaDestino,
                Transaccione = transaccion
            });
        }
        else
        {
            var t = transferencia.Transaccione;
            t.IdCuenta = dto.IdCuentaOrigen;
            t.Nombre = nombre;
            t.Monto = dto.Monto;
            t.Fecha = dto.Fecha == default ? t.Fecha : dto.Fecha;
            t.Descripcion = dto.Descripcion;
            t.Estado = dto.Estado;

            transferencia.IdCuentaOrigen = dto.IdCuentaOrigen;
            transferencia.IdCuentaDestino = dto.IdCuentaDestino;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result> EliminarCompleta(int idTransferencia)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var tx = await contexto.Database.BeginTransactionAsync();

        var idTransaccion = await contexto.Transferencias
            .Where(x => x.IdTransferencia == idTransferencia)
            .Select(x => (int?)x.IdTransaccion)
            .FirstOrDefaultAsync();

        if (idTransaccion is null)
            return Result.Success(false);

        await contexto.Transferencias
            .Where(x => x.IdTransferencia == idTransferencia)
            .ExecuteDeleteAsync();

        await contexto.Transacciones
            .Where(x => x.IdTransaccion == idTransaccion)
            .ExecuteDeleteAsync();

        await tx.CommitAsync();
        return Result.Success(true);
    }
}