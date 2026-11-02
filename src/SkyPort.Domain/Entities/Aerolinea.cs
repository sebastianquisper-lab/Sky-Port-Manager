using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Aerolinea
{
    public int AerolineaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string CodigoIata { get; set; } = null!;

    public string Pais { get; set; } = null!;

    public bool? Activa { get; set; }

    public virtual ICollection<Aeronave> Aeronave { get; set; } = new List<Aeronave>();
}
