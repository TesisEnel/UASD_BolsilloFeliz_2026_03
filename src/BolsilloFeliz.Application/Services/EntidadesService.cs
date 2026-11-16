using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class EntidadesService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<EntidadDto, int>
{
    public async Task<Result> Guardar(EntidadDto dto)
    {
        if (!await Existe(dto.IdEntidad))
            return Result.Success(await Insertar(dto));

        return Result.Success(await Modificar(dto));
    }

    private async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Entidades.AnyAsync(x => x.IdEntidad == id);
    }

    private async Task<bool> Insertar(EntidadDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Entidades.Add(new Entidade
        {
            IdUsuario = dto.IdUsuario,
            Nombre = dto.Nombre,
            TipoEntidad = dto.TipoEntidad,
            Identificacion = dto.Identificacion,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion
        });
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(EntidadDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var entidad = await contexto.Entidades.FindAsync(dto.IdEntidad);
        if (entidad is null)
            return false;

        // IdUsuario no se modifica: un registro no cambia de dueño
        entidad.Nombre = dto.Nombre;
        entidad.TipoEntidad = dto.TipoEntidad;
        entidad.Identificacion = dto.Identificacion;
        entidad.Telefono = dto.Telefono;
        entidad.Email = dto.Email;
        entidad.Direccion = dto.Direccion;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<EntidadDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.Entidades.AsNoTracking())
            .FirstOrDefaultAsync(x => x.IdEntidad == id);
    }

    public async Task<Result<List<EntidadDto>>> GetList(Expression<Func<EntidadDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.Entidades.AsNoTracking())
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.Entidades
            .Where(x => x.IdEntidad == id)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    private static IQueryable<EntidadDto> Proyectar(IQueryable<Entidade> query) =>
        query.Select(x => new EntidadDto
        {
            IdEntidad = x.IdEntidad,
            IdUsuario = x.IdUsuario,
            Nombre = x.Nombre,
            TipoEntidad = x.TipoEntidad,
            Identificacion = x.Identificacion,
            Telefono = x.Telefono,
            Email = x.Email,
            Direccion = x.Direccion
        });
}
