using Microsoft.Extensions.DependencyInjection;
using Strem.Core.DI;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Integrations;
using Strem.Flows.Extensions;
using Strem.OBS.v4.Services.Client;

namespace Strem.OBS.v4.Plugin;

public class OBSModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, OBSPluginStartup>();
        
        // OBS Client
        services.AddSingleton<IObservableOBSClient, ObservableOBSClient>();
        
        // Register Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Integrations
        services.AddSingleton<IIntegrationDescriptor, OBSIntegrationDescriptor>();
    }
}