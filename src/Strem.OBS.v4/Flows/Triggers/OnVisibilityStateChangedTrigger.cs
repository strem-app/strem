using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.OBS.v4.Services.Client;
using Strem.OBS.v4.Variables;
using Strem.OBS.v4.Extensions;

namespace Strem.OBS.v4.Flows.Triggers;

public class OnVisibilityStateChangedTrigger : FlowTrigger<OnVisibilityStateChangedTriggerData>
{
    public override string Code => OnVisibilityStateChangedTriggerData.TriggerCode;
    public override string Version => OnVisibilityStateChangedTriggerData.TriggerVersion;

    public override string Name => "On Visibility State Changed";
    public override string Category => "OBS v4";
    public override string Description => "Triggers when the visibility state has changed on a specific source";

    public static VariableEntry ObsSourceVariable = new("source.name", OBSVars.OBSContext);
    public static VariableEntry ObsSourceMuteStateVariable = new("source.visibility-state", OBSVars.OBSContext);
    public static VariableEntry ObsSourceIsMutedVariable = new("source.is-visible", OBSVars.OBSContext);

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ObsSourceVariable.ToDescriptor(), ObsSourceMuteStateVariable.ToDescriptor(), ObsSourceIsMutedVariable.ToDescriptor()
    };

    public IObservableOBSClient ObsClient { get; }

    public OnVisibilityStateChangedTrigger(ILogger<FlowTrigger<OnVisibilityStateChangedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableOBSClient obsClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        ObsClient = obsClient;
    }

    public override bool CanExecute() => AppState.HasOBSHost();

    public override async Task<IObservable<IVariables>> Execute(OnVisibilityStateChangedTriggerData data)
    {
        if(string.IsNullOrEmpty(data.SourceName))
        { return Observable.Empty<IVariables>(); }
        
        var observableChain = ObsClient.OnSceneItemVisibilityChanged;
        if (data.TriggerOnStart)
        {
            var visibilityState = await GetInitialValue(data);
            var args = new SceneItemVisibilityChangedEventArgs { ItemName = data.SourceName, IsVisible = visibilityState };
            observableChain = observableChain.StartWith(args);
        }

        return observableChain
            .Where(x => MatchesEvent(data, x))
            .Select(PopulateVariables);
    }

    public async Task<bool> GetInitialValue(OnVisibilityStateChangedTriggerData data)
    {
        if(!ObsClient.IsConnected) { return false; }
        var sceneName = AppState.GetCurrentSceneName();
        var sceneItemProperties = await ObsClient.GetSceneItemProperties(data.SourceName, sceneName);
        return sceneItemProperties.Visible;
    }

    public bool MatchesEvent(OnVisibilityStateChangedTriggerData data, SceneItemVisibilityChangedEventArgs args)
    {
        if(!args.ItemName.Equals(data.SourceName, StringComparison.OrdinalIgnoreCase))
        { return false; }
                
        if(data.TriggerOnVisible && args.IsVisible) { return true; }
        if(data.TriggerOnInvisible && !args.IsVisible) { return true; }
        return false;
    }
    
    public IVariables PopulateVariables(SceneItemVisibilityChangedEventArgs args)
    {
        var variables = new Core.Variables.Variables();
        variables.Set(ObsSourceVariable, args.ItemName);
        variables.Set(ObsSourceIsMutedVariable, args.IsVisible);
        variables.Set(ObsSourceMuteStateVariable, args.IsVisible ? "Visible" : "Invisible");
        return variables;
    }
}