using System;
using System.Collections.Generic;

namespace SkyPort.Domain.Entities;

public partial class Ruta
{
    public int RutaId { get; set; }

    public string CodigoOrigen { get; set; } = null!;

    public string CiudadOrigen { get; set; } = null!;

    public string CodigoDestino { get; set; } = null!;

    public string CiudadDestino { get; set; } = null!;

    public int DuracionMinutos { get; set; }

    public virtual ICollection<Vuelo> Vuelo { get; set; } = new List<Vuelo>();
}
