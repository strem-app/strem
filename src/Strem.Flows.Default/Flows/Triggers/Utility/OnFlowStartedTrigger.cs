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

namespace Strem.Flows.Default.Flows.Triggers.Utility;

public class OnFlowStartedTrigger : FlowTrigger<OnFlowStartedTriggerData>
{
    public override string Code => OnFlowStartedTriggerData.TriggerCode;
    public override string Version => OnFlowStartedTriggerData.TriggerVersion;

    public static VariableEntry FlowNameVariableEntry = new("flow.name");

    public override string Name => "On Flow Started";
    public override string Category => "Utility";
    public override string Description => "Triggers when the matching flow starts executing";
    public override VariableDescriptor[] VariableOutputs { get; } = new[] { FlowNameVariableEntry.ToDescriptor() };

    public IFlowStore FlowStore { get; }
    
    public OnFlowStartedTrigger(ILogger<FlowTrigger<OnFlowStartedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IFlowStore flowStore) : base(logger, flowStringProcessor, appState, eventBus)
    {
        FlowStore = flowStore;
    }

    public override bool CanExecute() => true;

    public override async Task<IObservable<IVariables>> Execute(OnFlowStartedTriggerData data)
    {
        return EventBus.Receive<FlowStartedEvent>()
            .Where(x => x.FlowId == data.FlowId)
            .Select(x =>
            {
                var newVariables = new Core.Variables.Variables();
                var flow = FlowStore.Get(x.FlowId);
                newVariables.Set(FlowNameVariableEntry, flow!.Name);
                return newVariables;
            });
    }
}