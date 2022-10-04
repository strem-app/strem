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

public class DisableFlowTask : FlowTask<DisableFlowTaskData>
{
    public override string Code => DisableFlowTaskData.TaskCode;
    public override string Version => DisableFlowTaskData.TaskVersion;
    
    public override string Name => "Disable Flow";
    public override string Category => "Flows";
    public override string Description => "Disables a flow if it is enabled";

    public IFlowStore FlowStore { get; }

    public DisableFlowTask(ILogger<FlowTask<DisableFlowTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowStore flowStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowStore = flowStore;
    }

    public override bool CanExecute() => true;

    public override async Task<ExecutionResult> Execute(DisableFlowTaskData data, IVariables flowVars)
    {
        var hasFlow = FlowStore.Has(data.FlowId);
        if(!hasFlow) { return ExecutionResult.Failed($"Cannot enable flow as it does not exist: {data.FlowId}"); }

        var flow = FlowStore.Get(data.FlowId)!;
        if(!flow.Enabled) { return ExecutionResult.FailedButContinue($"Flow is already disabled for: {data.FlowId}"); }

        flow.Enabled = false;
        EventBus.PublishAsync(new FlowDisabledEvent(flow.Id));
        return ExecutionResult.Success();
    }
}