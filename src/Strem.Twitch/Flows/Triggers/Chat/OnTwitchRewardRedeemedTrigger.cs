using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchRewardRedeemedTrigger : FlowTrigger<OnTwitchRewardRedeemedTriggerData>
{
    public override string Code => OnTwitchRewardRedeemedTriggerData.TriggerCode;
    public override string Version => OnTwitchRewardRedeemedTriggerData.TriggerVersion;

    public static VariableEntry RewardChannelVariable = new("reward.channel", TwitchVars.Context);
    public static VariableEntry RewardUsernameVariable = new("reward.username", TwitchVars.Context);
    public static VariableEntry RewardIdVariable = new("reward.id", TwitchVars.Context);
    public static VariableEntry RewardNameVariable = new("reward.name", TwitchVars.Context);
    
    public override string Name => "On Twitch Reward Redeemed";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a user redeems a channel point reward";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        RewardChannelVariable.ToDescriptor(), RewardUsernameVariable.ToDescriptor(), 
        RewardIdVariable.ToDescriptor(), RewardNameVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchRewardRedeemedTrigger(ILogger<FlowTrigger<OnTwitchRewardRedeemedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelRedemptions);

    public IVariables PopulateVariables(ChannelPointsCustomRewardRedemptionArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(RewardChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(RewardUsernameVariable, arg.Notification.Payload.Event.UserName);
        flowVars.Set(RewardNameVariable, arg.Notification.Payload.Event.Reward.Title);
        flowVars.Set(RewardIdVariable, arg.Notification.Payload.Event.Reward.Id);
        return flowVars;
    }

    public bool DoesRewardMeetCriteria(OnTwitchRewardRedeemedTriggerData data, ChannelPointsCustomRewardRedemptionArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        if(!string.IsNullOrEmpty(data.RequiredRewardName))
        { return string.Equals(args.Notification.Payload.Event.Reward.Title, data.RequiredRewardName, StringComparison.OrdinalIgnoreCase); }

        return true;
    }
    
    public async void SubscribeToEventIfNeeded(OnTwitchRewardRedeemedTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        if (!TwitchEventSub.HasSubscribedTo(EventSubTypes.ChannelPointsCustomRewardRedemptionAdd, processedChannel))
        { await TwitchEventSub.SubscribeOnChannel(EventSubTypes.ChannelPointsCustomRewardRedemptionAdd, processedChannel); }
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchRewardRedeemedTriggerData data)
    {
        SubscribeToEventIfNeeded(data);
        
        return TwitchEventSub.OnChannelPointsCustomRewardRedemptionAdd
            .Where(x => DoesRewardMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}