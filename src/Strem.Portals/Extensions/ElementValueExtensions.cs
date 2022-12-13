using Strem.Portals.Data;

namespace Strem.Portals.Extensions;

public static class ElementValueExtensions
{
    public static string DefaultValueKey = "default-value";
    public static string MinValueKey = "min-value";
    public static string MaxValueKey = "max-value";

    public static int GetFromKey(GridElementData element, string key)
    {
        return element.Values.ContainsKey(key)
            ? element.Values[key]
            : default;
    }
    
    public static void SetFromKey(GridElementData element, string key, int value)
    { element.Values[key] = value; }

    public static int DefaultValue(this GridElementData element) => GetFromKey(element, DefaultValueKey);
    public static void DefaultValue(this GridElementData element, int value) => SetFromKey(element, DefaultValueKey, value);
    public static int MinValue(this GridElementData element) => GetFromKey(element, MinValueKey);
    public static void MinValue(this GridElementData element, int value) => SetFromKey(element, MinValueKey, value);
    public static int MaxValue(this GridElementData element)
    { return element.Values.ContainsKey(MaxValueKey) ? element.Values[MaxValueKey] : int.MaxValue; }

    public static void MaxValue(this GridElementData element, int value) => SetFromKey(element, MaxValueKey, value);
}