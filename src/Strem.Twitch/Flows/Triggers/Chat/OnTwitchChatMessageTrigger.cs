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
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchChatMessageTrigger : FlowTrigger<OnTwitchChatMessageTriggerData>
{
    public override string Code => OnTwitchChatMessageTriggerData.TriggerCode;
    public override string Version => OnTwitchChatMessageTriggerData.TriggerVersion;

    public static VariableEntry ChatMessageVariable = new("chat.message", TwitchVars.TwitchContext);
    public static VariableEntry RawChatMessageVariable = new("chat.raw-message", TwitchVars.TwitchContext);
    public static VariableEntry BitsSentVariable = new("chat.message.bits-sent", TwitchVars.TwitchContext);
    public static VariableEntry BitsValueVariable = new("chat.message.bits-value", TwitchVars.TwitchContext);
    public static VariableEntry RewardIdVariable = new("chat.message.reward-id", TwitchVars.TwitchContext);
    public static VariableEntry IsNoisyVariable = new("chat.message.is-noisy", TwitchVars.TwitchContext);
    public static VariableEntry SubscriptionLengthVariable = new("chat.message.subscription-length", TwitchVars.TwitchContext);
    public static VariableEntry IsHighlightedVariable = new("chat.message.is-highlighted", TwitchVars.TwitchContext);
    public static VariableEntry UserTypeVariable = new("chat.user-type", TwitchVars.TwitchContext);
    public static VariableEntry UsernameVariable = new("chat.username", TwitchVars.TwitchContext);
    public static VariableEntry UserIdVariable = new("chat.user-id", TwitchVars.TwitchContext);
    
    public override string Name => "On Twitch Chat Message";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a twitch chat message is received";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ChatMessageVariable.ToDescriptor(), BitsSentVariable.ToDescriptor(), BitsValueVariable.ToDescriptor(),
        RewardIdVariable.ToDescriptor(), IsNoisyVariable.ToDescriptor(), SubscriptionLengthVariable.ToDescriptor(),
        IsHighlightedVariable.ToDescriptor(), UserTypeVariable.ToDescriptor(), UsernameVariable.ToDescriptor(),
        UserIdVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchChatMessageTrigger(ILogger<FlowTrigger<OnTwitchChatMessageTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ReadChat);

    public IVariables PopulateVariables(ChatMessage message)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(ChatMessageVariable, message.Message);
        flowVars.Set(RawChatMessageVariable, message.RawIrcMessage);
        flowVars.Set(BitsSentVariable, message.Bits.ToString());
        flowVars.Set(BitsValueVariable, message.BitsInDollars.ToString());
        flowVars.Set(RewardIdVariable, message.CustomRewardId);
        flowVars.Set(IsNoisyVariable, (message.Noisy == Noisy.True).ToString());
        flowVars.Set(SubscriptionLengthVariable, message.SubscribedMonthCount.ToString());
        flowVars.Set(IsHighlightedVariable, message.IsHighlighted.ToString());
        flowVars.Set(UserTypeVariable, message.UserType.ToString());
        flowVars.Set(UsernameVariable, message.Username);
        flowVars.Set(UserIdVariable, message.UserId);
        return flowVars;
    }

    public bool IsUserAboveMinimumRequired(UserType userTypeRequired, UserType messageUserType)
    { return messageUserType >= userTypeRequired; }

    public bool DoesMessageTextMeetRequirements(TextMatchType matchTypeType, string matchText, string message)
    { return matchTypeType == TextMatchType.None || message.MatchesText(matchTypeType, matchText); }

    public bool DoesMessageMeetCriteria(OnTwitchChatMessageTriggerData data, ChatMessage message)
    {
        // TODO: This check may need to be removed depending on if we support multiple channels supported
        var userChannel = AppState.GetTwitchUsername();
        if(!message.Channel.Equals(userChannel)) { return false; }
        
        if(!IsUserAboveMinimumRequired(data.MinimumUserType, message.UserType)) { return false; }
        if(data.IsVip && !message.IsVip) { return false; }
        if(data.IsSubscriber && !message.IsSubscriber) { return false; }
        if(data.HasBits && message.Bits <= 0) { return false; }
        if(data.HasChannelReward && string.IsNullOrEmpty(message.CustomRewardId)) { return false; }
        if (!DoesMessageTextMeetRequirements(data.MatchTypeType, data.MatchText, message.Message)) { return false; }

        return true;
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchChatMessageTriggerData data)
    {
        return TwitchClient.OnMessageReceived
            .Where(x => DoesMessageMeetCriteria(data, x.ChatMessage))
            .Select(x => PopulateVariables(x.ChatMessage));
    }
}