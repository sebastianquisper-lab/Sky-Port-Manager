using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Airport
{
    public int AirportId { get; set; }

    public int CityId { get; set; }

    public string AirportName { get; set; } = null!;

    public string IataCode { get; set; } = null!;

    public string IcaoCode { get; set; } = null!;

    public string? Address { get; set; }

    public string TimeZone { get; set; } = null!;

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AircraftMaintenance> AircraftMaintenances { get; set; } = new List<AircraftMaintenance>();

    public virtual ICollection<AircraftStand> AircraftStands { get; set; } = new List<AircraftStand>();

    public virtual ICollection<AirlineAirport> AirlineAirports { get; set; } = new List<AirlineAirport>();

    public virtual ICollection<BaggageTracking> BaggageTrackings { get; set; } = new List<BaggageTracking>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<FlightRoute> FlightRouteDestinationAirports { get; set; } = new List<FlightRoute>();

    public virtual ICollection<FlightRoute> FlightRouteOriginAirports { get; set; } = new List<FlightRoute>();

    public virtual ICollection<Runway> Runways { get; set; } = new List<Runway>();

    public virtual ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();
}
