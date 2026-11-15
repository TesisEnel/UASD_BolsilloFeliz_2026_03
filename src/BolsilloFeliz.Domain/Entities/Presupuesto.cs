using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Presupuesto
{
    public int IdPresupuesto { get; set; }

    public int IdUsuario { get; set; }

    public int IdCategoria { get; set; }

    public int? IdCuenta { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual Cuenta? Cuenta { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
