using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Executors;
using Strem.Flows.Extensions;
using Strem.Flows.Processors;
using Strem.OBS.Extensions;
using Strem.OBS.Types;

namespace Strem.OBS.Flows.Tasks;

public class SetInputVolumeTask : FlowTask<SetInputVolumeTaskData>
{
    public override string Code => SetInputVolumeTaskData.TaskCode;
    public override string Version => SetInputVolumeTaskData.TaskVersion;
    
    public override string Name => "Set Input Volume";
    public override string Category => "OBS";
    public override string Description => "Sets the given volume input percentage between 0-100";

    public IObservableOBSWebSocket ObsClient { get; }

    public SetInputVolumeTask(ILogger<FlowTask<SetInputVolumeTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<ExecutionResult> Execute(SetInputVolumeTaskData data, IVariables flowVars)
    {
        if (!FlowStringProcessor.TryProcessInt(data.Volume, flowVars, out var requestedVolume))
        { return ExecutionResult.Failed("Cannot process value provided for volume"); }

        if (requestedVolume is < 0 or > 100)
        { return ExecutionResult.Failed("Volume is not between 0-100"); }
        
        var currentVolume = ObsClient.GetInputVolume(data.InputName);
        var volumePercentage = currentVolume.VolumeDb.DecibelToPercent();
        if(volumePercentage == requestedVolume) { return ExecutionResult.Success(); }

        var volumeDecibels = requestedVolume.PercentToDecibel();
        ObsClient.SetInputVolume(data.InputName, volumeDecibels, true);
        return ExecutionResult.Success();
    }
}