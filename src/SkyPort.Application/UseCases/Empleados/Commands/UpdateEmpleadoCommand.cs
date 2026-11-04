using System.ComponentModel.DataAnnotations;
using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Empleados.Commands;

public sealed record UpdateEmpleadoCommand(
    int EmpleadoId,
    string NumeroDocumento,
    string Nombres,
    string Apellidos,
    string Cargo,
    string? Email,
    bool? Activo) : IRequest<Result>;

public sealed class UpdateEmpleadoCommandHandler(IUnitOfWork work)
    : IRequestHandler<UpdateEmpleadoCommand, Result>
{
    public async Task<Result> Handle(UpdateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var employee = await work.Empleados.GetByIdAsync(request.EmpleadoId, cancellationToken);
        if (employee is null) return Result.Failure(Error.NotFound("Empleado no encontrado."));

        var document = request.NumeroDocumento?.Trim() ?? string.Empty;
        var firstName = request.Nombres?.Trim() ?? string.Empty;
        var lastName = request.Apellidos?.Trim() ?? string.Empty;
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        if (document.Length is < 1 or > 20 || firstName.Length is < 1 or > 80 || lastName.Length is < 1 or > 80 ||
            email?.Length > 120 || email is not null && !new EmailAddressAttribute().IsValid(email) ||
            request.Cargo is not (Cargo.Piloto or Cargo.Copiloto or Cargo.TripulanteCabina or Cargo.PersonalTierra or Cargo.Administrativo) ||
            request.Activo is null)
            return Result.Failure(Error.Validation("Los datos del empleado no son válidos."));

        if (await work.Empleados.FirstOrDefaultAsync(
                x => x.EmpleadoId != employee.EmpleadoId && x.NumeroDocumento == document,
                cancellationToken) is not null)
            return Result.Failure(Error.Conflict("Ya existe un empleado con ese documento."));

        employee.NumeroDocumento = document;
        employee.Nombres = firstName;
        employee.Apellidos = lastName;
        employee.Cargo = request.Cargo;
        employee.Email = email;
        employee.Activo = request.Activo.Value;
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result.Failure(Error.Conflict("Ya existe un empleado con ese documento."));
        }

        return Result.Success();
    }
}
