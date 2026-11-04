using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SkyPort.Web.Models;

public sealed class BoletoViewModel
{
    [Required(ErrorMessage = "Selecciona una reserva.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una reserva válida.")]
    public int ReservaId { get; set; }

    public IReadOnlyList<SelectListItem> Reservas { get; set; } = [];
}
