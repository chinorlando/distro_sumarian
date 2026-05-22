using System;
using System.Collections.Generic;

namespace OPERACION_OMM.Models;

public partial class Movimiento
{
    public DateTime Fecha { get; set; }

    public string? Tipo { get; set; }

    public decimal? Importe { get; set; }

    public string? NroCuenta { get; set; }

    public virtual Cuentum? oCuenta { get; set; }
}

public class MovimientoSaldo
{
    public string? NroCuenta { get; set; }
    public decimal? Saldo { get; set; }
}

public class MovimientoCuentas
{
    public string? CuentaOrigenID { get; set; }
    public string? CuentaDestinoID { get; set; }
    public decimal Monto { get; set; }
}

public class MovimientoRequest
{
    public Movimiento? Movimiento { get; set; }
    public bool Debito { get; set; }
}
