using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sky.Application.UseCases.PaymentMethod.Commands;
using Sky.Application.UseCases.PaymentMethod.Queries;

namespace Sky.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("api/payment-methods")]
public sealed class PaymentMethodsController(ISender sender) : ControllerBase
{
    public sealed record CreatePaymentMethodRequest(string MethodName);
    public sealed record UpdatePaymentMethodRequest(string MethodName);

    [HttpPost]
    public async Task<IActionResult> CreatePaymentMethod(CreatePaymentMethodRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new CreatePaymentMethodCommand(body.MethodName), ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpDelete("{paymentMethodId:int}")]
    public async Task<IActionResult> DeletePaymentMethod(int paymentMethodId, CancellationToken ct)
        => await sender.Send(new DeletePaymentMethodCommand(paymentMethodId), ct) ? NoContent() : NotFound();
    [HttpPut("{paymentMethodId:int}")]
    public async Task<IActionResult> UpdatePaymentMethod(int paymentMethodId, UpdatePaymentMethodRequest body, CancellationToken ct)
    {
        var result = await sender.Send(new UpdatePaymentMethodCommand(paymentMethodId, body.MethodName), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("{paymentMethodId:int}")]
    public async Task<IActionResult> GetPaymentMethodById(int paymentMethodId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentMethodByIdQuery(paymentMethodId), ct);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetPaymentMethods(CancellationToken ct)
        => Ok(await sender.Send(new GetPaymentMethodsQuery(), ct));
}
