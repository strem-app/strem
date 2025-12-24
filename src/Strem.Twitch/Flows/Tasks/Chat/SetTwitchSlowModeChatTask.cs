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

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchSlowModeChatTask : FlowTask<SetTwitchSlowModeChatTaskData>
{
    public override string Code => SetTwitchSlowModeChatTaskData.TaskCode;
    public override string Version => SetTwitchSlowModeChatTaskData.TaskVersion;
    
    public override string Name => "Set Slow Mode Chat";
    public override string Category => "Twitch";
    public override string Description => "Enables or Disables slow mode on chat for a channel";

    public ITwitchAPI TwitchApi { get; }

    public SetTwitchSlowModeChatTask(ILogger<FlowTask<SetTwitchSlowModeChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ManageModerationChatSettings);

    public override async Task<ExecutionResult> Execute(SetTwitchSlowModeChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        var hadIssue = false;
        if (!FlowStringProcessor.TryProcessInt(data.TimeoutValue, flowVars, out var timeValue))
        {
            Logger.Warning("Unable to process timeoutValue in SetTwitchEmoteOnlyChatTask");
            timeValue = 1;
            hadIssue = true;
        }
        
        var destinationUserId = await TwitchApi.GetUserIdFromName(processedChannel);
        if (destinationUserId is null)
        { return ExecutionResult.Failed($"Couldnt Find UserId For Channel [{ processedChannel }]"); }

        var userId = AppState.GetTwitchUserId();
        var newChatSettings = new ChatSettings() { SlowMode = data.SetSlowMode, SlowModeWaitTime = timeValue};

        try { await TwitchApi.Helix.Chat.UpdateChatSettingsAsync(destinationUserId, userId, newChatSettings); }
        catch (Exception e) { return ExecutionResult.Failed($"Unable to set slow mode only chat: {e.Message}"); }
        
        return hadIssue ? ExecutionResult.FailedButContinue("Couldnt process number correctly, defaulted to 1") : ExecutionResult.Success();
    }
}