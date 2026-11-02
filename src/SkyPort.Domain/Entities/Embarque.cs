using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Embarque
{
    public int EmbarqueId { get; set; }

    public int BoletoId { get; set; }

    public int? PuertaId { get; set; }

    public DateTime FechaHora { get; set; }

    public virtual Boleto Boleto { get; set; } = null!;

    public virtual Puerta? Puerta { get; set; }
}
