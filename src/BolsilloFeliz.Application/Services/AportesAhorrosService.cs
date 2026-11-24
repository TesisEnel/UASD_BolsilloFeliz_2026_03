using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class AportesAhorrosService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<AporteAhorroDto, int>
{
    public async Task<Result> Guardar(AporteAhorroDto dto)
    {
        if (!await Existe(dto.IdAporteAhorro))
            return Result.Success(await Insertar(dto));

        return Result.Success(await Modificar(dto));
    }

    private async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.AportesAhorros.AnyAsync(x => x.IdAporteAhorro == id);
    }

    private async Task<bool> Insertar(AporteAhorroDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.AportesAhorros.Add(new AporteAhorro
        {
            IdUsuario = dto.IdUsuario,
            IdMetaAhorro = dto.IdMetaAhorro,
            IdCuenta = dto.IdCuenta,
            Monto = dto.Monto,
            Fecha = dto.Fecha,
            Nota = dto.Nota
        });
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(AporteAhorroDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var entidad = await contexto.AportesAhorros.FindAsync(dto.IdAporteAhorro);
        if (entidad is null)
            return false;

        // IdUsuario no se modifica: un registro no cambia de dueño
        entidad.IdMetaAhorro = dto.IdMetaAhorro;
        entidad.IdCuenta = dto.IdCuenta;
        entidad.Monto = dto.Monto;
        entidad.Fecha = dto.Fecha;
        entidad.Nota = dto.Nota;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<AporteAhorroDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var e = await contexto.AportesAhorros
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAporteAhorro == id);

        if (e is null)
            return (AporteAhorroDto?)null;

        return new AporteAhorroDto
        {
            IdAporteAhorro = e.IdAporteAhorro,
            IdUsuario = e.IdUsuario,
            IdMetaAhorro = e.IdMetaAhorro,
            IdCuenta = e.IdCuenta,
            Monto = e.Monto,
            Fecha = e.Fecha,
            Nota = e.Nota
        };
    }

    public async Task<Result<List<AporteAhorroDto>>> GetList(Expression<Func<AporteAhorroDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.AportesAhorros.AsNoTracking())
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.AportesAhorros
            .Where(x => x.IdAporteAhorro == id)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    private static IQueryable<AporteAhorroDto> Proyectar(IQueryable<AporteAhorro> query) =>
        query.Select(x => new AporteAhorroDto
        {
            IdAporteAhorro = x.IdAporteAhorro,
            IdUsuario = x.IdUsuario,
            IdMetaAhorro = x.IdMetaAhorro,
            IdCuenta = x.IdCuenta,
            Monto = x.Monto,
            Fecha = x.Fecha,
            Nota = x.Nota
        });
}
