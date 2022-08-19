using Strem.Core.Events.Bus;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Flows.Tasks.Stream;

public class SetStreamTitleTask : FlowTask<SetStreamTitleTaskData>
{
    public override string Code => SetStreamTitleTaskData.TaskCode;
    public override string Version => SetStreamTitleTaskData.TaskVersion;
    
    public override string Name => "Set Stream Title";
    public override string Category => "Twitch";
    public override string Description => "Sets the streams title";

    public ITwitchAPI TwitchApi { get; }

    public SetStreamTitleTask(ILogger<FlowTask<SetStreamTitleTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchAPI twitchApi) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchApi = twitchApi;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ApiScopes.RunChannelCommercials);

    public override async Task<bool> Execute(SetStreamTitleTaskData data, IVariables flowVars)
    {
        var processedTitle = FlowStringProcessor.Process(data.Title, flowVars);
        var channelInformation = new ModifyChannelInformationRequest { Title = processedTitle };
        await TwitchApi.Helix.Channels.ModifyChannelInformationAsync(AppState.GetTwitchUserId(), channelInformation);
        return true;
    }
}