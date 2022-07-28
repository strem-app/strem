namespace Strem.Infrastructure.Services.Logging;

public interface IAutoLogger : IDisposable
{
    void EnableAutoLogging();
    void DisableAutoLogging();
}