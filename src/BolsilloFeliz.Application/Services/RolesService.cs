using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class RolesService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<RolDto, int>
{
    public async Task<Result> Guardar(RolDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var rol = await contexto.Roles.FirstOrDefaultAsync(rol => rol.IdRol == dto.IdRol);

        var duplicado = await contexto.Roles
            .AnyAsync(r => r.Nombre == dto.Nombre && r.IdRol != dto.IdRol);

        if (duplicado)
            return Result.Failure(new Error("ROL_DUPLICADO", "Ya existe un rol con ese nombre."));

        if (rol is null)
        {
            rol = new Rol
            {
                Nombre = dto.Nombre
            };

            contexto.Roles.Add(rol);
        }
        else
        {
            rol.Nombre = dto.Nombre;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<RolDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var rol = await contexto.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(rol => rol.IdRol == id);

        if (rol is null)
            return (RolDto?)null;

        return new RolDto
        {
            IdRol = rol.IdRol,
            Nombre = rol.Nombre
        };
    }

    public async Task<Result<List<RolDto>>> GetList(Expression<Func<RolDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Roles
            .AsNoTracking()
            .Select(rol => new RolDto
            {
                IdRol = rol.IdRol,
                Nombre = rol.Nombre
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
            var eliminado = await contexto.Roles
                .Where(rol => rol.IdRol == id)
                .ExecuteDeleteAsync() > 0;

            return Result.Success(eliminado);
        }
        catch (Exception ex) when (ex is DbUpdateException
                                   || ex is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        {
            // 547 = violación de llave foránea
            return Result.Failure(new Error("TIENE_DATOS_RELACIONADOS",
                "No se puede eliminar: el rol tiene usuarios asociados."));
        }
    }
}