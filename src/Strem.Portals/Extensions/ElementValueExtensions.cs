using Strem.Portals.Data;

namespace Strem.Portals.Extensions;

public static class ElementValueExtensions
{
    public static string CurrentValueKey = "current-value";
    public static string MinValueKey = "min-value";
    public static string MaxValueKey = "max-value";

    public static string GetFromKey(GridElementData element, string key)
    {
        return element.Values.ContainsKey(key)
            ? element.Values[key]
            : string.Empty;
    }
    
    public static int GetIntFromKey(GridElementData element, string key)
    {
        if (!element.Values.ContainsKey(key)) { return default; }
        return !int.TryParse(element.Values[key], out var intValue) ? default : intValue;
    }
    
    public static void SetFromKey(GridElementData element, string key, string value)
    { element.Values[key] = value; }
    
    public static void SetFromKey(GridElementData element, string key, int value)
    { SetFromKey(element, key, value.ToString()); }

    public static int CurrentValue(this GridElementData element) => GetIntFromKey(element, CurrentValueKey);
    public static void CurrentValue(this GridElementData element, int value) => SetFromKey(element, CurrentValueKey, value);
    public static int MinValue(this GridElementData element) => GetIntFromKey(element, MinValueKey);
    public static void MinValue(this GridElementData element, int value) => SetFromKey(element, MinValueKey, value);
    public static int MaxValue(this GridElementData element)
    { return element.Values.ContainsKey(MaxValueKey) ? GetIntFromKey(element, MaxValueKey) : int.MaxValue; }

    public static void MaxValue(this GridElementData element, int value) => SetFromKey(element, MaxValueKey, value);
}