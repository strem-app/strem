namespace Strem.Data.Stores;

public interface IInMemoryStore<T, in TK>
{
    IReadOnlyCollection<T> Data { get; }

    T Add(T data);
    T? Get(TK id);
    bool Has(TK id);
    bool Remove(TK id);
}