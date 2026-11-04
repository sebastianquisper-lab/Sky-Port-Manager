namespace SkyPort.Domain.Exceptions;

public sealed class ReferenceConflictException(Exception innerException)
    : Exception("La operacion entra en conflicto con registros relacionados.", innerException);
