using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using StreamElements.WebSocket.Models.Tip;
using StreamElements.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.StreamElements.Variables;

namespace Strem.StreamElements.Flows.Triggers;

public class OnStreamElementsTipTrigger : FlowTrigger<OnStreamElementsTipTriggerData>
{
    public override string Code => OnStreamElementsTipTriggerData.TriggerCode;
    public override string Version => OnStreamElementsTipTriggerData.TriggerVersion;

    public static VariableEntry TipUsernameVariable = new("tip.username", StreamElementsVars.StreamElementsContext);
    public static VariableEntry TipMessageVariable = new("tip.message", StreamElementsVars.StreamElementsContext);
    public static VariableEntry TipAmountVariable = new("tip.amount", StreamElementsVars.StreamElementsContext);
    public static VariableEntry TipCurrencyVariable = new("tip.currency", StreamElementsVars.StreamElementsContext);
    
    public override string Name => "On StreamElements Tip";
    public override string Category => "StreamElements";
    public override string Description => "Triggers when you receive a tip via StreamElements";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        TipUsernameVariable.ToDescriptor(), TipMessageVariable.ToDescriptor(), 
        TipAmountVariable.ToDescriptor(), TipCurrencyVariable.ToDescriptor()
    };

    public IObservableStreamElementsClient StreamElementsClient { get; }
    
    public OnStreamElementsTipTrigger(ILogger<FlowTrigger<OnStreamElementsTipTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableStreamElementsClient client) : base(logger, flowStringProcessor, appState, eventBus)
    {
        StreamElementsClient = client;
    }

    public override bool CanExecute() => StreamElementsClient.WebSocketClient.IsConnected;

    public IVariables PopulateVariables(Tip arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(TipUsernameVariable, arg.Username);
        flowVars.Set(TipMessageVariable, arg.Amount);
        flowVars.Set(TipAmountVariable, arg.Message);
        flowVars.Set(TipCurrencyVariable, arg.Currency);
        return flowVars;
    }

    public override async Task<IObservable<IVariables>> Execute(OnStreamElementsTipTriggerData data)
    {
        return StreamElementsClient.OnTip
            .Select(PopulateVariables);
    }
}