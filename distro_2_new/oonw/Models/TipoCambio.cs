using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OPERACION_OMM_NEW.Models;

public partial class TipoCambio
{
    public int Id { get; set; }

    public DateOnly Fecha { get; set; }

    public string MonedaOrigen { get; set; } = null!;

    public string MonedaDestino { get; set; } = null!;

    public decimal Tasa { get; set; }
    [JsonIgnore]
    public virtual Moneda? OMonedaDestino { get; set; } = null!;
    [JsonIgnore]
    public virtual Moneda? OMonedaOrigen { get; set; } = null!;
}
