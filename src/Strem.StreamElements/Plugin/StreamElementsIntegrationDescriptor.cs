using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Variables;
using Strem.StreamElements.Components.Integrations;

namespace Strem.StreamElements.Plugin;

public class StreamElementsIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "Stream Elements Integration";
    public string Code => "stream-elements-integration";

    public VariableDescriptor[] VariableOutputs { get; } = Array.Empty<VariableDescriptor>();

    public Type ComponentType { get; } = typeof(StreamElementsIntegrationComponent);
}