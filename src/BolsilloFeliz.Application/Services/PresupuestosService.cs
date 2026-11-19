using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class PresupuestosService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<PresupuestoDto, int>
{
    public async Task<Result> Guardar(PresupuestoDto dto)
    {
        // CK_Presupuesto_Fechas
        if (dto.FechaFin < dto.FechaInicio)
            return Result.Failure(new Error("FECHAS_INVALIDAS",
                "La fecha fin no puede ser anterior a la fecha de inicio."));

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var presupuesto = await contexto.Presupuestos.FirstOrDefaultAsync(presupuesto => presupuesto.IdPresupuesto == dto.IdPresupuesto);

        if (presupuesto is null)
        {
            presupuesto = new Presupuesto
            {
                IdUsuario = dto.IdUsuario,
                IdCategoria = dto.IdCategoria,
                IdCuenta = dto.IdCuenta,
                Nombre = dto.Nombre,
                Monto = dto.Monto,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Descripcion = dto.Descripcion,
                Estado = dto.Estado
            };

            contexto.Presupuestos.Add(presupuesto);
        }
        else
        {
            presupuesto.IdCategoria = dto.IdCategoria;
            presupuesto.IdCuenta = dto.IdCuenta;
            presupuesto.Nombre = dto.Nombre;
            presupuesto.Monto = dto.Monto;
            presupuesto.FechaInicio = dto.FechaInicio;
            presupuesto.FechaFin = dto.FechaFin;
            presupuesto.Descripcion = dto.Descripcion;
            presupuesto.Estado = dto.Estado;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<PresupuestoDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var presupuesto = await contexto.Presupuestos
            .AsNoTracking()
            .FirstOrDefaultAsync(presupuesto => presupuesto.IdPresupuesto == id);

        if (presupuesto is null)
            return (PresupuestoDto?)null;

        return new PresupuestoDto
        {
            IdPresupuesto = presupuesto.IdPresupuesto,
            IdUsuario = presupuesto.IdUsuario,
            IdCategoria = presupuesto.IdCategoria,
            IdCuenta = presupuesto.IdCuenta,
            Nombre = presupuesto.Nombre,
            Monto = presupuesto.Monto,
            FechaInicio = presupuesto.FechaInicio,
            FechaFin = presupuesto.FechaFin,
            Descripcion = presupuesto.Descripcion,
            Estado = presupuesto.Estado
        };
    }

    public async Task<Result<List<PresupuestoDto>>> GetList(Expression<Func<PresupuestoDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Presupuestos
            .AsNoTracking()
            .Select(presupuesto => new PresupuestoDto
            {
                IdPresupuesto = presupuesto.IdPresupuesto,
                IdUsuario = presupuesto.IdUsuario,
                IdCategoria = presupuesto.IdCategoria,
                IdCuenta = presupuesto.IdCuenta,
                Nombre = presupuesto.Nombre,
                Monto = presupuesto.Monto,
                FechaInicio = presupuesto.FechaInicio,
                FechaFin = presupuesto.FechaFin,
                Descripcion = presupuesto.Descripcion,
                Estado = presupuesto.Estado
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Presupuestos
            .Where(presupuesto => presupuesto.IdPresupuesto == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}