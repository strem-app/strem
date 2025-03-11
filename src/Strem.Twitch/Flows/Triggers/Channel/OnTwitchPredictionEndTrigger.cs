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

public class OnTwitchPredictionEndTrigger : FlowTrigger<OnTwitchPredictionEndTriggerData>, IUsesTwitchEventSub<OnTwitchPredictionEndTriggerData>
{
    public override string Code => OnTwitchPredictionEndTriggerData.TriggerCode;
    public override string Version => OnTwitchPredictionEndTriggerData.TriggerVersion;

    public static VariableEntry PredictionChannelVariable = new("prediction.channel", TwitchVars.Context);
    public static VariableEntry PredictionTitleVariable = new("prediction.title", TwitchVars.Context);
    public static VariableEntry PredictionStartedAtVariable = new("prediction.started-at", TwitchVars.Context);
    public static VariableEntry PredictionEndedAtVariable = new("prediction.ends-at", TwitchVars.Context);
    public static VariableEntry PredictionChoicesVariable = new("prediction.choices", TwitchVars.Context);
    public static VariableEntry PredictionWinningIdVariable = new("prediction.winning-id", TwitchVars.Context);
    public static VariableEntry PredictionWinningTextVariable = new("prediction.winning-text", TwitchVars.Context);
    
    public override string Name => "On Twitch Prediction Ended";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel predication ends";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        PredictionChannelVariable.ToDescriptor(), PredictionTitleVariable.ToDescriptor(), 
        PredictionStartedAtVariable.ToDescriptor(), PredictionEndedAtVariable.ToDescriptor(),
        PredictionChoicesVariable.ToDescriptor(), PredictionWinningIdVariable.ToDescriptor(),
        PredictionWinningTextVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchPredictionEndTrigger(ILogger<FlowTrigger<OnTwitchPredictionEndTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelPredictions);

    public IVariables PopulateVariables(ChannelPredictionEndArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(PredictionChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(PredictionTitleVariable, arg.Notification.Payload.Event.Title);
        flowVars.Set(PredictionStartedAtVariable, arg.Notification.Payload.Event.StartedAt);
        flowVars.Set(PredictionEndedAtVariable, arg.Notification.Payload.Event.EndedAt);
        flowVars.Set(PredictionChoicesVariable, string.Join(",", arg.Notification.Payload.Event.Outcomes.Select(x => x.Title)));
        flowVars.Set(PredictionWinningIdVariable, arg.Notification.Payload.Event.WinningOutcomeId);

        var winningOutcome = arg.Notification.Payload.Event.Outcomes
            .SingleOrDefault(x => x.Id == arg.Notification.Payload.Event.WinningOutcomeId);
        flowVars.Set(PredictionWinningTextVariable, winningOutcome?.Title ?? "Unknown Outcome");
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchPredictionEndTriggerData data, ChannelPredictionEndArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchPredictionEndTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelPredictionEnd, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchPredictionEndTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelPredictionEnd
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}