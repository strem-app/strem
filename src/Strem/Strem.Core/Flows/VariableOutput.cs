using Strem.Core.Variables;

namespace Strem.Core.Flows;

public struct VariableOutput
{
    public VariableEntry VariableEntry;
    public bool IsMandatory;

    public VariableOutput(VariableEntry variableEntry, bool isMandatory)
    {
        VariableEntry = variableEntry;
        IsMandatory = isMandatory;
    }
}