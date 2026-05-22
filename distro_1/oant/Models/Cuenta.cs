using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OPERACION_OMM.Models;

public partial class Cuenta
{
    public string NroCuenta { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string Moneda { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public decimal Saldo { get; set; }

    [JsonIgnore] // Añade esto arriba de la lista de movimientos
    public virtual ICollection<Movimiento> Movimiento { get; set; } = new List<Movimiento>();
}

// DTO para recibir la solicitud de transferencia desde el frontend
public class Transferencia
{
    public string CuentaOrigen { get; set; } = null!;
    public string CuentaDestino { get; set; } = null!;
    public decimal Monto { get; set; }
}