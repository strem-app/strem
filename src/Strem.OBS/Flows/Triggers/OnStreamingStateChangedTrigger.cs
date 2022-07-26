﻿using System.Reactive.Linq;
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

public class OnStreamingStateChangedTrigger : FlowTrigger<OnStreamingStateChangedTriggerData>
{
    public override string Code => OnStreamingStateChangedTriggerData.TriggerCode;
    public override string Version => OnStreamingStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Streaming State Changed";
    public override string Category => "OBS";
    public override string Description => "Triggers when OBS starts or stops streaming";

    public static VariableEntry ObsIsStreamingVariable = new("is-streaming", OBSVars.OBSContext);
    public static VariableEntry ObsStreamingStateVariable = new("streaming-state", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsIsStreamingVariable.ToDescriptor(), ObsStreamingStateVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnStreamingStateChangedTrigger(ILogger<FlowTrigger<OnStreamingStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<IObservable<IVariables>> Execute(OnStreamingStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnStreamStateChanged;
        if (data.TriggerOnStart)
        {
            var isStreaming = await GetInitialValue(data);
            var args = new StreamStateChangedEventArgs(new OutputStateChanged { IsActive = isStreaming });
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
            .Where(x => x.OutputState.State is OutputState.OBS_WEBSOCKET_OUTPUT_STARTED or OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED)
            .Where(x => MatchesEvent(data, x))
            .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnStreamingStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        return ObsClient.GetStreamStatus()?.IsActive ?? false;
    }

    public bool MatchesEvent(OnStreamingStateChangedTriggerData data, StreamStateChangedEventArgs args)
    {
        var isStreaming = args.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED;
        if(data.TriggerOnStarted && isStreaming) { return true; }
        if(data.TriggerOnStopped && !isStreaming) { return true; }
        return false;
    }
    
    public IVariables PopulateVariables(StreamStateChangedEventArgs args)
    {
        var isStreaming = args.OutputState.State == OutputState.OBS_WEBSOCKET_OUTPUT_STARTED;
        var variables = new Core.Variables.Variables();
        variables.Set(ObsIsStreamingVariable, isStreaming);
        variables.Set(ObsStreamingStateVariable, isStreaming ? "Streaming" : "Not Streaming");
        return variables;
    }
}