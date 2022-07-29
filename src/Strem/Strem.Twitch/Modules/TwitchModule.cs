using Strem.Core.DI;
using Strem.Twitch.Services;
using Strem.Twitch.Services.OAuth;

namespace Strem.Twitch.Modules;

public class TwitchModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        services.AddSingleton<ITwitchOAuthClient, TwitchOAuthClient>();
    }
}