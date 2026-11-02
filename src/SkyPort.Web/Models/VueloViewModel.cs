using System.ComponentModel.DataAnnotations;
using SkyPort.Application.UseCases.Vuelos.Dtos;

namespace SkyPort.Web.Models;

public sealed class VueloViewModel : IValidatableObject
{
    public int VueloId { get; set; }

    [Required(ErrorMessage = "Ingresa el número de vuelo.")]
    [RegularExpression("^[A-Za-z]{2}[0-9]{3,4}$", ErrorMessage = "Usa 2 letras y 3 o 4 dígitos.")]
    [Display(Name = "Número de vuelo")]
    public string NumeroVuelo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una ruta.")]
    [Display(Name = "Ruta")]
    public int RutaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una aeronave.")]
    [Display(Name = "Aeronave")]
    public int AeronaveId { get; set; }

    [Display(Name = "Puerta")]
    public int? PuertaId { get; set; }

    [Required(ErrorMessage = "Ingresa la fecha y hora de salida.")]
    [Display(Name = "Salida")]
    public DateTime FechaSalida { get; set; }

    [Required(ErrorMessage = "Ingresa la fecha y hora de llegada.")]
    [Display(Name = "Llegada")]
    public DateTime FechaLlegada { get; set; }

    public VueloDetailDto? Detalle { get; set; }

    public IReadOnlyList<RutaLookupDto> Rutas { get; set; } = Array.Empty<RutaLookupDto>();

    public IReadOnlyList<AeronaveLookupDto> Aeronaves { get; set; } = Array.Empty<AeronaveLookupDto>();

    public IReadOnlyList<PuertaLookupDto> Puertas { get; set; } = Array.Empty<PuertaLookupDto>();

    public IReadOnlyList<string> EstadosSiguientes { get; set; } = Array.Empty<string>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaLlegada <= FechaSalida)
        {
            yield return new ValidationResult(
                "La llegada debe ser posterior a la salida.",
                [nameof(FechaLlegada)]);
        }
    }
}
