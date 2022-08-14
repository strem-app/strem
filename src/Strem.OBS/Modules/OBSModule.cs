using Microsoft.Extensions.DependencyInjection;
using Strem.Core.DI;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries.Integrations;
using Strem.Core.Plugins;
using Strem.Infrastructure.Services.Api;
using Strem.OBS.Plugin;
using Strem.OBS.Services.Client;

namespace Strem.OBS.Modules;

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