namespace SkyPort.Domain.Exceptions;

public sealed class DuplicateKeyException : Exception
{
    public DuplicateKeyException(string message)
        : base(message)
    {
    }

    public DuplicateKeyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
