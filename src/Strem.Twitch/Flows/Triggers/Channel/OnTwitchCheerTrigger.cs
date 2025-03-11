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

public class OnTwitchCheerTrigger : FlowTrigger<OnTwitchCheerTriggerData>, IUsesTwitchEventSub<OnTwitchCheerTriggerData>
{
    public override string Code => OnTwitchCheerTriggerData.TriggerCode;
    public override string Version => OnTwitchCheerTriggerData.TriggerVersion;

    public static VariableEntry CheerChannelVariable = new("cheer.channel", TwitchVars.Context);
    public static VariableEntry CheerUsernameVariable = new("cheer.username", TwitchVars.Context);
    public static VariableEntry CheerAtVariable = new("cheer.at", TwitchVars.Context);
    public static VariableEntry CheerBitsVariable = new("cheer.bits", TwitchVars.Context);
    
    public override string Name => "On Twitch Channel Cheered";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a user cheers on a channel";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        CheerChannelVariable.ToDescriptor(), CheerUsernameVariable.ToDescriptor(), 
        CheerAtVariable.ToDescriptor()
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchCheerTrigger(ILogger<FlowTrigger<OnTwitchCheerTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadBits);

    public IVariables PopulateVariables(ChannelCheerArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(CheerChannelVariable, arg.Notification.Payload.Event.BroadcasterUserName);
        flowVars.Set(CheerUsernameVariable, arg.Notification.Payload.Event.UserName ?? "Anon");
        flowVars.Set(CheerAtVariable, DateTime.Now);
        flowVars.Set(CheerBitsVariable, arg.Notification.Payload.Event.Bits);
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchCheerTriggerData data, ChannelCheerArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterUserName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchCheerTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelCheer, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchCheerTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelCheer
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}