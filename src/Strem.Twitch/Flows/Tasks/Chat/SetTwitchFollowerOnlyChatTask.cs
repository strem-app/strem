using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Extensions;
using Strem.Twitch.Types;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Flows.Tasks.Chat;

public class SetTwitchFollowerOnlyChatTask : FlowTask<SetTwitchFollowerOnlyChatTaskData>
{
    public override string Code => SetTwitchFollowerOnlyChatTaskData.TaskCode;
    public override string Version => SetTwitchFollowerOnlyChatTaskData.TaskVersion;
    
    public override string Name => "Set Follower Only Chat";
    public override string Category => "Twitch";
    public override string Description => "Enables or Disables follower only chat for a channel";

    public ITwitchClient TwitchClient { get; }

    public SetTwitchFollowerOnlyChatTask(ILogger<FlowTask<SetTwitchFollowerOnlyChatTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, ITwitchClient twitchClient) : base(logger, flowStringProcessor, appState, eventBus)
    {
        TwitchClient = twitchClient;
    }

    public override bool CanExecute() => AppState.HasTwitchOAuth() && AppState.HasTwitchScope(ChatScopes.ModerateChannel);

    public override async Task<bool> Execute(SetTwitchFollowerOnlyChatTaskData data, IVariables flowVars)
    {
        var channel = string.IsNullOrEmpty(data.Channel) ? AppState.GetTwitchUsername() : data.Channel;
        var processedChannel = FlowStringProcessor.Process(channel, flowVars);
        if (!FlowStringProcessor.TryProcessInt(data.FollowerTimeValue, flowVars, out var timeValue))
        {
            Logger.Warning("Unable to process FollowerTimeValue in SetTwitchFollowerOnlyChatTask");
            timeValue = 1;
        }

        var followerTimeRequired = data.FollowerTimeUnit.ToTimeSpan(timeValue);
        
        if(data.SetFollowerOnlyChat)
        { TwitchClient.FollowersOnlyOn(processedChannel, followerTimeRequired); }
        else
        { TwitchClient.FollowersOnlyOff(new JoinedChannel(processedChannel)); }
        
        return true;
    }
}