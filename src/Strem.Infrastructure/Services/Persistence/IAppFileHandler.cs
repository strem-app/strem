using Strem.Core.Flows;
using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence;

public interface IAppFileHandler
{
    Task<AppState> LoadAppState();
    Task<FlowStore> LoadFlowStore();
}