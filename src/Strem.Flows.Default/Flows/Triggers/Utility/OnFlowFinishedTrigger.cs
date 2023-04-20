using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Events;
using Strem.Flows.Processors;
using Strem.Flows.Services.Stores;
using Strem.Flows.Types;

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnFlowFinishedTrigger : FlowTrigger<OnFlowFinishedTriggerData>
{
    public override string Code => OnFlowFinishedTriggerData.TriggerCode;
    public override string Version => OnFlowFinishedTriggerData.TriggerVersion;

    public static VariableEntry FlowNameVariableEntry = new("flow.name");

    public override string Name => "On Flow Finished";
    public override string Category => "Utility";
    public override string Description => "Triggers whenever the matching flow has finished executing";
    public override VariableDescriptor[] VariableOutputs { get; } = new[] { FlowNameVariableEntry.ToDescriptor() };

    public IFlowStore FlowStore { get; }
    
    public OnFlowFinishedTrigger(ILogger<FlowTrigger<OnFlowFinishedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowStore flowStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowStore = flowStore;
    }

    public override bool CanExecute() => true;

    public override async Task<IObservable<IVariables>> Execute(OnFlowFinishedTriggerData data)
    {
        return EventBus.Receive<FlowFinishedEvent>()
            .Where(x => x.FlowId == data.FlowId)
            .Where(x => MatchesExecutionState(x, data))
            .Select(x =>
            {
                var newVariables = new Core.Variables.Variables();
                var flow = FlowStore.Get(x.FlowId);
                newVariables.Set(FlowNameVariableEntry, flow!.Name);
                return newVariables;
            });
    }

    private bool MatchesExecutionState(FlowFinishedEvent eventData, OnFlowFinishedTriggerData flowData)
    {
        if (flowData.OnlyOnSuccessfulExecution)
        { return eventData.executionResultType is ExecutionResultType.Success or ExecutionResultType.FailedButContinue; }

        return true;
    }
}