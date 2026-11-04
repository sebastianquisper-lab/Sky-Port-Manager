using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Application.UseCases.Pasajeros;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Pasajeros.Commands;

public sealed record UpdatePasajeroCommand(
    int PasajeroId, string TipoDocumento, string NumeroDocumento, string Nombres,
    string Apellidos, DateOnly FechaNacimiento, string Nacionalidad, string? Email, string? Telefono)
    : IRequest<Result<PasajeroDto>>;

public sealed class UpdatePasajeroCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePasajeroCommand, Result<PasajeroDto>>
{
    public async Task<Result<PasajeroDto>> Handle(UpdatePasajeroCommand request, CancellationToken cancellationToken)
    {
        var values = PasajeroCommandValues.From(request);
        var validationError = values.Validate();
        if (validationError is not null) return Result<PasajeroDto>.Failure(validationError);

        var pasajero = await unitOfWork.Pasajeros.GetByIdAsync(request.PasajeroId, cancellationToken);
        if (pasajero is null) return Result<PasajeroDto>.Failure(Error.NotFound("No se encontró el pasajero."));

        var documentoEnUso = await unitOfWork.Pasajeros.FirstOrDefaultAsync(
            otro => otro.PasajeroId != request.PasajeroId && otro.TipoDocumento == values.TipoDocumento && otro.NumeroDocumento == values.NumeroDocumento,
            cancellationToken);
        if (documentoEnUso is not null)
            return Result<PasajeroDto>.Failure(Error.Conflict("Ya existe un pasajero con ese tipo y número de documento."));

        values.ApplyTo(pasajero);
        try { await unitOfWork.CompleteAsync(cancellationToken); }
        catch (DuplicateKeyException)
        {
            return Result<PasajeroDto>.Failure(Error.Conflict("Ya existe un pasajero con ese tipo y número de documento."));
        }

        return Result<PasajeroDto>.Success(PasajeroCommandValues.ToDto(pasajero));
    }
}
