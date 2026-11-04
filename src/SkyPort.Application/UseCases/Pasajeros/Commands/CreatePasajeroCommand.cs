using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Application.UseCases.Pasajeros;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Pasajeros.Commands;

public sealed record CreatePasajeroCommand(
    string TipoDocumento, string NumeroDocumento, string Nombres, string Apellidos,
    DateOnly FechaNacimiento, string Nacionalidad, string? Email, string? Telefono)
    : IRequest<Result<PasajeroDto>>;

public sealed class CreatePasajeroCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePasajeroCommand, Result<PasajeroDto>>
{
    public async Task<Result<PasajeroDto>> Handle(CreatePasajeroCommand request, CancellationToken cancellationToken)
    {
        var values = PasajeroCommandValues.From(request);
        var validationError = values.Validate();
        if (validationError is not null) return Result<PasajeroDto>.Failure(validationError);

        if (await unitOfWork.Pasajeros.FirstOrDefaultAsync(
                pasajero => pasajero.TipoDocumento == values.TipoDocumento && pasajero.NumeroDocumento == values.NumeroDocumento,
                cancellationToken) is not null)
            return Result<PasajeroDto>.Failure(Error.Conflict("Ya existe un pasajero con ese tipo y número de documento."));

        var pasajero = values.ToEntity();
        await unitOfWork.Pasajeros.AddAsync(pasajero, cancellationToken);
        try { await unitOfWork.CompleteAsync(cancellationToken); }
        catch (DuplicateKeyException)
        {
            return Result<PasajeroDto>.Failure(Error.Conflict("Ya existe un pasajero con ese tipo y número de documento."));
        }

        return Result<PasajeroDto>.Success(PasajeroCommandValues.ToDto(pasajero));
    }
}
