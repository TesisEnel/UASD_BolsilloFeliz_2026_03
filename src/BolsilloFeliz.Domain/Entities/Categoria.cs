using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Tipo { get; set; }

    public string? Descripcion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
