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
        if (!await Existe(dto.IdMoneda))
            return Result.Success(await Insertar(dto));

        return Result.Success(await Modificar(dto));
    }

    private async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Monedas.AnyAsync(x => x.IdMoneda == id);
    }

    private async Task<bool> Insertar(MonedaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Monedas.Add(new Moneda
        {
            Codigo = dto.Codigo,
            Nombre = dto.Nombre,
            Simbolo = dto.Simbolo,
            Activa = dto.Activa
        });
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(MonedaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var entidad = await contexto.Monedas.FindAsync(dto.IdMoneda);
        if (entidad is null)
            return false;

        // IdUsuario no se modifica: un registro no cambia de dueño
        entidad.Codigo = dto.Codigo;
        entidad.Nombre = dto.Nombre;
        entidad.Simbolo = dto.Simbolo;
        entidad.Activa = dto.Activa;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<MonedaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.Monedas.AsNoTracking())
            .FirstOrDefaultAsync(x => x.IdMoneda == id);
    }

    public async Task<Result<List<MonedaDto>>> GetList(Expression<Func<MonedaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.Monedas.AsNoTracking())
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.Monedas
            .Where(x => x.IdMoneda == id)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    private static IQueryable<MonedaDto> Proyectar(IQueryable<Moneda> query) =>
        query.Select(x => new MonedaDto
        {
            IdMoneda = x.IdMoneda,
            Codigo = x.Codigo,
            Nombre = x.Nombre,
            Simbolo = x.Simbolo,
            Activa = x.Activa
        });
}
