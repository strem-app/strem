using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Processors;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Flows.Triggers.Chat;

public class OnTwitchChatCommandTrigger : FlowTrigger<OnTwitchChatCommandTriggerData>
{
    public override string Code => OnTwitchChatCommandTriggerData.TriggerCode;
    public override string Version => OnTwitchChatCommandTriggerData.TriggerVersion;

    public static VariableEntry ChatChannelVariable = new("chat.channel", TwitchVars.TwitchContext);
    public static VariableEntry ChatCommandVariable = new("chat.command", TwitchVars.TwitchContext);
    public static VariableEntry ChatCommandArgsVariable = new("chat.command.args", TwitchVars.TwitchContext);
    public static VariableEntry RawChatMessageVariable = new("chat.raw-message", TwitchVars.TwitchContext);
    public static VariableEntry BitsSentVariable = new("chat.message.bits-sent", TwitchVars.TwitchContext);
    public static VariableEntry BitsValueVariable = new("chat.message.bits-value", TwitchVars.TwitchContext);
    public static VariableEntry UserTypeVariable = new("chat.user-type", TwitchVars.TwitchContext);
    public static VariableEntry UsernameVariable = new("chat.username", TwitchVars.TwitchContext);
    public static VariableEntry UserIdVariable = new("chat.user-id", TwitchVars.TwitchContext);
    
    public override string Name => "On Twitch Chat Command";
    public override string Category => "Twitch";
    public override string Description => "Triggers when a twitch command is received (i.e !command)";

    public override VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        ChatCommandVariable.ToDescriptor(), BitsSentVariable.ToDescriptor(), BitsValueVariable.ToDescriptor(),
        ChatCommandArgsVariable.ToDescriptor(), UserTypeVariable.ToDescriptor(), UsernameVariable.ToDescriptor(),
        UserIdVariable.ToDescriptor(), RawChatMessageVariable.ToDescriptor()
    };

    public ICommandStringProcessor CommandStringProcessor { get; }
    public IObservableTwitchClient TwitchClient { get; }
    
    public OnTwitchChatCommandTrigger(ILogger<FlowTrigger<OnTwitchChatCommandTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IObservableTwitchClient twitchClient, ICommandStringProcessor commandStringProcessor) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
        CommandStringProcessor = commandStringProcessor;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ReadChat);

    public IVariables PopulateVariables(ChatMessage message)
    {
        var parsedCommand = CommandStringProcessor.Process(message.Message);
        
        var flowVars = new Core.Variables.Variables();
        flowVars.Set(ChatChannelVariable, message.Channel);
        flowVars.Set(ChatCommandVariable, parsedCommand.CommandName);
        flowVars.Set(ChatCommandArgsVariable, parsedCommand.CommandArgs);
        flowVars.Set(RawChatMessageVariable, message.RawIrcMessage);
        flowVars.Set(BitsSentVariable, message.Bits.ToString());
        flowVars.Set(BitsValueVariable, message.BitsInDollars.ToString());
        flowVars.Set(UserTypeVariable, message.UserType.ToString());
        flowVars.Set(UsernameVariable, message.Username);
        flowVars.Set(UserIdVariable, message.UserId);
        return flowVars;
    }

    public bool IsUserAboveMinimumRequired(UserType userTypeRequired, UserType messageUserType)
    { return messageUserType >= userTypeRequired; }

    public bool DoesChannelMatch(OnTwitchChatCommandTriggerData data, ChatMessage message)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.Channel);
        var channel = isDefaultChannel ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel ?? string.Empty, new Core.Variables.Variables());
        if(!message.Channel.Equals(processedChannel)) { return false; }
        return true;
    }
    
    public bool DoesMessageMeetCriteria(OnTwitchChatCommandTriggerData data, ChatMessage message)
    {
        if(!DoesChannelMatch(data, message)){ return false; }
        if(!IsUserAboveMinimumRequired(data.MinimumUserType, message.UserType)) { return false; }
        if(data.IsVip && !message.IsVip) { return false; }
        if(data.IsSubscriber && !message.IsSubscriber) { return false; }
        if(data.HasBits && message.Bits <= 0) { return false; }

        var parsedCommand = CommandStringProcessor.Process(message.Message);
        if(string.IsNullOrEmpty(parsedCommand.CommandName)) { return false; }
        if(data.RequiresArg && string.IsNullOrEmpty(parsedCommand.CommandArgs)) { return false; }

        return data.CommandName.Equals(parsedCommand.CommandName, StringComparison.OrdinalIgnoreCase);
    }
    
    public void JoinChannelIfNeeded(OnTwitchChatCommandTriggerData data)
    {
        var isDefaultChannel = string.IsNullOrEmpty(data.Channel);
        if(isDefaultChannel){ return; }
        
        var processedChannel = FlowStringProcessor.Process(data.Channel ?? string.Empty, new Core.Variables.Variables());
        if (!TwitchClient.Client.HasJoinedChannel(processedChannel))
        {
            TwitchClient.Client.JoinChannel(processedChannel);
            Logger.Information($"[{data.Code}:{data.Id}] has joined twitch channel: {processedChannel}");
        }
    }

    public override async Task<IObservable<IVariables>> Execute(OnTwitchChatCommandTriggerData data)
    {
        JoinChannelIfNeeded(data);
            
        return TwitchClient.OnMessageReceived
            .Where(x => DoesMessageMeetCriteria(data, x.ChatMessage))
            .Select(x => PopulateVariables(x.ChatMessage));
    }
}