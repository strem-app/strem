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

public class SetSourceMuteStateTask : FlowTask<SetSourceMuteStateTaskData>
{
    public override string Code => SetSourceMuteStateTaskData.TaskCode;
    public override string Version => SetSourceMuteStateTaskData.TaskVersion;
    
    public override string Name => "Set Source Mute State";
    public override string Category => "OBS v4";
    public override string Description => "Sets if a given source should be muted or not";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetSourceMuteStateTask(ILogger<FlowTask<SetSourceMuteStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<ExecutionResult> Execute(SetSourceMuteStateTaskData data, IVariables flowVars)
    {
        var muted = false;   
        if (data.Status == MuteStatus.Unmute) { muted = false; }
        else if(data.Status == MuteStatus.Mute) { muted = true; }
        else if(data.Status == MuteStatus.Toggle)
        {
            var isMuted = await ObsClient.GetMute(data.SourceName);
            muted = !isMuted;
        }

        await ObsClient.SetMute(data.SourceName, muted);
        return ExecutionResult.Success();
    }
}