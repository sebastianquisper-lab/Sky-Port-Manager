using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class FlightRunwayUsage
{
    public int UsageId { get; set; }

    public int FlightId { get; set; }

    public int RunwayId { get; set; }

    public string Operation { get; set; } = null!;

    public DateTime UsedAtUtc { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Runway Runway { get; set; } = null!;
}
