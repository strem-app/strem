using System.Collections;

namespace Strem.Core.State;

public class Variables : IVariables
{
    public Dictionary<StateEntry, string> InternalVariables { get; }

    public Variables(Dictionary<StateEntry, string> state = null)
    { InternalVariables = state ?? new Dictionary<StateEntry, string>(); }

    public string Get(StateEntry stateEntry) => InternalVariables[stateEntry];
    public void Set(StateEntry stateEntry, string value) => InternalVariables[stateEntry] = value;
    public void Delete(StateEntry stateEntry) => InternalVariables.Remove(stateEntry);
    public IEnumerable<KeyValuePair<StateEntry, string>> GetAll() => InternalVariables;
}