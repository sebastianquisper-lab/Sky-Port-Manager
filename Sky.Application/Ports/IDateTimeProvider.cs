namespace Sky.Application.Ports;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
