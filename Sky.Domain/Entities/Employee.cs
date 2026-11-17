using System;
using System.Collections.Generic;

namespace Sky.Domain.Entities;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int AirportId { get; set; }

    public int DocumentTypeId { get; set; }

    public int EmployeeRoleId { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateOnly HireDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AircraftMaintenance> AircraftMaintenances { get; set; } = new List<AircraftMaintenance>();

    public virtual Airport Airport { get; set; } = null!;

    public virtual AppUser? AppUser { get; set; }

    public virtual ICollection<BaggageTracking> BaggageTrackings { get; set; } = new List<BaggageTracking>();

    public virtual ICollection<Boarding> Boardings { get; set; } = new List<Boarding>();

    public virtual ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();

    public virtual DocumentType DocumentType { get; set; } = null!;

    public virtual ICollection<EmployeeLicense> EmployeeLicenses { get; set; } = new List<EmployeeLicense>();

    public virtual EmployeeRole EmployeeRole { get; set; } = null!;

    public virtual ICollection<FlightCrewAssignment> FlightCrewAssignments { get; set; } = new List<FlightCrewAssignment>();

    public virtual ICollection<FlightDelay> FlightDelays { get; set; } = new List<FlightDelay>();

    public virtual ICollection<FlightStatusHistory> FlightStatusHistories { get; set; } = new List<FlightStatusHistory>();

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
