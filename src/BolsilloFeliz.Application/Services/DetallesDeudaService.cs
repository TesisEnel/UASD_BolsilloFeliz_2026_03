using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class DetallesDeudaService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<DetallesDeudaDto, int>
{
    public async Task<Result> Guardar(DetallesDeudaDto dto)
    {
        if (!await Existe(dto.IdDetalleDeuda))
            return Result.Success(await Insertar(dto));

        return Result.Success(await Modificar(dto));
    }

    private async Task<bool> Existe(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.DetallesDeudas.AnyAsync(x => x.IdDetalleDeuda == id);
    }

    private async Task<bool> Insertar(DetallesDeudaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.DetallesDeudas.Add(new DetalleDeuda
        {
            IdUsuario = dto.IdUsuario,
            IdDeuda = dto.IdDeuda,
            IdTransaccion = dto.IdTransaccion,
            MontoPagado = dto.MontoPagado,
            FechaPago = dto.FechaPago == default ? DateTime.Now : dto.FechaPago,
            Descripcion = dto.Descripcion
        });
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(DetallesDeudaDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var entidad = await contexto.DetallesDeudas.FindAsync(dto.IdDetalleDeuda);
        if (entidad is null)
            return false;

        // IdUsuario no se modifica: un registro no cambia de dueño
        entidad.IdDeuda = dto.IdDeuda;
        entidad.IdTransaccion = dto.IdTransaccion;
        entidad.MontoPagado = dto.MontoPagado;
        entidad.FechaPago = dto.FechaPago;
        entidad.Descripcion = dto.Descripcion;

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<DetallesDeudaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.DetallesDeudas.AsNoTracking())
            .FirstOrDefaultAsync(x => x.IdDetalleDeuda == id);
    }

    public async Task<Result<List<DetallesDeudaDto>>> GetList(Expression<Func<DetallesDeudaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await Proyectar(contexto.DetallesDeudas.AsNoTracking())
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.DetallesDeudas
            .Where(x => x.IdDetalleDeuda == id)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    private static IQueryable<DetallesDeudaDto> Proyectar(IQueryable<DetalleDeuda> query) =>
        query.Select(x => new DetallesDeudaDto
        {
            IdDetalleDeuda = x.IdDetalleDeuda,
            IdUsuario = x.IdUsuario,
            IdDeuda = x.IdDeuda,
            IdTransaccion = x.IdTransaccion,
            MontoPagado = x.MontoPagado,
            FechaPago = x.FechaPago,
            Descripcion = x.Descripcion
        });
}
