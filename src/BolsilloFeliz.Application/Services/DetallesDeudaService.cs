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
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var detalleDeuda = await contexto.DetallesDeudas.FirstOrDefaultAsync(d => d.IdDetalleDeuda == dto.IdDetalleDeuda);

        if (detalleDeuda is null)
        {
            detalleDeuda = new DetalleDeuda
            {
                IdUsuario = dto.IdUsuario,
                IdDeuda = dto.IdDeuda,
                IdTransaccion = dto.IdTransaccion,
                MontoPagado = dto.MontoPagado,
                FechaPago = dto.FechaPago == default ? DateTime.Now : dto.FechaPago,
                Descripcion = dto.Descripcion
            };

            contexto.DetallesDeudas.Add(detalleDeuda);
        }
        else
        {
            detalleDeuda.IdDeuda = dto.IdDeuda;
            detalleDeuda.IdTransaccion = dto.IdTransaccion;
            detalleDeuda.MontoPagado = dto.MontoPagado;
            detalleDeuda.FechaPago = dto.FechaPago;
            detalleDeuda.Descripcion = dto.Descripcion;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<DetallesDeudaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var detalleDeuda = await contexto.DetallesDeudas
            .AsNoTracking()
            .FirstOrDefaultAsync(detalleDeuda => detalleDeuda.IdDetalleDeuda == id);

        if (detalleDeuda is null)
            return (DetallesDeudaDto?)null;

        return new DetallesDeudaDto
        {
            IdDetalleDeuda = detalleDeuda.IdDetalleDeuda,
            IdUsuario = detalleDeuda.IdUsuario,
            IdDeuda = detalleDeuda.IdDeuda,
            IdTransaccion = detalleDeuda.IdTransaccion,
            MontoPagado = detalleDeuda.MontoPagado,
            FechaPago = detalleDeuda.FechaPago,
            Descripcion = detalleDeuda.Descripcion
        };
    }

    public async Task<Result<List<DetallesDeudaDto>>> GetList(Expression<Func<DetallesDeudaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.DetallesDeudas
            .AsNoTracking()
            .Select(detalleDeuda => new DetallesDeudaDto
            {
                IdDetalleDeuda = detalleDeuda.IdDetalleDeuda,
                IdUsuario = detalleDeuda.IdUsuario,
                IdDeuda = detalleDeuda.IdDeuda,
                IdTransaccion = detalleDeuda.IdTransaccion,
                MontoPagado = detalleDeuda.MontoPagado,
                FechaPago = detalleDeuda.FechaPago,
                Descripcion = detalleDeuda.Descripcion
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.DetallesDeudas
            .Where(detalleDeuda => detalleDeuda.IdDetalleDeuda == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
