namespace Strem.Core.Utils;

public interface ICloner
{
    public T Clone<T>(T instance);
    void Clone<T>(T instance, T destination);
}