namespace Strem.Core.State;

public interface IVariables : IDisposable
{
    IObservable<VariableEntry> OnVariableChanged { get; }
    
    string Get(VariableEntry variableEntry);
    void Set(VariableEntry variableEntry, string value);
    void Delete(VariableEntry variableEntry);
    IEnumerable<KeyValuePair<VariableEntry, string>> GetAll();
}