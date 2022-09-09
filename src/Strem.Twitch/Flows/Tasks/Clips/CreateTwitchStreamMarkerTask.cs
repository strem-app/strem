using Strem.Core.Events.Bus;
using Strem.Flows.Executors;
using Strem.Flows.Processors;
using Strem.Flows.Data.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Api.Helix.Models.Streams.CreateStreamMarker;
using TwitchLib.Api.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Clips;

public class CreateTwitchStreamMarkerTask : FlowTask<CreateTwitchStreamMarkerTaskData>
{
    public override string Code => CreateTwitchStreamMarkerTaskData.TaskCode;
    public override string Version => CreateTwitchStreamMarkerTaskData.TaskVersion;
    
    public override string Name => "Create Stream Marker";
    public override string Category => "Twitch";
    public override string Description => "Creates a stream marker at the current time for highlights to use";

    public ITwitchAPI TwitchApi { get; }

    public CreateTwitchStreamMarkerTask(ILogger<FlowTask<CreateTwitchStreamMarkerTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ApiScopes.ManageClips);
    
    public override async Task<ExecutionResult> Execute(CreateTwitchStreamMarkerTaskData data, IVariables flowVars)
    {
        var markerRequest = new CreateStreamMarkerRequest { Description = data.Description, UserId = AppState.GetTwitchUserId()};
        var response = await TwitchApi.Helix.Streams.CreateStreamMarkerAsync(markerRequest);
        return (response.Data.Length > 0) ? ExecutionResult.Success() : ExecutionResult.Failed("Twitch didn't create marker for some reason");
    }
}