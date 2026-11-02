using MediatR;
using SkyPort.Application.Common;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Rutas.Commands;

public sealed record CreateRutaCommand(
    string CodigoOrigen,
    string CiudadOrigen,
    string CodigoDestino,
    string CiudadDestino,
    int DuracionMinutos) : IRequest<Result<int>>;

public sealed class CreateRutaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRutaCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRutaCommand request, CancellationToken cancellationToken)
    {
        var inputResult = RutaInput.Create(
            request.CodigoOrigen,
            request.CiudadOrigen,
            request.CodigoDestino,
            request.CiudadDestino,
            request.DuracionMinutos);
        if (!inputResult.IsSuccess)
        {
            return Result<int>.Failure(inputResult.Error!);
        }

        var input = inputResult.Value!;
        var duplicate = await unitOfWork.Rutas.FirstOrDefaultAsync(
            route => route.CodigoOrigen == input.CodigoOrigen && route.CodigoDestino == input.CodigoDestino,
            cancellationToken);
        if (duplicate is not null)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe una ruta con ese origen y destino."));
        }

        var route = new Ruta
        {
            CodigoOrigen = input.CodigoOrigen,
            CiudadOrigen = input.CiudadOrigen,
            CodigoDestino = input.CodigoDestino,
            CiudadDestino = input.CiudadDestino,
            DuracionMinutos = input.DuracionMinutos
        };

        await unitOfWork.Rutas.AddAsync(route, cancellationToken);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(route.RutaId);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe una ruta con ese origen y destino."));
        }
    }
}
