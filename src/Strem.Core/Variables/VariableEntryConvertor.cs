using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Strem.Core.Variables;

public class VariableEntryConvertor : JsonConverter
{
    public static readonly Type VariableEntryType = typeof(KeyValuePair<VariableEntry, string>);

    public override bool CanConvert(Type objectType) => objectType == VariableEntryType;
    public override bool CanRead => true;
    
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var entry = value == null ? 
            new KeyValuePair<VariableEntry, string>() : 
            (KeyValuePair<VariableEntry, string>)value;
    
        var jObject = new JObject();
        jObject.Add("name", entry.Key.Name);
        if(!string.IsNullOrEmpty(entry.Key.Context))
        { jObject.Add("context", entry.Key.Context); }
        jObject.Add("value", entry.Value);
        jObject.WriteTo(writer);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var entry = JObject.Load(reader);

        VariableEntry variableEntry;
        if (entry.ContainsKey("context"))
        { variableEntry = new VariableEntry(entry["name"].ToString(), entry["context"].ToString()); }
        else
        { variableEntry = new VariableEntry(entry["name"].ToString()); }

        return new KeyValuePair<VariableEntry, string>(variableEntry, entry["value"].ToString());
    }
}