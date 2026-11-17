using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class MonedasService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<MonedaDto, int>
{
    public async Task<Result> Guardar(MonedaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var moneda = await contexto.Monedas.FirstOrDefaultAsync(moneda => moneda.IdMoneda == dto.IdMoneda);

        if (moneda is null)
        {
            moneda = new Moneda
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Simbolo = dto.Simbolo,
                Activa = dto.Activa
            };

            contexto.Monedas.Add(moneda);
        }
        else
        {
            moneda.Codigo = dto.Codigo;
            moneda.Nombre = dto.Nombre;
            moneda.Simbolo = dto.Simbolo;
            moneda.Activa = dto.Activa;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<MonedaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var moneda = await contexto.Monedas
            .AsNoTracking()
            .FirstOrDefaultAsync(moneda => moneda.IdMoneda == id);

        if (moneda is null)
            return (MonedaDto?)null;

        return new MonedaDto
        {
            IdMoneda = moneda.IdMoneda,
            Codigo = moneda.Codigo,
            Nombre = moneda.Nombre,
            Simbolo = moneda.Simbolo,
            Activa = moneda.Activa
        };
    }

    public async Task<Result<List<MonedaDto>>> GetList(Expression<Func<MonedaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Monedas
            .AsNoTracking()
            .Select(moneda => new MonedaDto
            {
                IdMoneda = moneda.IdMoneda,
                Codigo = moneda.Codigo,
                Nombre = moneda.Nombre,
                Simbolo = moneda.Simbolo,
                Activa = moneda.Activa
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Monedas
            .Where(moneda => moneda.IdMoneda == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
