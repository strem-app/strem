namespace Strem.Core.State;

public interface IVariables
{
    public string Get(StateEntry stateEntry);
    public void Set(StateEntry stateEntry, string value);
    public void Delete(StateEntry stateEntry);
}