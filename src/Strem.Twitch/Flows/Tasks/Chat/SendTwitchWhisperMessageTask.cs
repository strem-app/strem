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
using TwitchLib.Api.Interfaces;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SendTwitchWhisperMessageTask : FlowTask<SendTwitchWhisperMessageTaskData>
{
    public override string Code => SendTwitchWhisperMessageTaskData.TaskCode;
    public override string Version => SendTwitchWhisperMessageTaskData.TaskVersion;
    
    public override string Name => "Send Whisper Message";
    public override string Category => "Twitch";
    public override string Description => "Sends a whisper to a user on twitch";

    public ITwitchAPI TwitchApi { get; }

    public SendTwitchWhisperMessageTask(ILogger<FlowTask<SendTwitchWhisperMessageTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.ManageUsersWhispers);

    public override async Task<ExecutionResult> Execute(SendTwitchWhisperMessageTaskData data, IVariables flowVars)
    {
        var currentUserId = AppState.GetTwitchUserId();
        var destinationUserId = await TwitchApi.GetUserIdFromName(data.Username);
        if (destinationUserId is null)
        { return ExecutionResult.Failed($"Couldnt Find UserId For User [{ data.Username }]"); }
        
        var processedMessage = FlowStringProcessor.Process(data.Message, flowVars);
        // TODO: unsure how we know if the user is a previously messaged one, maybe push that to the data object
        await TwitchApi.Helix.Whispers.SendWhisperAsync(currentUserId, destinationUserId, processedMessage, true);
        return ExecutionResult.Success();
    }
}