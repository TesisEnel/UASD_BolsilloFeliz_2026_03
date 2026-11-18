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
        var connectionString = configuration.GetConnectionString("BolsilloFelizDB");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "La cadena de conexión 'BolsilloFelizDB' no está configurada.");
        }

        services.AddDbContextFactory<BolsilloFelizDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}