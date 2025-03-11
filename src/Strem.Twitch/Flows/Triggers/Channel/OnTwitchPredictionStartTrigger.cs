using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Processors;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace Strem.Twitch.Flows.Triggers.Channel;

public class OnTwitchPredictionStartTrigger : FlowTrigger<OnTwitchPredictionStartTriggerData>, IUsesTwitchEventSub<OnTwitchPredictionStartTriggerData>
{
    public override string Code => OnTwitchPredictionStartTriggerData.TriggerCode;
    public override string Version => OnTwitchPredictionStartTriggerData.TriggerVersion;

    public static VariableEntry PredictionChannelVariable = new("prediction.channel", TwitchVars.Context);
    public static VariableEntry PredictionTitleVariable = new("prediction.title", TwitchVars.Context);
    public static VariableEntry PredictionStartedAtVariable = new("prediction.started-at", TwitchVars.Context);
    public static VariableEntry PredictionEndsAtVariable = new("prediction.ends-at", TwitchVars.Context);
    public static VariableEntry PredictionChoicesVariable = new("prediction.choices", TwitchVars.Context);
    
    public override string Name => "On Twitch Prediction Started";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel predication starts";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        PredictionChannelVariable.ToDescriptor(), PredictionTitleVariable.ToDescriptor(), 
        PredictionStartedAtVariable.ToDescriptor(), PredictionEndsAtVariable.ToDescriptor(),
        PredictionChoicesVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchPredictionStartTrigger(ILogger<FlowTrigger<OnTwitchPredictionStartTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelPredictions);

    public IVariables PopulateVariables(ChannelPredictionBeginArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(PredictionChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(PredictionTitleVariable, arg.Notification.Payload.Event.Title);
        flowVars.Set(PredictionStartedAtVariable, arg.Notification.Payload.Event.StartedAt);
        flowVars.Set(PredictionEndsAtVariable, arg.Notification.Payload.Event.LocksAt);
        flowVars.Set(PredictionChoicesVariable, string.Join(",", arg.Notification.Payload.Event.Outcomes.Select(x => x.Title)));
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchPredictionStartTriggerData data, ChannelPredictionBeginArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchPredictionStartTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelPredictionBegin, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchPredictionStartTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelPredictionBegin
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}