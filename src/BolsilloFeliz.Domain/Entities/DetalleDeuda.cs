using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class DetalleDeuda
{
    public int IdDetalleDeuda { get; set; }

    public int IdUsuario { get; set; }

    public int IdDeuda { get; set; }

    public int IdTransaccion { get; set; }

    public decimal MontoPagado { get; set; }

    public DateTime FechaPago { get; set; }

    public string? Descripcion { get; set; }

    public virtual Deuda Deuda { get; set; } = null!;

    public virtual Transaccion Transaccione { get; set; } = null!;
}
