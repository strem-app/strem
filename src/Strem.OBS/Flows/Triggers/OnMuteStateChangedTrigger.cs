using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
using OBSWebsocketDotNet.Types.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.OBS.Extensions;
using Strem.OBS.Variables;

namespace Strem.OBS.Flows.Triggers;

public class OnMuteStateChangedTrigger : FlowTrigger<OnMuteStateChangedTriggerData>
{
    public override string Code => OnMuteStateChangedTriggerData.TriggerCode;
    public override string Version => OnMuteStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Mute State Changed";
    public override string Category => "OBS";
    public override string Description => "Triggers when a mute state has changed on a specific source";

    public static VariableEntry ObsSourceVariable = new("source.name", OBSVars.OBSContext);
    public static VariableEntry ObsSourceMuteStateVariable = new("source.mute-state", OBSVars.OBSContext);
    public static VariableEntry ObsSourceIsMutedVariable = new("source.is-muted", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsSourceVariable.ToDescriptor(), ObsSourceMuteStateVariable.ToDescriptor(), ObsSourceIsMutedVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnMuteStateChangedTrigger(ILogger<FlowTrigger<OnMuteStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<IObservable<IVariables>> Execute(OnMuteStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnInputMuteStateChanged;
        if (data.TriggerOnStart)
        {
            var muteState = await GetInitialValue(data);
            var args = new InputMuteStateChangedEventArgs(data.SourceName, muteState);
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
                .Where(x => MatchesEvent(data, x))
                .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnMuteStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        return ObsClient.GetInputMute(data.SourceName);
    }

    public bool MatchesEvent(OnMuteStateChangedTriggerData data, InputMuteStateChangedEventArgs args)
    {
        if(!args.InputName.Equals(data.SourceName, StringComparison.OrdinalIgnoreCase))
        { return false; }
                
        if(data.TriggerOnMuted && args.InputMuted) { return true; }
        if(data.TriggerOnUnmuted && !args.InputMuted) { return true; }

        return false;
    }
    
    public IVariables PopulateVariables(InputMuteStateChangedEventArgs args)
    {
        var variables = new Core.Variables.Variables();
        variables.Set(ObsSourceVariable, args.InputName);
        variables.Set(ObsSourceIsMutedVariable, args.InputMuted);
        variables.Set(ObsSourceMuteStateVariable, args.InputMuted ? "Muted" : "Unmuted");
        return variables;
    }
}