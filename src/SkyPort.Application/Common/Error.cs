namespace SkyPort.Application.Common;

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Forbidden
}

public sealed record Error(ErrorType Type, string Message)
{
    public static Error NotFound(string message) => new(ErrorType.NotFound, message);

    public static Error Validation(string message) => new(ErrorType.Validation, message);

    public static Error Conflict(string message) => new(ErrorType.Conflict, message);

    public static Error Forbidden(string message) => new(ErrorType.Forbidden, message);

    public static Error Config(string message) => new(ErrorType.Validation, message);
}
