using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Airline
{
    public int AirlineId { get; set; }

    public int CountryId { get; set; }

    public string AirlineName { get; set; } = null!;

    public string IataCode { get; set; } = null!;

    public string IcaoCode { get; set; } = null!;

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();

    public virtual ICollection<AirlineAirport> AirlineAirports { get; set; } = new List<AirlineAirport>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Fare> Fares { get; set; } = new List<Fare>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
