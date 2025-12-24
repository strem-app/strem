using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Flows.Extensions;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Api.Helix.Models.Chat.ChatSettings;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchFollowerOnlyChatTask : FlowTask<SetTwitchFollowerOnlyChatTaskData>
{
    public override string Code => SetTwitchFollowerOnlyChatTaskData.TaskCode;
    public override string Version => SetTwitchFollowerOnlyChatTaskData.TaskVersion;
    
    public override string Name => "Set Follower Only Chat";
    public override string Category => "Twitch";
    public override string Description => "Enables or Disables follower only chat for a channel";

    public ITwitchAPI TwitchApi { get; }

    public SetTwitchFollowerOnlyChatTask(ILogger<FlowTask<SetTwitchFollowerOnlyChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ManageModerationChatSettings);

    public override async Task<ExecutionResult> Execute(SetTwitchFollowerOnlyChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);

        if (!FlowStringProcessor.TryProcessInt(data.FollowerTimeValue, flowVars, out var timeValue))
        {
            Logger.Warning("Unable to process FollowerTimeValue in SetTwitchFollowerOnlyChatTask");
            timeValue = 1;
        }
        
        var destinationUserId = await TwitchApi.GetUserIdFromName(processedChannel);
        if (destinationUserId is null)
        { return ExecutionResult.Failed($"Couldnt Find UserId For Channel [{ processedChannel }]"); }

        var userId = AppState.GetTwitchUserId();
        var newChatSettings = new ChatSettings() { FollowerMode = data.SetFollowerOnlyChat, FollowerModeDuration = timeValue};

        try { await TwitchApi.Helix.Chat.UpdateChatSettingsAsync(destinationUserId, userId, newChatSettings); }
        catch (Exception e) { return ExecutionResult.Failed($"Unable to set follower only chat: {e.Message}"); }
        
        return ExecutionResult.Success();
    }
}