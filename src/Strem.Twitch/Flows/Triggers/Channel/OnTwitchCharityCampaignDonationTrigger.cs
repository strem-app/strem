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

public class OnTwitchCharityCampaignDonationTrigger : FlowTrigger<OnTwitchCharityCampaignDonationTriggerData>, IUsesTwitchEventSub<OnTwitchCharityCampaignDonationTriggerData>
{
    public override string Code => OnTwitchCharityCampaignDonationTriggerData.TriggerCode;
    public override string Version => OnTwitchCharityCampaignDonationTriggerData.TriggerVersion;

    public static VariableEntry DonationChannelVariable = new("donation.channel", TwitchVars.Context);
    public static VariableEntry DonationCharityNameVariable = new("donation.charity-name", TwitchVars.Context);
    public static VariableEntry DonationUsernameVariable = new("donation.username", TwitchVars.Context);
    public static VariableEntry DonationAtVariable = new("donation.at", TwitchVars.Context);
    public static VariableEntry DonationCampaignIdVariable = new("donation.campaign-id", TwitchVars.Context);
    public static VariableEntry DonationAmountVariable = new("donation.amount-text", TwitchVars.Context);
    
    public override string Name => "On Twitch Charity Donation";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a user donates to a charity campaign";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        DonationChannelVariable.ToDescriptor(), DonationCharityNameVariable.ToDescriptor(),
        DonationUsernameVariable.ToDescriptor(), DonationAtVariable.ToDescriptor(),
        DonationCampaignIdVariable.ToDescriptor(), DonationAmountVariable.ToDescriptor(),
    };

    public IObservableTwitchEventSub TwitchEventSub { get; set; }
    
    public OnTwitchCharityCampaignDonationTrigger(ILogger<FlowTrigger<OnTwitchCharityCampaignDonationTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchEventSub twitchEventSub) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchEventSub = twitchEventSub;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ReadCharity);

    public IVariables PopulateVariables(ChannelCharityCampaignDonateArgs arg)
    {
        var flowVars = new Core.Variables.Variables();
        var eventData = arg.Notification.Payload.Event;
        flowVars.Set(DonationChannelVariable, eventData.BroadcasterName);
        flowVars.Set(DonationCharityNameVariable, eventData.CharityName);
        flowVars.Set(DonationUsernameVariable, eventData.UserName);
        flowVars.Set(DonationAtVariable, DateTime.Now);
        flowVars.Set(DonationCampaignIdVariable, eventData.CampaignId);

        var majorCurrency = eventData.Amount.Value.ProcessTwitchCurrency(eventData.Amount.DecimalPlaces);
        var donationAmount = $"{majorCurrency} {arg.Notification.Payload.Event.Amount.Currency}";
        flowVars.Set(DonationAmountVariable, donationAmount);
        return flowVars;
    }

    public bool DoesEventMeetCriteria(OnTwitchCharityCampaignDonationTriggerData data, ChannelCharityCampaignDonateArgs args)
    {
        if (!string.IsNullOrEmpty(data.RequiredChannel))
        {
            if(!data.RequiredChannel.Equals(args.Notification.Payload.Event.BroadcasterName, StringComparison.OrdinalIgnoreCase))
            { return false; }
        }
        
        return true;
    }
    
    public async Task SetupEventSubscriptions(OnTwitchCharityCampaignDonationTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.RequiredChannel);
        var channelToUse = isDefaultChannel ? AppState.GetTwitchUsername() : data.RequiredChannel;
        
        var processedChannel = FlowStringProcessor.Process(channelToUse, new Core.Variables.Variables());
        await TwitchEventSub.SubscribeOnChannelIfNeeded(EventSubTypes.ChannelCharityCampaignDonate, processedChannel);
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchCharityCampaignDonationTriggerData data)
    {
        await SetupEventSubscriptions(data);
        
        return TwitchEventSub.OnChannelCharityCampaignDonate
            .Where(x => DoesEventMeetCriteria(data, x))
            .Select(PopulateVariables);
    }
}