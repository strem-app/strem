using Strem.Core.DI;
using Strem.Core.Plugins;
using Strem.Twitch.Plugin;
using Strem.Twitch.Services;
using Strem.Twitch.Services.OAuth;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Modules;

public class TwitchModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, TwitchPluginStartup>();
        
        // General
        services.AddSingleton<ITwitchClient, TwitchClient>();
        services.AddSingleton<ITwitchAPI, TwitchAPI>();
        
        // OAuth
        services.AddSingleton<ITwitchOAuthClient, TwitchOAuthClient>();
    }
}