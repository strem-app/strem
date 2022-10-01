using Strem.Core.Extensions;
using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Variables;
using Strem.Twitter.Components.Integrations;
using Strem.Twitter.Variables;

namespace Strem.Twitter.Plugin;

public class TwitterIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "Twitter Integration";
    public string Code => "twitter-integration";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        TwitterVars.Username.ToDescriptor()
    };

    public Type ComponentType { get; } = typeof(TwitterIntegrationComponent);
}