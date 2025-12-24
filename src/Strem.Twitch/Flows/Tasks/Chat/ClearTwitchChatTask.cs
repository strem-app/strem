using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Api.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class ClearTwitchChatTask : FlowTask<ClearTwitchChatTaskData>
{
    public override string Code => ClearTwitchChatTaskData.TaskCode;
    public override string Version => ClearTwitchChatTaskData.TaskVersion;
    
    public override string Name => "Clear Chat";
    public override string Category => "Twitch";
    public override string Description => "Clears twitch chat assuming you have permissions on the channel";

    public ITwitchAPI TwitchApi { get; }

    public ClearTwitchChatTask(ILogger<FlowTask<ClearTwitchChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ManageUsersWhispers);

    public override async Task<ExecutionResult> Execute(ClearTwitchChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        
        var destinationUserId = await TwitchApi.GetUserIdFromName(processedChannel);
        if (destinationUserId is null)
        { return ExecutionResult.Failed($"Couldnt Find UserId For Channel [{ processedChannel }]"); }

        var userId = AppState.GetTwitchUserId();

        await TwitchApi.Helix.Moderation.DeleteChatMessagesAsync(destinationUserId, userId);
        return ExecutionResult.Success();
    }
}