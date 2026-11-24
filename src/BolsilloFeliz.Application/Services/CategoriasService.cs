using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class CategoriasService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<CategoriaDto, int>
{
    public async Task<Result> Guardar(CategoriaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var categoria = await contexto.Categorias.FirstOrDefaultAsync(categoria => categoria.IdCategoria == dto.IdCategoria);

        // Índice único (IdUsuario, Nombre, Tipo)
        var idUsuario = categoria?.IdUsuario ?? dto.IdUsuario;

        var duplicada = await contexto.Categorias.AnyAsync(c =>
            c.IdUsuario == idUsuario
            && c.Nombre == dto.Nombre
            && c.Tipo == dto.Tipo
            && c.IdCategoria != dto.IdCategoria);

        if (duplicada)
            return Result.Failure(new Error("CATEGORIA_DUPLICADA",
                "Ya existe una categoría con ese nombre y tipo."));

        if (categoria is null)
        {
            categoria = new Categoria
            {
                IdUsuario = dto.IdUsuario,
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
                Descripcion = dto.Descripcion
            };

            contexto.Categorias.Add(categoria);
        }
        else
        {
            categoria.Nombre = dto.Nombre;
            categoria.Tipo = dto.Tipo;
            categoria.Descripcion = dto.Descripcion;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<CategoriaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var categoria = await contexto.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(categoria => categoria.IdCategoria == id);

        if (categoria is null)
            return (CategoriaDto?)null;

        return new CategoriaDto
        {
            IdCategoria = categoria.IdCategoria,
            IdUsuario = categoria.IdUsuario,
            Nombre = categoria.Nombre,
            Tipo = categoria.Tipo,
            Descripcion = categoria.Descripcion
        };
    }

    public async Task<Result<List<CategoriaDto>>> GetList(Expression<Func<CategoriaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Categorias
            .AsNoTracking()
            .Select(categoria => new CategoriaDto
            {
                IdCategoria = categoria.IdCategoria,
                IdUsuario = categoria.IdUsuario,
                Nombre = categoria.Nombre,
                Tipo = categoria.Tipo,
                Descripcion = categoria.Descripcion
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
            var eliminado = await contexto.Categorias
                .Where(categoria => categoria.IdCategoria == id)
                .ExecuteDeleteAsync() > 0;

            return Result.Success(eliminado);
        }
        catch (Exception ex) when (ex is DbUpdateException
                                   || ex is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        {
            // 547 = violación de llave foránea
            return Result.Failure(new Error("TIENE_DATOS_RELACIONADOS",
                "No se puede eliminar: la categoría tiene transacciones o presupuestos asociados."));
        }
    }
}