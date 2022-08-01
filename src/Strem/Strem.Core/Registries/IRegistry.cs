namespace Strem.Core.Registries;

public interface IRegistry<T>
{
    void Add(T element);
    void Remove(T element);
    T Get(string elementId);
    IEnumerable<T> GetAll();
}