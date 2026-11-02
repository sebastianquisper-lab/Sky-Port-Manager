using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Vuelo
{
    public int VueloId { get; set; }

    public string NumeroVuelo { get; set; } = null!;

    public int RutaId { get; set; }

    public int AeronaveId { get; set; }

    public int? PuertaId { get; set; }

    public DateTime FechaSalida { get; set; }

    public DateTime FechaLlegada { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Aeronave Aeronave { get; set; } = null!;

    public virtual Puerta? Puerta { get; set; }

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();

    public virtual Ruta Ruta { get; set; } = null!;

    public virtual ICollection<Tripulacionvuelo> Tripulacionvuelo { get; set; } = new List<Tripulacionvuelo>();
}
