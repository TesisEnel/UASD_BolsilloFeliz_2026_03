using BolsilloFeliz.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BolsilloFeliz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<MonedasService>();
        services.AddScoped<EntidadesService>();
        services.AddScoped<DeudasService>();
        services.AddScoped<DetallesDeudaService>();
        services.AddScoped<PrestamosService>();
        services.AddScoped<DetallesPrestamoService>();
        services.AddScoped<NotificacionesService>();
        services.AddScoped<MetasAhorrosService>();
        services.AddScoped<AportesAhorrosService>();
        services.AddScoped<UsuariosService>();
        services.AddScoped<RolesService>();
        services.AddScoped<CategoriasService>();
        services.AddScoped<CuentasService>();
        services.AddScoped<PresupuestosService>();
        services.AddScoped<TransaccionesService>();
        services.AddScoped<TransferenciasService>();
        services.AddScoped<DashboardService>();   

        return services;
    }
}
