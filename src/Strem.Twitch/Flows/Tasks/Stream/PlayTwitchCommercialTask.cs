using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Stream;

public class PlayTwitchCommercialTask : FlowTask<PlayTwitchCommercialTaskData>
{
    public override string Code => PlayTwitchCommercialTaskData.TaskCode;
    public override string Version => PlayTwitchCommercialTaskData.TaskVersion;
    
    public override string Name => "Play Twitch Commercial";
    public override string Category => "Twitch";
    public override string Description => "Plays a commercial on the given twitch channel";

    public ITwitchClient TwitchClient { get; }

    public PlayTwitchCommercialTask(ILogger<FlowTask<PlayTwitchCommercialTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => TwitchClient.IsConnected && AppState.HasTwitchScope(ApiScopes.RunChannelCommercials);

    public override async Task<ExecutionResult> Execute(PlayTwitchCommercialTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        TwitchClient.StartCommercial(processedChannel, data.CommercialLength);
        return ExecutionResult.Success();
    }
}