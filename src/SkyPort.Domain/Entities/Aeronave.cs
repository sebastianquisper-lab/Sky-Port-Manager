using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Aeronave
{
    public int AeronaveId { get; set; }

    public int AerolineaId { get; set; }

    public string Matricula { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int Capacidad { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Aerolinea Aerolinea { get; set; } = null!;

    public virtual ICollection<Vuelo> Vuelo { get; set; } = new List<Vuelo>();
}
