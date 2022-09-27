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

public class SetStreamStateTask : FlowTask<SetStreamStateTaskData>
{
    public override string Code => SetStreamStateTaskData.TaskCode;
    public override string Version => SetStreamStateTaskData.TaskVersion;
    
    public override string Name => "Set Streaming State";
    public override string Category => "OBS";
    public override string Description => "Starts or stops streaming on OBS";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetStreamStateTask(ILogger<FlowTask<SetStreamStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetStreamStateTaskData data, IVariables flowVars)
    {
        if (data.StartStream) { ObsClient.StartStream(); }
        else { ObsClient.StopStream(); }

        return ExecutionResult.Success();
    }
}