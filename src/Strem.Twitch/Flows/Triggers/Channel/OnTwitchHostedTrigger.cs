﻿using System.Reactive.Linq;
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

namespace Strem.Twitch.Flows.Triggers.Channel;

public class OnTwitchHostedTrigger : FlowTrigger<OnTwitchHostedTriggerData>
{
    public override string Code => OnTwitchHostedTriggerData.TriggerCode;
    public override string Version => OnTwitchHostedTriggerData.TriggerVersion;

    public static VariableEntry HostingChannelVariable = new("hosting.channel", TwitchVars.Context);
    public static VariableEntry HostedChannelVariable = new("hosted.channel", TwitchVars.Context);
    
    public override string Name => "On Twitch Hosted";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel is hosted";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        HostingChannelVariable.ToDescriptor(), HostedChannelVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchHostedTrigger(ILogger<FlowTrigger<OnTwitchHostedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.ReadWhispers);

    public IVariables PopulateVariables(OnNowHostingArgs args)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(HostingChannelVariable, args.Channel);
        flowVars.Set(HostedChannelVariable, args.HostedChannel);
        return flowVars;
    }

    public bool DoesMessageMeetCriteria(OnTwitchHostedTriggerData data, OnNowHostingArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Channel, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchHostedTriggerData data)
    {
        return TwitchClient.OnNowHosting
            .Where(x => DoesMessageMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}