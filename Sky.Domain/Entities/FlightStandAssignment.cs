using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class FlightStandAssignment
{
    public int AssignmentId { get; set; }

    public int FlightId { get; set; }

    public int StandId { get; set; }

    public DateTime AssignedFromUtc { get; set; }

    public DateTime AssignedToUtc { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual AircraftStand Stand { get; set; } = null!;
}
