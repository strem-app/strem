using Strem.Core.Flows;
using Strem.Core.Variables;

namespace Strem.Core.Extensions;

public static class VariableEntryExtensions
{
    public static VariableOutput ToOutput(this VariableEntry entry, bool isMandatory = true)
    { return new VariableOutput(entry, isMandatory); }
}