using System;
using System.Collections.Generic;

namespace OPERACION_OMM.Models;

public partial class TipoCambio
{
    public int Id { get; set; }

    public DateOnly Fecha { get; set; }

    public string MonedaOrigen { get; set; } = null!;

    public string MonedaDestino { get; set; } = null!;

    public decimal Tasa { get; set; }

    public virtual Monedum? oMonedaDestino { get; set; } = null!;

    public virtual Monedum? oMonedaOrigen { get; set; } = null!;
}
