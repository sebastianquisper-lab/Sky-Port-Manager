using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Runway
{
    public int RunwayId { get; set; }

    public int AirportId { get; set; }

    public string RunwayCode { get; set; } = null!;

    public int LengthM { get; set; }

    public string? SurfaceType { get; set; }

    public bool IsActive { get; set; }

    public virtual Airport Airport { get; set; } = null!;

    public virtual ICollection<FlightRunwayUsage> FlightRunwayUsages { get; set; } = new List<FlightRunwayUsage>();
}
