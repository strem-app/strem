using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Triggers;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchRaidTrigger : FlowTrigger<OnTwitchRaidTriggerData>
{
    public override string Code => OnTwitchRaidTriggerData.TriggerCode;
    public override string Version => OnTwitchRaidTriggerData.TriggerVersion;

    public static VariableEntry RaidChannelVariable = new("raid.channel", TwitchVars.TwitchContext);
    public static VariableEntry RaidUsernameVariable = new("raid.username", TwitchVars.TwitchContext);
    public static VariableEntry RaidCountVariable = new("raid.count", TwitchVars.TwitchContext);
    
    public override string Name => "On Twitch Raid";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel is raided";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        RaidChannelVariable.ToDescriptor(), RaidCountVariable.ToDescriptor(), 
        RaidCountVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchRaidTrigger(ILogger<FlowTrigger<OnTwitchRaidTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ReadWhispers);

    public IVariables PopulateVariables(OnRaidNotificationArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(RaidChannelVariable, arg.Channel);
        flowVars.Set(RaidUsernameVariable, arg.RaidNotification.DisplayName);
        flowVars.Set(RaidCountVariable, arg.RaidNotification.MsgParamViewerCount);
        return flowVars;
    }

    public bool DoesMessageMeetCriteria(OnTwitchRaidTriggerData data, OnRaidNotificationArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Channel, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchRaidTriggerData data)
    {
        return TwitchClient.OnRaidNotification
            .Where(x => DoesMessageMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}