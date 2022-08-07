using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Core.Extensions;

public static class IVariablesExtensions
{
    public static bool Has(this IVariables variables, string key, string context = VariableEntry.DefaultContext)
    { return variables.Has(new VariableEntry(key, context)); }
    
    public static string Get(this IVariables variables, string key, string context = VariableEntry.DefaultContext)
    { return variables.Get(new VariableEntry(key, context)); }
    
    public static T Get<T>(this IVariables variables, string key, string context = VariableEntry.DefaultContext) where T : struct
    { return Get<T>(variables, new VariableEntry(key, context)); }
    
    public static T Get<T>(this IVariables variables, VariableEntry entry) where T : struct
    {
        var data = variables.Get(entry);
        if(string.IsNullOrEmpty(data))
        { return default; }

        try
        { return (T)Convert.ChangeType(data, typeof(T)); }
        catch { return default;}
    }
    
    public static void Set<T>(this IVariables variables, VariableEntry entry, T value) where T : struct
    { variables.Set(entry, value.ToString()); }
    
    public static void Set<T>(this IVariables variables, string key, T value) where T : struct
    { Set(variables, new VariableEntry(key), value); }
    
    public static void Set<T>(this IVariables variables, string key, string context, T value) where T : struct
    { Set(variables, new VariableEntry(key, context), value); }
    
    public static void Set(this IVariables variables, string key, string context, string value)
    { variables.Set(new VariableEntry(key, context), value); }
    
    public static void Set(this IVariables variables, string key, string value)
    { variables.Set(new VariableEntry(key), value); }
    
    public static void Delete(this IVariables variables, string key, string context = VariableEntry.DefaultContext)
    { variables.Delete(new VariableEntry(key, context)); }
}