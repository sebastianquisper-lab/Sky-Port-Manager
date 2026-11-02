using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Terminal
{
    public int TerminalId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Puerta> Puerta { get; set; } = new List<Puerta>();
}
