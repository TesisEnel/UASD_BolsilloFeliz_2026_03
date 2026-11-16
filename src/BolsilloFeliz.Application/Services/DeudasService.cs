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
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var deuda = await contexto.Deudas.FirstOrDefaultAsync(deuda => deuda.IdDeuda == dto.IdDeuda);

        if (deuda is null)
        {
            deuda = new Deuda
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
            };

            contexto.Deudas.Add(deuda);
        }
        else
        {
            deuda.IdEntidad = dto.IdEntidad;
            deuda.IdTransaccionDesembolso = dto.IdTransaccionDesembolso;
            deuda.Nombre = dto.Nombre;
            deuda.MontoOriginal = dto.MontoOriginal;
            deuda.FechaDeuda = dto.FechaDeuda;
            deuda.FechaVencimiento = dto.FechaVencimiento;
            deuda.CuotaMensual = dto.CuotaMensual;
            deuda.Estado = dto.Estado;
            deuda.Descripcion = dto.Descripcion;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<DeudaDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var deuda = await contexto.Deudas
            .AsNoTracking()
            .FirstOrDefaultAsync(deuda => deuda.IdDeuda == id);

        if (deuda is null)
            return (DeudaDto?)null;

        return new DeudaDto
        {
            IdDeuda = deuda.IdDeuda,
            IdUsuario = deuda.IdUsuario,
            IdEntidad = deuda.IdEntidad,
            IdTransaccionDesembolso = deuda.IdTransaccionDesembolso,
            Nombre = deuda.Nombre,
            MontoOriginal = deuda.MontoOriginal,
            FechaDeuda = deuda.FechaDeuda,
            FechaVencimiento = deuda.FechaVencimiento,
            CuotaMensual = deuda.CuotaMensual,
            Estado = deuda.Estado,
            Descripcion = deuda.Descripcion
        };
    }

    public async Task<Result<List<DeudaDto>>> GetList(Expression<Func<DeudaDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Deudas
            .AsNoTracking()
            .Select(deuda => new DeudaDto
            {
                IdDeuda = deuda.IdDeuda,
                IdUsuario = deuda.IdUsuario,
                IdEntidad = deuda.IdEntidad,
                IdTransaccionDesembolso = deuda.IdTransaccionDesembolso,
                Nombre = deuda.Nombre,
                MontoOriginal = deuda.MontoOriginal,
                FechaDeuda = deuda.FechaDeuda,
                FechaVencimiento = deuda.FechaVencimiento,
                CuotaMensual = deuda.CuotaMensual,
                Estado = deuda.Estado,
                Descripcion = deuda.Descripcion
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Deudas
            .Where(deuda => deuda.IdDeuda == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
