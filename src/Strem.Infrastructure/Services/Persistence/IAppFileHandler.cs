using Strem.Core.Flows;
using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence;

public interface IAppFileHandler
{
    Task CreateAppFilesIfMissing();
    Task<AppState> LoadAppState();
    Task<FlowStore> LoadFlowStore();
    Task SaveAppState(IAppState appState);
}