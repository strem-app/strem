namespace Strem.Core.Flows;

public interface IFlowElement
{
    string Version { get; }
    string Code { get; }
    string Name { get; }
    string Category { get; }
    string Description { get; }
    
    public abstract VariableDescriptor[] VariableOutputs { get; }

}