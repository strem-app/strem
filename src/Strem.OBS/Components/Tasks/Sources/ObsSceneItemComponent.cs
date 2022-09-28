using Microsoft.AspNetCore.Components;
using Obs.v5.WebSocket.Reactive;
using Strem.Flows.Components.Tasks;
using Strem.Flows.Data.Tasks;
using Strem.OBS.Extensions;

namespace Strem.OBS.Components.Tasks.Sources;

public abstract class ObsSceneItemComponent<T> : CustomTaskComponent<T> where T : class, IFlowTaskData
{
    public IReadOnlyCollection<string> SceneItems { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<string> SceneNames { get; set; } = Array.Empty<string>();

    [Inject]
    public IObservableOBSWebSocket ObsClient { get; protected set; }
    
    public void RefreshSceneNames()
    {
        SceneNames = ObsClient.GetSceneList()?.Scenes.Select(x => x.Name).ToArray() ?? Array.Empty<string>();
    }

    public void RefreshSceneItems(string sceneName)
    {
        if(string.IsNullOrEmpty(sceneName)) { return; }
        SceneItems = ObsClient.GetAllSceneItems(sceneName);
        InvokeAsync(StateHasChanged);
    }
    
    protected override async Task OnInitializedAsync()
    {
        if (ObsClient.IsConnected)
        { RefreshSceneNames(); }

        await base.OnInitializedAsync();
    }
}
