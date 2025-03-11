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

public class OnTwitchHypeTrainEndTrigger : FlowTrigger<OnTwitchHypeTrainEndTriggerData>, IUsesTwitchEventSub<OnTwitchHypeTrainEndTriggerData>
{
    public override string Code => OnTwitchHypeTrainEndTriggerData.TriggerCode;
    public override string Version => OnTwitchHypeTrainEndTriggerData.TriggerVersion;

    public static VariableEntry HypeChannelVariable = new("hypetrain.channel", TwitchVars.Context);
    public static VariableEntry HypeTrainEndedAtVariable = new("hypetrain.endedAt", TwitchVars.Context);
    public static VariableEntry HypeTrainCooldownEndsAtVariable = new("hypetrain.cooldownEndsAt", TwitchVars.Context);
    
    public override string Name => "On Twitch Hype Train Ended";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a hype train ends";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        HypeChannelVariable.ToDescriptor(), HypeTrainEndedAtVariable.ToDescriptor(),
        HypeTrainCooldownEndsAtVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchHypeTrainEndTrigger(ILogger<FlowTrigger<OnTwitchHypeTrainEndTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadChannelRedemptions);

    public IVariables PopulateVariables(ChannelHypeTrainEndArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(HypeChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(HypeTrainEndedAtVariable, arg.Notification.Payload.Event.EndedAt);
        flowVars.Set(HypeTrainCooldownEndsAtVariable, arg.Notification.Payload.Event.CooldownEndsAt);
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchHypeTrainEndTriggerData data, ChannelHypeTrainEndArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchHypeTrainEndTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        if (!TwitchEventSub.HasSubscribedTo(EventSubTypes.ChannelHypeTrainBegin, processedChannel))
        { await TwitchEventSub.SubscribeOnChannel(EventSubTypes.ChannelHypeTrainBegin, processedChannel); }
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchHypeTrainEndTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelHypeTrainEnd
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}