using Strem.Core.Variables;

namespace Strem.Core.State;

public interface IAppState
{
    IVariables UserVariables { get; }
    IVariables TransientVariables { get; }
    
    IVariables AppVariables { get; }
}