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

    public ITwitchClient TwitchClient { get; }

    public SetTwitchSlowModeChatTask(ILogger<FlowTask<SetTwitchSlowModeChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.ModerateChannel);

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

        var timeout = data.TimeoutUnit.ToTimeSpan(timeValue);
        
        if(data.SetSlowMode)
        { TwitchClient.SlowModeOn(processedChannel, timeout); }
        else
        { TwitchClient.SlowModeOff(processedChannel); }
        
        return hadIssue ? ExecutionResult.FailedButContinue("Couldnt process number correctly, defaulted to 1") : ExecutionResult.Success();
    }
}