﻿using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Integrations;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services.Api;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Services.OAuth;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.EventSub.Websockets.Extensions;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Interfaces;

namespace Strem.Twitch.Plugin;

public class TwitchModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, TwitchPluginStartup>();
        
        // General
        services.AddSingleton<ITwitchAPI, TwitchAPI>();
        services.AddSingleton<ITwitchClient>(CreateTwitchClient);
        services.AddSingleton<ITwitchPubSub, TwitchPubSub>();
        services.AddSingleton<IObservableTwitchClient, ObservableTwitchClient>();
        services.AddSingleton<IObservableTwitchPubSub, ObservableTwitchPubSub>();

        services.AddTwitchLibEventSubWebsockets();
        services.AddSingleton<IObservableTwitchEventSub, ObservableTwitchEventSub>();
        
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