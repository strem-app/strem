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

public class OnTwitchGoalStartTrigger : FlowTrigger<OnTwitchGoalStartTriggerData>, IUsesTwitchEventSub<OnTwitchGoalStartTriggerData>
{
    public override string Code => OnTwitchGoalStartTriggerData.TriggerCode;
    public override string Version => OnTwitchGoalStartTriggerData.TriggerVersion;

    public static VariableEntry GoalChannelVariable = new("goal.channel", TwitchVars.Context);
    public static VariableEntry GoalTypeVariable = new("goal.type", TwitchVars.Context);
    public static VariableEntry GoalStartedAtVariable = new("goal.started-at", TwitchVars.Context);
    public static VariableEntry GoalTotalAmountVariable = new("goal.total-amount", TwitchVars.Context);
    
    public override string Name => "On Twitch Goal Started";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel goal starts";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        GoalChannelVariable.ToDescriptor(), GoalTypeVariable.ToDescriptor(), 
        GoalStartedAtVariable.ToDescriptor(), GoalTotalAmountVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchGoalStartTrigger(ILogger<FlowTrigger<OnTwitchGoalStartTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelGoals);

    public IVariables PopulateVariables(ChannelGoalBeginArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(GoalChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(GoalTypeVariable, arg.Notification.Payload.Event.Type);
        flowVars.Set(GoalStartedAtVariable, arg.Notification.Payload.Event.StartedAt);
        flowVars.Set(GoalTotalAmountVariable, arg.Notification.Payload.Event.TargetAmount);
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchGoalStartTriggerData data, ChannelGoalBeginArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchGoalStartTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelGoalBegin, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchGoalStartTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelGoalBegin
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}