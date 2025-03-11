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

public class OnTwitchPollStartTrigger : FlowTrigger<OnTwitchPollStartTriggerData>, IUsesTwitchEventSub<OnTwitchPollStartTriggerData>
{
    public override string Code => OnTwitchPollStartTriggerData.TriggerCode;
    public override string Version => OnTwitchPollStartTriggerData.TriggerVersion;

    public static VariableEntry PollChannelVariable = new("poll.channel", TwitchVars.Context);
    public static VariableEntry PollTitleVariable = new("poll.title", TwitchVars.Context);
    public static VariableEntry PollStartedAtVariable = new("poll.started-at", TwitchVars.Context);
    public static VariableEntry PollEndsAtVariable = new("poll.ends-at", TwitchVars.Context);
    public static VariableEntry PollChoicesVariable = new("poll.choices", TwitchVars.Context);
    public static VariableEntry PollBitsPerVoteVariable = new("poll.bits-per-vote", TwitchVars.Context);
    public static VariableEntry PollChannelPointsPerVoteVariable = new("poll.channel-points-per-vote", TwitchVars.Context);
    
    public override string Name => "On Twitch Poll Started";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel poll starts";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        PollChannelVariable.ToDescriptor(), PollTitleVariable.ToDescriptor(), 
        PollStartedAtVariable.ToDescriptor(), PollEndsAtVariable.ToDescriptor(),
        PollChoicesVariable.ToDescriptor(), PollBitsPerVoteVariable.ToDescriptor(),
        PollChannelPointsPerVoteVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchPollStartTrigger(ILogger<FlowTrigger<OnTwitchPollStartTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelPolls);

    public IVariables PopulateVariables(ChannelPollBeginArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(PollChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(PollTitleVariable, arg.Notification.Payload.Event.Title);
        flowVars.Set(PollStartedAtVariable, arg.Notification.Payload.Event.StartedAt);
        flowVars.Set(PollEndsAtVariable, arg.Notification.Payload.Event.EndsAt);
        flowVars.Set(PollChoicesVariable, string.Join(",", arg.Notification.Payload.Event.Choices.Select(x => x.Title)));
        flowVars.Set(PollBitsPerVoteVariable, arg.Notification.Payload.Event.BitsVoting.AmountPerVote);
        flowVars.Set(PollChannelPointsPerVoteVariable, arg.Notification.Payload.Event.ChannelPointsVoting.AmountPerVote);
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchPollStartTriggerData data, ChannelPollBeginArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchPollStartTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelPollBegin, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchPollStartTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelPollBegin
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}