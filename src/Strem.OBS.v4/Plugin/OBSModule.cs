using Microsoft.Extensions.DependencyInjection;
using Obs.v4.WebSocket.Reactive;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Integrations;
using Strem.Flows.Extensions;

namespace Strem.OBS.v4.Plugin;

public class OBSModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, OBSPluginStartup>();
        
        // OBS Client
        services.AddSingleton<IObservableOBSWebSocket, ObservableOBSWebSocket>();
        
        // Register Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Integrations
        services.AddSingleton<IIntegrationDescriptor, OBSIntegrationDescriptor>();
    }
}