using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Extensions;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Registries.Integrations;
using Strem.Core.Flows.Registries.Menus;
using Strem.Core.Flows.Registries.Tasks;
using Strem.Core.Flows.Registries.Triggers;

namespace Strem.Application;

public class ExecutionEngineHandler
{
    public void StartRegistries(IServiceProvider services)
    {
        var taskRegistry = services.GetService<ITaskRegistry>();
        var taskDescriptors = services.GetServices<TaskDescriptor>();
        taskRegistry?.AddMany(taskDescriptors);
        
        var triggerRegistry = services.GetService<ITriggerRegistry>();
        var triggerDescriptors = services.GetServices<TriggerDescriptor>();
        triggerRegistry?.AddMany(triggerDescriptors);

        var integrationRegistry = services.GetService<IIntegrationRegistry>();
        var integrationDescriptors = services.GetServices<IIntegrationDescriptor>();
        integrationRegistry?.AddMany(integrationDescriptors);
        
        var menuRegistry = services.GetService<IMenuRegistry>();
        var menuDescriptors = services.GetServices<MenuDescriptor>();
        menuRegistry?.AddMany(menuDescriptors);
    }
    
    public async Task StartExecutionEngine(IServiceProvider services)
    {
        var executionEngine = services.GetService<IFlowExecutionEngine>();
        StartRegistries(services);
        await executionEngine?.StartEngine();
    }
}