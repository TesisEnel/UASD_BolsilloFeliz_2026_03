using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Services.Tests.Infrastructure;

/// <summary>Base SQLite en memoria para ejecutar consultas reales de EF Core,
/// incluyendo ExecuteDelete/ExecuteUpdate, sin conectarse a SQL Server.</summary>
public sealed class SqliteTestContextFactory : IDbContextFactory<BolsilloFelizDbContext>, IDisposable
{
    private readonly SqliteConnection connection;

    public SqliteTestContextFactory()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    public BolsilloFelizDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BolsilloFelizDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;
        return new BolsilloFelizDbContext(options);
    }

    public Task<BolsilloFelizDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(CreateDbContext());

    public void Dispose() => connection.Dispose();
}
