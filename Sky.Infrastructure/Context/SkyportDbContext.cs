using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sky.Domain.Entities;

namespace Sky.Infrastructure.Context;

public partial class SkyportDbContext : DbContext
{
    public SkyportDbContext(DbContextOptions<SkyportDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aircraft> Aircraft { get; set; }

    public virtual DbSet<AircraftMaintenance> AircraftMaintenances { get; set; }

    public virtual DbSet<AircraftManufacturer> AircraftManufacturers { get; set; }

    public virtual DbSet<AircraftModel> AircraftModels { get; set; }

    public virtual DbSet<AircraftSeat> AircraftSeats { get; set; }

    public virtual DbSet<AircraftStand> AircraftStands { get; set; }

    public virtual DbSet<AircraftStatus> AircraftStatuses { get; set; }

    public virtual DbSet<Airline> Airlines { get; set; }

    public virtual DbSet<AirlineAirport> AirlineAirports { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<AppRole> AppRoles { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Baggage> Baggages { get; set; }

    public virtual DbSet<BaggageStatus> BaggageStatuses { get; set; }

    public virtual DbSet<BaggageTracking> BaggageTrackings { get; set; }

    public virtual DbSet<BaggageType> BaggageTypes { get; set; }

    public virtual DbSet<Boarding> Boardings { get; set; }

    public virtual DbSet<BoardingGate> BoardingGates { get; set; }

    public virtual DbSet<BoardingPass> BoardingPasses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingFlightPassenger> BookingFlightPassengers { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<CabinClass> CabinClasses { get; set; }

    public virtual DbSet<CheckIn> CheckIns { get; set; }

    public virtual DbSet<CheckInCounter> CheckInCounters { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<DelayReason> DelayReasons { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeLicense> EmployeeLicenses { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Fare> Fares { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightCrewAssignment> FlightCrewAssignments { get; set; }

    public virtual DbSet<FlightDelay> FlightDelays { get; set; }

    public virtual DbSet<FlightRoute> FlightRoutes { get; set; }

    public virtual DbSet<FlightRunwayUsage> FlightRunwayUsages { get; set; }

    public virtual DbSet<FlightStandAssignment> FlightStandAssignments { get; set; }

    public virtual DbSet<FlightStatus> FlightStatuses { get; set; }

    public virtual DbSet<FlightStatusHistory> FlightStatusHistories { get; set; }

    public virtual DbSet<MaintenanceType> MaintenanceTypes { get; set; }

    public virtual DbSet<Passenger> Passengers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Runway> Runways { get; set; }

    public virtual DbSet<Terminal> Terminals { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<VFlightBoard> VFlightBoards { get; set; }

    public virtual DbSet<VFlightOccupancy> VFlightOccupancies { get; set; }

    public virtual DbSet<VLicensesExpiring> VLicensesExpirings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone", "recovery_code" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in", "like", "ilike", "is", "match", "imatch", "isdistinct" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.AircraftId).HasName("aircraft_pkey");

            entity.ToTable("aircraft");

            entity.HasIndex(e => e.AirlineId, "idx_aircraft_airline");

            entity.HasIndex(e => e.ModelId, "idx_aircraft_model");

            entity.HasIndex(e => e.AircraftStatusId, "idx_aircraft_status");

            entity.HasIndex(e => new { e.AircraftId, e.AirlineId }, "uq_aircraft_id_airline").IsUnique();

            entity.HasIndex(e => e.RegistrationCode, "uq_aircraft_registration").IsUnique();

            entity.HasIndex(e => e.SerialNumber, "uq_aircraft_serial").IsUnique();

            entity.Property(e => e.AircraftId).HasColumnName("aircraft_id");
            entity.Property(e => e.AircraftStatusId).HasColumnName("aircraft_status_id");
            entity.Property(e => e.AirlineId).HasColumnName("airline_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ManufactureYear).HasColumnName("manufacture_year");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.RegistrationCode)
                .HasMaxLength(20)
                .HasColumnName("registration_code");
            entity.Property(e => e.SeatCapacity).HasColumnName("seat_capacity");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .HasColumnName("serial_number");

            entity.HasOne(d => d.AircraftStatus).WithMany(p => p.Aircraft)
                .HasForeignKey(d => d.AircraftStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aircraft_status");

            entity.HasOne(d => d.Airline).WithMany(p => p.Aircraft)
                .HasForeignKey(d => d.AirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aircraft_airline");

            entity.HasOne(d => d.Model).WithMany(p => p.Aircraft)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aircraft_model");
        });

        modelBuilder.Entity<AircraftMaintenance>(entity =>
        {
            entity.HasKey(e => e.MaintenanceId).HasName("aircraft_maintenance_pkey");

            entity.ToTable("aircraft_maintenance");

            entity.HasIndex(e => new { e.AircraftId, e.StartDateUtc }, "idx_maint_aircraft");

            entity.HasIndex(e => e.AirportId, "idx_maint_airport");

            entity.Property(e => e.MaintenanceId).HasColumnName("maintenance_id");
            entity.Property(e => e.AircraftId).HasColumnName("aircraft_id");
            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.EndDateUtc).HasColumnName("end_date_utc");
            entity.Property(e => e.MaintenanceTypeId).HasColumnName("maintenance_type_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnName("notes");
            entity.Property(e => e.StartDateUtc).HasColumnName("start_date_utc");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValueSql("'SCHEDULED'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TechnicianEmployeeId).HasColumnName("technician_employee_id");

            entity.HasOne(d => d.Aircraft).WithMany(p => p.AircraftMaintenances)
                .HasForeignKey(d => d.AircraftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_maint_aircraft");

            entity.HasOne(d => d.Airport).WithMany(p => p.AircraftMaintenances)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_maint_airport");

            entity.HasOne(d => d.MaintenanceType).WithMany(p => p.AircraftMaintenances)
                .HasForeignKey(d => d.MaintenanceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_maint_type");

            entity.HasOne(d => d.TechnicianEmployee).WithMany(p => p.AircraftMaintenances)
                .HasForeignKey(d => d.TechnicianEmployeeId)
                .HasConstraintName("fk_maint_technician");
        });

        modelBuilder.Entity<AircraftManufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("aircraft_manufacturer_pkey");

            entity.ToTable("aircraft_manufacturer");

            entity.HasIndex(e => e.ManufacturerName, "uq_manufacturer_name").IsUnique();

            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.ManufacturerName)
                .HasMaxLength(120)
                .HasColumnName("manufacturer_name");
        });

        modelBuilder.Entity<AircraftModel>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("aircraft_model_pkey");

            entity.ToTable("aircraft_model");

            entity.HasIndex(e => new { e.ManufacturerId, e.ModelName }, "uq_model_manufacturer_name").IsUnique();

            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.ModelName)
                .HasMaxLength(100)
                .HasColumnName("model_name");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.AircraftModels)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_model_manufacturer");
        });

        modelBuilder.Entity<AircraftSeat>(entity =>
        {
            entity.HasKey(e => e.AircraftSeatId).HasName("aircraft_seat_pkey");

            entity.ToTable("aircraft_seat");

            entity.HasIndex(e => e.CabinClassId, "idx_seat_cabin_class");

            entity.HasIndex(e => new { e.AircraftId, e.SeatNumber }, "uq_seat_aircraft_number").IsUnique();

            entity.Property(e => e.AircraftSeatId).HasColumnName("aircraft_seat_id");
            entity.Property(e => e.AircraftId).HasColumnName("aircraft_id");
            entity.Property(e => e.CabinClassId).HasColumnName("cabin_class_id");
            entity.Property(e => e.IsAisle).HasColumnName("is_aisle");
            entity.Property(e => e.IsEmergencyExit).HasColumnName("is_emergency_exit");
            entity.Property(e => e.IsWindow).HasColumnName("is_window");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(10)
                .HasColumnName("seat_number");

            entity.HasOne(d => d.Aircraft).WithMany(p => p.AircraftSeats)
                .HasForeignKey(d => d.AircraftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_seat_aircraft");

            entity.HasOne(d => d.CabinClass).WithMany(p => p.AircraftSeats)
                .HasForeignKey(d => d.CabinClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_seat_cabin_class");
        });

        modelBuilder.Entity<AircraftStand>(entity =>
        {
            entity.HasKey(e => e.StandId).HasName("aircraft_stand_pkey");

            entity.ToTable("aircraft_stand");

            entity.HasIndex(e => new { e.AirportId, e.StandCode }, "uq_stand_airport_code").IsUnique();

            entity.Property(e => e.StandId).HasColumnName("stand_id");
            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsRemote).HasColumnName("is_remote");
            entity.Property(e => e.StandCode)
                .HasMaxLength(10)
                .HasColumnName("stand_code");

            entity.HasOne(d => d.Airport).WithMany(p => p.AircraftStands)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stand_airport");
        });

        modelBuilder.Entity<AircraftStatus>(entity =>
        {
            entity.HasKey(e => e.AircraftStatusId).HasName("aircraft_status_pkey");

            entity.ToTable("aircraft_status");

            entity.HasIndex(e => e.StatusName, "uq_aircraft_status_name").IsUnique();

            entity.Property(e => e.AircraftStatusId).HasColumnName("aircraft_status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(40)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Airline>(entity =>
        {
            entity.HasKey(e => e.AirlineId).HasName("airline_pkey");

            entity.ToTable("airline");

            entity.HasIndex(e => e.CountryId, "idx_airline_country");

            entity.HasIndex(e => e.IataCode, "uq_airline_iata").IsUnique();

            entity.HasIndex(e => e.IcaoCode, "uq_airline_icao").IsUnique();

            entity.HasIndex(e => e.AirlineName, "uq_airline_name").IsUnique();

            entity.Property(e => e.AirlineId).HasColumnName("airline_id");
            entity.Property(e => e.AirlineName)
                .HasMaxLength(120)
                .HasColumnName("airline_name");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(150)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(25)
                .HasColumnName("contact_phone");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.IataCode)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("iata_code");
            entity.Property(e => e.IcaoCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("icao_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");

            entity.HasOne(d => d.Country).WithMany(p => p.Airlines)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_airline_country");
        });

        modelBuilder.Entity<AirlineAirport>(entity =>
        {
            entity.HasKey(e => new { e.AirlineId, e.AirportId }).HasName("airline_airport_pkey");

            entity.ToTable("airline_airport");

            entity.HasIndex(e => e.AirportId, "idx_aa_airport");

            entity.Property(e => e.AirlineId).HasColumnName("airline_id");
            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.IsHub).HasColumnName("is_hub");
            entity.Property(e => e.SinceDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("since_date");

            entity.HasOne(d => d.Airline).WithMany(p => p.AirlineAirports)
                .HasForeignKey(d => d.AirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aa_airline");

            entity.HasOne(d => d.Airport).WithMany(p => p.AirlineAirports)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aa_airport");
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.AirportId).HasName("airport_pkey");

            entity.ToTable("airport");

            entity.HasIndex(e => e.CityId, "idx_airport_city");

            entity.HasIndex(e => e.IataCode, "uq_airport_iata").IsUnique();

            entity.HasIndex(e => e.IcaoCode, "uq_airport_icao").IsUnique();

            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.AirportName)
                .HasMaxLength(150)
                .HasColumnName("airport_name");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.IataCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("iata_code");
            entity.Property(e => e.IcaoCode)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("icao_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("longitude");
            entity.Property(e => e.TimeZone)
                .HasMaxLength(50)
                .HasDefaultValueSql("'UTC'::character varying")
                .HasColumnName("time_zone");

            entity.HasOne(d => d.City).WithMany(p => p.Airports)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_airport_city");
        });

        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.HasKey(e => e.AppRoleId).HasName("app_role_pkey");

            entity.ToTable("app_role");

            entity.HasIndex(e => e.RoleName, "uq_app_role_name").IsUnique();

            entity.Property(e => e.AppRoleId).HasColumnName("app_role_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(40)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("app_user_pkey");

            entity.ToTable("app_user");

            entity.HasIndex(e => e.AppRoleId, "idx_user_role");

            entity.HasIndex(e => e.EmployeeId, "uq_user_employee").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AppRoleId).HasColumnName("app_role_id");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at_utc");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginUtc).HasColumnName("last_login_utc");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.AppRole).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.AppRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role");

            entity.HasOne(d => d.Employee).WithOne(p => p.AppUser)
                .HasForeignKey<AppUser>(d => d.EmployeeId)
                .HasConstraintName("fk_user_employee");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("audit_log_pkey");

            entity.ToTable("audit_log");

            entity.HasIndex(e => e.CreatedAtUtc, "idx_audit_created");

            entity.HasIndex(e => new { e.TableName, e.RecordId }, "idx_audit_table_record");

            entity.Property(e => e.AuditId).HasColumnName("audit_id");
            entity.Property(e => e.Action)
                .HasMaxLength(10)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at_utc");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.NewValues)
                .HasColumnType("jsonb")
                .HasColumnName("new_values");
            entity.Property(e => e.OldValues)
                .HasColumnType("jsonb")
                .HasColumnName("old_values");
            entity.Property(e => e.RecordId)
                .HasMaxLength(50)
                .HasColumnName("record_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(80)
                .HasColumnName("table_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_audit_user");
        });

        modelBuilder.Entity<Baggage>(entity =>
        {
            entity.HasKey(e => e.BaggageId).HasName("baggage_pkey");

            entity.ToTable("baggage");

            entity.HasIndex(e => e.BaggageStatusId, "idx_baggage_status");

            entity.HasIndex(e => e.TicketId, "idx_baggage_ticket");

            entity.HasIndex(e => e.TagNumber, "uq_baggage_tag").IsUnique();

            entity.Property(e => e.BaggageId).HasColumnName("baggage_id");
            entity.Property(e => e.BaggageStatusId).HasColumnName("baggage_status_id");
            entity.Property(e => e.BaggageTypeId).HasColumnName("baggage_type_id");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at_utc");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.TagNumber)
                .HasMaxLength(20)
                .HasColumnName("tag_number");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.WeightKg)
                .HasPrecision(5, 2)
                .HasColumnName("weight_kg");

            entity.HasOne(d => d.BaggageStatus).WithMany(p => p.Baggages)
                .HasForeignKey(d => d.BaggageStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_baggage_status");

            entity.HasOne(d => d.BaggageType).WithMany(p => p.Baggages)
                .HasForeignKey(d => d.BaggageTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_baggage_type");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Baggages)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_baggage_ticket");
        });

        modelBuilder.Entity<BaggageStatus>(entity =>
        {
            entity.HasKey(e => e.BaggageStatusId).HasName("baggage_status_pkey");

            entity.ToTable("baggage_status");

            entity.HasIndex(e => e.StatusName, "uq_baggage_status_name").IsUnique();

            entity.Property(e => e.BaggageStatusId).HasColumnName("baggage_status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(40)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<BaggageTracking>(entity =>
        {
            entity.HasKey(e => e.TrackingId).HasName("baggage_tracking_pkey");

            entity.ToTable("baggage_tracking");

            entity.HasIndex(e => new { e.BaggageId, e.EventTimeUtc }, "idx_tracking_baggage_time");

            entity.Property(e => e.TrackingId).HasColumnName("tracking_id");
            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.BaggageId).HasColumnName("baggage_id");
            entity.Property(e => e.BaggageStatusId).HasColumnName("baggage_status_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EventTimeUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("event_time_utc");
            entity.Property(e => e.LocationDescription)
                .HasMaxLength(100)
                .HasColumnName("location_description");

            entity.HasOne(d => d.Airport).WithMany(p => p.BaggageTrackings)
                .HasForeignKey(d => d.AirportId)
                .HasConstraintName("fk_tracking_airport");

            entity.HasOne(d => d.Baggage).WithMany(p => p.BaggageTrackings)
                .HasForeignKey(d => d.BaggageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tracking_baggage");

            entity.HasOne(d => d.BaggageStatus).WithMany(p => p.BaggageTrackings)
                .HasForeignKey(d => d.BaggageStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tracking_status");

            entity.HasOne(d => d.Employee).WithMany(p => p.BaggageTrackings)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_tracking_employee");
        });

        modelBuilder.Entity<BaggageType>(entity =>
        {
            entity.HasKey(e => e.BaggageTypeId).HasName("baggage_type_pkey");

            entity.ToTable("baggage_type");

            entity.HasIndex(e => e.TypeName, "uq_baggage_type_name").IsUnique();

            entity.Property(e => e.BaggageTypeId).HasColumnName("baggage_type_id");
            entity.Property(e => e.MaxWeightKg)
                .HasPrecision(5, 2)
                .HasColumnName("max_weight_kg");
            entity.Property(e => e.TypeName)
                .HasMaxLength(40)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Boarding>(entity =>
        {
            entity.HasKey(e => e.BoardingId).HasName("boarding_pkey");

            entity.ToTable("boarding");

            entity.HasIndex(e => e.GateId, "idx_boarding_gate");

            entity.HasIndex(e => e.TicketId, "uq_boarding_ticket").IsUnique();

            entity.Property(e => e.BoardingId).HasColumnName("boarding_id");
            entity.Property(e => e.BoardedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("boarded_at_utc");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.GateId).HasColumnName("gate_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.Boardings)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_boarding_employee");

            entity.HasOne(d => d.Gate).WithMany(p => p.Boardings)
                .HasForeignKey(d => d.GateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_boarding_gate");

            entity.HasOne(d => d.Ticket).WithOne(p => p.Boarding)
                .HasForeignKey<Boarding>(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_boarding_ticket");
        });

        modelBuilder.Entity<BoardingGate>(entity =>
        {
            entity.HasKey(e => e.GateId).HasName("boarding_gate_pkey");

            entity.ToTable("boarding_gate");

            entity.HasIndex(e => new { e.TerminalId, e.GateCode }, "uq_gate_terminal_code").IsUnique();

            entity.Property(e => e.GateId).HasColumnName("gate_id");
            entity.Property(e => e.GateCode)
                .HasMaxLength(10)
                .HasColumnName("gate_code");
            entity.Property(e => e.GateType)
                .HasMaxLength(15)
                .HasDefaultValueSql("'MIXED'::character varying")
                .HasColumnName("gate_type");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TerminalId).HasColumnName("terminal_id");

            entity.HasOne(d => d.Terminal).WithMany(p => p.BoardingGates)
                .HasForeignKey(d => d.TerminalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gate_terminal");
        });

        modelBuilder.Entity<BoardingPass>(entity =>
        {
            entity.HasKey(e => e.BoardingPassId).HasName("boarding_pass_pkey");

            entity.ToTable("boarding_pass");

            entity.HasIndex(e => e.Barcode, "uq_bpass_barcode").IsUnique();

            entity.HasIndex(e => e.TicketId, "uq_bpass_ticket").IsUnique();

            entity.Property(e => e.BoardingPassId).HasColumnName("boarding_pass_id");
            entity.Property(e => e.Barcode)
                .HasMaxLength(100)
                .HasColumnName("barcode");
            entity.Property(e => e.BoardingGroup)
                .HasMaxLength(5)
                .HasColumnName("boarding_group");
            entity.Property(e => e.IssuedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("issued_at_utc");
            entity.Property(e => e.SequenceNumber).HasColumnName("sequence_number");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Ticket).WithOne(p => p.BoardingPass)
                .HasForeignKey<BoardingPass>(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bpass_ticket");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("booking_pkey");

            entity.ToTable("booking");

            entity.HasIndex(e => e.BookingDateUtc, "idx_booking_date");

            entity.HasIndex(e => e.BookingStatusId, "idx_booking_status");

            entity.HasIndex(e => e.BookingCode, "uq_booking_code").IsUnique();

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingCode)
                .HasMaxLength(20)
                .HasColumnName("booking_code");
            entity.Property(e => e.BookingDateUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("booking_date_utc");
            entity.Property(e => e.BookingStatusId).HasColumnName("booking_status_id");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(150)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(25)
                .HasColumnName("contact_phone");
            entity.Property(e => e.ExpirationDateUtc).HasColumnName("expiration_date_utc");

            entity.HasOne(d => d.BookingStatus).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BookingStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_booking_status");
        });

        modelBuilder.Entity<BookingFlightPassenger>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.FlightId, e.PassengerId }).HasName("booking_flight_passenger_pkey");

            entity.ToTable("booking_flight_passenger");

            entity.HasIndex(e => e.FlightId, "idx_bfp_flight");

            entity.HasIndex(e => e.PassengerId, "idx_bfp_passenger");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingFlightPassengers)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bfp_booking");

            entity.HasOne(d => d.Flight).WithMany(p => p.BookingFlightPassengers)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bfp_flight");

            entity.HasOne(d => d.Passenger).WithMany(p => p.BookingFlightPassengers)
                .HasForeignKey(d => d.PassengerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bfp_passenger");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.BookingStatusId).HasName("booking_status_pkey");

            entity.ToTable("booking_status");

            entity.HasIndex(e => e.StatusName, "uq_booking_status_name").IsUnique();

            entity.Property(e => e.BookingStatusId).HasColumnName("booking_status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(40)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<CabinClass>(entity =>
        {
            entity.HasKey(e => e.CabinClassId).HasName("cabin_class_pkey");

            entity.ToTable("cabin_class");

            entity.HasIndex(e => e.ClassCode, "uq_cabin_class_code").IsUnique();

            entity.HasIndex(e => e.ClassName, "uq_cabin_class_name").IsUnique();

            entity.Property(e => e.CabinClassId).HasColumnName("cabin_class_id");
            entity.Property(e => e.ClassCode)
                .HasMaxLength(2)
                .HasColumnName("class_code");
            entity.Property(e => e.ClassName)
                .HasMaxLength(40)
                .HasColumnName("class_name");
        });

        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.HasKey(e => e.CheckInId).HasName("check_in_pkey");

            entity.ToTable("check_in");

            entity.HasIndex(e => e.CounterId, "idx_checkin_counter");

            entity.HasIndex(e => e.EmployeeId, "idx_checkin_employee");

            entity.HasIndex(e => e.TicketId, "uq_checkin_ticket").IsUnique();

            entity.Property(e => e.CheckInId).HasColumnName("check_in_id");
            entity.Property(e => e.Channel)
                .HasMaxLength(10)
                .HasDefaultValueSql("'COUNTER'::character varying")
                .HasColumnName("channel");
            entity.Property(e => e.CheckInTimeUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("check_in_time_utc");
            entity.Property(e => e.CounterId).HasColumnName("counter_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Counter).WithMany(p => p.CheckIns)
                .HasForeignKey(d => d.CounterId)
                .HasConstraintName("fk_checkin_counter");

            entity.HasOne(d => d.Employee).WithMany(p => p.CheckIns)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("fk_checkin_employee");

            entity.HasOne(d => d.Ticket).WithOne(p => p.CheckIn)
                .HasForeignKey<CheckIn>(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_checkin_ticket");
        });

        modelBuilder.Entity<CheckInCounter>(entity =>
        {
            entity.HasKey(e => e.CounterId).HasName("check_in_counter_pkey");

            entity.ToTable("check_in_counter");

            entity.HasIndex(e => new { e.TerminalId, e.CounterCode }, "uq_counter_terminal_code").IsUnique();

            entity.Property(e => e.CounterId).HasColumnName("counter_id");
            entity.Property(e => e.CounterCode)
                .HasMaxLength(10)
                .HasColumnName("counter_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TerminalId).HasColumnName("terminal_id");

            entity.HasOne(d => d.Terminal).WithMany(p => p.CheckInCounters)
                .HasForeignKey(d => d.TerminalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_counter_terminal");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("city_pkey");

            entity.ToTable("city");

            entity.HasIndex(e => new { e.CountryId, e.CityName }, "uq_city_country_name").IsUnique();

            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CityName)
                .HasMaxLength(120)
                .HasColumnName("city_name");
            entity.Property(e => e.CountryId).HasColumnName("country_id");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_city_country");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("country_pkey");

            entity.ToTable("country");

            entity.HasIndex(e => e.Iso2Code, "uq_country_iso2").IsUnique();

            entity.HasIndex(e => e.Iso3Code, "uq_country_iso3").IsUnique();

            entity.HasIndex(e => e.CountryName, "uq_country_name").IsUnique();

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CountryName)
                .HasMaxLength(100)
                .HasColumnName("country_name");
            entity.Property(e => e.Iso2Code)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("iso2_code");
            entity.Property(e => e.Iso3Code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("iso3_code");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyCode).HasName("currency_pkey");

            entity.ToTable("currency");

            entity.HasIndex(e => e.CurrencyName, "uq_currency_name").IsUnique();

            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("currency_code");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(60)
                .HasColumnName("currency_name");
            entity.Property(e => e.Symbol)
                .HasMaxLength(5)
                .HasColumnName("symbol");
        });

        modelBuilder.Entity<DelayReason>(entity =>
        {
            entity.HasKey(e => e.DelayReasonId).HasName("delay_reason_pkey");

            entity.ToTable("delay_reason");

            entity.HasIndex(e => e.ReasonCode, "uq_delay_reason_code").IsUnique();

            entity.Property(e => e.DelayReasonId).HasColumnName("delay_reason_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.ReasonCode)
                .HasMaxLength(10)
                .HasColumnName("reason_code");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.HasKey(e => e.DocumentTypeId).HasName("document_type_pkey");

            entity.ToTable("document_type");

            entity.HasIndex(e => e.Abbreviation, "uq_document_type_abbreviation").IsUnique();

            entity.HasIndex(e => e.TypeName, "uq_document_type_name").IsUnique();

            entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(10)
                .HasColumnName("abbreviation");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.HasIndex(e => e.AirportId, "idx_employee_airport");

            entity.HasIndex(e => e.LastName, "idx_employee_last_name");

            entity.HasIndex(e => e.EmployeeRoleId, "idx_employee_role");

            entity.HasIndex(e => new { e.DocumentTypeId, e.DocumentNumber }, "uq_employee_document").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(30)
                .HasColumnName("document_number");
            entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeRoleId).HasColumnName("employee_role_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(25)
                .HasColumnName("phone");

            entity.HasOne(d => d.Airport).WithMany(p => p.Employees)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_airport");

            entity.HasOne(d => d.DocumentType).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_document_type");

            entity.HasOne(d => d.EmployeeRole).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_role");
        });

        modelBuilder.Entity<EmployeeLicense>(entity =>
        {
            entity.HasKey(e => e.LicenseId).HasName("employee_license_pkey");

            entity.ToTable("employee_license");

            entity.HasIndex(e => e.EmployeeId, "idx_license_employee");

            entity.HasIndex(e => e.ExpiryDate, "idx_license_expiry");

            entity.HasIndex(e => new { e.LicenseType, e.LicenseNumber }, "uq_license_type_number").IsUnique();

            entity.Property(e => e.LicenseId).HasColumnName("license_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.IssueDate).HasColumnName("issue_date");
            entity.Property(e => e.IssuingCountryId).HasColumnName("issuing_country_id");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(50)
                .HasColumnName("license_number");
            entity.Property(e => e.LicenseType)
                .HasMaxLength(60)
                .HasColumnName("license_type");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLicenses)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_license_employee");

            entity.HasOne(d => d.IssuingCountry).WithMany(p => p.EmployeeLicenses)
                .HasForeignKey(d => d.IssuingCountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_license_country");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmployeeRoleId).HasName("employee_role_pkey");

            entity.ToTable("employee_role");

            entity.HasIndex(e => e.RoleName, "uq_employee_role_name").IsUnique();

            entity.Property(e => e.EmployeeRoleId).HasColumnName("employee_role_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(60)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Fare>(entity =>
        {
            entity.HasKey(e => e.FareId).HasName("fare_pkey");

            entity.ToTable("fare");

            entity.HasIndex(e => e.RouteId, "idx_fare_route");

            entity.HasIndex(e => new { e.AirlineId, e.RouteId, e.CabinClassId, e.ValidFrom }, "uq_fare_airline_route_class_from").IsUnique();

            entity.Property(e => e.FareId).HasColumnName("fare_id");
            entity.Property(e => e.AirlineId).HasColumnName("airline_id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(12, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.CabinClassId).HasColumnName("cabin_class_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("currency_code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");

            entity.HasOne(d => d.Airline).WithMany(p => p.Fares)
                .HasForeignKey(d => d.AirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fare_airline");

            entity.HasOne(d => d.CabinClass).WithMany(p => p.Fares)
                .HasForeignKey(d => d.CabinClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fare_cabin_class");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Fares)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fare_currency");

            entity.HasOne(d => d.Route).WithMany(p => p.Fares)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fare_route");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("flight_pkey");

            entity.ToTable("flight");

            entity.HasIndex(e => e.AircraftId, "idx_flight_aircraft");

            entity.HasIndex(e => e.ScheduledArrivalUtc, "idx_flight_arrival");

            entity.HasIndex(e => e.ArrivalGateId, "idx_flight_arrival_gate");

            entity.HasIndex(e => e.ScheduledDepartureUtc, "idx_flight_departure");

            entity.HasIndex(e => e.DepartureGateId, "idx_flight_departure_gate");

            entity.HasIndex(e => e.RouteId, "idx_flight_route");

            entity.HasIndex(e => e.FlightStatusId, "idx_flight_status");

            entity.HasIndex(e => new { e.AirlineId, e.FlightNumber, e.ScheduledDepartureUtc }, "uq_flight_airline_number_departure").IsUnique();

            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.ActualArrivalUtc).HasColumnName("actual_arrival_utc");
            entity.Property(e => e.ActualDepartureUtc).HasColumnName("actual_departure_utc");
            entity.Property(e => e.AircraftId).HasColumnName("aircraft_id");
            entity.Property(e => e.AirlineId).HasColumnName("airline_id");
            entity.Property(e => e.ArrivalGateId).HasColumnName("arrival_gate_id");
            entity.Property(e => e.DepartureGateId).HasColumnName("departure_gate_id");
            entity.Property(e => e.FlightNumber)
                .HasMaxLength(15)
                .HasColumnName("flight_number");
            entity.Property(e => e.FlightStatusId).HasColumnName("flight_status_id");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.ScheduledArrivalUtc).HasColumnName("scheduled_arrival_utc");
            entity.Property(e => e.ScheduledDepartureUtc).HasColumnName("scheduled_departure_utc");

            entity.HasOne(d => d.Airline).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_flight_airline");

            entity.HasOne(d => d.ArrivalGate).WithMany(p => p.FlightArrivalGates)
                .HasForeignKey(d => d.ArrivalGateId)
                .HasConstraintName("fk_flight_arrival_gate");

            entity.HasOne(d => d.DepartureGate).WithMany(p => p.FlightDepartureGates)
                .HasForeignKey(d => d.DepartureGateId)
                .HasConstraintName("fk_flight_departure_gate");

            entity.HasOne(d => d.FlightStatus).WithMany(p => p.Flights)
                .HasForeignKey(d => d.FlightStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_flight_status");

            entity.HasOne(d => d.Route).WithMany(p => p.Flights)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_flight_route");

            entity.HasOne(d => d.Aircraft).WithMany(p => p.Flights)
                .HasPrincipalKey(p => new { p.AircraftId, p.AirlineId })
                .HasForeignKey(d => new { d.AircraftId, d.AirlineId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_flight_aircraft_airline");
        });

        modelBuilder.Entity<FlightCrewAssignment>(entity =>
        {
            entity.HasKey(e => new { e.FlightId, e.EmployeeId }).HasName("flight_crew_assignment_pkey");

            entity.ToTable("flight_crew_assignment");

            entity.HasIndex(e => e.EmployeeId, "idx_crew_employee");

            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.AssignedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("assigned_at_utc");
            entity.Property(e => e.CrewPosition)
                .HasMaxLength(60)
                .HasColumnName("crew_position");

            entity.HasOne(d => d.Employee).WithMany(p => p.FlightCrewAssignments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_crew_employee");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightCrewAssignments)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_crew_flight");
        });

        modelBuilder.Entity<FlightDelay>(entity =>
        {
            entity.HasKey(e => e.FlightDelayId).HasName("flight_delay_pkey");

            entity.ToTable("flight_delay");

            entity.HasIndex(e => e.FlightId, "idx_delay_flight");

            entity.HasIndex(e => e.DelayReasonId, "idx_delay_reason");

            entity.Property(e => e.FlightDelayId).HasColumnName("flight_delay_id");
            entity.Property(e => e.DelayMinutes).HasColumnName("delay_minutes");
            entity.Property(e => e.DelayReasonId).HasColumnName("delay_reason_id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.RegisteredAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("registered_at_utc");
            entity.Property(e => e.RegisteredByEmployeeId).HasColumnName("registered_by_employee_id");

            entity.HasOne(d => d.DelayReason).WithMany(p => p.FlightDelays)
                .HasForeignKey(d => d.DelayReasonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_delay_reason");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightDelays)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_delay_flight");

            entity.HasOne(d => d.RegisteredByEmployee).WithMany(p => p.FlightDelays)
                .HasForeignKey(d => d.RegisteredByEmployeeId)
                .HasConstraintName("fk_delay_employee");
        });

        modelBuilder.Entity<FlightRoute>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("flight_route_pkey");

            entity.ToTable("flight_route");

            entity.HasIndex(e => e.DestinationAirportId, "idx_route_destination");

            entity.HasIndex(e => new { e.OriginAirportId, e.DestinationAirportId }, "uq_route_origin_destination").IsUnique();

            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.DestinationAirportId).HasColumnName("destination_airport_id");
            entity.Property(e => e.DistanceKm)
                .HasPrecision(10, 2)
                .HasColumnName("distance_km");
            entity.Property(e => e.EstimatedDurationMinutes).HasColumnName("estimated_duration_minutes");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.OriginAirportId).HasColumnName("origin_airport_id");

            entity.HasOne(d => d.DestinationAirport).WithMany(p => p.FlightRouteDestinationAirports)
                .HasForeignKey(d => d.DestinationAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_route_destination");

            entity.HasOne(d => d.OriginAirport).WithMany(p => p.FlightRouteOriginAirports)
                .HasForeignKey(d => d.OriginAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_route_origin");
        });

        modelBuilder.Entity<FlightRunwayUsage>(entity =>
        {
            entity.HasKey(e => e.UsageId).HasName("flight_runway_usage_pkey");

            entity.ToTable("flight_runway_usage");

            entity.HasIndex(e => new { e.RunwayId, e.UsedAtUtc }, "idx_fru_runway");

            entity.HasIndex(e => new { e.FlightId, e.Operation }, "uq_fru_flight_operation").IsUnique();

            entity.Property(e => e.UsageId).HasColumnName("usage_id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.Operation)
                .HasMaxLength(10)
                .HasColumnName("operation");
            entity.Property(e => e.RunwayId).HasColumnName("runway_id");
            entity.Property(e => e.UsedAtUtc).HasColumnName("used_at_utc");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightRunwayUsages)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fru_flight");

            entity.HasOne(d => d.Runway).WithMany(p => p.FlightRunwayUsages)
                .HasForeignKey(d => d.RunwayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fru_runway");
        });

        modelBuilder.Entity<FlightStandAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("flight_stand_assignment_pkey");

            entity.ToTable("flight_stand_assignment");

            entity.HasIndex(e => e.FlightId, "idx_fsa_flight");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AssignedFromUtc).HasColumnName("assigned_from_utc");
            entity.Property(e => e.AssignedToUtc).HasColumnName("assigned_to_utc");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.StandId).HasColumnName("stand_id");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightStandAssignments)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fsa_flight");

            entity.HasOne(d => d.Stand).WithMany(p => p.FlightStandAssignments)
                .HasForeignKey(d => d.StandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fsa_stand");
        });

        modelBuilder.Entity<FlightStatus>(entity =>
        {
            entity.HasKey(e => e.FlightStatusId).HasName("flight_status_pkey");

            entity.ToTable("flight_status");

            entity.HasIndex(e => e.StatusName, "uq_flight_status_name").IsUnique();

            entity.Property(e => e.FlightStatusId).HasColumnName("flight_status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(40)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<FlightStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("flight_status_history_pkey");

            entity.ToTable("flight_status_history");

            entity.HasIndex(e => new { e.FlightId, e.ChangedAtUtc }, "idx_fsh_flight_time");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.ChangedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("changed_at_utc");
            entity.Property(e => e.ChangedByEmployeeId).HasColumnName("changed_by_employee_id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.FlightStatusId).HasColumnName("flight_status_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");

            entity.HasOne(d => d.ChangedByEmployee).WithMany(p => p.FlightStatusHistories)
                .HasForeignKey(d => d.ChangedByEmployeeId)
                .HasConstraintName("fk_fsh_employee");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightStatusHistories)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fsh_flight");

            entity.HasOne(d => d.FlightStatus).WithMany(p => p.FlightStatusHistories)
                .HasForeignKey(d => d.FlightStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fsh_status");
        });

        modelBuilder.Entity<MaintenanceType>(entity =>
        {
            entity.HasKey(e => e.MaintenanceTypeId).HasName("maintenance_type_pkey");

            entity.ToTable("maintenance_type");

            entity.HasIndex(e => e.TypeName, "uq_maintenance_type_name").IsUnique();

            entity.Property(e => e.MaintenanceTypeId).HasColumnName("maintenance_type_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.TypeName)
                .HasMaxLength(60)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.PassengerId).HasName("passenger_pkey");

            entity.ToTable("passenger");

            entity.HasIndex(e => e.LastName, "idx_passenger_last_name");

            entity.HasIndex(e => e.NationalityCountryId, "idx_passenger_nationality");

            entity.HasIndex(e => new { e.DocumentTypeId, e.DocumentNumber }, "uq_passenger_document").IsUnique();

            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(30)
                .HasColumnName("document_number");
            entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.NationalityCountryId).HasColumnName("nationality_country_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(25)
                .HasColumnName("phone");

            entity.HasOne(d => d.DocumentType).WithMany(p => p.Passengers)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_passenger_document_type");

            entity.HasOne(d => d.NationalityCountry).WithMany(p => p.Passengers)
                .HasForeignKey(d => d.NationalityCountryId)
                .HasConstraintName("fk_passenger_nationality");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payment_pkey");

            entity.ToTable("payment");

            entity.HasIndex(e => e.BookingId, "idx_payment_booking");

            entity.HasIndex(e => e.PaymentStatusId, "idx_payment_status");

            entity.HasIndex(e => e.TransactionReference, "uq_payment_transaction").IsUnique();

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at_utc");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("currency_code");
            entity.Property(e => e.PaidAtUtc).HasColumnName("paid_at_utc");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.PaymentStatusId).HasColumnName("payment_status_id");
            entity.Property(e => e.TransactionReference)
                .HasMaxLength(100)
                .HasColumnName("transaction_reference");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_booking");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_currency");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_method");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_payment_status");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("payment_method_pkey");

            entity.ToTable("payment_method");

            entity.HasIndex(e => e.MethodName, "uq_payment_method_name").IsUnique();

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.MethodName)
                .HasMaxLength(40)
                .HasColumnName("method_name");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentStatusId).HasName("payment_status_pkey");

            entity.ToTable("payment_status");

            entity.HasIndex(e => e.StatusName, "uq_payment_status_name").IsUnique();

            entity.Property(e => e.PaymentStatusId).HasColumnName("payment_status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(40)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("refund_pkey");

            entity.ToTable("refund");

            entity.HasIndex(e => e.PaymentId, "idx_refund_payment");

            entity.HasIndex(e => e.TicketId, "idx_refund_ticket");

            entity.Property(e => e.RefundId).HasColumnName("refund_id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.ProcessedByEmployeeId).HasColumnName("processed_by_employee_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.RefundDateUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("refund_date_utc");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_refund_payment");

            entity.HasOne(d => d.ProcessedByEmployee).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.ProcessedByEmployeeId)
                .HasConstraintName("fk_refund_employee");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("fk_refund_ticket");
        });

        modelBuilder.Entity<Runway>(entity =>
        {
            entity.HasKey(e => e.RunwayId).HasName("runway_pkey");

            entity.ToTable("runway");

            entity.HasIndex(e => new { e.AirportId, e.RunwayCode }, "uq_runway_airport_code").IsUnique();

            entity.Property(e => e.RunwayId).HasColumnName("runway_id");
            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LengthM).HasColumnName("length_m");
            entity.Property(e => e.RunwayCode)
                .HasMaxLength(10)
                .HasColumnName("runway_code");
            entity.Property(e => e.SurfaceType)
                .HasMaxLength(30)
                .HasColumnName("surface_type");

            entity.HasOne(d => d.Airport).WithMany(p => p.Runways)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_runway_airport");
        });

        modelBuilder.Entity<Terminal>(entity =>
        {
            entity.HasKey(e => e.TerminalId).HasName("terminal_pkey");

            entity.ToTable("terminal");

            entity.HasIndex(e => new { e.AirportId, e.TerminalCode }, "uq_terminal_airport_code").IsUnique();

            entity.Property(e => e.TerminalId).HasColumnName("terminal_id");
            entity.Property(e => e.AirportId).HasColumnName("airport_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TerminalCode)
                .HasMaxLength(10)
                .HasColumnName("terminal_code");
            entity.Property(e => e.TerminalName)
                .HasMaxLength(100)
                .HasColumnName("terminal_name");

            entity.HasOne(d => d.Airport).WithMany(p => p.Terminals)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_terminal_airport");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("ticket_pkey");

            entity.ToTable("ticket");

            entity.HasIndex(e => e.BookingId, "idx_ticket_booking");

            entity.HasIndex(e => e.CabinClassId, "idx_ticket_cabin");

            entity.HasIndex(e => e.PassengerId, "idx_ticket_passenger");

            entity.HasIndex(e => e.TicketStatusId, "idx_ticket_status");

            entity.HasIndex(e => e.TicketNumber, "uq_ticket_number").IsUnique();

            entity.HasIndex(e => new { e.FlightId, e.PassengerId }, "uq_ticket_passenger_flight").IsUnique();

            entity.HasIndex(e => new { e.FlightId, e.SeatNumber }, "uq_ticket_seat_flight").IsUnique();

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CabinClassId).HasColumnName("cabin_class_id");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValueSql("'PEN'::bpchar")
                .IsFixedLength()
                .HasColumnName("currency_code");
            entity.Property(e => e.FareId).HasColumnName("fare_id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.IssueDateUtc)
                .HasDefaultValueSql("now()")
                .HasColumnName("issue_date_utc");
            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(10)
                .HasColumnName("seat_number");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(12, 2)
                .HasColumnName("tax_amount");
            entity.Property(e => e.TicketNumber)
                .HasMaxLength(30)
                .HasColumnName("ticket_number");
            entity.Property(e => e.TicketStatusId).HasColumnName("ticket_status_id");

            entity.HasOne(d => d.CabinClass).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CabinClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_cabin_class");

            entity.HasOne(d => d.CurrencyCodeNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_currency");

            entity.HasOne(d => d.Fare).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FareId)
                .HasConstraintName("fk_ticket_fare");

            entity.HasOne(d => d.TicketStatus).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TicketStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_status");

            entity.HasOne(d => d.BookingFlightPassenger).WithMany(p => p.Tickets)
                .HasForeignKey(d => new { d.BookingId, d.FlightId, d.PassengerId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_booking_flight_passenger");
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.TicketStatusId).HasName("ticket_status_pkey");

            entity.ToTable("ticket_status");

            entity.HasIndex(e => e.StatusName, "uq_ticket_status_name").IsUnique();

            entity.Property(e => e.TicketStatusId).HasColumnName("ticket_status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(40)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<VFlightBoard>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_flight_board");

            entity.Property(e => e.ActualArrivalUtc).HasColumnName("actual_arrival_utc");
            entity.Property(e => e.ActualDepartureUtc).HasColumnName("actual_departure_utc");
            entity.Property(e => e.AirlineName)
                .HasMaxLength(120)
                .HasColumnName("airline_name");
            entity.Property(e => e.ArrivalGate)
                .HasMaxLength(10)
                .HasColumnName("arrival_gate");
            entity.Property(e => e.DepartureGate)
                .HasMaxLength(10)
                .HasColumnName("departure_gate");
            entity.Property(e => e.DestinationIata)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("destination_iata");
            entity.Property(e => e.FlightCode).HasColumnName("flight_code");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.FlightStatus)
                .HasMaxLength(40)
                .HasColumnName("flight_status");
            entity.Property(e => e.OriginIata)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("origin_iata");
            entity.Property(e => e.ScheduledArrivalUtc).HasColumnName("scheduled_arrival_utc");
            entity.Property(e => e.ScheduledDepartureUtc).HasColumnName("scheduled_departure_utc");
        });

        modelBuilder.Entity<VFlightOccupancy>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_flight_occupancy");

            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.OccupancyPercent).HasColumnName("occupancy_percent");
            entity.Property(e => e.RegistrationCode)
                .HasMaxLength(20)
                .HasColumnName("registration_code");
            entity.Property(e => e.SeatCapacity).HasColumnName("seat_capacity");
            entity.Property(e => e.TicketsSold).HasColumnName("tickets_sold");
        });

        modelBuilder.Entity<VLicensesExpiring>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_licenses_expiring");

            entity.Property(e => e.DaysLeft).HasColumnName("days_left");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeName).HasColumnName("employee_name");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.LicenseNumber)
                .HasMaxLength(50)
                .HasColumnName("license_number");
            entity.Property(e => e.LicenseType)
                .HasMaxLength(60)
                .HasColumnName("license_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


