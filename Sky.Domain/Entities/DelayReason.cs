using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class DelayReason
{
    public int DelayReasonId { get; set; }

    public string ReasonCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<FlightDelay> FlightDelays { get; set; } = new List<FlightDelay>();
}
