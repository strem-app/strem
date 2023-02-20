using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using StreamElements.WebSocket.Models.Store;
using StreamElements.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.StreamElements.Variables;

namespace Strem.StreamElements.Flows.Triggers;

public class OnStreamElementsRedemptionTrigger : FlowTrigger<OnStreamElementsRedemptionTriggerData>
{
    public override string Code => OnStreamElementsRedemptionTriggerData.TriggerCode;
    public override string Version => OnStreamElementsRedemptionTriggerData.TriggerVersion;

    public static VariableEntry RedemptionUserVariable = new("redemption.user", StreamElementsVars.StreamElementsContext);
    public static VariableEntry RedemptionStoreItemVariable = new("redemption.item-name", StreamElementsVars.StreamElementsContext);
    public static VariableEntry RedemptionMessageVariable = new("redemption.message", StreamElementsVars.StreamElementsContext);
    public static VariableEntry RedemptionStoreItemIdVariable = new("redemption.item-id", StreamElementsVars.StreamElementsContext);
    
    public override string Name => "On StreamElements Redemption";
    public override string Category => "StreamElements";
    public override string Description => "Triggers when a store item is redeemed in chat";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        RedemptionUserVariable.ToDescriptor(), RedemptionStoreItemVariable.ToDescriptor(),
        RedemptionMessageVariable.ToDescriptor(), RedemptionStoreItemIdVariable.ToDescriptor()
    };

    public IObservableStreamElementsClient StreamElementsClient { get; }
    
    public OnStreamElementsRedemptionTrigger(ILogger<FlowTrigger<OnStreamElementsRedemptionTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableStreamElementsClient client) : base(logger, flowStringProcessor, appState, eventBus)
    {
        StreamElementsClient = client;
    }

    public override bool CanExecute() => StreamElementsClient.WebSocketClient.IsConnected;

    public IVariables PopulateVariables(StoreRedemption arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(RedemptionStoreItemVariable, arg.StoreItemName);
        flowVars.Set(RedemptionUserVariable, arg.Username);
        flowVars.Set(RedemptionMessageVariable, arg.Message);
        flowVars.Set(RedemptionStoreItemIdVariable, arg.ItemId);
        return flowVars;
    }

    public bool StoreItemMatches(StoreRedemption arg, OnStreamElementsRedemptionTriggerData data)
    {
        var processedInput = FlowStringProcessor.Process(data.StoreItemName, new Core.Variables.Variables());
        return arg.StoreItemName.MatchesText(data.NameMatchType, processedInput);
    }

    public override async Task<IObservable<IVariables>> Execute(OnStreamElementsRedemptionTriggerData data)
    {
        return StreamElementsClient.OnStoreRedemption
            .Where(x => StoreItemMatches(x, data))
            .Select(PopulateVariables);
    }
}