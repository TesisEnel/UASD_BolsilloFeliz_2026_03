using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class CuentasService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<CuentaDto, int>
{
    public async Task<Result> Guardar(CuentaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var cuenta = await contexto.Cuentas.FirstOrDefaultAsync(cuenta => cuenta.IdCuenta == dto.IdCuenta);

        if (cuenta is null)
        {
            cuenta = new Cuenta
            {
                IdUsuario = dto.IdUsuario,
                IdMoneda = dto.IdMoneda,
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                SaldoInicial = dto.SaldoInicial,
                Descripcion = dto.Descripcion
            };

            contexto.Cuentas.Add(cuenta);
        }
        else
        {
            cuenta.IdMoneda = dto.IdMoneda;
            cuenta.Nombre = dto.Nombre;
            cuenta.Tipo = dto.Tipo;
            cuenta.SaldoInicial = dto.SaldoInicial;
            cuenta.Descripcion = dto.Descripcion;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<CuentaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var cuenta = await contexto.Cuentas
            .AsNoTracking()
            .FirstOrDefaultAsync(cuenta => cuenta.IdCuenta == id);

        if (cuenta is null)
            return (CuentaDto?)null;

        return new CuentaDto
        {
            IdCuenta = cuenta.IdCuenta,
            IdUsuario = cuenta.IdUsuario,
            IdMoneda = cuenta.IdMoneda,
            Nombre = cuenta.Nombre,
            Tipo = cuenta.Tipo,
            SaldoInicial = cuenta.SaldoInicial,
            Descripcion = cuenta.Descripcion
        };
    }

    public async Task<Result<List<CuentaDto>>> GetList(Expression<Func<CuentaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Cuentas
            .AsNoTracking()
            .Select(cuenta => new CuentaDto
            {
                IdCuenta = cuenta.IdCuenta,
                IdUsuario = cuenta.IdUsuario,
                IdMoneda = cuenta.IdMoneda,
                Nombre = cuenta.Nombre,
                Tipo = cuenta.Tipo,
                SaldoInicial = cuenta.SaldoInicial,
                Descripcion = cuenta.Descripcion
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
            var eliminado = await contexto.Cuentas
                .Where(cuenta => cuenta.IdCuenta == id)
                .ExecuteDeleteAsync() > 0;

            return Result.Success(eliminado);
        }
        catch (Exception ex) when (ex is DbUpdateException
                                   || ex is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        {
            // 547 = violación de llave foránea
            return Result.Failure(new Error("TIENE_DATOS_RELACIONADOS",
                "No se puede eliminar: la cuenta tiene transacciones, presupuestos u otros datos asociados."));
        }
    }
}