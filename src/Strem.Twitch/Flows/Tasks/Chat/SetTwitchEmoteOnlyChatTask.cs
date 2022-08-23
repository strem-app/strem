using Strem.Core.Events.Bus;
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

public class SetTwitchEmoteOnlyChatTask : FlowTask<SetTwitchEmoteOnlyChatTaskData>
{
    public override string Code => SetTwitchEmoteOnlyChatTaskData.TaskCode;
    public override string Version => SetTwitchEmoteOnlyChatTaskData.TaskVersion;
    
    public override string Name => "Set Emote Only Chat";
    public override string Category => "Twitch";
    public override string Description => "Enables or Disables emote only chat for a channel";

    public ITwitchClient TwitchClient { get; }

    public SetTwitchEmoteOnlyChatTask(ILogger<FlowTask<SetTwitchEmoteOnlyChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ModerateChannel);

    public override async Task<ExecutionResult> Execute(SetTwitchEmoteOnlyChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        if(data.SetEmoteOnlyChat)
        { TwitchClient.EmoteOnlyOn(processedChannel); }
        else
        { TwitchClient.EmoteOnlyOff(processedChannel); }
        
        return ExecutionResult.Success;
    }
}