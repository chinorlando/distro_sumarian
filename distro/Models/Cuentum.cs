using System;
using System.Collections.Generic;

namespace OPERACION_OMM.Models;

public partial class Cuentum
{
    public string NroCuenta { get; set; } = null!;

    public string? Tipo { get; set; }

    public string? Moneda { get; set; }

    public string? Nombre { get; set; }

    public decimal? Saldo { get; set; }

    public virtual Monedum? oMoneda { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
