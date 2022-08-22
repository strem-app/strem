using Strem.Core.Flows;
using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence;

public interface IAppFileHandler
{
    Task<AppState> LoadAppState();
    Task<FlowStore> LoadFlowStore();
    
    Task SaveAppState(IAppState appState);
    Task SaveUserState(IAppState appState);
    Task SaveFlowStore(IFlowStore flowStore);

    Task CreateAppFilesIfMissing();
    Task BackupFiles();
}