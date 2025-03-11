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

namespace Strem.Twitch.Flows.Triggers.Subs;

public class OnTwitchChannelFollowedTrigger : FlowTrigger<OnTwitchChannelFollowedTriggerData>, IUsesTwitchEventSub<OnTwitchChannelFollowedTriggerData>
{
    public override string Code => OnTwitchChannelFollowedTriggerData.TriggerCode;
    public override string Version => OnTwitchChannelFollowedTriggerData.TriggerVersion;

    public static VariableEntry FollowChannelVariable = new("follow.channel", TwitchVars.Context);
    public static VariableEntry FollowUsernameVariable = new("follow.username", TwitchVars.Context);
    public static VariableEntry FollowAtVariable = new("follow.at", TwitchVars.Context);
    
    public override string Name => "On Twitch Channel Followed";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a user follows a channel";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        FollowChannelVariable.ToDescriptor(), FollowUsernameVariable.ToDescriptor(), 
        FollowAtVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchChannelFollowedTrigger(ILogger<FlowTrigger<OnTwitchChannelFollowedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadUsersFollows);

    public IVariables PopulateVariables(ChannelFollowArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(FollowChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(FollowUsernameVariable, arg.Notification.Payload.Event.UserName);
        flowVars.Set(FollowAtVariable, arg.Notification.Payload.Event.FollowedAt);
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchChannelFollowedTriggerData data, ChannelFollowArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchChannelFollowedTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelFollow, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchChannelFollowedTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelFollow
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}