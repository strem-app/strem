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
using Strem.OBS.v4.Services.Client;
using Strem.OBS.v4.Variables;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Triggers;

public class OnStreamingStateChangedTrigger : FlowTrigger<OnStreamingStateChangedTriggerData>
{
    public override string Code => OnStreamingStateChangedTriggerData.TriggerCode;
    public override string Version => OnStreamingStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Streaming State Changed";
    public override string Category => "OBS v4";
    public override string Description => "Triggers when OBS starts or stops streaming";

    public static VariableEntry ObsIsStreamingVariable = new("is-streaming", OBSVars.OBSContext);
    public static VariableEntry ObsStreamingStateVariable = new("streaming-state", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsIsStreamingVariable.ToDescriptor(), ObsStreamingStateVariable.ToDescriptor()
    };

    public IObservableOBSClient ObsClient { get; }

    public OnStreamingStateChangedTrigger(ILogger<FlowTrigger<OnStreamingStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<IObservable<IVariables>> Execute(OnStreamingStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnStreamingStateChanged;
        if (data.TriggerOnStart)
        {
            var isStreaming = await GetInitialValue(data);
            var args = new OutputStateChangedEventArgs { OutputState = isStreaming ? OutputState.Started : OutputState.Stopped  };
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
            .Where(x => x.OutputState is OutputState.Started or OutputState.Stopped)
            .Where(x => MatchesEvent(data, x))
            .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnStreamingStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        var result = await ObsClient.GetStreamingStatus();
        return result.IsStreaming;
    }

    public bool MatchesEvent(OnStreamingStateChangedTriggerData data, OutputStateChangedEventArgs args)
    {
        var isStreaming = args.OutputState == OutputState.Started;
        if(data.TriggerOnStarted && isStreaming) { return true; }
        if(data.TriggerOnStopped && !isStreaming) { return true; }
        return false;
    }
    
    public IVariables PopulateVariables(OutputStateChangedEventArgs args)
    {
        var isStreaming = args.OutputState == OutputState.Started;
        var variables = new Core.Variables.Variables();
        variables.Set(ObsIsStreamingVariable, isStreaming);
        variables.Set(ObsStreamingStateVariable, isStreaming ? "Streaming" : "Not Streaming");
        return variables;
    }
}