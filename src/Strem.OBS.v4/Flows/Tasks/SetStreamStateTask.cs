using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Obs.v4.WebSocket.Reactive;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Tasks;

public class SetStreamStateTask : FlowTask<SetStreamStateTaskData>
{
    public override string Code => SetStreamStateTaskData.TaskCode;
    public override string Version => SetStreamStateTaskData.TaskVersion;
    
    public override string Name => "Set Streaming State";
    public override string Category => "OBS v4";
    public override string Description => "Starts or stops streaming on OBS";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetStreamStateTask(ILogger<FlowTask<SetStreamStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost() && ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetStreamStateTaskData data, IVariables flowVars)
    {
        if (data.StartStream) { await ObsClient.StartStreaming(); }
        else { await ObsClient.StopStreaming(); }

        return ExecutionResult.Success();
    }
}