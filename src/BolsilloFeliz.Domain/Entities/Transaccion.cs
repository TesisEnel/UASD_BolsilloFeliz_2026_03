using System;
using System.Collections.Generic;

namespace BolsilloFeliz.Domain.Entities;

public partial class Transaccion
{
    public int IdTransaccion { get; set; }

    public int IdUsuario { get; set; }

    public int? IdCuenta { get; set; }

    public int? IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Monto { get; set; }

    public string Tipo { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string? Descripcion { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Categoria? Categoria { get; set; }

    public virtual Cuenta? Cuenta { get; set; }

    public virtual ICollection<DetalleDeuda> DetallesDeuda { get; set; } = new List<DetalleDeuda>();

    public virtual ICollection<DetallePrestamo> DetallesPrestamos { get; set; } = new List<DetallePrestamo>();

    public virtual ICollection<Deuda> Deuda { get; set; } = new List<Deuda>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual ICollection<Transferencia> Transferencia { get; set; } = new List<Transferencia>();
}
