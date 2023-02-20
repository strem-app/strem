using Microsoft.Extensions.DependencyInjection;
using StreamElements.WebSocket;
using StreamElements.WebSocket.Reactive;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Integrations;
using Strem.Flows.Extensions;
namespace Strem.StreamElements.Plugin;

public class StreamElementsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, StreamElementsPluginStartup>();
        
        // General
        services.AddSingleton<IStreamElementsClient>(new StreamElementsClient());
        services.AddSingleton<IObservableStreamElementsClient, ObservableStreamElementsClient>();
        
        // Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
        // Integration Components
        services.AddSingleton<IIntegrationDescriptor, StreamElementsIntegrationDescriptor>();
    }
}