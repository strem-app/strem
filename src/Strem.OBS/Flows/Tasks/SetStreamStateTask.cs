using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.Extensions;
using Strem.OBS.Services.Client;

namespace Strem.OBS.Flows.Tasks;

public class SetStreamStateTask : FlowTask<SetStreamStateTaskData>
{
    public override string Code => SetStreamStateTaskData.TaskCode;
    public override string Version => SetStreamStateTaskData.TaskVersion;
    
    public override string Name => "Set Streaming State";
    public override string Category => "OBS v4";
    public override string Description => "Starts or stops streaming on OBS";

    public IObservableOBSClient ObsClient { get; }

    public SetStreamStateTask(ILogger<FlowTask<SetStreamStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
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