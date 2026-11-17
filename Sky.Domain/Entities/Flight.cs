using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Flight
{
    public int FlightId { get; set; }

    public int AirlineId { get; set; }

    public int AircraftId { get; set; }

    public int RouteId { get; set; }

    public int FlightStatusId { get; set; }

    public string FlightNumber { get; set; } = null!;

    public DateTime ScheduledDepartureUtc { get; set; }

    public DateTime ScheduledArrivalUtc { get; set; }

    public DateTime? ActualDepartureUtc { get; set; }

    public DateTime? ActualArrivalUtc { get; set; }

    public int? DepartureGateId { get; set; }

    public int? ArrivalGateId { get; set; }

    public virtual Aircraft Aircraft { get; set; } = null!;

    public virtual Airline Airline { get; set; } = null!;

    public virtual BoardingGate? ArrivalGate { get; set; }

    public virtual ICollection<BookingFlightPassenger> BookingFlightPassengers { get; set; } = new List<BookingFlightPassenger>();

    public virtual BoardingGate? DepartureGate { get; set; }

    public virtual ICollection<FlightCrewAssignment> FlightCrewAssignments { get; set; } = new List<FlightCrewAssignment>();

    public virtual ICollection<FlightDelay> FlightDelays { get; set; } = new List<FlightDelay>();

    public virtual ICollection<FlightRunwayUsage> FlightRunwayUsages { get; set; } = new List<FlightRunwayUsage>();

    public virtual ICollection<FlightStandAssignment> FlightStandAssignments { get; set; } = new List<FlightStandAssignment>();

    public virtual FlightStatus FlightStatus { get; set; } = null!;

    public virtual ICollection<FlightStatusHistory> FlightStatusHistories { get; set; } = new List<FlightStatusHistory>();

    public virtual FlightRoute Route { get; set; } = null!;
}
