using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OPERACION_OMM_NEW.Models;

public partial class Moneda
{
    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Simbolo { get; set; }
    [JsonIgnore]
    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();
    [JsonIgnore]
    public virtual ICollection<TipoCambio> OTipoCambioMonedaDestino{ get; set; } = new List<TipoCambio>();
    [JsonIgnore]
    public virtual ICollection<TipoCambio> OTipoCambioMonedaOrigen{ get; set; } = new List<TipoCambio>();
}
