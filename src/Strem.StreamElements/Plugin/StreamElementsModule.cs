using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;

namespace Strem.StreamElements.Plugin;

public class StreamElementsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, StreamElementsPluginStartup>();
        
        /*
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
        */
    }
}