using Strem.Core.Flows;
using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence;

public interface IAppFileHandler
{
    Task<AppState> LoadAppState();
    Task<FlowStore> LoadFlowStore();
    Task<TodoStore> LoadTodoStore();
    
    Task SaveAppState(IAppState appState);
    Task SaveUserState(IAppState appState);
    Task SaveFlowState(IFlowStore flowStore);
    Task SaveTodoState(ITodoStore todoStore);

    Task CreateAppFilesIfMissing();
    Task BackupFiles();
}