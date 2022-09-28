using Microsoft.AspNetCore.Components;
using Obs.v5.WebSocket.Reactive;
using OBSWebsocketDotNet.Types;
using Strem.Flows.Components.Tasks;
using Strem.Flows.Data.Tasks;
using Strem.OBS.Extensions;
using Strem.OBS.Types;

namespace Strem.OBS.Components;

public abstract class ObsInputComponent<T> : CustomTaskComponent<T> where T : class, IFlowTaskData
{
    public IReadOnlyCollection<string> InputNames { get; set; } = Array.Empty<string>();
    public string AllowedSourceKind { get; set; } = SourceKindType.Any;

    [Inject]
    public IObservableOBSWebSocket ObsClient { get; protected set; }

    private bool MatchesSourceKind(InputBasicInfo info)
    {
        if (AllowedSourceKind == SourceKindType.Any)
        { return true; }

        if (AllowedSourceKind == SourceKindType.CaptureSource && !info.IsCaptureSource())
        { return false; }

        return info.InputKind == AllowedSourceKind;
    }
    
    public void RefreshInputs()
    {
        InputNames = ObsClient.GetInputList()?.Where(MatchesSourceKind).Select(x => x.InputName).ToArray() ?? Array.Empty<string>();
    }
    
    protected override async Task OnInitializedAsync()
    {
        if (ObsClient.IsConnected)
        { RefreshInputs(); }

        await base.OnInitializedAsync();
    }
}