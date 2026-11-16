using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Deuda
{
    public int IdDeuda { get; set; }

    public int IdUsuario { get; set; }

    public int IdEntidad { get; set; }

    public int? IdTransaccionDesembolso { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal MontoOriginal { get; set; }

    public DateOnly FechaDeuda { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public decimal? CuotaMensual { get; set; }

    public string Estado { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<DetalleDeuda> DetallesDeuda { get; set; } = new List<DetalleDeuda>();

    public virtual Entidad Entidade { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual Transaccion? Transaccione { get; set; }
}
