using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.OBS.Extensions;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetSourceVisibilityTask : FlowTask<SetSourceVisibilityTaskData>
{
    public override string Code => SetSourceVisibilityTaskData.TaskCode;
    public override string Version => SetSourceVisibilityTaskData.TaskVersion;
    
    public override string Name => "Set Source Visibility";
    public override string Category => "OBS";
    public override string Description => "Sets the visibility of a given source";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetSourceVisibilityTask(ILogger<FlowTask<SetSourceVisibilityTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<ExecutionResult> Execute(SetSourceVisibilityTaskData data, IVariables flowVars)
    {
        var sceneItemId = ObsClient.GetSceneItemId(data.SceneName, data.SceneItemName, 0);

        var isVisible = false;        
        if (data.Status == VisibilityStatus.Visible) { isVisible = true; }
        else if(data.Status == VisibilityStatus.Invisible) { isVisible = false; }
        else if (data.Status == VisibilityStatus.Toggle)
        { isVisible = !ObsClient.GetSceneItemEnabled(data.SceneName, sceneItemId); }
        
        ObsClient.SetSceneItemEnabled(data.SceneName, sceneItemId, isVisible);
        return ExecutionResult.Success();
    }
}