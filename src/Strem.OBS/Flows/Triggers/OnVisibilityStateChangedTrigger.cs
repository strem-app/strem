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

    public static VariableEntry ObsSceneNameVariable = new("source.scene-name", OBSVars.OBSContext);
    public static VariableEntry ObsSourceNameVariable = new("source.name", OBSVars.OBSContext);
    public static VariableEntry ObsSourceNameFullVariable = new("source.name-full", OBSVars.OBSContext);
    public static VariableEntry ObsSourceMuteStateVariable = new("source.visibility-state", OBSVars.OBSContext);
    public static VariableEntry ObsSourceIsMutedVariable = new("source.is-visible", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsSceneNameVariable.ToDescriptor(), ObsSourceNameVariable.ToDescriptor(), ObsSourceNameFullVariable.ToDescriptor(),
        ObsSourceMuteStateVariable.ToDescriptor(), ObsSourceIsMutedVariable.ToDescriptor()
    };

    public IObservableOBSWebSocket ObsClient { get; }

    public OnVisibilityStateChangedTrigger(ILogger<FlowTrigger<OnVisibilityStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSWebSocket obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => ObsClient.IsConnected;

    public override async Task<IObservable<IVariables>> Execute(OnVisibilityStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SceneItemName) || string.IsNullOrEmpty(data.SceneName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnSceneItemEnableStateChanged;
        var sceneItemInformation = ObsClient.GetSceneOrGroupItemId(data.SceneName, data.SceneItemName);

        if (data.TriggerOnStart)
        {
            var visibilityState = await GetInitialValue(data);
            var args = new SceneItemEnableStateChangedEventArgs(data.SceneName, sceneItemInformation.itemId, visibilityState);
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
            .Where(x => MatchesEvent(data, x, sceneItemInformation.parentScene, sceneItemInformation.itemId))
            .Select(x => PopulateVariables(x, data));
    }

    public async Task<bool> GetInitialValue(OnVisibilityStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        var sceneInformation = ObsClient.GetSceneOrGroupItemId(data.SceneName, data.SceneItemName);
        return ObsClient.GetSceneItemEnabled(sceneInformation.parentScene, sceneInformation.itemId);
    }

    public bool MatchesEvent(OnVisibilityStateChangedTriggerData data, SceneItemEnableStateChangedEventArgs args, string parentSceneName, int sceneItemId)
    {
        if(args.SceneName != parentSceneName) { return false; }
        if(args.SceneItemId != sceneItemId) { return false; }
        if(data.TriggerOnVisible && args.SceneItemEnabled) { return true; }
        if(data.TriggerOnInvisible && !args.SceneItemEnabled) { return true; }
        return false;
    }
    
    public IVariables PopulateVariables(SceneItemEnableStateChangedEventArgs args, OnVisibilityStateChangedTriggerData data)
    {
        var variables = new Core.Variables.Variables();
        var splitNames = data.SceneItemName.Split(IObservableOBSWebSocketExtensions.NestedSceneItemSeparator);
        variables.Set(ObsSceneNameVariable, data.SceneName);
        variables.Set(ObsSourceNameVariable, splitNames.Last());
        variables.Set(ObsSourceNameFullVariable, data.SceneItemName);
        variables.Set(ObsSourceIsMutedVariable, args.SceneItemEnabled);
        variables.Set(ObsSourceMuteStateVariable, args.SceneItemEnabled ? "Visible" : "Invisible");
        return variables;
    }
}