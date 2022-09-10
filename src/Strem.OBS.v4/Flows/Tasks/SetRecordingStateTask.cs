using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.v4.Services.Client;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Tasks;

public class SetRecordingStateTask : FlowTask<SetRecordingStateTaskData>
{
    public override string Code => SetRecordingStateTaskData.TaskCode;
    public override string Version => SetRecordingStateTaskData.TaskVersion;
    
    public override string Name => "Set Recording State";
    public override string Category => "OBS v4";
    public override string Description => "Starts or stops recording on OBS";

    public IObservableOBSClient ObsClient { get; }

    public SetRecordingStateTask(ILogger<FlowTask<SetRecordingStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost() && ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetRecordingStateTaskData data, IVariables flowVars)
    {
        if (data.StartRecording) { await ObsClient.StartRecording(); }
        else { await ObsClient.StopRecording(); }

        return ExecutionResult.Success();
    }
}