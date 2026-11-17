using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Aircraft
{
    public int AircraftId { get; set; }

    public int AirlineId { get; set; }

    public int ModelId { get; set; }

    public int AircraftStatusId { get; set; }

    public string RegistrationCode { get; set; } = null!;

    public string? SerialNumber { get; set; }

    public short SeatCapacity { get; set; }

    public short? ManufactureYear { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AircraftMaintenance> AircraftMaintenances { get; set; } = new List<AircraftMaintenance>();

    public virtual ICollection<AircraftSeat> AircraftSeats { get; set; } = new List<AircraftSeat>();

    public virtual AircraftStatus AircraftStatus { get; set; } = null!;

    public virtual Airline Airline { get; set; } = null!;

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual AircraftModel Model { get; set; } = null!;
}
