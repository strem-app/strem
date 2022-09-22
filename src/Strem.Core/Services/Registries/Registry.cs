namespace Strem.Core.Services.Registries;

public abstract class Registry<T> : IRegistry<T> where T : class
{
    public Dictionary<string, T> Data { get; } = new();

    public abstract string GetId(T data);
    
    public void Add(T element)
    {
        var id = GetId(element);
        if(Data.ContainsKey(id))
        { throw new Exception($"Attempt to register {id}, but it has already been registered"); }
        
        Data.Add(id, element);
    }

    public void Remove(T element)
    {
        var id = GetId(element);
        Data.Remove(id);
    } 
    
    public void Remove(string id) => Data.Remove(id);
    public T? Get(string id) => Has(id) ? Data[id] : null;
    public bool Has(string id) => Data.ContainsKey(id);
    public IEnumerable<T> GetAll() => Data.Values;
}