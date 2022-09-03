using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Client.Events;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchHostingTrigger : FlowTrigger<OnTwitchHostingTriggerData>
{
    public override string Code => OnTwitchHostingTriggerData.TriggerCode;
    public override string Version => OnTwitchHostingTriggerData.TriggerVersion;

    public static VariableEntry HostingChannelVariable = new("hosting.channel", TwitchVars.TwitchContext);
    public static VariableEntry HostedChannelVariable = new("hosted.channel", TwitchVars.TwitchContext);
    
    public override string Name => "On Twitch Hosting";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a channel is hosting";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        HostingChannelVariable.ToDescriptor(), HostedChannelVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchHostingTrigger(ILogger<FlowTrigger<OnTwitchHostingTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ReadWhispers);

    public IVariables PopulateVariables(OnNowHostingArgs args)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(HostingChannelVariable, args.Channel);
        flowVars.Set(HostedChannelVariable, args.HostedChannel);
        return flowVars;
    }

    public bool DoesMessageMeetCriteria(OnTwitchHostingTriggerData data, OnNowHostingArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Channel, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchHostingTriggerData data)
    {
        return TwitchClient.OnNowHosting
            .Where(x => DoesMessageMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}