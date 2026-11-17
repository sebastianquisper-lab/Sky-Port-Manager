using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AircraftSeat
{
    public int AircraftSeatId { get; set; }

    public int AircraftId { get; set; }

    public int CabinClassId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool IsWindow { get; set; }

    public bool IsAisle { get; set; }

    public bool IsEmergencyExit { get; set; }

    public virtual Aircraft Aircraft { get; set; } = null!;

    public virtual CabinClass CabinClass { get; set; } = null!;
}
