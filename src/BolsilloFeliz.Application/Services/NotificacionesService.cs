using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class NotificacionesService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<NotificacionDto, int>
{
    public async Task<Result> Guardar(NotificacionDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var notificacion = await contexto.Notificaciones.FirstOrDefaultAsync(notificacion => notificacion.IdNotificacion == dto.IdNotificacion);

        if (notificacion is null)
        {
            notificacion = new Notificacion
            {
                IdUsuario = dto.IdUsuario,
                Titulo = dto.Titulo,
                Mensaje = dto.Mensaje,
                Tipo = dto.Tipo,
                Fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha,
                Leida = dto.Leida
            };

            contexto.Notificaciones.Add(notificacion);
        }
        else
        {
            notificacion.Titulo = dto.Titulo;
            notificacion.Mensaje = dto.Mensaje;
            notificacion.Tipo = dto.Tipo;
            notificacion.Fecha = dto.Fecha;
            notificacion.Leida = dto.Leida;
        }

        return Result.Success(await contexto.SaveChangesAsync() > 0);
    }

    public async Task<Result<NotificacionDto?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var notificacion = await contexto.Notificaciones
            .AsNoTracking()
            .FirstOrDefaultAsync(notificacion => notificacion.IdNotificacion == id);

        if (notificacion is null)
            return (NotificacionDto?)null;

        return new NotificacionDto
        {
            IdNotificacion = notificacion.IdNotificacion,
            IdUsuario = notificacion.IdUsuario,
            Titulo = notificacion.Titulo,
            Mensaje = notificacion.Mensaje,
            Tipo = notificacion.Tipo,
            Fecha = notificacion.Fecha,
            Leida = notificacion.Leida
        };
    }

    public async Task<Result<List<NotificacionDto>>> GetList(Expression<Func<NotificacionDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var lista = await contexto.Notificaciones
            .AsNoTracking()
            .Select(notificacion => new NotificacionDto
            {
                IdNotificacion = notificacion.IdNotificacion,
                IdUsuario = notificacion.IdUsuario,
                Titulo = notificacion.Titulo,
                Mensaje = notificacion.Mensaje,
                Tipo = notificacion.Tipo,
                Fecha = notificacion.Fecha,
                Leida = notificacion.Leida
            })
            .Where(criterio)
            .ToListAsync();

        return lista;
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var eliminado = await contexto.Notificaciones
            .Where(notificacion => notificacion.IdNotificacion == id)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
