using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Equipaje
{
    public int EquipajeId { get; set; }

    public int BoletoId { get; set; }

    public string Codigo { get; set; } = null!;

    public decimal PesoKg { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Boleto Boleto { get; set; } = null!;
}
