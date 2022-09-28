using Microsoft.AspNetCore.Components;
using Obs.v5.WebSocket.Reactive;
using Strem.Flows.Components.Tasks;
using Strem.Flows.Data.Tasks;
using Strem.OBS.Extensions;
using Strem.OBS.Types;

namespace Strem.OBS.Components;

public abstract class ObsSceneItemComponent<T> : ObsSceneComponent<T> where T : class, IFlowTaskData
{
    public IReadOnlyCollection<string> SceneItems { get; set; } = Array.Empty<string>();
    public string AllowedSourceKind { get; set; } = SourceKindType.Any;

    [Inject]
    public IObservableOBSWebSocket ObsClient { get; protected set; }
    
    public void RefreshSceneItems(string sceneName)
    {
        if(string.IsNullOrEmpty(sceneName)) { return; }
        SceneItems = ObsClient.GetAllSceneItems(sceneName, AllowedSourceKind);
        InvokeAsync(StateHasChanged);
    }
}