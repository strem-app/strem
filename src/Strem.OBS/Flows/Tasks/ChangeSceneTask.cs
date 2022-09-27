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

public class ChangeSceneTask : FlowTask<ChangeSceneTaskData>
{
    public override string Code => ChangeSceneTaskData.TaskCode;
    public override string Version => ChangeSceneTaskData.TaskVersion;
    
    public override string Name => "Change Scene";
    public override string Category => "OBS";
    public override string Description => "Changes the active scene in OBS";

    public IObservableOBSWebSocket ObsClient { get; }

    public ChangeSceneTask(ILogger<FlowTask<ChangeSceneTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(ChangeSceneTaskData data, IVariables flowVars)
    {
        if(string.IsNullOrEmpty(data.NewScene))
        { return ExecutionResult.Failed("NewScene is empty and is required"); }
        
        ObsClient.SetCurrentProgramScene(data.NewScene);
        return ExecutionResult.Success();
    }
}
