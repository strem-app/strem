namespace Strem.Infrastructure.Services.Persistence;

public interface IStateAutoSaver : IDisposable
{
    void EnableAutoSaving();
    void DisableAutoSaving();
}