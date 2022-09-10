using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Channels;

public class LeaveTwitchChannelTask : FlowTask<LeaveTwitchChannelTaskData>
{
    public override string Code => LeaveTwitchChannelTaskData.TaskCode;
    public override string Version => LeaveTwitchChannelTaskData.TaskVersion;
    
    public override string Name => "Leave Channel";
    public override string Category => "Twitch";
    public override string Description => "Leaves a twitch channel which disables triggers/tasks for the channel";

    public ITwitchClient TwitchClient { get; }

    public LeaveTwitchChannelTask(ILogger<FlowTask<LeaveTwitchChannelTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => TwitchClient.IsConnected;

    public override async Task<ExecutionResult> Execute(LeaveTwitchChannelTaskData data, IVariables flowVars)
    {
        var processedChannel = FlowStringProcessor.Process(data.Channel, flowVars);
        
        if(!TwitchClient.HasJoinedChannel(processedChannel))
        { return ExecutionResult.FailedButContinue("Not connected to channel so skipping"); }
        
        TwitchClient.LeaveChannel(processedChannel);
        return ExecutionResult.Success();
    }
}