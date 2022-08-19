namespace Strem.Infrastructure.Services.Api;

public interface IApiWebHost
{
    bool IsRunning { get; }
    void StartHost();
    void StopHost();
}