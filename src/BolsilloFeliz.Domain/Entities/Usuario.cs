using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<AporteAhorro> AportesAhorros { get; set; } = new List<AporteAhorro>();

    public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();

    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();

    public virtual ICollection<Deuda> Deuda { get; set; } = new List<Deuda>();

    public virtual ICollection<Entidad> Entidades { get; set; } = new List<Entidad>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<MetaAhorro> MetasAhorros { get; set; } = new List<MetaAhorro>();

    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
