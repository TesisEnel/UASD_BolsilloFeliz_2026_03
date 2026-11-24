namespace BolsilloFeliz.Application.DTOs;

public class DashboardDto
{
    public List<SaldoCuentaDto> Saldos { get; set; } = new();
    public List<MovimientoDto> Proximas { get; set; } = new();
    public List<MovimientoDto> Ultimas { get; set; } = new();
    public List<ResumenSemanalDto> Semanas { get; set; } = new();
}

public class SaldoCuentaDto
{
    public int IdCuenta { get; set; }
    public string Nombre { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string CodigoMoneda { get; set; } = "";
    public string SimboloMoneda { get; set; } = "";
    public decimal Saldo { get; set; }
}

public class MovimientoDto
{
    public int IdTransaccion { get; set; }
    public string Nombre { get; set; } = "";
    public string Tipo { get; set; } = "";
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "";
    public string? Cuenta { get; set; }
    // true = suma al saldo, false = resta
    public bool EsIngreso { get; set; }
}

public class ResumenSemanalDto
{
    // Lunes de la semana
    public DateTime Inicio { get; set; }
    public decimal Ingresos { get; set; }
    public decimal Gastos { get; set; }
}