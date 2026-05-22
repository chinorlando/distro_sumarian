using System;
using System.Collections.Generic;

namespace OPERACION_OMM.Models;

public partial class Monedum
{
    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Simbolo { get; set; }

    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();

    public virtual ICollection<TipoCambio> TipoCambioMonedaDestinoNavigations { get; set; } = new List<TipoCambio>();

    public virtual ICollection<TipoCambio> TipoCambioMonedaOrigenNavigations { get; set; } = new List<TipoCambio>();
}
