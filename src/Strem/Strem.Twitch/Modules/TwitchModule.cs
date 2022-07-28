using Strem.Core.DI;
using Strem.Twitch.Services;

namespace Strem.Twitch.Modules;

public class TwitchModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        services.AddSingleton<ITwitchOAuthClient, TwitchOAuthClient>();
    }
}