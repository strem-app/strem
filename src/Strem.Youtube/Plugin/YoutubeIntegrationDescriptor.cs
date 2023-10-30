using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Variables;
using Strem.Youtube.Components.Integrations;

namespace Strem.Youtube.Plugin;

public class YoutubeIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "Youtube Integration";
    public string Code => "youtube-integration";

    public VariableDescriptor[] VariableOutputs { get; } = Array.Empty<VariableDescriptor>();

    public Type ComponentType { get; } = typeof(YoutubeIntegrationComponent);
}