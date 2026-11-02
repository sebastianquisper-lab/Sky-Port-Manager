using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SkyPort.Domain.Entities;

namespace SkyPort.Persistence.Context;

public partial class SkyPortContext : DbContext
{
    public SkyPortContext(DbContextOptions<SkyPortContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aerolinea> Aerolinea { get; set; }

    public virtual DbSet<Aeronave> Aeronave { get; set; }

    public virtual DbSet<Boleto> Boleto { get; set; }

    public virtual DbSet<Checkin> Checkin { get; set; }

    public virtual DbSet<Embarque> Embarque { get; set; }

    public virtual DbSet<Empleado> Empleado { get; set; }

    public virtual DbSet<Equipaje> Equipaje { get; set; }

    public virtual DbSet<Pasajero> Pasajero { get; set; }

    public virtual DbSet<Puerta> Puerta { get; set; }

    public virtual DbSet<Reserva> Reserva { get; set; }

    public virtual DbSet<Ruta> Ruta { get; set; }

    public virtual DbSet<Terminal> Terminal { get; set; }

    public virtual DbSet<Tripulacionvuelo> Tripulacionvuelo { get; set; }

    public virtual DbSet<Vuelo> Vuelo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Aerolinea>(entity =>
        {
            entity.HasKey(e => e.AerolineaId).HasName("PRIMARY");

            entity.ToTable("aerolinea");

            entity.HasIndex(e => e.CodigoIata, "UQ_Aerolinea_CodigoIata").IsUnique();

            entity.HasIndex(e => e.Nombre, "UQ_Aerolinea_Nombre").IsUnique();

            entity.Property(e => e.Activa)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.CodigoIata).HasMaxLength(2);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Pais).HasMaxLength(60);
        });

        modelBuilder.Entity<Aeronave>(entity =>
        {
            entity.HasKey(e => e.AeronaveId).HasName("PRIMARY");

            entity.ToTable("aeronave");

            entity.HasIndex(e => e.AerolineaId, "IX_Aeronave_AerolineaId");

            entity.HasIndex(e => e.Matricula, "UQ_Aeronave_Matricula").IsUnique();

            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.Matricula).HasMaxLength(20);
            entity.Property(e => e.Modelo).HasMaxLength(60);

            entity.HasOne(d => d.Aerolinea).WithMany(p => p.Aeronave)
                .HasForeignKey(d => d.AerolineaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Aeronave_AerolineaId");
        });

        modelBuilder.Entity<Boleto>(entity =>
        {
            entity.HasKey(e => e.BoletoId).HasName("PRIMARY");

            entity.ToTable("boleto");

            entity.HasIndex(e => e.CodigoBoleto, "UQ_Boleto_CodigoBoleto").IsUnique();

            entity.HasIndex(e => e.ReservaId, "UQ_Boleto_ReservaId").IsUnique();

            entity.Property(e => e.CodigoBoleto).HasMaxLength(14);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Reserva).WithOne(p => p.Boleto)
                .HasForeignKey<Boleto>(d => d.ReservaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Boleto_ReservaId");
        });

        modelBuilder.Entity<Checkin>(entity =>
        {
            entity.HasKey(e => e.CheckInId).HasName("PRIMARY");

            entity.ToTable("checkin");

            entity.HasIndex(e => e.EmpleadoId, "IX_CheckIn_EmpleadoId");

            entity.HasIndex(e => e.BoletoId, "UQ_CheckIn_BoletoId").IsUnique();

            entity.Property(e => e.FechaHora).HasColumnType("datetime");

            entity.HasOne(d => d.Boleto).WithOne(p => p.Checkin)
                .HasForeignKey<Checkin>(d => d.BoletoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CheckIn_BoletoId");

            entity.HasOne(d => d.Empleado).WithMany(p => p.Checkin)
                .HasForeignKey(d => d.EmpleadoId)
                .HasConstraintName("FK_CheckIn_EmpleadoId");
        });

        modelBuilder.Entity<Embarque>(entity =>
        {
            entity.HasKey(e => e.EmbarqueId).HasName("PRIMARY");

            entity.ToTable("embarque");

            entity.HasIndex(e => e.PuertaId, "IX_Embarque_PuertaId");

            entity.HasIndex(e => e.BoletoId, "UQ_Embarque_BoletoId").IsUnique();

            entity.Property(e => e.FechaHora).HasColumnType("datetime");

            entity.HasOne(d => d.Boleto).WithOne(p => p.Embarque)
                .HasForeignKey<Embarque>(d => d.BoletoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Embarque_BoletoId");

            entity.HasOne(d => d.Puerta).WithMany(p => p.Embarque)
                .HasForeignKey(d => d.PuertaId)
                .HasConstraintName("FK_Embarque_PuertaId");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.EmpleadoId).HasName("PRIMARY");

            entity.ToTable("empleado");

            entity.HasIndex(e => e.NumeroDocumento, "UQ_Empleado_NumeroDocumento").IsUnique();

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Apellidos).HasMaxLength(80);
            entity.Property(e => e.Cargo).HasMaxLength(40);
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.Nombres).HasMaxLength(80);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(20);
        });

        modelBuilder.Entity<Equipaje>(entity =>
        {
            entity.HasKey(e => e.EquipajeId).HasName("PRIMARY");

            entity.ToTable("equipaje");

            entity.HasIndex(e => e.BoletoId, "IX_Equipaje_BoletoId");

            entity.HasIndex(e => e.Codigo, "UQ_Equipaje_Codigo").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(14);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.PesoKg).HasPrecision(5, 2);

            entity.HasOne(d => d.Boleto).WithMany(p => p.Equipaje)
                .HasForeignKey(d => d.BoletoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Equipaje_BoletoId");
        });

        modelBuilder.Entity<Pasajero>(entity =>
        {
            entity.HasKey(e => e.PasajeroId).HasName("PRIMARY");

            entity.ToTable("pasajero");

            entity.HasIndex(e => new { e.TipoDocumento, e.NumeroDocumento }, "UQ_Pasajero_TipoDocumento_NumeroDocumento").IsUnique();

            entity.Property(e => e.Apellidos).HasMaxLength(80);
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.Nacionalidad).HasMaxLength(60);
            entity.Property(e => e.Nombres).HasMaxLength(80);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(20);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.TipoDocumento).HasMaxLength(20);
        });

        modelBuilder.Entity<Puerta>(entity =>
        {
            entity.HasKey(e => e.PuertaId).HasName("PRIMARY");

            entity.ToTable("puerta");

            entity.HasIndex(e => new { e.TerminalId, e.Codigo }, "UQ_Puerta_TerminalId_Codigo").IsUnique();

            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.Disponible)
                .IsRequired()
                .HasDefaultValueSql("'1'");

            entity.HasOne(d => d.Terminal).WithMany(p => p.Puerta)
                .HasForeignKey(d => d.TerminalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puerta_TerminalId");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.ReservaId).HasName("PRIMARY");

            entity.ToTable("reserva");

            entity.HasIndex(e => e.PasajeroId, "IX_Reserva_PasajeroId");

            entity.HasIndex(e => e.Codigo, "UQ_Reserva_Codigo").IsUnique();

            entity.HasIndex(e => new { e.VueloId, e.AsientoActivo }, "UQ_Reserva_VueloId_AsientoActivo").IsUnique();

            entity.HasIndex(e => new { e.VueloId, e.PasajeroActivoId }, "UQ_Reserva_VueloId_PasajeroActivoId").IsUnique();

            entity.Property(e => e.Asiento).HasMaxLength(4);
            entity.Property(e => e.AsientoActivo)
                .HasMaxLength(4)
                .HasComputedColumnSql("if((`Estado` = _utf8mb4'Confirmada'),`Asiento`,NULL)", true);
            entity.Property(e => e.Codigo).HasMaxLength(10);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.FechaReserva)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PasajeroActivoId).HasComputedColumnSql("if((`Estado` = _utf8mb4'Confirmada'),`PasajeroId`,NULL)", true);

            entity.HasOne(d => d.Pasajero).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.PasajeroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_PasajeroId");

            entity.HasOne(d => d.Vuelo).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.VueloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reserva_VueloId");
        });

        modelBuilder.Entity<Ruta>(entity =>
        {
            entity.HasKey(e => e.RutaId).HasName("PRIMARY");

            entity.ToTable("ruta");

            entity.HasIndex(e => new { e.CodigoOrigen, e.CodigoDestino }, "UQ_Ruta_CodigoOrigen_CodigoDestino").IsUnique();

            entity.Property(e => e.CiudadDestino).HasMaxLength(80);
            entity.Property(e => e.CiudadOrigen).HasMaxLength(80);
            entity.Property(e => e.CodigoDestino).HasMaxLength(3);
            entity.Property(e => e.CodigoOrigen).HasMaxLength(3);
        });

        modelBuilder.Entity<Terminal>(entity =>
        {
            entity.HasKey(e => e.TerminalId).HasName("PRIMARY");

            entity.ToTable("terminal");

            entity.HasIndex(e => e.Nombre, "UQ_Terminal_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Tripulacionvuelo>(entity =>
        {
            entity.HasKey(e => new { e.VueloId, e.EmpleadoId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("tripulacionvuelo");

            entity.HasIndex(e => e.EmpleadoId, "IX_TripulacionVuelo_EmpleadoId");

            entity.Property(e => e.RolAbordo)
                .HasMaxLength(30)
                .HasColumnName("RolABordo");

            entity.HasOne(d => d.Empleado).WithMany(p => p.Tripulacionvuelo)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripulacionVuelo_EmpleadoId");

            entity.HasOne(d => d.Vuelo).WithMany(p => p.Tripulacionvuelo)
                .HasForeignKey(d => d.VueloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripulacionVuelo_VueloId");
        });

        modelBuilder.Entity<Vuelo>(entity =>
        {
            entity.HasKey(e => e.VueloId).HasName("PRIMARY");

            entity.ToTable("vuelo");

            entity.HasIndex(e => e.AeronaveId, "IX_Vuelo_AeronaveId");

            entity.HasIndex(e => e.FechaSalida, "IX_Vuelo_FechaSalida");

            entity.HasIndex(e => e.PuertaId, "IX_Vuelo_PuertaId");

            entity.HasIndex(e => e.RutaId, "IX_Vuelo_RutaId");

            entity.HasIndex(e => new { e.NumeroVuelo, e.FechaSalida }, "UQ_Vuelo_NumeroVuelo_FechaSalida").IsUnique();

            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.FechaLlegada).HasColumnType("datetime");
            entity.Property(e => e.FechaSalida).HasColumnType("datetime");
            entity.Property(e => e.NumeroVuelo).HasMaxLength(10);

            entity.HasOne(d => d.Aeronave).WithMany(p => p.Vuelo)
                .HasForeignKey(d => d.AeronaveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vuelo_AeronaveId");

            entity.HasOne(d => d.Puerta).WithMany(p => p.Vuelo)
                .HasForeignKey(d => d.PuertaId)
                .HasConstraintName("FK_Vuelo_PuertaId");

            entity.HasOne(d => d.Ruta).WithMany(p => p.Vuelo)
                .HasForeignKey(d => d.RutaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vuelo_RutaId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
