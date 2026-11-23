using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class MetasAhorrosService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<MetaAhorroDto, int>
{
    public async Task<Result> Guardar(MetaAhorroDto dto)
    {
        if (!await Existe(dto.IdMetaAhorro))
            return Result.Success(await Insertar(dto));

        return Result.Success(await Modificar(dto));
    }

    private async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.MetasAhorros.AnyAsync(x => x.IdMetaAhorro == id);
    }

    private async Task<bool> Insertar(MetaAhorroDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.MetasAhorros.Add(new MetaAhorro
        {
            IdUsuario = dto.IdUsuario,
            IdMoneda = dto.IdMoneda,
            Nombre = dto.Nombre,
            MontoObjetivo = dto.MontoObjetivo,
            FechaInicio = dto.FechaInicio,
            FechaObjetivo = dto.FechaObjetivo,
            Descripcion = dto.Descripcion,
            Estado = dto.Estado
        });
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(MetaAhorroDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var entidad = await contexto.MetasAhorros.FindAsync(dto.IdMetaAhorro);
        if (entidad is null)
            return false;

        // IdUsuario no se modifica: un registro no cambia de dueño
        entidad.IdMoneda = dto.IdMoneda;
        entidad.Nombre = dto.Nombre;
        entidad.MontoObjetivo = dto.MontoObjetivo;
        entidad.FechaInicio = dto.FechaInicio;
        entidad.FechaObjetivo = dto.FechaObjetivo;
        entidad.Descripcion = dto.Descripcion;
        entidad.Estado = dto.Estado;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<MetaAhorroDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var e = await contexto.MetasAhorros
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdMetaAhorro == id);

        if (e is null)
            return (MetaAhorroDto?)null;

        return new MetaAhorroDto
        {
            IdMetaAhorro = e.IdMetaAhorro,
            IdUsuario = e.IdUsuario,
            IdMoneda = e.IdMoneda,
            Nombre = e.Nombre,
            MontoObjetivo = e.MontoObjetivo,
            FechaInicio = e.FechaInicio,
            FechaObjetivo = e.FechaObjetivo,
            Descripcion = e.Descripcion,
            Estado = e.Estado
        };
    }

    public async Task<Result<List<MetaAhorroDto>>> GetList(Expression<Func<MetaAhorroDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.MetasAhorros.AsNoTracking())
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.MetasAhorros
            .Where(x => x.IdMetaAhorro == id)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    private static IQueryable<MetaAhorroDto> Proyectar(IQueryable<MetaAhorro> query) =>
        query.Select(x => new MetaAhorroDto
        {
            IdMetaAhorro = x.IdMetaAhorro,
            IdUsuario = x.IdUsuario,
            IdMoneda = x.IdMoneda,
            Nombre = x.Nombre,
            MontoObjetivo = x.MontoObjetivo,
            FechaInicio = x.FechaInicio,
            FechaObjetivo = x.FechaObjetivo,
            Descripcion = x.Descripcion,
            Estado = x.Estado
        });
}
