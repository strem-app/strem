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

public class OnTwitchPollEndTrigger : FlowTrigger<OnTwitchPollEndTriggerData>, IUsesTwitchEventSub<OnTwitchPollEndTriggerData>
{
    public override string Code => OnTwitchPollEndTriggerData.TriggerCode;
    public override string Version => OnTwitchPollEndTriggerData.TriggerVersion;

    public static VariableEntry PollChannelVariable = new("poll.channel", TwitchVars.Context);
    public static VariableEntry PollTitleVariable = new("poll.title", TwitchVars.Context);
    public static VariableEntry PollStartedAtVariable = new("poll.started-at", TwitchVars.Context);
    public static VariableEntry PollEndedAtVariable = new("poll.ended-at", TwitchVars.Context);
    public static VariableEntry PollChoicesVariable = new("poll.choices", TwitchVars.Context);
    public static VariableEntry PollBitsPerVoteVariable = new("poll.bits-per-vote", TwitchVars.Context);
    public static VariableEntry PollChannelPointsPerVoteVariable = new("poll.channel-points-per-vote", TwitchVars.Context);
    public static VariableEntry PollStatusVariable = new("poll.status", TwitchVars.Context);
    
    public override string Name => "On Twitch Poll Ended";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel poll ends";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        PollChannelVariable.ToDescriptor(), PollTitleVariable.ToDescriptor(), 
        PollStartedAtVariable.ToDescriptor(), PollEndedAtVariable.ToDescriptor(),
        PollChoicesVariable.ToDescriptor(), PollBitsPerVoteVariable.ToDescriptor(),
        PollChannelPointsPerVoteVariable.ToDescriptor(), PollStatusVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchPollEndTrigger(ILogger<FlowTrigger<OnTwitchPollEndTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelPolls);

    public IVariables PopulateVariables(ChannelPollEndArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(PollChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(PollTitleVariable, arg.Notification.Payload.Event.Title);
        flowVars.Set(PollStartedAtVariable, arg.Notification.Payload.Event.StartedAt);
        flowVars.Set(PollEndedAtVariable, arg.Notification.Payload.Event.EndedAt);
        flowVars.Set(PollChoicesVariable, string.Join(",", arg.Notification.Payload.Event.Choices.Select(x => x.Title)));
        flowVars.Set(PollBitsPerVoteVariable, arg.Notification.Payload.Event.BitsVoting.AmountPerVote);
        flowVars.Set(PollChannelPointsPerVoteVariable, arg.Notification.Payload.Event.ChannelPointsVoting.AmountPerVote);
        flowVars.Set(PollStatusVariable, arg.Notification.Payload.Event.Status);
        
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchPollEndTriggerData data, ChannelPollEndArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchPollEndTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelPollEnd, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchPollEndTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelPollEnd
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}