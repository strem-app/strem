namespace Strem.Core.Variables;

public record VariableDescriptor(VariableEntry VariableEntry, bool IsMandatory)
{
    public VariableEntry VariableEntry = VariableEntry;
    public bool IsMandatory = IsMandatory;
}