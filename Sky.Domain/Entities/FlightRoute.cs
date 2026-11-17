using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class FlightRoute
{
    public int RouteId { get; set; }

    public int OriginAirportId { get; set; }

    public int DestinationAirportId { get; set; }

    public decimal DistanceKm { get; set; }

    public int EstimatedDurationMinutes { get; set; }

    public bool IsActive { get; set; }

    public virtual Airport DestinationAirport { get; set; } = null!;

    public virtual ICollection<Fare> Fares { get; set; } = new List<Fare>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual Airport OriginAirport { get; set; } = null!;
}
