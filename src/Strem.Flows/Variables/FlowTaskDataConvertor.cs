using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Strem.Core.Extensions;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Variables;

public class FlowTaskDataConvertor : JsonConverter
{
    public static readonly Type FlowDataType = typeof(IFlowTaskData);
    public static readonly string StaticCodeFieldName = "TaskCode";
    public static readonly string JsonCodeFieldName = "Code";
    public static readonly Dictionary<string, Type> CachedTypeLookups = new();

    public override bool CanConvert(Type objectType) => objectType == FlowDataType;
    public override bool CanRead => true;
    public override bool CanWrite => false;

    static FlowTaskDataConvertor()
    { CacheTaskTypes(); }

    public static void CacheTaskTypes()
    {
        var matchingTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes().WhereClassesImplement(FlowDataType));

        foreach (var type in matchingTypes)
        {
            // TODO: We can hopefully enforce this at implementation level in C#11
            var staticCodeField = type.GetField(StaticCodeFieldName, BindingFlags.Static | BindingFlags.Public);
            var codeValue = (string)staticCodeField.GetValue(null);
            CachedTypeLookups.Add(codeValue, type);
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    { serializer.Serialize(writer, value); }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var flowElementData = JObject.ReadFrom(reader);
        var code = flowElementData[JsonCodeFieldName].ToString();

        if (!CachedTypeLookups.ContainsKey(code))
        { return null; }

        var strongTaskType = CachedTypeLookups[code];
        return flowElementData.ToObject(strongTaskType);
    }
}