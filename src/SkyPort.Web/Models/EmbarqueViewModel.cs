using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SkyPort.Application.Dtos.Embarques;

namespace SkyPort.Web.Models;

public sealed class EmbarqueViewModel
{
    [Range(1, int.MaxValue)]
    [Display(Name = "Boleto con check-in realizado")]
    public int BoletoId { get; set; }

    [ValidateNever]
    public IReadOnlyList<EmbarqueBoletoOptionDto> Boletos { get; set; } = [];
}
