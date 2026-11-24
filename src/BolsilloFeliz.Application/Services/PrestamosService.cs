using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class PrestamosService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<PrestamoDto, int>
{
    public async Task<Result> Guardar(PrestamoDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var prestamo = await contexto.Prestamos.FirstOrDefaultAsync(prestamo => prestamo.IdPrestamo == dto.IdPrestamo);

        if (prestamo is null)
        {
            prestamo = new Prestamo
            {
                IdUsuario = dto.IdUsuario,
                IdEntidad = dto.IdEntidad,
                IdTransaccionDesembolso = dto.IdTransaccionDesembolso,
                Nombre = dto.Nombre,
                MontoOriginal = dto.MontoOriginal,
                FechaPrestamo = dto.FechaPrestamo,
                FechaVencimiento = dto.FechaVencimiento,
                CuotaMensual = dto.CuotaMensual,
                Estado = dto.Estado,
                Descripcion = dto.Descripcion
            };

            contexto.Prestamos.Add(prestamo);
        }
        else
        {
            prestamo.IdEntidad = dto.IdEntidad;
            prestamo.IdTransaccionDesembolso = dto.IdTransaccionDesembolso;
            prestamo.Nombre = dto.Nombre;
            prestamo.MontoOriginal = dto.MontoOriginal;
            prestamo.FechaPrestamo = dto.FechaPrestamo;
            prestamo.FechaVencimiento = dto.FechaVencimiento;
            prestamo.CuotaMensual = dto.CuotaMensual;
            prestamo.Estado = dto.Estado;
            prestamo.Descripcion = dto.Descripcion;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<PrestamoDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var prestamo = await contexto.Prestamos
            .AsNoTracking()
            .FirstOrDefaultAsync(prestamo => prestamo.IdPrestamo == id);

        if (prestamo is null)
            return (PrestamoDto?)null;

        return new PrestamoDto
        {
            IdPrestamo = prestamo.IdPrestamo,
            IdUsuario = prestamo.IdUsuario,
            IdEntidad = prestamo.IdEntidad,
            IdTransaccionDesembolso = prestamo.IdTransaccionDesembolso,
            Nombre = prestamo.Nombre,
            MontoOriginal = prestamo.MontoOriginal,
            FechaPrestamo = prestamo.FechaPrestamo,
            FechaVencimiento = prestamo.FechaVencimiento,
            CuotaMensual = prestamo.CuotaMensual,
            Estado = prestamo.Estado,
            Descripcion = prestamo.Descripcion
        };
    }

    public async Task<Result<List<PrestamoDto>>> GetList(Expression<Func<PrestamoDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Prestamos
            .AsNoTracking()
            .Select(prestamo => new PrestamoDto
            {
                IdPrestamo = prestamo.IdPrestamo,
                IdUsuario = prestamo.IdUsuario,
                IdEntidad = prestamo.IdEntidad,
                IdTransaccionDesembolso = prestamo.IdTransaccionDesembolso,
                Nombre = prestamo.Nombre,
                MontoOriginal = prestamo.MontoOriginal,
                FechaPrestamo = prestamo.FechaPrestamo,
                FechaVencimiento = prestamo.FechaVencimiento,
                CuotaMensual = prestamo.CuotaMensual,
                Estado = prestamo.Estado,
                Descripcion = prestamo.Descripcion
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Prestamos
            .Where(prestamo => prestamo.IdPrestamo == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
