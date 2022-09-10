namespace Strem.Core.Variables;

public interface IVariables : IDisposable
{
    IObservable<KeyValuePair<VariableEntry, string>> OnVariableChanged { get; }
    
    bool Has(VariableEntry variableEntry);
    string Get(VariableEntry variableEntry);
    void Set(VariableEntry variableEntry, string value);
    void Delete(VariableEntry variableEntry);
    IEnumerable<KeyValuePair<VariableEntry, string>> GetAll();
}