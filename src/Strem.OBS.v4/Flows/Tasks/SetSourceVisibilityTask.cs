using Microsoft.Extensions.Logging;

using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Obs.v4.WebSocket.Reactive;
using Strem.OBS.v4.Types;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Tasks;

public class SetSourceVisibilityTask : FlowTask<SetSourceVisibilityTaskData>
{
    public override string Code => SetSourceVisibilityTaskData.TaskCode;
    public override string Version => SetSourceVisibilityTaskData.TaskVersion;
    
    public override string Name => "Set Source Visibility";
    public override string Category => "OBS v4";
    public override string Description => "Sets the visibility of a given source";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetSourceVisibilityTask(ILogger<FlowTask<SetSourceVisibilityTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<ExecutionResult> Execute(SetSourceVisibilityTaskData data, IVariables flowVars)
    {
        var sceneName = string.IsNullOrEmpty(data.SceneName) ? AppState.GetCurrentSceneName() : data.SceneName;
        var properties = await ObsClient.GetSceneItemProperties(data.SourceName, sceneName);
        if (properties == null) { return ExecutionResult.Failed("Cannot get scene properties"); }
        
        if (data.Status == VisibilityStatus.Visible) { properties.Visible = true; }
        else if(data.Status == VisibilityStatus.Invisible) { properties.Visible = false; }
        else if (data.Status == VisibilityStatus.Toggle)
        { properties.Visible = !properties.Visible; }
        
        await ObsClient.SetSceneItemProperties(properties, sceneName);
        return ExecutionResult.Success();
    }
}