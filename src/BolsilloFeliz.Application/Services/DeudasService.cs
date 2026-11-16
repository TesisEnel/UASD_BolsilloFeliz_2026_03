using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class DeudasService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<DeudaDto, int>
{
    public async Task<Result> Guardar(DeudaDto dto)
    {
        if (!await Existe(dto.IdDeuda))
            return Result.Success(await Insertar(dto));

        return Result.Success(await Modificar(dto));
    }

    private async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Deudas.AnyAsync(x => x.IdDeuda == id);
    }

    private async Task<bool> Insertar(DeudaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Deudas.Add(new Deuda
        {
            IdUsuario = dto.IdUsuario,
            IdEntidad = dto.IdEntidad,
            IdTransaccionDesembolso = dto.IdTransaccionDesembolso,
            Nombre = dto.Nombre,
            MontoOriginal = dto.MontoOriginal,
            FechaDeuda = dto.FechaDeuda,
            FechaVencimiento = dto.FechaVencimiento,
            CuotaMensual = dto.CuotaMensual,
            Estado = dto.Estado,
            Descripcion = dto.Descripcion
        });
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(DeudaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var entidad = await contexto.Deudas.FindAsync(dto.IdDeuda);
        if (entidad is null)
            return false;

        // IdUsuario no se modifica: un registro no cambia de dueño
        entidad.IdEntidad = dto.IdEntidad;
        entidad.IdTransaccionDesembolso = dto.IdTransaccionDesembolso;
        entidad.Nombre = dto.Nombre;
        entidad.MontoOriginal = dto.MontoOriginal;
        entidad.FechaDeuda = dto.FechaDeuda;
        entidad.FechaVencimiento = dto.FechaVencimiento;
        entidad.CuotaMensual = dto.CuotaMensual;
        entidad.Estado = dto.Estado;
        entidad.Descripcion = dto.Descripcion;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<DeudaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.Deudas.AsNoTracking())
            .FirstOrDefaultAsync(x => x.IdDeuda == id);
    }

    public async Task<Result<List<DeudaDto>>> GetList(Expression<Func<DeudaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.Deudas.AsNoTracking())
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.Deudas
            .Where(x => x.IdDeuda == id)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    private static IQueryable<DeudaDto> Proyectar(IQueryable<Deuda> query) =>
        query.Select(x => new DeudaDto
        {
            IdDeuda = x.IdDeuda,
            IdUsuario = x.IdUsuario,
            IdEntidad = x.IdEntidad,
            IdTransaccionDesembolso = x.IdTransaccionDesembolso,
            Nombre = x.Nombre,
            MontoOriginal = x.MontoOriginal,
            FechaDeuda = x.FechaDeuda,
            FechaVencimiento = x.FechaVencimiento,
            CuotaMensual = x.CuotaMensual,
            Estado = x.Estado,
            Descripcion = x.Descripcion
        });
}
