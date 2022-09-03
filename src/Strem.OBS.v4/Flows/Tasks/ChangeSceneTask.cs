using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.v4.Services.Client;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Tasks;

public class ChangeSceneTask : FlowTask<ChangeSceneTaskData>
{
    public override string Code => ChangeSceneTaskData.TaskCode;
    public override string Version => ChangeSceneTaskData.TaskVersion;
    
    public override string Name => "Change Scene";
    public override string Category => "OBS v4";
    public override string Description => "Changes the active scene in OBS";

    public IObservableOBSClient ObsClient { get; }

    public ChangeSceneTask(ILogger<FlowTask<ChangeSceneTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost() && ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(ChangeSceneTaskData data, IVariables flowVars)
    {
        if(string.IsNullOrEmpty(data.NewScene))
        { return ExecutionResult.Failed("NewScene is empty and is required"); }
        
        await ObsClient.SetCurrentScene(data.NewScene);
        return ExecutionResult.Success();
    }
}
