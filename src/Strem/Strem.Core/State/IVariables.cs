namespace Strem.Core.State;

public interface IVariables
{
    string Get(StateEntry stateEntry);
    void Set(StateEntry stateEntry, string value);
    void Delete(StateEntry stateEntry);
    IEnumerable<KeyValuePair<StateEntry, string>> GetAll();
}