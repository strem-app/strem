using Microsoft.AspNetCore.Components;
using Obs.v5.WebSocket.Reactive;
using Strem.Flows.Components.Tasks;
using Strem.Flows.Data.Tasks;

namespace Strem.OBS.Components;

public abstract class ObsSceneComponent<T> : CustomTaskComponent<T> where T : class, IFlowTaskData
{
    public IReadOnlyCollection<string> SceneNames { get; set; } = Array.Empty<string>();

    [Inject]
    public IObservableOBSWebSocket ObsClient { get; protected set; }
    
    public void RefreshSceneNames()
    { 
        SceneNames = ObsClient.GetSceneList()
            ?.Scenes.Select(x => x.Name).ToArray() ?? Array.Empty<string>();
    }
    
    protected override async Task OnInitializedAsync()
    {
        if (ObsClient.IsConnected)
        { RefreshSceneNames(); }

        await base.OnInitializedAsync();
    }
}