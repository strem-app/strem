using Strem.Core.Variables;

namespace Strem.Core.Flows;

public struct VariableDescriptor
{
    public VariableEntry VariableEntry;
    public bool IsMandatory;

    public VariableDescriptor(VariableEntry variableEntry, bool isMandatory)
    {
        VariableEntry = variableEntry;
        IsMandatory = isMandatory;
    }
}