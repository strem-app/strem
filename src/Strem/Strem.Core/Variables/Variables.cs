using System.Reactive.Subjects;
using Newtonsoft.Json;

namespace Strem.Core.Variables;

public class Variables : IVariables
{
    public Dictionary<VariableEntry, string> Data { get; }
    
    [JsonIgnore]
    public Subject<VariableEntry> OnChangedSubject { get; } = new();
    [JsonIgnore]
    public IObservable<VariableEntry> OnVariableChanged => OnChangedSubject;
   
    public Variables(Dictionary<VariableEntry, string> state = null)
    { Data = state ?? new Dictionary<VariableEntry, string>(); }

    public VariableEntry FindFullyQualifiedEntry(string variableName)
    {
        var fullyQualifiedEntry = Data.Keys
            .Where(x => x.Name == variableName)
            .ToArray();

        return fullyQualifiedEntry.Length == 1 ? fullyQualifiedEntry[0] : default;
    }
    
    public bool Has(VariableEntry variableEntry)
    {
        var matches = Data.ContainsKey(variableEntry);
        if(matches) { return true; }

        if (variableEntry.HasContext)
        { return false; }
        
        var qualifiedEntry = FindFullyQualifiedEntry(variableEntry.Name);
        return !qualifiedEntry.IsEmpty;
    }

    public string Get(VariableEntry variableEntry)
    {
        if(Data.ContainsKey(variableEntry))
        { return Data[variableEntry]; }
        
        if(variableEntry.HasContext)
        { return string.Empty; }
        
        var qualifiedEntry = FindFullyQualifiedEntry(variableEntry.Name);
        return qualifiedEntry.IsEmpty ? string.Empty : Data[qualifiedEntry];
    }
    
    public IEnumerable<KeyValuePair<VariableEntry, string>> GetAll() => Data;
    
    public void Delete(VariableEntry variableEntry)
    {
        Data.Remove(variableEntry);
        OnChangedSubject.OnNext(variableEntry);
    }

    public void Set(VariableEntry variableEntry, string value)
    {
        Data[variableEntry] = value;
        OnChangedSubject.OnNext(variableEntry);
    }

    public void Dispose()
    { OnChangedSubject.Dispose(); }
}