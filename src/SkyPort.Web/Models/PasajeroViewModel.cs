using System.ComponentModel.DataAnnotations;
using SkyPort.Application.Dtos.Pasajeros;

namespace SkyPort.Web.Models;

public sealed class PasajeroViewModel
{
    public int PasajeroId { get; set; }

    [Required(ErrorMessage = "Selecciona el tipo de documento.")]
    [RegularExpression("^(DNI|Pasaporte|CE)$", ErrorMessage = "Selecciona DNI, Pasaporte o CE.")]
    [Display(Name = "Tipo de documento")]
    public string TipoDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa el número de documento.")]
    [StringLength(12, MinimumLength = 6, ErrorMessage = "El número debe tener entre 6 y 12 caracteres.")]
    [Display(Name = "Número de documento")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa los nombres.")]
    [StringLength(80)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa los apellidos.")]
    [StringLength(80)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa la fecha de nacimiento.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateOnly FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Ingresa la nacionalidad.")]
    [StringLength(60)]
    [Display(Name = "Nacionalidad")]
    public string Nacionalidad { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [StringLength(120)]
    [Display(Name = "Correo electrónico")]
    public string? Email { get; set; }

    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    public static PasajeroViewModel FromDto(PasajeroDto pasajero) => new()
    {
        PasajeroId = pasajero.PasajeroId,
        TipoDocumento = pasajero.TipoDocumento,
        NumeroDocumento = pasajero.NumeroDocumento,
        Nombres = pasajero.Nombres,
        Apellidos = pasajero.Apellidos,
        FechaNacimiento = pasajero.FechaNacimiento,
        Nacionalidad = pasajero.Nacionalidad,
        Email = pasajero.Email,
        Telefono = pasajero.Telefono
    };
}
