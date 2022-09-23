using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.OBS.Extensions;

namespace Strem.OBS.Flows.Tasks;

public class SetRecordingStateTask : FlowTask<SetRecordingStateTaskData>
{
    public override string Code => SetRecordingStateTaskData.TaskCode;
    public override string Version => SetRecordingStateTaskData.TaskVersion;
    
    public override string Name => "Set Recording State";
    public override string Category => "OBS";
    public override string Description => "Starts or stops recording on OBS";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetRecordingStateTask(ILogger<FlowTask<SetRecordingStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost() && ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetRecordingStateTaskData data, IVariables flowVars)
    {
        if (data.StartRecording) { ObsClient.StartRecord(); }
        else { ObsClient.StopRecord(); }

        return ExecutionResult.Success();
    }
}