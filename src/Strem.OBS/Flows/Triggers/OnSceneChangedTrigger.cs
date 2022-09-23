using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Obs.v5.WebSocket.Reactive;
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

public class OnSceneChangedTrigger : FlowTrigger<OnSceneChangedTriggerData>
{
    public override string Code => OnSceneChangedTriggerData.TriggerCode;
    public override string Version => OnSceneChangedTriggerData.TriggerVersion;

    public override string Name => "On Scene Changed";
    public override string Category => "OBS";
    public override string Description => "Triggers when a scene has been changed, with optional name checking";

    public static VariableEntry ObsSourceVariable = new("scene.name", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsSourceVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnSceneChangedTrigger(ILogger<FlowTrigger<OnSceneChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost() && ObsClient.IsConnected;

    public override async Task<IObservable<IVariables>> Execute(OnSceneChangedTriggerData data)
    {
        bool NameMatchesIfRequired(ProgramSceneChangedEventArgs x) => string.IsNullOrEmpty(data.MatchingSceneName) || 
                                                                      x.SceneName.Equals(data.MatchingSceneName, StringComparison.OrdinalIgnoreCase);
        
        return ObsClient.OnCurrentProgramSceneChanged
            .Where(NameMatchesIfRequired )
            .Select(PopulateVariables);
    }
    
    public IVariables PopulateVariables(ProgramSceneChangedEventArgs args)
    {
        var variables = new Core.Variables.Variables();
        variables.Set(ObsSourceVariable, args.SceneName);
        return variables;
    }
}