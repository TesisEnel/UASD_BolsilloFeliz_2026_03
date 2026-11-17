using BolsilloFeliz.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BolsilloFeliz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<MonedasService>();
        services.AddScoped<UsuariosService>();
        //services.AddScoped<EntidadesService>();
        //services.AddScoped<DeudasService>();
        //services.AddScoped<DetallesDeudaService>();

        return services;
    }
}