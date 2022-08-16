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
using TwitchLib.Client.Models;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchWhisperMessageTrigger : FlowTrigger<OnTwitchWhisperMessageTriggerData>
{
    public override string Code => OnTwitchWhisperMessageTriggerData.TriggerCode;
    public override string Version => OnTwitchWhisperMessageTriggerData.TriggerVersion;

    public static VariableEntry ChatMessageVariable = new("chat.message", TwitchVars.TwitchContext);
    public static VariableEntry RawChatMessageVariable = new("chat.raw-message", TwitchVars.TwitchContext);
    public static VariableEntry UserTypeVariable = new("chat.user-type", TwitchVars.TwitchContext);
    public static VariableEntry UsernameVariable = new("chat.username", TwitchVars.TwitchContext);
    public static VariableEntry UserIdVariable = new("chat.user-id", TwitchVars.TwitchContext);
    
    public override string Name => "On Twitch Whisper Message";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a twitch chat message is received";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ChatMessageVariable.ToDescriptor(), UserTypeVariable.ToDescriptor(), 
        UsernameVariable.ToDescriptor(), UserIdVariable.ToDescriptor()
    };

    public IObservableTwitchClient TwitchClient { get; set; }
    
    public OnTwitchWhisperMessageTrigger(ILogger<FlowTrigger<OnTwitchWhisperMessageTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ReadWhispers);

    public IVariables PopulateVariables(WhisperMessage message)
    {
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(ChatMessageVariable, message.Message);
        flowVars.Set(RawChatMessageVariable, message.RawIrcMessage);
        flowVars.Set(UserTypeVariable, message.UserType.ToString());
        flowVars.Set(UsernameVariable, message.Username);
        flowVars.Set(UserIdVariable, message.UserId);
        return flowVars;
    }

    public bool DoesMessageMeetCriteria(OnTwitchWhisperMessageTriggerData data, WhisperMessage message)
    {
        if (data.MatchTypeType != TextMatchType.None)
        {
            if (!message.Message.MatchesText(data.MatchTypeType, data.MatchText))
            { return false; }
        }

        return true;
    }

    public override IObservable<IVariables> Execute(OnTwitchWhisperMessageTriggerData data)
    {
        return TwitchClient.OnWhisperReceived
            .Where(x => DoesMessageMeetCriteria(data, x.WhisperMessage))
            .Select(x => PopulateVariables(x.WhisperMessage));
    }
}