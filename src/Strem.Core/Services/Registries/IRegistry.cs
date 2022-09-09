namespace Strem.Core.Services.Registries;

public interface IRegistry<T>
{
    void Add(T element);
    void Remove(T element);
    T Get(string id);
    IEnumerable<T> GetAll();
}