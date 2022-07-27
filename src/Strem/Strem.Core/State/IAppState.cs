namespace Strem.Core.State;

public interface IAppState
{
    IVariables UserVariables { get; }
    IVariables AppVariables { get; }
    IVariables TransientVariables { get; }
}