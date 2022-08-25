using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class ClearTwitchChatTask : FlowTask<ClearTwitchChatTaskData>
{
    public override string Code => ClearTwitchChatTaskData.TaskCode;
    public override string Version => ClearTwitchChatTaskData.TaskVersion;
    
    public override string Name => "Clear Chat";
    public override string Category => "Twitch";
    public override string Description => "Clears twitch chat assuming you have permissions on the channel";

    public ITwitchClient TwitchClient { get; }

    public ClearTwitchChatTask(ILogger<FlowTask<ClearTwitchChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ModerateChannel);

    public override async Task<ExecutionResult> Execute(ClearTwitchChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        TwitchClient.ClearChat(processedChannel);
        return ExecutionResult.Success();
    }
}