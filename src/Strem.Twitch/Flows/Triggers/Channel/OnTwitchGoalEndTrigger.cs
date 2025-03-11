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

public class OnTwitchGoalEndTrigger : FlowTrigger<OnTwitchGoalEndTriggerData>, IUsesTwitchEventSub<OnTwitchGoalEndTriggerData>
{
    public override string Code => OnTwitchGoalEndTriggerData.TriggerCode;
    public override string Version => OnTwitchGoalEndTriggerData.TriggerVersion;

    public static VariableEntry GoalChannelVariable = new("goal.channel", TwitchVars.Context);
    public static VariableEntry GoalTypeVariable = new("goal.type", TwitchVars.Context);
    public static VariableEntry GoalStartedAtVariable = new("goal.started-at", TwitchVars.Context);
    public static VariableEntry GoalEndedAtVariable = new("goal.ended-at", TwitchVars.Context);
    public static VariableEntry GoalCurrentAmountVariable = new("goal.current-amount", TwitchVars.Context);
    public static VariableEntry GoalTotalAmountVariable = new("goal.total-amount", TwitchVars.Context);
    public static VariableEntry GoalAchievedVariable = new("goal.achieved", TwitchVars.Context);
    
    public override string Name => "On Twitch Goal Ended";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel goal ends";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        GoalChannelVariable.ToDescriptor(), GoalTypeVariable.ToDescriptor(), 
        GoalStartedAtVariable.ToDescriptor(), GoalEndedAtVariable.ToDescriptor(),
        GoalCurrentAmountVariable.ToDescriptor(), GoalTotalAmountVariable.ToDescriptor(),
        GoalAchievedVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchGoalEndTrigger(ILogger<FlowTrigger<OnTwitchGoalEndTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelGoals);

    public IVariables PopulateVariables(ChannelGoalEndArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(GoalChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(GoalTypeVariable, arg.Notification.Payload.Event.Type);
        flowVars.Set(GoalStartedAtVariable, arg.Notification.Payload.Event.StartedAt);
        flowVars.Set(GoalEndedAtVariable, arg.Notification.Payload.Event.EndedAt);
        flowVars.Set(GoalCurrentAmountVariable, arg.Notification.Payload.Event.CurrentAmount);
        flowVars.Set(GoalTotalAmountVariable, arg.Notification.Payload.Event.TargetAmount);
        flowVars.Set(GoalAchievedVariable, arg.Notification.Payload.Event.IsAchieved);
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchGoalEndTriggerData data, ChannelGoalEndArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchGoalEndTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelGoalEnd, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchGoalEndTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelGoalEnd
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}