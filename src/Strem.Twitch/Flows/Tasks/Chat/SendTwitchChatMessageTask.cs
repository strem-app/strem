using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
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

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.SendChat);

    public override async Task<ExecutionResult> Execute(SendTwitchChatMessageTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedMessage = FlowStringProcessor.Process(data.Message, flowVars);
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);

        if (TwitchClient.HasJoinedChannel(processedChannel))
        { 
            TwitchClient.SendMessage(processedChannel, processedMessage);
            return ExecutionResult.Success();
        }
        
        TwitchClient.TemporarilyJoinChannelFor(channel, () => TwitchClient.SendMessage(processedChannel, processedMessage));
        return ExecutionResult.Success();
    }
}