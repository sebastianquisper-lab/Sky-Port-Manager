using SkyPort.Application.Common;
using SkyPort.Application.Common.Constants;
using SkyPort.Domain.Ports;

namespace SkyPort.Application.UseCases.Vuelos.Rules;

public sealed class VueloSchedulingRules(IUnitOfWork unitOfWork)
{
    private static readonly TimeSpan AircraftTurnaround = TimeSpan.FromMinutes(45);
    private static readonly TimeSpan GateTurnaround = TimeSpan.FromMinutes(30);

    public async Task<Result> ValidateAsync(
        int aeronaveId,
        int? puertaId,
        DateTime fechaSalida,
        DateTime fechaLlegada,
        int? vueloIdIgnorado,
        bool esCreacion,
        CancellationToken cancellationToken = default)
    {
        if (esCreacion && fechaSalida < DateTime.Now)
        {
            return Result.Failure(Error.Validation("La salida de un vuelo nuevo no puede estar en el pasado."));
        }

        var aircraft = await unitOfWork.Aeronaves.GetByIdAsync(aeronaveId, cancellationToken);
        if (aircraft is null)
        {
            return Result.Failure(Error.Validation("La aeronave seleccionada no existe."));
        }

        if (aircraft.Estado != EstadoAeronave.Operativa)
        {
            return Result.Failure(Error.Conflict($"La aeronave {aircraft.Matricula} no está operativa."));
        }

        string? gateCode = null;
        if (puertaId.HasValue)
        {
            var gate = await unitOfWork.Puertas.GetByIdAsync(puertaId.Value, cancellationToken);
            if (gate is null)
            {
                return Result.Failure(Error.Validation("La puerta seleccionada no existe."));
            }

            if (gate.Disponible != true)
            {
                return Result.Failure(Error.Conflict($"La puerta {gate.Codigo} no está disponible."));
            }

            gateCode = gate.Codigo;
        }

        var candidates = await unitOfWork.Vuelos.FindAsync(
            flight => flight.Estado != EstadoVuelo.Cancelado &&
                      (!vueloIdIgnorado.HasValue || flight.VueloId != vueloIdIgnorado.Value) &&
                      (flight.AeronaveId == aeronaveId ||
                       (puertaId.HasValue && flight.PuertaId == puertaId.Value)),
            cancellationToken);

        var aircraftConflict = candidates.FirstOrDefault(flight =>
            flight.AeronaveId == aeronaveId &&
            fechaSalida < flight.FechaLlegada.Add(AircraftTurnaround) &&
            flight.FechaSalida < fechaLlegada.Add(AircraftTurnaround));
        if (aircraftConflict is not null)
        {
            return Result.Failure(Error.Conflict(
                $"La aeronave {aircraft.Matricula} tiene un conflicto de horario con el vuelo {aircraftConflict.NumeroVuelo}."));
        }

        if (puertaId.HasValue)
        {
            var gateConflict = candidates.FirstOrDefault(flight =>
                flight.PuertaId == puertaId.Value &&
                fechaSalida < flight.FechaLlegada.Add(GateTurnaround) &&
                flight.FechaSalida < fechaLlegada.Add(GateTurnaround));
            if (gateConflict is not null)
            {
                return Result.Failure(Error.Conflict(
                    $"La puerta {gateCode ?? puertaId.Value.ToString()} tiene un conflicto de horario con el vuelo {gateConflict.NumeroVuelo}."));
            }
        }

        return Result.Success();
    }
}
