using System.Reactive.Subjects;
using Newtonsoft.Json;

namespace Strem.Core.State;

public class Variables : IVariables
{
    public Dictionary<VariableEntry, string> Data { get; }
    
    [JsonIgnore]
    public Subject<VariableEntry> OnChangedSubject { get; } = new();
    [JsonIgnore]
    public IObservable<VariableEntry> OnVariableChanged => OnChangedSubject;
   
    public Variables(Dictionary<VariableEntry, string> state = null)
    { Data = state ?? new Dictionary<VariableEntry, string>(); }

    public string Get(VariableEntry variableEntry) => Data.ContainsKey(variableEntry) ? Data[variableEntry] : string.Empty;
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