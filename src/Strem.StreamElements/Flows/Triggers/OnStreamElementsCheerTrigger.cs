using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using StreamElements.WebSocket.Models.Cheer;
using StreamElements.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.StreamElements.Variables;

namespace Strem.StreamElements.Flows.Triggers;

public class OnStreamElementsCheerTrigger : FlowTrigger<OnStreamElementsCheerTriggerData>
{
    public override string Code => OnStreamElementsCheerTriggerData.TriggerCode;
    public override string Version => OnStreamElementsCheerTriggerData.TriggerVersion;

    public static VariableEntry CheerUsernameVariable = new("cheer.username", StreamElementsVars.Context);
    public static VariableEntry CheerMessageVariable = new("cheer.message", StreamElementsVars.Context);
    public static VariableEntry CheerAmountVariable = new("cheer.amount", StreamElementsVars.Context);
    public static VariableEntry CheerDisplayNameVariable = new("cheer.displayname", StreamElementsVars.Context);
    
    public override string Name => "On StreamElements Cheer";
    public override string Category => "StreamElements";
    public override string Description => "Triggers when you receive a cheer";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        CheerUsernameVariable.ToDescriptor(), CheerMessageVariable.ToDescriptor(), 
        CheerAmountVariable.ToDescriptor(), CheerDisplayNameVariable.ToDescriptor()
    };

    public IObservableStreamElementsClient StreamElementsClient { get; }
    
    public OnStreamElementsCheerTrigger(ILogger<FlowTrigger<OnStreamElementsCheerTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableStreamElementsClient client) : base(logger, flowStringProcessor, appState, eventBus)
    {
        StreamElementsClient = client;
    }

    public override bool CanExecute() => StreamElementsClient.WebSocketClient.IsConnected;

    public IVariables PopulateVariables(Cheer arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(CheerUsernameVariable, arg.Username);
        flowVars.Set(CheerMessageVariable, arg.Message);
        flowVars.Set(CheerAmountVariable, arg.Amount);
        flowVars.Set(CheerDisplayNameVariable, arg.DisplayName);
        return flowVars;
    }

    public override async Task<IObservable<IVariables>> Execute(OnStreamElementsCheerTriggerData data)
    {
        return StreamElementsClient.OnCheer
            .Select(PopulateVariables);
    }
}