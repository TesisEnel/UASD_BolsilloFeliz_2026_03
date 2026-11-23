using Aplicada1.Core;
using BolsilloFeliz.Application.DTOs;
using BolsilloFeliz.Domain.Entities;
using BolsilloFeliz.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BolsilloFeliz.Application.Services;

public class UsuariosService(
    IDbContextFactory<BolsilloFelizDbContext> contextFactory
) : IServiceResult<UsuarioDto, int>
{
    private const int RolUsuarioPorDefecto = 2;

    // Proyección a DTO: nunca trae PasswordHash desde la BD
    private static readonly Expression<Func<Usuario, UsuarioDto>> ADto = u => new UsuarioDto
    {
        IdUsuario = u.IdUsuario,
        IdRol = u.IdRol,
        Nombres = u.Nombres,
        Apellidos = u.Apellidos,
        Email = u.Email,
        FechaRegistro = u.FechaRegistro,
        Estado = u.Estado
    };

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();

    public async Task<Result> Guardar(UsuarioDto dto)
    {
        var email = NormalizarEmail(dto.Email);

        if (await EmailExiste(email, dto.IdUsuario))
            return Result.Failure(new Error("EMAIL_DUPLICADO", "Ya existe un usuario con ese email."));

        if (!await Existe(dto.IdUsuario))
        {
            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
                return Result.Failure(new Error("PASSWORD_INVALIDA",
                    "La contraseña es obligatoria y debe tener al menos 8 caracteres."));

            var inserto = await Insertar(dto, email);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(dto, email);
            return Result.Success(modifico);
        }
    }

    private async Task<bool> Existe(int idUsuario)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Usuarios
            .AnyAsync(u => u.IdUsuario == idUsuario);
    }

    private async Task<bool> EmailExiste(string email, int excluirIdUsuario)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Usuarios
            .AnyAsync(u => u.Email == email && u.IdUsuario != excluirIdUsuario);
    }

    private async Task<bool> Insertar(UsuarioDto dto, string email)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var usuario = new Usuario
        {
            IdRol = RolUsuarioPorDefecto,   // el rol nunca viene del cliente
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FechaRegistro = DateTime.UtcNow,
            Estado = dto.Estado
        };

        contexto.Usuarios.Add(usuario);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(UsuarioDto dto, string email)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var usuario = await contexto.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == dto.IdUsuario);

        if (usuario is null)
            return false;

        // No se modifica el rol ni la contraseña desde aquí
        usuario.Nombres = dto.Nombres.Trim();
        usuario.Apellidos = dto.Apellidos.Trim();
        usuario.Email = email;
        usuario.Estado = dto.Estado;

        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<Result<UsuarioDto?>> Buscar(int idUsuario)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Usuarios
            .AsNoTracking()
            .Where(u => u.IdUsuario == idUsuario)
            .Select(ADto)
            .FirstOrDefaultAsync();
    }

    public async Task<Result<List<UsuarioDto>>> GetList(Expression<Func<UsuarioDto, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        return await contexto.Usuarios
            .AsNoTracking()
            .Select(ADto)
            .Where(criterio)
            .ToListAsync();
    }

    // Desactivación lógica: el usuario tiene muchas tablas relacionadas
    // (cuentas, transacciones, etc.) y un DELETE real fallaría por las llaves foráneas.

    public async Task<Result> Eliminar(int idUsuario)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        try
        {
            var eliminado = await contexto.Usuarios
                .Where(u => u.IdUsuario == idUsuario)
                .ExecuteDeleteAsync() > 0;

            return Result.Success(eliminado);
        }
        catch (Exception ex) when (ex is DbUpdateException
                                   || ex is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        {
            // 547 = violación de llave foránea
            return Result.Failure(new Error("TIENE_DATOS_RELACIONADOS",
                "No se puede eliminar: el usuario tiene cuentas, transacciones u otros datos asociados."));
        }
    }

    // Método adicional (igual que GetPrestamosPendientes en el ejemplo)
    public async Task<Result> CambiarPassword(int idUsuario, CambiarPasswordDto dto)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var usuario = await contexto.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

        if (usuario is null)
            return Result.Failure(new Error("NO_ENCONTRADO", "El usuario no existe."));

        if (!BCrypt.Net.BCrypt.Verify(dto.PasswordActual, usuario.PasswordHash))
            return Result.Failure(new Error("PASSWORD_INCORRECTA", "La contraseña actual es incorrecta."));

        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordNueva);
        await contexto.SaveChangesAsync();

        return Result.Success();
    }

   

    public async Task<Result<UsuarioDto?>> IniciarSesion(LoginDto dto)
    {
        var email = NormalizarEmail(dto.Email);

        await using var contexto = await contextFactory.CreateDbContextAsync();

        var usuario = await contexto.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.Estado);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return (UsuarioDto?)null;

        return new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            IdRol = usuario.IdRol,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,
            Email = usuario.Email,
            FechaRegistro = usuario.FechaRegistro,
            Estado = usuario.Estado
        };
    }
}