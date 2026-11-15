using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Entidade
{
    public int IdEntidad { get; set; }

    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string TipoEntidad { get; set; } = null!;

    public string? Identificacion { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? Direccion { get; set; }

    public virtual ICollection<Deuda> Deuda { get; set; } = new List<Deuda>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
