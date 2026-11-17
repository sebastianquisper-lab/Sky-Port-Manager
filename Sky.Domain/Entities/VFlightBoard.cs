using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class VFlightBoard
{
    public int? FlightId { get; set; }

    public string? FlightCode { get; set; }

    public string? AirlineName { get; set; }

    public string? OriginIata { get; set; }

    public string? DestinationIata { get; set; }

    public DateTime? ScheduledDepartureUtc { get; set; }

    public DateTime? ScheduledArrivalUtc { get; set; }

    public DateTime? ActualDepartureUtc { get; set; }

    public DateTime? ActualArrivalUtc { get; set; }

    public string? FlightStatus { get; set; }

    public string? DepartureGate { get; set; }

    public string? ArrivalGate { get; set; }
}
