using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Moneda
{
    public int IdMoneda { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Simbolo { get; set; } = null!;

    public bool Activa { get; set; }

    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();

    public virtual ICollection<MetaAhorro> MetasAhorros { get; set; } = new List<MetaAhorro>();
}
