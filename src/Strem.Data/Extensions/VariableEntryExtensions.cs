using Strem.Core.Variables;

namespace Strem.Data.Extensions;

public static class VariableEntryExtensions
{
    public static string ToCompositeKey(this VariableEntry entry)
    { return entry.HasContext ? $"{entry.Name}.{entry.Context}" : entry.Name; }
}