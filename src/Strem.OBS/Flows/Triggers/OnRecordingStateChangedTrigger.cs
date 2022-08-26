using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.Extensions;
using Strem.OBS.Services.Client;
using Strem.OBS.Variables;

namespace Strem.OBS.Flows.Triggers;

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

    public IObservableOBSClient ObsClient { get; }

    public OnRecordingStateChangedTrigger(ILogger<FlowTrigger<OnRecordingStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
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