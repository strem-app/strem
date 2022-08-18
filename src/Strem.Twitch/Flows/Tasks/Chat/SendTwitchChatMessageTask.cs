using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SendTwitchChatMessageTask : FlowTask<SendTwitchChatMessageTaskData>
{
    public override string Code => SendTwitchChatMessageTaskData.TaskCode;
    public override string Version => SendTwitchChatMessageTaskData.TaskVersion;
    
    public override string Name => "Send Chat Message";
    public override string Category => "Twitch";
    public override string Description => "Sends a message to a twitch channel, defaults to yours if not provided";

    public ITwitchClient TwitchClient { get; }

    public SendTwitchChatMessageTask(ILogger<FlowTask<SendTwitchChatMessageTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.SendChat);

    public override async Task<bool> Execute(SendTwitchChatMessageTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedMessage = FlowStringProcessor.Process(data.Message, flowVars);
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        TwitchClient.SendMessage(processedChannel, processedMessage);
        return true;
    }
}