using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.StreamElements.Services.Client;
using Strem.StreamElements.Variables;

namespace Strem.StreamElements.Flows.Triggers;

public class OnStreamElementsRedemptionTrigger : FlowTrigger<OnStreamElementsRedemptionTriggerData>
{
    public override string Code => OnStreamElementsRedemptionTriggerData.TriggerCode;
    public override string Version => OnStreamElementsRedemptionTriggerData.TriggerVersion;

    public static VariableEntry RaidChannelVariable = new("raid.channel", StreamElementsVars.StreamElementsContext);
    
    public override string Name => "On StreamElements Redemption";
    public override string Category => "StreamElements";
    public override string Description => "Triggers when a store item is redeemed in chat";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        RaidChannelVariable.ToDescriptor()
    };

    public IObservableStreamElementsClient StreamElementsClient { get; }
    
    public OnStreamElementsRedemptionTrigger(ILogger<FlowTrigger<OnStreamElementsRedemptionTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableStreamElementsClient client) : base(logger, flowStringProcessor, appState, eventBus)
    {
        StreamElementsClient = client;
    }

    public override bool CanExecute() => StreamElementsClient.Client.IsConnected;

    public IVariables PopulateVariables(string arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(RaidChannelVariable, arg);
        return flowVars;
    }

    public override async Task<IObservable<IVariables>> Execute(OnStreamElementsRedemptionTriggerData data)
    {
        return StreamElementsClient.OnUnknownSimpleUpdate
            .Select(PopulateVariables);
    }
}