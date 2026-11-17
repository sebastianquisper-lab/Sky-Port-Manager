using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class VFlightOccupancy
{
    public int? FlightId { get; set; }

    public string? RegistrationCode { get; set; }

    public short? SeatCapacity { get; set; }

    public long? TicketsSold { get; set; }

    public decimal? OccupancyPercent { get; set; }
}
