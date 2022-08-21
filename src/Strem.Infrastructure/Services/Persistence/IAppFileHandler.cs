using Strem.Core.Flows;
using Strem.Core.Portals;
using Strem.Core.State;
using Strem.Core.Todo;

namespace Strem.Infrastructure.Services.Persistence;

public interface IAppFileHandler
{
    Task<AppState> LoadAppState();
    Task<FlowStore> LoadFlowStore();
    Task<TodoStore> LoadTodoStore();
    Task<PortalStore> LoadPortalStore();
    
    Task SaveAppState(IAppState appState);
    Task SaveUserState(IAppState appState);
    Task SaveFlowStore(IFlowStore flowStore);
    Task SaveTodoStore(ITodoStore todoStore);
    Task SavePortalStore(IPortalStore portalStore);

    Task CreateAppFilesIfMissing();
    Task BackupFiles();
}