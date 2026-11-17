using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class FlightStatusHistory
{
    public int HistoryId { get; set; }

    public int FlightId { get; set; }

    public int FlightStatusId { get; set; }

    public int? ChangedByEmployeeId { get; set; }

    public DateTime ChangedAtUtc { get; set; }

    public string? Notes { get; set; }

    public virtual Employee? ChangedByEmployee { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual FlightStatus FlightStatus { get; set; } = null!;
}
