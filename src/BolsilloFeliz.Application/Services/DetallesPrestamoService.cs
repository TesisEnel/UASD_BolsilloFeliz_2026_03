using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class DetallesPrestamoService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<DetallesPrestamoDto, int>
{
    public async Task<Result> Guardar(DetallesPrestamoDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var detallePrestamo = await contexto.DetallesPrestamos.FirstOrDefaultAsync(detallePrestamo => detallePrestamo.IdDetallePrestamo == dto.IdDetallePrestamo);

        if (detallePrestamo is null)
        {
            detallePrestamo = new DetallePrestamo
            {
                IdUsuario = dto.IdUsuario,
                IdPrestamo = dto.IdPrestamo,
                IdTransaccion = dto.IdTransaccion,
                MontoPagado = dto.MontoPagado,
                FechaPago = dto.FechaPago == default ? DateTime.Now : dto.FechaPago,
                Descripcion = dto.Descripcion
            };

            contexto.DetallesPrestamos.Add(detallePrestamo);
        }
        else
        {
            detallePrestamo.IdPrestamo = dto.IdPrestamo;
            detallePrestamo.IdTransaccion = dto.IdTransaccion;
            detallePrestamo.MontoPagado = dto.MontoPagado;
            detallePrestamo.FechaPago = dto.FechaPago;
            detallePrestamo.Descripcion = dto.Descripcion;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<DetallesPrestamoDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var detallePrestamo = await contexto.DetallesPrestamos
            .AsNoTracking()
            .FirstOrDefaultAsync(detallePrestamo => detallePrestamo.IdDetallePrestamo == id);

        if (detallePrestamo is null)
            return (DetallesPrestamoDto?)null;

        return new DetallesPrestamoDto
        {
            IdDetallePrestamo = detallePrestamo.IdDetallePrestamo,
            IdUsuario = detallePrestamo.IdUsuario,
            IdPrestamo = detallePrestamo.IdPrestamo,
            IdTransaccion = detallePrestamo.IdTransaccion,
            MontoPagado = detallePrestamo.MontoPagado,
            FechaPago = detallePrestamo.FechaPago,
            Descripcion = detallePrestamo.Descripcion
        };
    }

    public async Task<Result<List<DetallesPrestamoDto>>> GetList(Expression<Func<DetallesPrestamoDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.DetallesPrestamos
            .AsNoTracking()
            .Select(detallePrestamo => new DetallesPrestamoDto
            {
                IdDetallePrestamo = detallePrestamo.IdDetallePrestamo,
                IdUsuario = detallePrestamo.IdUsuario,
                IdPrestamo = detallePrestamo.IdPrestamo,
                IdTransaccion = detallePrestamo.IdTransaccion,
                MontoPagado = detallePrestamo.MontoPagado,
                FechaPago = detallePrestamo.FechaPago,
                Descripcion = detallePrestamo.Descripcion
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.DetallesPrestamos
            .Where(detallePrestamo => detallePrestamo.IdDetallePrestamo == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
