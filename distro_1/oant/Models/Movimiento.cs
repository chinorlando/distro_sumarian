using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OPERACION_OMM.Models;

public partial class Movimiento
{
    public string NroCuenta { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Importe { get; set; }

    [JsonIgnore]
    public virtual Cuenta? OCuenta { get; set; }
}
