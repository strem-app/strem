using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Channels;

public class JoinTwitchChannelTask : FlowTask<JoinTwitchChannelTaskData>
{
    public override string Code => JoinTwitchChannelTaskData.TaskCode;
    public override string Version => JoinTwitchChannelTaskData.TaskVersion;
    
    public override string Name => "Join Channel";
    public override string Category => "Twitch";
    public override string Description => "Joins a twitch channel which enables triggers/tasks for other channels to work";

    public ITwitchClient TwitchClient { get; }

    public JoinTwitchChannelTask(ILogger<FlowTask<JoinTwitchChannelTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => TwitchClient.IsConnected;

    public override async Task<ExecutionResult> Execute(JoinTwitchChannelTaskData data, IVariables flowVars)
    {
        var processedChannel = FlowStringProcessor.Process(data.Channel, flowVars);
        
        if(TwitchClient.HasJoinedChannel(processedChannel))
        { return ExecutionResult.FailedButContinue("Already within channel, continuing"); }
        
        TwitchClient.JoinChannel(processedChannel);
        return ExecutionResult.Success();
    }
}