namespace Strem.Core.Services.Registries;

public interface IRegistry<T> where T : class
{
    void Add(T element);
    void Remove(string id);
    void Remove(T element);
    T? Get(string id);
    bool Has(string id);
    IEnumerable<T> GetAll();
}