using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Types.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.OBS.Extensions;
using Strem.OBS.Variables;

namespace Strem.OBS.Flows.Triggers;

public class OnInputVolumeChangedTrigger : FlowTrigger<OnInputVolumeChangedTriggerData>
{
    public override string Code => OnInputVolumeChangedTriggerData.TriggerCode;
    public override string Version => OnInputVolumeChangedTriggerData.TriggerVersion;

    public override string Name => "On Input Volume Changed";
    public override string Category => "OBS";
    public override string Description => "Triggers when an input's volume has been changed";

    public static readonly VariableEntry ObsInputNameVariable = new("input.name", OBSVars.OBSContext);
    public static readonly VariableEntry ObsInputVolumeDecibelValueVariable = new("input.volume.decibel", OBSVars.OBSContext);
    public static readonly VariableEntry ObsInputVolumePercentageValueVariable = new("input.volume.percentage", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsInputNameVariable.ToDescriptor(), ObsInputVolumeDecibelValueVariable.ToDescriptor(),
        ObsInputVolumePercentageValueVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnInputVolumeChangedTrigger(ILogger<FlowTrigger<OnInputVolumeChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<IObservable<IVariables>> Execute(OnInputVolumeChangedTriggerData data)
    {
        bool NameMatchesIfRequired(InputVolumeChangedEventArgs x) => x.Volume.InputName.Equals(data.InputName, StringComparison.OrdinalIgnoreCase);
     
        var observableChain = ObsClient.OnInputVolumeChanged.Throttle(TimeSpan.FromMilliseconds(500));
        if (data.TriggerOnStart)
        {
            var currentVolume = await GetInitialValue(data);
            var args = new InputVolumeChangedEventArgs(new InputVolume
            {
                InputName = data.InputName,
                InputVolumeMul = currentVolume
            });
            observableChain = observableChain.StartWith(args);
        }
        
        return observableChain
            .Where(NameMatchesIfRequired)
            .Select(PopulateVariables);
    }
    
    public async Task<float> GetInitialValue(OnInputVolumeChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return default; }
        var volume = ObsClient.GetInputVolume(data.InputName);
        return volume.VolumeMul;
    }
    
    public IVariables PopulateVariables(InputVolumeChangedEventArgs args)
    {
        var variables = new Core.Variables.Variables();
        variables.Set(ObsInputNameVariable, args.Volume.InputName);
        variables.Set(ObsInputVolumeDecibelValueVariable, args.Volume.InputVolumeDb);
        variables.Set(ObsInputVolumePercentageValueVariable, args.Volume.InputVolumeDb.DecibelToPercent());
        return variables;
    }
}