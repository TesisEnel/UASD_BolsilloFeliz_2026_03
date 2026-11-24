using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;

namespace BolsilloFeliz.Services.Tests.Infrastructure;

public abstract class ServiceTestBase : IDisposable
{
    protected SqliteTestContextFactory Factory { get; } = new();
    protected BolsilloFelizDbContext Db => Factory.CreateDbContext();

    protected async Task SeedUsuario(int id = 1)
    {
        await using var db = Db;
        if (!db.Usuarios.Any(x => x.IdUsuario == id))
        {
            // Rol 2 es el rol predeterminado utilizado por UsuariosService.
            if (!db.Roles.Any(x => x.IdRol == 2)) db.Roles.Add(new Rol { IdRol = 2, Nombre = "Usuario" });
            db.Usuarios.Add(new Usuario { IdUsuario = id, IdRol = 2, Nombres = "Ana", Apellidos = "Prueba", Email = $"ana{id}@test.com", PasswordHash = "hash", FechaRegistro = DateTime.UtcNow, Estado = true });
            await db.SaveChangesAsync();
        }
    }

    protected async Task SeedMoneda(int id = 1)
    {
        await using var db = Db;
        if (!db.Monedas.Any(x => x.IdMoneda == id))
        {
            db.Monedas.Add(new Moneda { IdMoneda = id, Codigo = "DOP", Nombre = "Peso dominicano", Simbolo = "RD$", Activa = true });
            await db.SaveChangesAsync();
        }
    }

    public void Dispose() => Factory.Dispose();
}
