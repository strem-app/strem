using Strem.Core.Variables;

namespace Strem.Core.Extensions;

public static class VariableEntryExtensions
{
    public static VariableDescriptor ToDescriptor(this VariableEntry entry, bool isMandatory = true)
    { return new VariableDescriptor(entry, isMandatory); }
    
    public static bool IsEmpty(this VariableEntry? entry) => string.IsNullOrEmpty(entry?.Name);
    public static bool HasContext(this VariableEntry? entry)  => !string.IsNullOrEmpty(entry?.Context);
}