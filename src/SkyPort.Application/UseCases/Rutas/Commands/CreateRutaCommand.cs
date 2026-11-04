using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Rules.Rutas;
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
        var input = RutaInput.Create(
            request.CodigoOrigen,
            request.CiudadOrigen,
            request.CodigoDestino,
            request.CiudadDestino,
            request.DuracionMinutos);
        if (!input.IsSuccess || input.Value is null)
        {
            return Result<int>.Failure(input.Error!);
        }

        var value = input.Value;
        var duplicate = await unitOfWork.Rutas.FirstOrDefaultAsync(
            route => route.CodigoOrigen == value.CodigoOrigen && route.CodigoDestino == value.CodigoDestino,
            cancellationToken);
        if (duplicate is not null)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe una ruta con ese origen y destino."));
        }

        var route = new Ruta
        {
            CodigoOrigen = value.CodigoOrigen,
            CiudadOrigen = value.CiudadOrigen,
            CodigoDestino = value.CodigoDestino,
            CiudadDestino = value.CiudadDestino,
            DuracionMinutos = value.DuracionMinutos
        };

        await unitOfWork.Rutas.AddAsync(route, cancellationToken);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe una ruta con ese origen y destino."));
        }

        return Result<int>.Success(route.RutaId);
    }
}
