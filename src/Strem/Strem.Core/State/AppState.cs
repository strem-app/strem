using Strem.Core.Variables;

namespace Strem.Core.State;

public class AppState : IAppState
{
    public IVariables UserVariables { get; }
    public IVariables AppVariables { get; }
    public IVariables TransientVariables { get; }

    public AppState(IVariables userVariables, IVariables appVariables, IVariables transientVariables)
    {
        UserVariables = userVariables;
        AppVariables = appVariables;
        TransientVariables = transientVariables;
    }
}