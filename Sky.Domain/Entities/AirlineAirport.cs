using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class AirlineAirport
{
    public int AirlineId { get; set; }

    public int AirportId { get; set; }

    public DateOnly SinceDate { get; set; }

    public bool IsHub { get; set; }

    public virtual Airline Airline { get; set; } = null!;

    public virtual Airport Airport { get; set; } = null!;
}
