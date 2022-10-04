using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Events;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Services.Stores;

namespace Strem.Flows.Default.Flows.Tasks.Flows;

public class EnableFlowTask : FlowTask<EnableFlowTaskData>
{
    public override string Code => EnableFlowTaskData.TaskCode;
    public override string Version => EnableFlowTaskData.TaskVersion;
    
    public override string Name => "Enable Flow";
    public override string Category => "Flows";
    public override string Description => "Enables a flow if it is disabled";

    public IFlowStore FlowStore { get; }

    public EnableFlowTask(ILogger<FlowTask<EnableFlowTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowStore flowStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowStore = flowStore;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(EnableFlowTaskData data, IVariables flowVars)
    {
        var hasFlow = FlowStore.Has(data.FlowId);
        if(!hasFlow) { return ExecutionResult.Failed($"Cannot enable flow as it does not exist: {data.FlowId}"); }

        var flow = FlowStore.Get(data.FlowId)!;
        if(flow.Enabled) { return ExecutionResult.FailedButContinue($"Flow is already enabled for: {data.FlowId}"); }

        flow.Enabled = true;
        EventBus.PublishAsync(new FlowEnabledEvent(flow.Id));
        return ExecutionResult.Success();
    }
}