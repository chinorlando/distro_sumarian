using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OPERACION_OMM_NEW.Models;

public partial class Cuenta
{
    public string NroCuenta { get; set; } = null!;

    public string? Tipo { get; set; }

    public string? Moneda { get; set; }

    public string? Nombre { get; set; }

    public decimal? Saldo { get; set; }
    [JsonIgnore]
    public virtual Moneda? OMoneda { get; set; }
    [JsonIgnore]
    public virtual ICollection<Movimiento> Movimiento { get; set; } = new List<Movimiento>();
}

public class TransferenciaRequest
{
    public string CuentaOrigen { get; set; }
    public string CuentaDestino { get; set; }
    public decimal Monto { get; set; }
}
