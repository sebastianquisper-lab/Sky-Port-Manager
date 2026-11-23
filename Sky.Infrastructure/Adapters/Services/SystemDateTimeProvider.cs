using Sky.Application.Ports;

namespace Sky.Infrastructure.Adapters.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
