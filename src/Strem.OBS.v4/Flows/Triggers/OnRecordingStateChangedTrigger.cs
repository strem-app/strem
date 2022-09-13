using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v4.WebSocket;
using Obs.v4.WebSocket.Reactive;
using Obs.v4.WebSocket.Types;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.v4.Variables;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Triggers;

public class OnRecordingStateChangedTrigger : FlowTrigger<OnRecordingStateChangedTriggerData>
{
    public override string Code => OnRecordingStateChangedTriggerData.TriggerCode;
    public override string Version => OnRecordingStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Recording State Changed";
    public override string Category => "OBS v4";
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
        
        var observableChain = ObsClient.OnRecordingStateChanged;
        if (data.TriggerOnStart)
        {
            var isRecording = await GetInitialValue(data);
            var args = new OutputStateChangedEventArgs { OutputState = isRecording ? OutputState.Started : OutputState.Stopped  };
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
            .Where(x => x.OutputState is OutputState.Started or OutputState.Stopped)
            .Where(x => MatchesEvent(data, x))
            .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnRecordingStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        var result = await ObsClient.GetStreamingStatus();
        return result.IsRecording;
    }

    public bool MatchesEvent(OnRecordingStateChangedTriggerData data, OutputStateChangedEventArgs args)
    {
        var isRecording = args.OutputState == OutputState.Started;
        if(data.TriggerOnStarted && isRecording) { return true; }
        if(data.TriggerOnStopped && !isRecording) { return true; }
        return false;
    }
    
    public IVariables PopulateVariables(OutputStateChangedEventArgs args)
    {
        var isRecording = args.OutputState == OutputState.Started;
        var variables = new Core.Variables.Variables();
        variables.Set(ObsIsRecordingVariable, isRecording);
        variables.Set(ObsRecordingStateVariable, isRecording ? "Recording" : "Not Recording");
        return variables;
    }
}