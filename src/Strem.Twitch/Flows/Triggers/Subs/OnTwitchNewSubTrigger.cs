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
using TwitchLib.Client.Events;

namespace Strem.Twitch.Flows.Triggers.Subs;

public class OnTwitchNewSubTrigger : FlowTrigger<OnTwitchNewSubTriggerData>
{
    public override string Code => OnTwitchNewSubTriggerData.TriggerCode;
    public override string Version => OnTwitchNewSubTriggerData.TriggerVersion;
    
    public override string Name => "On Twitch First Time Sub";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a user subscribes to a channel for the first time";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        OnTwitchSubTrigger.SubChannelVariable.ToDescriptor(), OnTwitchSubTrigger.SubReceiverVariable.ToDescriptor(), 
        OnTwitchSubTrigger.SubMonthsVariable.ToDescriptor(), OnTwitchSubTrigger.SubTierVariable.ToDescriptor(),
        OnTwitchSubTrigger.SubTierNameVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchNewSubTrigger(ILogger<FlowTrigger<OnTwitchNewSubTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.ReadChat);

    public IVariables PopulateVariables(OnNewSubscriberArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(OnTwitchSubTrigger.SubChannelVariable, arg.Channel);
        flowVars.Set(OnTwitchSubTrigger.SubReceiverVariable, arg.Subscriber.DisplayName);
        flowVars.Set(OnTwitchSubTrigger.SubMonthsVariable, 0);
        flowVars.Set(OnTwitchSubTrigger.SubTierVariable, arg.Subscriber.SubscriptionPlan.ToString());
        flowVars.Set(OnTwitchSubTrigger.SubTierNameVariable, arg.Subscriber.SubscriptionPlanName);
        return flowVars;
    }

    public bool DoesMessageMeetCriteria(OnTwitchNewSubTriggerData data, OnNewSubscriberArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Channel, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchNewSubTriggerData data)
    {
        return TwitchClient.OnNewSubscriber
            .Where(x => DoesMessageMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}