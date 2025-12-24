using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Chat.ChatSettings;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchEmoteOnlyChatTask : FlowTask<SetTwitchEmoteOnlyChatTaskData>
{
    public override string Code => SetTwitchEmoteOnlyChatTaskData.TaskCode;
    public override string Version => SetTwitchEmoteOnlyChatTaskData.TaskVersion;
    
    public override string Name => "Set Emote Only Chat";
    public override string Category => "Twitch";
    public override string Description => "Enables or Disables emote only chat for a channel";

    public ITwitchAPI TwitchApi { get; }

    public SetTwitchEmoteOnlyChatTask(ILogger<FlowTask<SetTwitchEmoteOnlyChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ManageModerationChatSettings);

    public override async Task<ExecutionResult> Execute(SetTwitchEmoteOnlyChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);

        var destinationUserId = await TwitchApi.GetUserIdFromName(processedChannel);
        if (destinationUserId is null)
        { return ExecutionResult.Failed($"Couldnt Find UserId For Channel [{ processedChannel }]"); }

        var userId = AppState.GetTwitchUserId();
        var newChatSettings = new ChatSettings() { EmoteMode = data.SetEmoteOnlyChat };

        try { await TwitchApi.Helix.Chat.UpdateChatSettingsAsync(destinationUserId, userId, newChatSettings); }
        catch (Exception e) { return ExecutionResult.Failed($"Unable to set emote only chat: {e.Message}"); }
        
        return ExecutionResult.Success();
    }
}