namespace Strem.Core.Flows.Registries.Integrations;

public interface IIntegrationDescriptor
{
    public string Title { get; }
    public string Code { get; }
    public VariableDescriptor[] VariableOutputs { get; }
    public Type ComponentType { get; }
}