using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class FlightStatus
{
    public int FlightStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<FlightStatusHistory> FlightStatusHistories { get; set; } = new List<FlightStatusHistory>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
