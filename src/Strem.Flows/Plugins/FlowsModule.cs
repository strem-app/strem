using Strem.Core.DI;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Data;
using Strem.Flows.Data;
using Strem.Flows.Executors;
using Strem.Flows.Executors.Logging;
using Strem.Flows.Processors;
using Strem.Flows.Services.Data;
using Strem.Flows.Services.Registries.Tasks;
using Strem.Flows.Services.Registries.Triggers;
using Strem.Flows.Services.Repositories;
using Strem.Flows.Services.Stores;

namespace Strem.Flows.Plugins;

public class FlowsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, FlowsPluginStartup>();
        
        // Processors
        services.AddSingleton<IFlowStringProcessor, FlowStringProcessor>();
        services.AddSingleton<ICommandStringProcessor, CommandStringProcessor>();
        
        // Registries
        services.AddSingleton<ITaskRegistry, TaskRegistry>();
        services.AddSingleton<ITriggerRegistry, TriggerRegistry>();
        
        // Execution
        services.AddSingleton<ITaskExecutor, TaskExecutor>();
        services.AddSingleton<ITriggerExecutor, TriggerExecutor>();
        services.AddSingleton<IFlowExecutionLogger, FlowExecutionLogger>();
        services.AddSingleton<IFlowExecutionEngine, IFlowExecutor, FlowExecutionEngine>();
        
        // Data
        services.AddSingleton<IFlowRepository, IRepository<Flow, Guid>, FlowRepository>();
        services.AddSingleton<IFlowStore, FlowStore>();
        services.AddSingleton<IFlowImporter, FlowImporter>();
        services.AddSingleton<IFlowExporter, FlowExporter>();
    }
}