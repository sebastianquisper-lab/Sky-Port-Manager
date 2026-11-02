using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Commands;

public sealed record UpdateRutaCommand(
    int RutaId,
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    int DuracionMinutos) : IRequest<Result>;

public sealed class UpdateRutaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRutaCommand, Result>
{
    public async Task<Result> Handle(UpdateRutaCommand request, CancellationToken cancellationToken)
    {
        var inputResult = RutaInput.Create(
            request.CodigoOrigen,
            request.CiudadOrigen,
            request.CodigoDestino,
            request.CiudadDestino,
            request.DuracionMinutos);
        if (!inputResult.IsSuccess)
        {
            return Result.Failure(inputResult.Error!);
        }

        var route = await unitOfWork.Rutas.GetByIdAsync(request.RutaId, cancellationToken);
        if (route is null)
        {
            return Result.Failure(Error.NotFound("No se encontró la ruta."));
        }

        var input = inputResult.Value!;
        var duplicate = await unitOfWork.Rutas.FirstOrDefaultAsync(
            candidate => candidate.RutaId != route.RutaId &&
                         candidate.CodigoOrigen == input.CodigoOrigen &&
                         candidate.CodigoDestino == input.CodigoDestino,
            cancellationToken);
        if (duplicate is not null)
        {
            return Result.Failure(Error.Conflict("Ya existe una ruta con ese origen y destino."));
        }

        route.CodigoOrigen = input.CodigoOrigen;
        route.CiudadOrigen = input.CiudadOrigen;
        route.CodigoDestino = input.CodigoDestino;
        route.CiudadDestino = input.CiudadDestino;
        route.DuracionMinutos = input.DuracionMinutos;
        unitOfWork.Rutas.Update(route);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DuplicateKeyException)
        {
            return Result.Failure(Error.Conflict("Ya existe una ruta con ese origen y destino."));
        }
    }
}
