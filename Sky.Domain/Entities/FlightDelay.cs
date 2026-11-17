using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class FlightDelay
{
    public int FlightDelayId { get; set; }

    public int FlightId { get; set; }

    public int DelayReasonId { get; set; }

    public int? RegisteredByEmployeeId { get; set; }

    public int DelayMinutes { get; set; }

    public string? Notes { get; set; }

    public DateTime RegisteredAtUtc { get; set; }

    public virtual DelayReason DelayReason { get; set; } = null!;

    public virtual Flight Flight { get; set; } = null!;

    public virtual Employee? RegisteredByEmployee { get; set; }
}
