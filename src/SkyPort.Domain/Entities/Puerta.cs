using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Puerta
{
    public int PuertaId { get; set; }

    public int TerminalId { get; set; }

    public string Codigo { get; set; } = null!;

    public bool? Disponible { get; set; }

    public virtual ICollection<Embarque> Embarque { get; set; } = new List<Embarque>();

    public virtual Terminal Terminal { get; set; } = null!;

    public virtual ICollection<Vuelo> Vuelo { get; set; } = new List<Vuelo>();
}
