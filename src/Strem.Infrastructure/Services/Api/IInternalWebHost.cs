namespace Strem.Infrastructure.Services.Api;

public interface IInternalWebHost
{
    bool IsRunning { get; }
    void StartHost();
    void StopHost();
}