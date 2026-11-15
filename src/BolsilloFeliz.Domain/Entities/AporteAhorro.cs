using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class AporteAhorro
{
    public int IdAporteAhorro { get; set; }

    public int IdUsuario { get; set; }

    public int IdMetaAhorro { get; set; }

    public int? IdCuenta { get; set; }

    public decimal Monto { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Nota { get; set; }

    public virtual Cuenta? Cuenta { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual MetaAhorro MetasAhorro { get; set; } = null!;
}
