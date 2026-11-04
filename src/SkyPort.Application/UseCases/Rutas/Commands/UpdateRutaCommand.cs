using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Commands;

public sealed record UpdateRutaCommand(int RutaId, int DuracionMinutos) : IRequest<Result>;

public sealed class UpdateRutaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRutaCommand, Result>
{
    public async Task<Result> Handle(UpdateRutaCommand request, CancellationToken cancellationToken)
    {
        if (request.RutaId <= 0)
        {
            return Result.Failure(Error.NotFound("No se encontró la ruta."));
        }

        if (request.DuracionMinutos <= 0)
        {
            return Result.Failure(Error.Validation("La duración debe ser positiva."));
        }

        var route = await unitOfWork.Rutas.GetByIdAsync(request.RutaId, cancellationToken);
        if (route is null)
        {
            return Result.Failure(Error.NotFound("No se encontró la ruta."));
        }

        route.DuracionMinutos = request.DuracionMinutos;
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
