using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet.Types;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.Extensions;
using Strem.OBS.Services.Client;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetSourceVisibilityTask : FlowTask<SetSourceVisibilityTaskData>
{
    public override string Code => SetSourceVisibilityTaskData.TaskCode;
    public override string Version => SetSourceVisibilityTaskData.TaskVersion;
    
    public override string Name => "Set Source Visibility";
    public override string Category => "OBS";
    public override string Description => "Sets the visibility of a given source";

    public IObservableOBSClient ObsClient { get; }

    public SetSourceVisibilityTask(ILogger<FlowTask<SetSourceVisibilityTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<bool> Execute(SetSourceVisibilityTaskData data, IVariables flowVars)
    {
        var sceneName = string.IsNullOrEmpty(data.SceneName) ? AppState.GetCurrentSceneName() : data.SceneName;
        var properties = await ObsClient.GetSceneItemProperties(data.SourceName, sceneName);
        if (properties == null) { return false; }
        
        if (data.Status == VisibilityStatus.Visible) { properties.Visible = true; }
        else if(data.Status == VisibilityStatus.Invisible) { properties.Visible = false; }
        else if (data.Status == VisibilityStatus.Toggle)
        { properties.Visible = !properties.Visible; }
        
        await ObsClient.SetSceneItemProperties(properties, sceneName);
        return true;
    }
}