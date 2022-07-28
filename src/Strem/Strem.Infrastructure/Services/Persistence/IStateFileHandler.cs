using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence;

public interface IStateFileHandler
{
    Task CreateStateFilesIfMissing();
    Task<AppState> LoadAppState();
    Task SaveAppState(IAppState appState);
}