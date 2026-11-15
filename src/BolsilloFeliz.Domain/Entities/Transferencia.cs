using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Transferencia
{
    public int IdTransferencia { get; set; }

    public int IdUsuario { get; set; }

    public int IdTransaccion { get; set; }

    public int IdCuentaOrigen { get; set; }

    public int IdCuentaDestino { get; set; }

    public virtual Cuenta Cuenta { get; set; } = null!;

    public virtual Cuenta CuentaNavigation { get; set; } = null!;

    public virtual Transaccion Transaccione { get; set; } = null!;
}
