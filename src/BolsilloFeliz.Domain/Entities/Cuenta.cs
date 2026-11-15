using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Cuenta
{
    public int IdCuenta { get; set; }

    public int IdUsuario { get; set; }

    public int IdMoneda { get; set; }

    public string Nombre { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public decimal SaldoInicial { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<AporteAhorro> AportesAhorros { get; set; } = new List<AporteAhorro>();

    public virtual Moneda IdMonedaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();

    public virtual ICollection<Transferencia> TransferenciaCuenta { get; set; } = new List<Transferencia>();

    public virtual ICollection<Transferencia> TransferenciaCuentaNavigations { get; set; } = new List<Transferencia>();
}
