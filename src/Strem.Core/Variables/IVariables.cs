namespace Strem.Core.Variables;

public interface IVariables : IDisposable
{
    IObservable<VariableEntry> OnVariableChanged { get; }
    
    bool Has(VariableEntry variableEntry);
    string Get(VariableEntry variableEntry);
    void Set(VariableEntry variableEntry, string value);
    void Delete(VariableEntry variableEntry);
    IEnumerable<KeyValuePair<VariableEntry, string>> GetAll();
}