using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet.Types;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.OBS.Extensions;
using Strem.OBS.Services.Client;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetSourceMuteStateTask : FlowTask<SetSourceMuteStateTaskData>
{
    public override string Code => SetSourceMuteStateTaskData.TaskCode;
    public override string Version => SetSourceMuteStateTaskData.TaskVersion;
    
    public override string Name => "Set Source Mute State";
    public override string Category => "OBS v4";
    public override string Description => "Sets if a given source should be muted or not";

    public IObservableOBSClient ObsClient { get; }

    public SetSourceMuteStateTask(ILogger<FlowTask<SetSourceMuteStateTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
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