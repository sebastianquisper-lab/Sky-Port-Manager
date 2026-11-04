using System.ComponentModel.DataAnnotations;
using MediatR;
using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Entities;
using SkyPort.Domain.Exceptions;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Empleados.Commands;

public sealed record CreateEmpleadoCommand(
    string NumeroDocumento,
    string Nombres,
    string Apellidos,
    string Cargo,
    string? Email,
    bool? Activo) : IRequest<Result<int>>;

public sealed class CreateEmpleadoCommandHandler(IUnitOfWork work)
    : IRequestHandler<CreateEmpleadoCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var document = request.NumeroDocumento?.Trim() ?? string.Empty;
        var firstName = request.Nombres?.Trim() ?? string.Empty;
        var lastName = request.Apellidos?.Trim() ?? string.Empty;
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

        if (document.Length is < 1 or > 20 || firstName.Length is < 1 or > 80 || lastName.Length is < 1 or > 80 ||
            email?.Length > 120 || email is not null && !new EmailAddressAttribute().IsValid(email) ||
            request.Cargo is not (Cargo.Piloto or Cargo.Copiloto or Cargo.TripulanteCabina or Cargo.PersonalTierra or Cargo.Administrativo) ||
            request.Activo is null)
            return Result<int>.Failure(Error.Validation("Los datos del empleado no son válidos."));

        if (await work.Empleados.FirstOrDefaultAsync(x => x.NumeroDocumento == document, cancellationToken) is not null)
            return Result<int>.Failure(Error.Conflict("Ya existe un empleado con ese documento."));

        var employee = new Empleado
        {
            NumeroDocumento = document,
            Nombres = firstName,
            Apellidos = lastName,
            Cargo = request.Cargo,
            Email = email,
            Activo = request.Activo.Value
        };
        await work.Empleados.AddAsync(employee, cancellationToken);
        try
        {
            await work.CompleteAsync(cancellationToken);
        }
        catch (DuplicateKeyException)
        {
            return Result<int>.Failure(Error.Conflict("Ya existe un empleado con ese documento."));
        }

        return Result<int>.Success(employee.EmpleadoId);
    }
}
