using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Twitch.Events.OAuth;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.OAuth;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client.Interfaces;

namespace Strem.Twitch.Plugin;

public class TwitchPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ITwitchOAuthClient TwitchOAuthClient { get; }
    public ITwitchAPI TwitchApi { get; }
    public ITwitchClient TwitchClient { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger<TwitchPluginStartup> Logger { get; }

    public TwitchPluginStartup(ITwitchOAuthClient twitchOAuthClient, ITwitchAPI twitchApi, ITwitchClient twitchClient, IEventBus eventBus, IAppState appState, ILogger<TwitchPluginStartup> logger)
    {
        TwitchOAuthClient = twitchOAuthClient;
        TwitchApi = twitchApi;
        TwitchClient = twitchClient;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
        Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(TwitchPluginSettings.RevalidatePeriodInMins))
            .Subscribe(x => VerifyToken())
            .AddTo(_subs);
        
        Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(TwitchPluginSettings.ChatReconnectInMins))
            .Subscribe(x => AttemptToConnectToChat())
            .AddTo(_subs);
        
        Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(TwitchPluginSettings.RefreshChannelPeriodInMins))
            .Subscribe(x => RefreshChannelDetails())
            .AddTo(_subs);
        
        EventBus.Receive<TwitchOAuthSuccessEvent>()
            .Subscribe(x => VerifyAndSetupConnections())
            .AddTo(_subs);
        
        EventBus.Receive<TwitchOAuthRevokedEvent>()
            .Subscribe(x => DisconnectEverything())
            .AddTo(_subs);
    }

    public void AttemptToConnectToChat()
    {
        if(!AppState.HasTwitchOAuth()) { return; }
        if(TwitchClient.IsConnected) { return; }
        if(!AppState.HasTwitchScope(ChatScopes.ReadChat)) { return; }
        
        var result = TwitchClient.ConnectOrRefresh(AppState);
        if(result.success)
        { Logger.Information($"Connected to twitch chat"); }
        else
        { Logger.LogError($"Unable to connect to twitch chat/pubsub due to error: {result.message}"); }
    }

    public async Task VerifyAndSetupConnections()
    {
        await VerifyToken();
        AttemptToConnectToChat();
        await RefreshChannelDetails();
    }

    public async Task DisconnectEverything()
    {
        if(TwitchClient.IsConnected)
        { TwitchClient.Disconnect(); }
    }

    public async Task VerifyToken()
    {
        Logger.Information("Revalidating Twitch Access Token");

        if (AppState.HasTwitchOAuth())
        { await TwitchOAuthClient.ValidateToken(); }
    }

    public async Task RefreshChannelDetails()
    {
        Logger.Information("Refreshing Twitch Channel & Stream Information");
        
        if (!AppState.HasTwitchOAuth()) { return; }
        
        TwitchApi.Settings.SetCredentials(AppState);
        var userId = AppState.AppVariables.Get(TwitchVars.UserId);
        var accessToken = AppState.GetTwitchOAuthToken();
        
        var channelInformation = await TwitchApi.Helix.Channels.GetChannelInformationAsync(userId, accessToken);
        if (channelInformation != null && channelInformation.Data.Length > 0)
        {
            var channelData = channelInformation.Data[0];
            AppState.TransientVariables.Set(TwitchVars.Username, channelData.BroadcasterName);
            AppState.TransientVariables.Set(TwitchVars.ChannelTitle, channelData.Title);
            AppState.TransientVariables.Set(TwitchVars.ChannelGame, channelData.GameName);
        }

        var streamInformation = await TwitchApi.Helix.Streams.GetStreamsAsync(userIds: new List<string> { userId });
        if (streamInformation != null && streamInformation.Streams.Length > 0)
        {
            var streamData = streamInformation.Streams[0];
            AppState.TransientVariables.Set(TwitchVars.StreamViewers, streamData.ViewerCount.ToString());
            AppState.TransientVariables.Set(TwitchVars.StreamThumbnailUrl, streamData.ThumbnailUrl);
        }
    }

    public void Dispose()
    { _subs?.Dispose(); }
}