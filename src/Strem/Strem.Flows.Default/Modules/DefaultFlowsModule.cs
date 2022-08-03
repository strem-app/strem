using Microsoft.Extensions.DependencyInjection;
using Strem.Core.DI;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries;
using Strem.Core.Flows.Tasks;
using Strem.Core.Flows.Triggers;
using Strem.Flows.Default.Components.Tasks;
using Strem.Flows.Default.Components.Triggers;
using Strem.Flows.Default.Flows.Tasks;
using Strem.Flows.Default.Flows.Tasks.Data;
using Strem.Flows.Default.Flows.Triggers;
using Strem.Flows.Default.Flows.Triggers.Data;

namespace Strem.Flows.Default.Modules;

public class DefaultFlowsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Tasks
        services.AddSingleton<FlowTask<WriteToLogTaskData>, WriteToLogTask>();
        
        // Task Descriptors
        services.AddTaskDescriptor<WriteToLogTaskData, WriteToLogTaskComponent>();
        
        // Triggers
        services.AddSingleton<FlowTrigger<OnIntervalTriggerData>, OnIntervalTrigger>();
        
        // Trigger Descriptors
        services.AddTriggerDescriptor<OnIntervalTriggerData, OnIntervalTriggerComponent>();
    }
}