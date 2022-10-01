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

public class SetSourceMuteStateTask : FlowTask<SetSourceMuteStateTaskData>
{
    public override string Code => SetSourceMuteStateTaskData.TaskCode;
    public override string Version => SetSourceMuteStateTaskData.TaskVersion;
    
    public override string Name => "Set Source Mute State";
    public override string Category => "OBS";
    public override string Description => "Sets if a given source should be muted or not";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetSourceMuteStateTask(ILogger<FlowTask<SetSourceMuteStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetSourceMuteStateTaskData data, IVariables flowVars)
    {
        var muted = false;   
        if (data.Status == MuteStatus.Unmute) { muted = false; }
        else if(data.Status == MuteStatus.Mute) { muted = true; }
        else if(data.Status == MuteStatus.Toggle)
        {
            var isMuted = ObsClient.GetInputMute(data.SourceName);
            muted = !isMuted;
        }

        ObsClient.SetInputMute(data.SourceName, muted);
        return ExecutionResult.Success();
    }
}