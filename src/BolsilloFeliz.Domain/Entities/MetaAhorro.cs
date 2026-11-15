using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class MetaAhorro
{
    public int IdMetaAhorro { get; set; }

    public int IdUsuario { get; set; }

    public int IdMoneda { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal MontoObjetivo { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaObjetivo { get; set; }

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<AporteAhorro> AportesAhorros { get; set; } = new List<AporteAhorro>();

    public virtual Moneda IdMonedaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
