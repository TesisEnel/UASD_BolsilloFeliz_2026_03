using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BolsilloFeliz.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BolsilloFeliz")
            ?? throw new InvalidOperationException(
                "Falta la cadena de conexión 'BolsilloFeliz' en ConnectionStrings.");

        services.AddDbContextFactory<BolsilloFelizDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}