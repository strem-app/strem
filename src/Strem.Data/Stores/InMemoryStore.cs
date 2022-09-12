namespace Strem.Data.Stores;

public abstract class InMemoryStore<T, TK> : IInMemoryStore<T, TK>
{
    public IRepository<T, TK> Repository { get; }
    public Dictionary<TK, T> Cache { get; }
    public IReadOnlyCollection<T> Data => Cache.Values;

    protected InMemoryStore(IRepository<T, TK> repository)
    {
        Repository = repository;
        Cache = Repository.GetAll().ToDictionary(GetKey, x => x);
    }

    protected virtual void OnAdded(T data){}
    protected virtual void OnRemoved(T data){}
    
    protected abstract TK GetKey(T data);
    
    public T Add(T data)
    {
        var id = GetKey(data);
        Cache.Add(id, data);
        Repository.Create(id, data);
        OnAdded(data);
        return data;
    }

    public T? Get(TK id)
    { return !Cache.ContainsKey(id) ? default : Cache[id]; }

    public bool Has(TK id)
    { return Cache.ContainsKey(id); }

    public bool Remove(TK id)
    {
        if(!Cache.ContainsKey(id)) { return false; }

        var entity = Get(id); 
        Cache.Remove(id);
        Repository.Delete(id);
        OnRemoved(entity!);
        return true;
    }
}