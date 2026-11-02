using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Pasajero
{
    public int PasajeroId { get; set; }

    public string TipoDocumento { get; set; } = null!;

    public string NumeroDocumento { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Nacionalidad { get; set; } = null!;

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();
}
