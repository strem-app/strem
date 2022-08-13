using Strem.Core.Flows;
using Strem.Core.Variables;

namespace Strem.Core.Extensions;

public static class VariableEntryExtensions
{
    public static VariableDescriptor ToDescriptor(this VariableEntry entry, bool isMandatory = true)
    { return new VariableDescriptor(entry, isMandatory); }
}