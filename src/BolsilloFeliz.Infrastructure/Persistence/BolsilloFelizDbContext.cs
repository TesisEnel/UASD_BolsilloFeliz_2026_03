using System;
using System.Collections.Generic;
using BolsilloFeliz.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BolsilloFeliz.Infrastructure.Persistence;

public partial class BolsilloFelizDbContext : DbContext
{
    public BolsilloFelizDbContext(DbContextOptions<BolsilloFelizDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AporteAhorro> AportesAhorros { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cuenta> Cuentas { get; set; }

    public virtual DbSet<DetalleDeuda> DetallesDeudas { get; set; }

    public virtual DbSet<DetallePrestamo> DetallesPrestamos { get; set; }

    public virtual DbSet<Deuda> Deudas { get; set; }

    public virtual DbSet<Entidade> Entidades { get; set; }

    public virtual DbSet<MetaAhorro> MetasAhorros { get; set; }

    public virtual DbSet<Moneda> Monedas { get; set; }

    public virtual DbSet<Notificacion> Notificaciones { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Presupuesto> Presupuestos { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<Transaccion> Transacciones { get; set; }

    public virtual DbSet<Transferencia> Transferencias { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AporteAhorro>(entity =>
        {
            entity.HasKey(e => e.IdAporteAhorro).HasName("PK__AportesA__12251539389F464F");

            entity.Property(e => e.Monto).HasColumnType("money");
            entity.Property(e => e.Nota).HasMaxLength(250);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.AportesAhorros)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AporteAhorro_Usuario");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.AportesAhorros)
                .HasPrincipalKey(p => new { p.IdCuenta, p.IdUsuario })
                .HasForeignKey(d => new { d.IdCuenta, d.IdUsuario })
                .HasConstraintName("FK_AporteAhorro_Cuenta");

            entity.HasOne(d => d.MetasAhorro).WithMany(p => p.AportesAhorros)
                .HasPrincipalKey(p => new { p.IdMetaAhorro, p.IdUsuario })
                .HasForeignKey(d => new { d.IdMetaAhorro, d.IdUsuario })
                .HasConstraintName("FK_AporteAhorro_Meta");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A105787DA8A");

            entity.HasIndex(e => new { e.IdCategoria, e.IdUsuario }, "UQ_Categoria_IdCategoria_IdUsuario").IsUnique();

            entity.HasIndex(e => new { e.IdUsuario, e.Nombre, e.Tipo }, "UQ_Categoria_Nombre_Usuario_Tipo").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Categoria)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Usuario");
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("PK__Cuentas__D41FD706C9503E57");

            entity.HasIndex(e => new { e.IdCuenta, e.IdUsuario }, "UQ_Cuenta_IdCuenta_IdUsuario").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.SaldoInicial).HasColumnType("money");
            entity.Property(e => e.Tipo).HasMaxLength(30);

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.IdMoneda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Moneda");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Usuario");
        });

        modelBuilder.Entity<DetalleDeuda>(entity =>
        {
            entity.HasKey(e => e.IdDetalleDeuda).HasName("PK__Detalles__BBE9846B2D512E87");

            entity.HasIndex(e => e.IdTransaccion, "UQ__Detalles__334B1F76EF811256").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaPago).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.MontoPagado).HasColumnType("money");

            entity.HasOne(d => d.Deuda).WithMany(p => p.DetallesDeuda)
                .HasPrincipalKey(p => new { p.IdDeuda, p.IdUsuario })
                .HasForeignKey(d => new { d.IdDeuda, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleDeuda_Deuda");

            entity.HasOne(d => d.Transaccione).WithMany(p => p.DetallesDeuda)
                .HasPrincipalKey(p => new { p.IdTransaccion, p.IdUsuario })
                .HasForeignKey(d => new { d.IdTransaccion, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleDeuda_Transaccion");
        });

        modelBuilder.Entity<DetallePrestamo>(entity =>
        {
            entity.HasKey(e => e.IdDetallePrestamo).HasName("PK__Detalles__D07748AAD0B1ED31");

            entity.HasIndex(e => e.IdTransaccion, "UQ__Detalles__334B1F76530168E2").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.FechaPago).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.MontoPagado).HasColumnType("money");

            entity.HasOne(d => d.Prestamo).WithMany(p => p.DetallesPrestamos)
                .HasPrincipalKey(p => new { p.IdPrestamo, p.IdUsuario })
                .HasForeignKey(d => new { d.IdPrestamo, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePrestamo_Prestamo");

            entity.HasOne(d => d.Transaccione).WithMany(p => p.DetallesPrestamos)
                .HasPrincipalKey(p => new { p.IdTransaccion, p.IdUsuario })
                .HasForeignKey(d => new { d.IdTransaccion, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePrestamo_Transaccion");
        });

        modelBuilder.Entity<Deuda>(entity =>
        {
            entity.HasKey(e => e.IdDeuda).HasName("PK__Deudas__7F8C86B186D0BBF7");

            entity.HasIndex(e => new { e.IdDeuda, e.IdUsuario }, "UQ_Deuda_IdDeuda_IdUsuario").IsUnique();

            entity.Property(e => e.CuotaMensual).HasColumnType("money");
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Activa");
            entity.Property(e => e.MontoOriginal).HasColumnType("money");
            entity.Property(e => e.Nombre).HasMaxLength(120);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Deuda)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deuda_Usuario");

            entity.HasOne(d => d.Entidade).WithMany(p => p.Deuda)
                .HasPrincipalKey(p => new { p.IdEntidad, p.IdUsuario })
                .HasForeignKey(d => new { d.IdEntidad, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deuda_Entidad");

            entity.HasOne(d => d.Transaccione).WithMany(p => p.Deuda)
                .HasPrincipalKey(p => new { p.IdTransaccion, p.IdUsuario })
                .HasForeignKey(d => new { d.IdTransaccionDesembolso, d.IdUsuario })
                .HasConstraintName("FK_Deuda_Transaccion");
        });

        modelBuilder.Entity<Entidade>(entity =>
        {
            entity.HasKey(e => e.IdEntidad).HasName("PK__Entidade__7D6628684C0025E0");

            entity.HasIndex(e => new { e.IdEntidad, e.IdUsuario }, "UQ_Entidad_IdEntidad_IdUsuario").IsUnique();

            entity.Property(e => e.Direccion).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Identificacion).HasMaxLength(30);
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.TipoEntidad).HasMaxLength(30);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Entidades)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Entidad_Usuario");
        });

        modelBuilder.Entity<MetaAhorro>(entity =>
        {
            entity.HasKey(e => e.IdMetaAhorro).HasName("PK__MetasAho__6738937CC8EAB941");

            entity.HasIndex(e => new { e.IdMetaAhorro, e.IdUsuario }, "UQ_MetaAhorro_IdMetaAhorro_IdUsuario").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.MontoObjetivo).HasColumnType("money");
            entity.Property(e => e.Nombre).HasMaxLength(120);

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.MetasAhorros)
                .HasForeignKey(d => d.IdMoneda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MetaAhorro_Moneda");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.MetasAhorros)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MetaAhorro_Usuario");
        });

        modelBuilder.Entity<Moneda>(entity =>
        {
            entity.HasKey(e => e.IdMoneda).HasName("PK__Monedas__AA690671FE45E851");

            entity.HasIndex(e => e.Codigo, "UQ__Monedas__06370DAC687F1658").IsUnique();

            entity.Property(e => e.Activa).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Simbolo).HasMaxLength(5);
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__F6CA0A8560407DFB");

            entity.Property(e => e.Fecha).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Mensaje).HasMaxLength(500);
            entity.Property(e => e.Tipo).HasMaxLength(30);
            entity.Property(e => e.Titulo).HasMaxLength(120);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notificacion_Usuario");
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.IdPrestamo).HasName("PK__Prestamo__6FF194C0636E7071");

            entity.HasIndex(e => new { e.IdPrestamo, e.IdUsuario }, "UQ_Prestamo_IdPrestamo_IdUsuario").IsUnique();

            entity.Property(e => e.CuotaMensual).HasColumnType("money");
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Activo");
            entity.Property(e => e.MontoOriginal).HasColumnType("money");
            entity.Property(e => e.Nombre).HasMaxLength(120);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_Usuario");

            entity.HasOne(d => d.Entidade).WithMany(p => p.Prestamos)
                .HasPrincipalKey(p => new { p.IdEntidad, p.IdUsuario })
                .HasForeignKey(d => new { d.IdEntidad, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamo_Entidad");

            entity.HasOne(d => d.Transaccione).WithMany(p => p.Prestamos)
                .HasPrincipalKey(p => new { p.IdTransaccion, p.IdUsuario })
                .HasForeignKey(d => new { d.IdTransaccionDesembolso, d.IdUsuario })
                .HasConstraintName("FK_Prestamo_Transaccion");
        });

        modelBuilder.Entity<Presupuesto>(entity =>
        {
            entity.HasKey(e => e.IdPresupuesto).HasName("PK__Presupue__D70FD19072131627");

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Monto).HasColumnType("money");
            entity.Property(e => e.Nombre).HasMaxLength(120);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Presupuestos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Presupuesto_Usuario");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Presupuestos)
                .HasPrincipalKey(p => new { p.IdCategoria, p.IdUsuario })
                .HasForeignKey(d => new { d.IdCategoria, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Presupuesto_Categoria");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.Presupuestos)
                .HasPrincipalKey(p => new { p.IdCuenta, p.IdUsuario })
                .HasForeignKey(d => new { d.IdCuenta, d.IdUsuario })
                .HasConstraintName("FK_Presupuesto_Cuenta");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.HasIndex(e => e.Nombre, "UQ_Roles_Nombre").IsUnique();

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.IdTransaccion).HasName("PK__Transacc__334B1F77274B4989");

            entity.HasIndex(e => new { e.IdTransaccion, e.IdUsuario }, "UQ_Transaccion_IdTransaccion_IdUsuario").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Completada");
            entity.Property(e => e.Fecha).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Monto).HasColumnType("money");
            entity.Property(e => e.Nombre).HasMaxLength(120);
            entity.Property(e => e.Tipo).HasMaxLength(30);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Transacciones)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaccion_Usuario");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Transacciones)
                .HasPrincipalKey(p => new { p.IdCategoria, p.IdUsuario })
                .HasForeignKey(d => new { d.IdCategoria, d.IdUsuario })
                .HasConstraintName("FK_Transaccion_Categoria");

            entity.HasOne(d => d.Cuenta).WithMany(p => p.Transacciones)
                .HasPrincipalKey(p => new { p.IdCuenta, p.IdUsuario })
                .HasForeignKey(d => new { d.IdCuenta, d.IdUsuario })
                .HasConstraintName("FK_Transaccion_Cuenta");
        });

        modelBuilder.Entity<Transferencia>(entity =>
        {
            entity.HasKey(e => e.IdTransferencia).HasName("PK__Transfer__6E5969EC6B8F3BFB");

            entity.HasIndex(e => e.IdTransaccion, "UQ__Transfer__334B1F76F4FB2670").IsUnique();

            entity.HasOne(d => d.Cuenta).WithMany(p => p.TransferenciaCuenta)
                .HasPrincipalKey(p => new { p.IdCuenta, p.IdUsuario })
                .HasForeignKey(d => new { d.IdCuentaDestino, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencia_CuentaDestino");

            entity.HasOne(d => d.CuentaNavigation).WithMany(p => p.TransferenciaCuentaNavigations)
                .HasPrincipalKey(p => new { p.IdCuenta, p.IdUsuario })
                .HasForeignKey(d => new { d.IdCuentaOrigen, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencia_CuentaOrigen");

            entity.HasOne(d => d.Transaccione).WithMany(p => p.Transferencia)
                .HasPrincipalKey(p => new { p.IdTransaccion, p.IdUsuario })
                .HasForeignKey(d => new { d.IdTransaccion, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencia_Transaccion");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK_Usuario");

            entity.HasIndex(e => e.Email, "UQ_Usuario_Email").IsUnique();

            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.Estado).HasDefaultValue(true, "DF_Usuario_Estado");
            entity.Property(e => e.FechaRegistro)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_Usuario_FechaRegistro");
            entity.Property(e => e.IdRol).HasDefaultValue(2, "DF_Usuario_IdRol");
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
