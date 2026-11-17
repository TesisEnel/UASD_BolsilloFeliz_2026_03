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
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var entidad = await contexto.Entidades.FirstOrDefaultAsync(entidad => entidad.IdEntidad == dto.IdEntidad);

        if (entidad is null)
        {
            entidad = new Entidad
            {
                IdUsuario = dto.IdUsuario,
                Nombre = dto.Nombre,
                TipoEntidad = dto.TipoEntidad,
                Identificacion = dto.Identificacion,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion
            };

            contexto.Entidades.Add(entidad);
        }
        else
        {
            entidad.Nombre = dto.Nombre;
            entidad.TipoEntidad = dto.TipoEntidad;
            entidad.Identificacion = dto.Identificacion;
            entidad.Telefono = dto.Telefono;
            entidad.Email = dto.Email;
            entidad.Direccion = dto.Direccion;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<EntidadDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var entidad = await contexto.Entidades
            .AsNoTracking()
            .FirstOrDefaultAsync(entidad => entidad.IdEntidad == id);

        if (entidad is null)
            return (EntidadDto?)null;

        return new EntidadDto
        {
            IdEntidad = entidad.IdEntidad,
            IdUsuario = entidad.IdUsuario,
            Nombre = entidad.Nombre,
            TipoEntidad = entidad.TipoEntidad,
            Identificacion = entidad.Identificacion,
            Telefono = entidad.Telefono,
            Email = entidad.Email,
            Direccion = entidad.Direccion
        };
    }

    public async Task<Result<List<EntidadDto>>> GetList(Expression<Func<EntidadDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Entidades
            .AsNoTracking()
            .Select(entidad => new EntidadDto
            {
                IdEntidad = entidad.IdEntidad,
                IdUsuario = entidad.IdUsuario,
                Nombre = entidad.Nombre,
                TipoEntidad = entidad.TipoEntidad,
                Identificacion = entidad.Identificacion,
                Telefono = entidad.Telefono,
                Email = entidad.Email,
                Direccion = entidad.Direccion
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Entidades
            .Where(entidad => entidad.IdEntidad == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
