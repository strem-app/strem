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
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchSubscriberOnlyChatTask : FlowTask<SetTwitchSubscriberOnlyChatTaskData>
{
    public override string Code => SetTwitchSubscriberOnlyChatTaskData.TaskCode;
    public override string Version => SetTwitchSubscriberOnlyChatTaskData.TaskVersion;
    
    public override string Name => "Set Subscriber Only Chat";
    public override string Category => "Twitch";
    public override string Description => "Enables or Disables subscriber only chat for a channel";

    public ITwitchClient TwitchClient { get; }

    public SetTwitchSubscriberOnlyChatTask(ILogger<FlowTask<SetTwitchSubscriberOnlyChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ChatScopes.ModerateChannel);

    public override async Task<ExecutionResult> Execute(SetTwitchSubscriberOnlyChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        
        if(data.SetSubscriberOnlyChat)
        { TwitchClient.SubscribersOnlyOn(processedChannel); }
        else
        { TwitchClient.SubscribersOnlyOff(processedChannel); }
        
        return ExecutionResult.Success();
    }
}