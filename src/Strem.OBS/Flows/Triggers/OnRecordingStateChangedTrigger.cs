using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
using OBSWebsocketDotNet.Types;
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

public class OnRecordingStateChangedTrigger : FlowTrigger<OnRecordingStateChangedTriggerData>
{
    public override string Code => OnRecordingStateChangedTriggerData.TriggerCode;
    public override string Version => OnRecordingStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Recording State Changed";
    public override string Category => "OBS";
    public override string Description => "Triggers when OBS starts or stops recording";

    public static VariableEntry ObsIsRecordingVariable = new("is-recording", OBSVars.OBSContext);
    public static VariableEntry ObsRecordingStateVariable = new("recording-state", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsIsRecordingVariable.ToDescriptor(), ObsRecordingStateVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnRecordingStateChangedTrigger(ILogger<FlowTrigger<OnRecordingStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<IObservable<IVariables>> Execute(OnRecordingStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnRecordStateChanged;
        if (data.TriggerOnStart)
        {
            var isRecording = await GetInitialValue(data);
            var args = new RecordStateChangedEventArgs(new RecordStateChanged(){ IsActive = isRecording });
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
            .Where(x => x.OutputState.State is OutputState.OBS_WEBSOCKET_OUTPUT_STARTED or OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED)
            .Where(x => MatchesEvent(data, x))
            .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnRecordingStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        return ObsClient.GetRecordStatus()?.IsRecording ?? false;
    }

    public bool MatchesEvent(OnRecordingStateChangedTriggerData data, RecordStateChangedEventArgs args)
    {
        var isRecording = args.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED;
        if(data.TriggerOnStarted && isRecording) { return true; }
        if(data.TriggerOnStopped && !isRecording) { return true; }
        return false;
    }
    
    public IVariables PopulateVariables(RecordStateChangedEventArgs args)
    {
        var isRecording = args.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED;
        var variables = new Core.Variables.Variables();
        variables.Set(ObsIsRecordingVariable, isRecording);
        variables.Set(ObsRecordingStateVariable, isRecording ? "Recording" : "Not Recording");
        return variables;
    }
}