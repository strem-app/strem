namespace Strem.Infrastructure.Services.Api;

public interface IApiWebHost
{
    void StartHost(ApiHostConfiguration configuration = null);
    void StopHost();
}