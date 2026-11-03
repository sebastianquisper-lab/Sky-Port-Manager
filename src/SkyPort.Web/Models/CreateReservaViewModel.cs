using System.ComponentModel.DataAnnotations;

namespace SkyPort.Web.Models;

public sealed class CreateReservaViewModel
{
    public int VueloId { get; set; }

    public string NumeroVuelo { get; set; } = string.Empty;

    public string Ruta { get; set; } = string.Empty;

    public DateTime FechaSalida { get; set; }

    public string EstadoVuelo { get; set; } = string.Empty;

    public int Capacidad { get; set; }

    public int ReservasConfirmadas { get; set; }

    public int AsientosLibres { get; set; }

    public IReadOnlyList<string> AsientosOcupados { get; set; } = Array.Empty<string>();

    [Required(ErrorMessage = "Busca y selecciona un pasajero antes de continuar.")]
    public int? PasajeroId { get; set; }

    public string? NombrePasajero { get; set; }

    public bool PasajeroNoEncontrado { get; set; }

    [Display(Name = "Tipo de documento")]
    public string? TipoDocumento { get; set; }

    [Display(Name = "Número de documento")]
    public string? NumeroDocumento { get; set; }

    [Required(ErrorMessage = "Selecciona un asiento disponible.")]
    public string? Asiento { get; set; }
}
