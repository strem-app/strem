using Strem.Core.DI;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries.Integrations;
using Strem.Core.Plugins;
using Strem.Infrastructure.Services.Api;
using Strem.Twitch.Plugin;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Services.OAuth;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Strem.Twitch.Modules;

public class TwitchModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, TwitchPluginStartup>();
        
        // General
        services.AddSingleton<ITwitchAPI, TwitchAPI>();
        services.AddSingleton<ITwitchClient>(CreateTwitchClient);
        services.AddSingleton<IObservableTwitchClient, ObservableTwitchClient>();
        
        // OAuth
        services.AddSingleton<ITwitchOAuthClient, TwitchOAuthClient>();
        
        // Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Integration Components
        services.AddSingleton<IIntegrationDescriptor, TwitchIntegrationDescriptor>();
    }

    public ITwitchClient CreateTwitchClient(IServiceProvider services)
    {
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        var socketClient = new WebSocketClient(clientOptions);
        return new TwitchClient(socketClient, ClientProtocol.WebSocket, services.GetService<ILogger<TwitchClient>>());
    }
}