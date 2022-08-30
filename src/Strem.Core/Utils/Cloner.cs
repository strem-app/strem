using Newtonsoft.Json;

namespace Strem.Core.Utils;

public class Cloner : ICloner
{
    public T Clone<T>(T instance)
    {
        var json = JsonConvert.SerializeObject(instance);
        return JsonConvert.DeserializeObject<T>(json);
    }
    
    public void Clone<T>(T instance, T destination)
    {
        var json = JsonConvert.SerializeObject(instance);
        JsonConvert.PopulateObject(json, destination);
    }
}