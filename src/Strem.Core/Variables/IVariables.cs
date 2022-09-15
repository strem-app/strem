namespace Strem.Core.Variables;

public interface IVariables : IEnumerable<KeyValuePair<VariableEntry, string>>
{
    IObservable<KeyValuePair<VariableEntry, string>> OnVariableChanged { get; }
    
    bool Has(VariableEntry variableEntry);
    string Get(VariableEntry variableEntry);
    void Set(VariableEntry variableEntry, string value);
    void Delete(VariableEntry variableEntry);
}