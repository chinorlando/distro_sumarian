using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OPERACION_OMM_NEW.Models;

public partial class Movimiento
{
    public DateTime Fecha { get; set; }

    public string? Tipo { get; set; }

    public decimal? Importe { get; set; }

    public string? NroCuenta { get; set; }
    [JsonIgnore]
    public virtual Cuenta? OCuenta { get; set; }
}
