using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Application.Services;

public class DashboardService(IDbContextFactory<BolsilloFelizDbContext> contextFactory)
{
    private static readonly string[] TiposIngreso = { "Ingreso", "Deuda recibida", "Abono de prestamo" };
    private static readonly string[] TiposGasto = { "Gasto", "Pago de deuda", "Prestamo otorgado" };

    public async Task<DashboardDto> Obtener(int idUsuario, int semanas = 8)
    {
        await using var ctx = await contextFactory.CreateDbContextAsync();

        var cuentas = await ctx.Cuentas.AsNoTracking()
            .Where(c => c.IdUsuario == idUsuario)
            .Select(c => new
            {
                c.IdCuenta,
                c.Nombre,
                c.Tipo,
                c.SaldoInicial,
                Codigo = c.IdMonedaNavigation.Codigo,
                Simbolo = c.IdMonedaNavigation.Simbolo
            })
            .OrderBy(c => c.IdCuenta)
            .ToListAsync();

        var transacciones = await ctx.Transacciones.AsNoTracking()
            .Where(t => t.IdUsuario == idUsuario && t.Estado != "Anulada")
            .ToListAsync();

        var transferencias = await ctx.Transferencias.AsNoTracking()
            .Where(t => t.IdUsuario == idUsuario)
            .ToListAsync();

        var porId = transacciones.ToDictionary(t => t.IdTransaccion);
        var nombreCuenta = cuentas.ToDictionary(c => c.IdCuenta, c => c.Nombre);

        // ---- Saldos (solo transacciones completadas) ----
        var saldos = cuentas.ToDictionary(c => c.IdCuenta, c => c.SaldoInicial);

        foreach (var t in transacciones.Where(t => t.Estado == "Completada"))
        {
            if (t.Tipo == "Transferencia") continue;
            if (t.IdCuenta is not int idc || !saldos.ContainsKey(idc)) continue;

            if (TiposIngreso.Contains(t.Tipo)) saldos[idc] += t.Monto;
            else if (TiposGasto.Contains(t.Tipo)) saldos[idc] -= t.Monto;
        }

        foreach (var tr in transferencias)
        {
            if (!porId.TryGetValue(tr.IdTransaccion, out var t) || t.Estado != "Completada") continue;
            if (saldos.ContainsKey(tr.IdCuentaOrigen)) saldos[tr.IdCuentaOrigen] -= t.Monto;
            if (saldos.ContainsKey(tr.IdCuentaDestino)) saldos[tr.IdCuentaDestino] += t.Monto;
        }

        var dto = new DashboardDto
        {
            Saldos = cuentas.Select(c => new SaldoCuentaDto
            {
                IdCuenta = c.IdCuenta,
                Nombre = c.Nombre,
                Tipo = c.Tipo,
                CodigoMoneda = c.Codigo,
                SimboloMoneda = c.Simbolo,
                Saldo = saldos[c.IdCuenta]
            }).ToList()
        };

        // ---- Próximas y últimas ----
        MovimientoDto Mapear(Domain.Entities.Transaccion t) => new()
        {
            IdTransaccion = t.IdTransaccion,
            Nombre = t.Nombre,
            Tipo = t.Tipo,
            Monto = t.Monto,
            Fecha = t.Fecha,
            Estado = t.Estado,
            Cuenta = t.IdCuenta is int id && nombreCuenta.TryGetValue(id, out var n) ? n : null,
            EsIngreso = TiposIngreso.Contains(t.Tipo)
        };

        dto.Proximas = transacciones.Where(t => t.Estado == "Pendiente")
            .OrderBy(t => t.Fecha).Take(30).Select(Mapear).ToList();

        dto.Ultimas = transacciones.Where(t => t.Estado == "Completada")
            .OrderByDescending(t => t.Fecha).Take(30).Select(Mapear).ToList();

        // ---- Resumen semanal (ingresos vs gastos completados) ----
        var hoy = DateTime.Today;
        var lunes = hoy.AddDays(-(((int)hoy.DayOfWeek + 6) % 7));
        var inicios = Enumerable.Range(0, semanas).Select(i => lunes.AddDays(-7 * (semanas - 1 - i))).ToList();

        foreach (var inicio in inicios)
        {
            var fin = inicio.AddDays(7);
            var delPeriodo = transacciones.Where(t => t.Estado == "Completada" && t.Fecha >= inicio && t.Fecha < fin).ToList();
            dto.Semanas.Add(new ResumenSemanalDto
            {
                Inicio = inicio,
                Ingresos = delPeriodo.Where(t => TiposIngreso.Contains(t.Tipo)).Sum(t => t.Monto),
                Gastos = delPeriodo.Where(t => TiposGasto.Contains(t.Tipo)).Sum(t => t.Monto)
            });
        }

        return dto;
    }
}