using Strem.Core.Variables;

namespace Strem.Core.Services.Registries.Integrations;

public interface IIntegrationDescriptor
{
    public string Title { get; }
    public string Code { get; }
    public VariableDescriptor[] VariableOutputs { get; }
    public Type ComponentType { get; }
}