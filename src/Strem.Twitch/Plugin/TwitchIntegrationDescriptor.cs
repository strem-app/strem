using Strem.Core.Extensions;
using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Variables;
using Strem.Twitch.Components.Integrations;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Plugin;

public class TwitchIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "Twitch Integration";
    public string Code => "twitch-integration";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        TwitchVars.Username.ToDescriptor(), TwitchVars.UserId.ToDescriptor(),
        TwitchVars.ChannelTitle.ToDescriptor(), TwitchVars.ChannelGame.ToDescriptor(),
        TwitchVars.StreamViewers.ToDescriptor(false), TwitchVars.StreamThumbnailUrl.ToDescriptor(false),
    };

    public Type ComponentType { get; } = typeof(TwitchIntegrationComponent);
}