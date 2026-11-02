using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Boleto
{
    public int BoletoId { get; set; }

    public int ReservaId { get; set; }

    public string CodigoBoleto { get; set; } = null!;

    public DateTime FechaEmision { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Checkin? Checkin { get; set; }

    public virtual Embarque? Embarque { get; set; }

    public virtual ICollection<Equipaje> Equipaje { get; set; } = new List<Equipaje>();

    public virtual Reserva Reserva { get; set; } = null!;
}
