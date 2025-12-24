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
using TwitchLib.Api.Helix.Models.Channels.StartCommercial;
using TwitchLib.Api.Interfaces;
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

    public ITwitchAPI TwitchApi { get; }

    public PlayTwitchCommercialTask(ILogger<FlowTask<PlayTwitchCommercialTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchAccessToken() && AppState.HasTwitchScope(ApiScopes.RunChannelCommercials);

    public override async Task<ExecutionResult> Execute(PlayTwitchCommercialTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        var userId = await TwitchApi.GetUserIdFromName(processedChannel);
        if (string.IsNullOrEmpty(userId))
        {
            Logger.Warning($"Couldnt Find UserId For Use [{ processedChannel }]");
            return ExecutionResult.Failed($"Couldnt Find UserId For Use [{ processedChannel }]");
        }
        
        var commercialRequest = new StartCommercialRequest { BroadcasterId = userId, Length = data.CommercialLength};
        await TwitchApi.Helix.Channels.StartCommercialAsync(commercialRequest);
        
        return ExecutionResult.Success();
    }
}