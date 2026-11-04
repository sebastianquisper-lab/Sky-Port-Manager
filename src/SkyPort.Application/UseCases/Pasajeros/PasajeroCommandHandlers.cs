using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Ports;
using SkyPort.Application.Dtos.Pasajeros;
using SkyPort.Application.UseCases.Pasajeros.Commands;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Pasajeros;

public sealed class DeletePasajeroCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePasajeroCommand, Result>
{
    public async Task<Result> Handle(DeletePasajeroCommand request, CancellationToken cancellationToken)
    {
        var pasajero = await unitOfWork.Pasajeros.GetByIdAsync(request.PasajeroId, cancellationToken);
        if (pasajero is null)
        {
            return Result.Failure(Error.NotFound("No se encontró el pasajero."));
        }

        var tieneReservas = await unitOfWork.Reservas.CountAsync(
            reserva => reserva.PasajeroId == request.PasajeroId,
            cancellationToken);
        if (tieneReservas > 0)
        {
            return Result.Failure(Error.Conflict("No se puede eliminar un pasajero que tiene reservas."));
        }

        unitOfWork.Pasajeros.Remove(pasajero);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}

internal sealed record PasajeroCommandValues(
    string TipoDocumento,
    string NumeroDocumento,
    string Nombres,
    string Apellidos,
    DateOnly FechaNacimiento,
    string Nacionalidad,
    string? Email,
    string? Telefono)
{
    public static PasajeroCommandValues From(CreatePasajeroCommand command) => new(
        command.TipoDocumento?.Trim() ?? string.Empty,
        command.NumeroDocumento?.Trim() ?? string.Empty,
        command.Nombres?.Trim() ?? string.Empty,
        command.Apellidos?.Trim() ?? string.Empty,
        command.FechaNacimiento,
        command.Nacionalidad?.Trim() ?? string.Empty,
        NormalizeOptional(command.Email),
        NormalizeOptional(command.Telefono));

    public static PasajeroCommandValues From(UpdatePasajeroCommand command) => new(
        command.TipoDocumento?.Trim() ?? string.Empty,
        command.NumeroDocumento?.Trim() ?? string.Empty,
        command.Nombres?.Trim() ?? string.Empty,
        command.Apellidos?.Trim() ?? string.Empty,
        command.FechaNacimiento,
        command.Nacionalidad?.Trim() ?? string.Empty,
        NormalizeOptional(command.Email),
        NormalizeOptional(command.Telefono));

    public Error? Validate() => PasajeroValidation.Validate(
        TipoDocumento,
        NumeroDocumento,
        Nombres,
        Apellidos,
        FechaNacimiento,
        Nacionalidad,
        Email,
        Telefono);

    public Pasajero ToEntity() => new()
    {
        TipoDocumento = TipoDocumento,
        NumeroDocumento = NumeroDocumento,
        Nombres = Nombres,
        Apellidos = Apellidos,
        FechaNacimiento = FechaNacimiento,
        Nacionalidad = Nacionalidad,
        Email = Email,
        Telefono = Telefono
    };

    public void ApplyTo(Pasajero pasajero)
    {
        pasajero.TipoDocumento = TipoDocumento;
        pasajero.NumeroDocumento = NumeroDocumento;
        pasajero.Nombres = Nombres;
        pasajero.Apellidos = Apellidos;
        pasajero.FechaNacimiento = FechaNacimiento;
        pasajero.Nacionalidad = Nacionalidad;
        pasajero.Email = Email;
        pasajero.Telefono = Telefono;
    }

    public static PasajeroDto ToDto(Pasajero pasajero) => new(
        pasajero.PasajeroId,
        pasajero.TipoDocumento,
        pasajero.NumeroDocumento,
        pasajero.Nombres,
        pasajero.Apellidos,
        pasajero.FechaNacimiento,
        pasajero.Nacionalidad,
        pasajero.Email,
        pasajero.Telefono);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
