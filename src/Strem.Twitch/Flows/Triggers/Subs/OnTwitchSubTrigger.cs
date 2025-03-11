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

public class OnTwitchSubTrigger : FlowTrigger<OnTwitchSubTriggerData>
{
    public override string Code => OnTwitchSubTriggerData.TriggerCode;
    public override string Version => OnTwitchSubTriggerData.TriggerVersion;

    public static VariableEntry SubChannelVariable = new("sub.channel", TwitchVars.Context);
    public static VariableEntry SubReceiverVariable = new("sub.receiver", TwitchVars.Context);
    public static VariableEntry SubMonthsVariable = new("sub.months", TwitchVars.Context);
    public static VariableEntry SubTierVariable = new("sub.tier", TwitchVars.Context);
    public static VariableEntry SubTierNameVariable = new("sub.tier-name", TwitchVars.Context);
    
    public override string Name => "On Twitch Sub";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a user subscribes to a channel";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        SubChannelVariable.ToDescriptor(), SubReceiverVariable.ToDescriptor(), 
        SubMonthsVariable.ToDescriptor(), SubTierVariable.ToDescriptor(),
        SubTierNameVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchSubTrigger(ILogger<FlowTrigger<OnTwitchSubTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.ReadChat);

    public IVariables PopulateVariables(OnReSubscriberArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(SubChannelVariable, arg.Channel);
        flowVars.Set(SubReceiverVariable, arg.ReSubscriber.DisplayName);
        flowVars.Set(SubMonthsVariable, arg.ReSubscriber.Months);
        flowVars.Set(SubTierVariable, arg.ReSubscriber.SubscriptionPlan.ToString());
        flowVars.Set(SubTierNameVariable, arg.ReSubscriber.SubscriptionPlanName);
        return flowVars;
    }

    public bool DoesMessageMeetCriteria(OnTwitchSubTriggerData data, OnReSubscriberArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Channel, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchSubTriggerData data)
    {
        return TwitchClient.OnReSubscriber
            .Where(x => DoesMessageMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}