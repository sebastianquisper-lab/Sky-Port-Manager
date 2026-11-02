using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Reserva
{
    public int ReservaId { get; set; }

    public string Codigo { get; set; } = null!;

    public int PasajeroId { get; set; }

    public int VueloId { get; set; }

    public string Asiento { get; set; } = null!;

    public DateTime FechaReserva { get; set; }

    public string Estado { get; set; } = null!;

    public string? AsientoActivo { get; set; }

    public int? PasajeroActivoId { get; set; }

    public virtual Boleto? Boleto { get; set; }

    public virtual Pasajero Pasajero { get; set; } = null!;

    public virtual Vuelo Vuelo { get; set; } = null!;
}
