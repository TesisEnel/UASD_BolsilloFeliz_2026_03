using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class TransaccionesService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<TransaccionDto, int>
{
    public async Task<Result> Guardar(TransaccionDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var transaccion = await contexto.Transacciones.FirstOrDefaultAsync(transaccion => transaccion.IdTransaccion == dto.IdTransaccion);

        if (transaccion is null)
        {
            transaccion = new Transaccion
            {
                IdUsuario = dto.IdUsuario,
                IdCuenta = dto.IdCuenta,
                IdCategoria = dto.IdCategoria,
                Nombre = dto.Nombre,
                Monto = dto.Monto,
                Tipo = dto.Tipo,
                Fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado
            };

            contexto.Transacciones.Add(transaccion);
        }
        else
        {
            transaccion.IdCuenta = dto.IdCuenta;
            transaccion.IdCategoria = dto.IdCategoria;
            transaccion.Nombre = dto.Nombre;
            transaccion.Monto = dto.Monto;
            transaccion.Tipo = dto.Tipo;
            transaccion.Fecha = dto.Fecha == default ? transaccion.Fecha : dto.Fecha;
            transaccion.Descripcion = dto.Descripcion;
            transaccion.Estado = dto.Estado;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<TransaccionDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var transaccion = await contexto.Transacciones
            .AsNoTracking()
            .FirstOrDefaultAsync(transaccion => transaccion.IdTransaccion == id);

        if (transaccion is null)
            return (TransaccionDto?)null;

        return new TransaccionDto
        {
            IdTransaccion = transaccion.IdTransaccion,
            IdUsuario = transaccion.IdUsuario,
            IdCuenta = transaccion.IdCuenta,
            IdCategoria = transaccion.IdCategoria,
            Nombre = transaccion.Nombre,
            Monto = transaccion.Monto,
            Tipo = transaccion.Tipo,
            Fecha = transaccion.Fecha,
            Descripcion = transaccion.Descripcion,
            Estado = transaccion.Estado
        };
    }

    public async Task<Result<List<TransaccionDto>>> GetList(Expression<Func<TransaccionDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Transacciones
            .AsNoTracking()
            .Select(transaccion => new TransaccionDto
            {
                IdTransaccion = transaccion.IdTransaccion,
                IdUsuario = transaccion.IdUsuario,
                IdCuenta = transaccion.IdCuenta,
                IdCategoria = transaccion.IdCategoria,
                Nombre = transaccion.Nombre,
                Monto = transaccion.Monto,
                Tipo = transaccion.Tipo,
                Fecha = transaccion.Fecha,
                Descripcion = transaccion.Descripcion,
                Estado = transaccion.Estado
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        try
        {
            var eliminado = await contexto.Transacciones
                .Where(transaccion => transaccion.IdTransaccion == id)
                .ExecuteDeleteAsync() > 0;

            return Result.Success(eliminado);
        }
        catch (Exception ex) when (ex is DbUpdateException
                                   || ex is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        {
            // 547 = violación de llave foránea
            return Result.Failure(new Error("TIENE_DATOS_RELACIONADOS",
                "No se puede eliminar: la transacción está asociada a deudas, préstamos o transferencias."));
        }
    }
}