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
using TwitchLib.Client.Events;

namespace Strem.Twitch.Flows.Triggers.Subs;

public class OnTwitchGiftSubTrigger : FlowTrigger<OnTwitchGiftSubTriggerData>
{
    public override string Code => OnTwitchGiftSubTriggerData.TriggerCode;
    public override string Version => OnTwitchGiftSubTriggerData.TriggerVersion;

    public static VariableEntry GiftSubChannelVariable = new("gift.channel", TwitchVars.Context);
    public static VariableEntry GiftSubGifterVariable = new("gift.gifter", TwitchVars.Context);
    public static VariableEntry GiftSubReceiverVariable = new("gift.receiver", TwitchVars.Context);
    public static VariableEntry GiftSubMonthsVariable = new("gift.months", TwitchVars.Context);
    public static VariableEntry GiftSubTierVariable = new("gift.tier", TwitchVars.Context);
    public static VariableEntry GiftSubTierNameVariable = new("gift.tier-name", TwitchVars.Context);
    
    public override string Name => "On Twitch Gift Sub";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a sub is gifted to a user for a channel";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        GiftSubChannelVariable.ToDescriptor(), GiftSubGifterVariable.ToDescriptor(), 
        GiftSubReceiverVariable.ToDescriptor(), GiftSubMonthsVariable.ToDescriptor(),
        GiftSubTierVariable.ToDescriptor(), GiftSubTierNameVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchGiftSubTrigger(ILogger<FlowTrigger<OnTwitchGiftSubTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.ReadWhispers);

    public IVariables PopulateVariables(OnGiftedSubscriptionArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(GiftSubChannelVariable, arg.Channel);
        flowVars.Set(GiftSubGifterVariable, arg.GiftedSubscription.DisplayName);
        flowVars.Set(GiftSubReceiverVariable, arg.GiftedSubscription.MsgParamRecipientDisplayName);
        flowVars.Set(GiftSubMonthsVariable, arg.GiftedSubscription.MsgParamMonths);
        flowVars.Set(GiftSubTierVariable, arg.GiftedSubscription.MsgParamSubPlan.ToString());
        flowVars.Set(GiftSubTierNameVariable, arg.GiftedSubscription.MsgParamSubPlanName);
        return flowVars;
    }

    public bool DoesMessageMeetCriteria(OnTwitchGiftSubTriggerData data, OnGiftedSubscriptionArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Channel, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchGiftSubTriggerData data)
    {
        return TwitchClient.OnGiftedSubscription
            .Where(x => DoesMessageMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}