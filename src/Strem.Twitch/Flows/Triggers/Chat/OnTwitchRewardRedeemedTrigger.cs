using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchRewardRedeemedTrigger : FlowTrigger<OnTwitchRewardRedeemedTriggerData>
{
    public override string Code => OnTwitchRewardRedeemedTriggerData.TriggerCode;
    public override string Version => OnTwitchRewardRedeemedTriggerData.TriggerVersion;

    public static VariableEntry RaidChannelVariable = new("raid.channel", TwitchVars.Context);
    public static VariableEntry RaidUsernameVariable = new("raid.username", TwitchVars.Context);
    public static VariableEntry RaidCountVariable = new("raid.count", TwitchVars.Context);
    
    public override string Name => "On Twitch Reward Redeemed";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a user redeems a channel point reward";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        RaidChannelVariable.ToDescriptor(), RaidUsernameVariable.ToDescriptor(), 
        RaidCountVariable.ToDescriptor()
    };

    public IObservableTwitchPubSub TwitchPubSub { get; set; }
    public ITwitchAPI TwitchApi { get; }
    public IObservableTwitchClient TwitchClient { get; }
    
    public OnTwitchRewardRedeemedTrigger(ILogger<FlowTrigger<OnTwitchRewardRedeemedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchPubSub twitchPubSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchPubSub = twitchPubSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.ReadWhispers);

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
    
    public void JoinChannelIfNeeded(OnTwitchRewardRedeemedTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        if(isDefaultChannel){ return; }
        
        var processedChannel = FlowStringProcessor.Process(data.RequiredChannel ?? string.Empty, new Core.Variables.Variables());
        TwitchApi.Helix.Users.GetUsersAsync()
        TwitchPubSub.PubSub.ListenToChannelPoints(processedChannel);
        if (!TwitchPubSub.PubSub.Client.HasJoinedChannel(processedChannel))
        {
            TwitchPubSub.PubSub.ListenToBitsEvents().Client.JoinChannel(processedChannel);
            Logger.Information($"[{data.Code}:{data.Id}] has joined twitch channel: {processedChannel}");
        }
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchBitsReceivedTriggerData data)
    {
        return TwitchClient.OnRaidNotification
            .Where(x => DoesMessageMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}