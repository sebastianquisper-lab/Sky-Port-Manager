using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class BaggageTracking
{
    public int TrackingId { get; set; }

    public int BaggageId { get; set; }

    public int BaggageStatusId { get; set; }

    public int? AirportId { get; set; }

    public int? EmployeeId { get; set; }

    public string? LocationDescription { get; set; }

    public DateTime EventTimeUtc { get; set; }

    public virtual Airport? Airport { get; set; }

    public virtual Baggage Baggage { get; set; } = null!;

    public virtual BaggageStatus BaggageStatus { get; set; } = null!;

    public virtual Employee? Employee { get; set; }
}
