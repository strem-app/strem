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

public class OnVisibilityStateChangedTrigger : FlowTrigger<OnVisibilityStateChangedTriggerData>
{
    public override string Code => OnVisibilityStateChangedTriggerData.TriggerCode;
    public override string Version => OnVisibilityStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Visibility State Changed";
    public override string Category => "OBS";
    public override string Description => "Triggers when the visibility state has changed on a specific source";

    public static VariableEntry ObsSourceVariable = new("source.name", OBSVars.OBSContext);
    public static VariableEntry ObsSourceMuteStateVariable = new("source.visibility-state", OBSVars.OBSContext);
    public static VariableEntry ObsSourceIsMutedVariable = new("source.is-visible", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsSourceVariable.ToDescriptor(), ObsSourceMuteStateVariable.ToDescriptor(), ObsSourceIsMutedVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnVisibilityStateChangedTrigger(ILogger<FlowTrigger<OnVisibilityStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<IObservable<IVariables>> Execute(OnVisibilityStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnSceneItemEnableStateChanged;
        var sceneName = AppState.GetCurrentSceneName();
        var sceneItemId = ObsClient.GetSceneItemId(sceneName, data.SourceName, 0);

        if (data.TriggerOnStart)
        {
            var visibilityState = await GetInitialValue(data);
            var args = new SceneItemEnableStateChangedEventArgs(sceneName, sceneItemId, visibilityState);
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
            .Where(x => MatchesEvent(data, x, sceneItemId))
            .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnVisibilityStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        var sceneName = AppState.GetCurrentSceneName();
        var sceneItemId = ObsClient.GetSceneItemId(sceneName, data.SourceName, 0);
        return ObsClient.GetSceneItemEnabled(sceneName, sceneItemId);
    }

    public bool MatchesEvent(OnVisibilityStateChangedTriggerData data, SceneItemEnableStateChangedEventArgs args, int sceneItemId)
    {
        if(args.SceneItemId != sceneItemId)
        { return false; }
                
        if(data.TriggerOnVisible && args.SceneItemEnabled) { return true; }
        if(data.TriggerOnInvisible && !args.SceneItemEnabled) { return true; }
        return false;
    }
    
    public IVariables PopulateVariables(SceneItemEnableStateChangedEventArgs args)
    {
        var sceneName = AppState.GetCurrentSceneName();
        var sceneItems = ObsClient.GetSceneItemList(sceneName);
        var sourceName = sceneItems.FirstOrDefault(x => x.ItemId == args.SceneItemId)?.SourceName ?? string.Empty;
        var variables = new Core.Variables.Variables();
        variables.Set(ObsSourceVariable, sourceName);
        variables.Set(ObsSourceIsMutedVariable, args.SceneItemEnabled);
        variables.Set(ObsSourceMuteStateVariable, args.SceneItemEnabled ? "Visible" : "Invisible");
        return variables;
    }
}