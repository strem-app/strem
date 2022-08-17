using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Registries.Integrations;
using Strem.Core.Flows.Registries.Tasks;
using Strem.Core.Flows.Registries.Triggers;
using Strem.Core.Plugins;

namespace Strem;

public class PluginBootstrapper
{
    public IServiceProvider Services { get; }

    public PluginBootstrapper(IServiceProvider services)
    {
        Services = services;
    }

    public async Task StartAllPlugins(IServiceProvider services)
    {
        var startupServices = services.GetServices<IPluginStartup>();
        foreach (var startupService in startupServices)
        { await startupService.StartPlugin(); }
    }

    public async Task StartExecutionEngine(IServiceProvider services)
    {
        var executionEngine = services.GetService<IFlowExecutionEngine>();

        var taskRegistry = services.GetService<ITaskRegistry>();
        var taskDescriptors = services.GetServices<TaskDescriptor>();
        taskRegistry?.AddMany(taskDescriptors);
        
        var triggerRegistry = services.GetService<ITriggerRegistry>();
        var triggerDescriptors = services.GetServices<TriggerDescriptor>();
        triggerRegistry?.AddMany(triggerDescriptors);

        var integrationRegistry = services.GetService<IIntegrationRegistry>();
        var integrationDescriptors = services.GetServices<IIntegrationDescriptor>();
        integrationRegistry?.AddMany(integrationDescriptors);
        
        executionEngine?.StartEngine();
    }
}