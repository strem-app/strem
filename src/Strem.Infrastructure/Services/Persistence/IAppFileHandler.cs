using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence;

public interface IAppFileHandler
{
    Task<AppState> LoadAppState();
}