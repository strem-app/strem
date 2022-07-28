using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Core.Extensions;

public static class IVariablesExtensions
{
    public static bool Has(this IVariables variables, string key, string context = VariableEntry.DefaultContext)
    { return variables.Has(new VariableEntry(key, context)); }
    
    public static string Get(this IVariables variables, string key, string context = VariableEntry.DefaultContext)
    { return variables.Get(new VariableEntry(key, context)); }
    
    public static void Set(this IVariables variables, string key, string context, string value)
    { variables.Set(new VariableEntry(key, context), value); }
    
    public static void Set(this IVariables variables, string key, string value)
    { variables.Set(new VariableEntry(key), value); }
    
    public static void Delete(this IVariables variables, string key, string context = VariableEntry.DefaultContext)
    { variables.Delete(new VariableEntry(key, context)); }
}