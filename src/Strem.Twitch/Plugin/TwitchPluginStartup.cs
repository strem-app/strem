using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Events.OAuth;
using Strem.Twitch.Extensions;
using Strem.Twitch.Services.Client;
using Strem.Twitch.Services.OAuth;
using Strem.Twitch.Types;
using Strem.Twitch.Variables;
using TwitchLib.Api.Interfaces;

namespace Strem.Twitch.Plugin;

public class TwitchPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ITwitchOAuthClient TwitchOAuthClient { get; }
    public ITwitchAPI TwitchApi { get; }
    public IObservableTwitchClient TwitchClient { get; }
    public IObservableTwitchPubSub TwitchPubSub { get; }
    public IObservableTwitchEventSub TwitchEventSub { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IApplicationConfig ApplicationConfig { get; }
    public ILogger<TwitchPluginStartup> Logger { get; }

    public string[] RequiredConfigurationKeys { get; } = new[] { TwitchPluginSettings.TwitchClientIdKey };

    public TwitchPluginStartup(ITwitchOAuthClient twitchOAuthClient, ITwitchAPI twitchApi, IObservableTwitchClient twitchClient, IObservableTwitchPubSub twitchPubSub, IEventBus eventBus, IAppState appState, ILogger<TwitchPluginStartup> logger, IApplicationConfig applicationConfig, IObservableTwitchEventSub twitchEventSub)
    {
        TwitchOAuthClient = twitchOAuthClient;
        TwitchApi = twitchApi;
        TwitchClient = twitchClient;
        TwitchPubSub = twitchPubSub;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        ApplicationConfig = applicationConfig;
        TwitchEventSub = twitchEventSub;
    }
    
    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        Observable.Timer(TimeSpan.FromMinutes(TwitchPluginSettings.RevalidatePeriodInMins))
            .Subscribe(x => VerifyToken())
            .AddTo(_subs);
        
        Observable.Timer(TimeSpan.FromMinutes(TwitchPluginSettings.ChatReconnectInMins))
            .Subscribe(x => AttemptToConnectToChat())
            .AddTo(_subs);
        
        Observable.Timer(TimeSpan.FromMinutes(TwitchPluginSettings.RefreshChannelPeriodInMins))
            .Subscribe(x => RefreshChannelDetails())
            .AddTo(_subs);

        TwitchClient.OnChannelStateChanged
            .Subscribe(x => RefreshChannelDetails())
            .AddTo(_subs);

        TwitchEventSub.OnError
            .Subscribe(x => Logger.LogError($"Error with Twitch EventSub [{x.Exception}]"))
            .AddTo(_subs);
        
        EventBus.Receive<TwitchOAuthSuccessEvent>()
            .Subscribe(x => VerifyAndSetupConnections())
            .AddTo(_subs);
        
        EventBus.Receive<TwitchOAuthRevokedEvent>()
            .Subscribe(x => DisconnectEverything())
            .AddTo(_subs);

        await VerifyAndSetupConnections();
    }

    public void AttemptToConnectToChat()
    {
        Logger.Information($"Checking if twitch chat can connect");
        if(!AppState.HasTwitchAccessToken()) { return; }
        if(TwitchClient.Client.IsConnected) { return; }
        if(!AppState.HasTwitchScope(ChatScopes.ReadChat)) { return; }
        
        Logger.Information($"Connecting to twitch chat");
        var result = TwitchClient.Client.ConnectOrRefresh(AppState);
        if(result.success)
        { Logger.Information($"Connected to twitch chat"); }
        else
        { Logger.LogError($"Unable to connect to twitch chat/pubsub due to error: {result.message}"); }
    }

    public void AttemptToConnectToPubSub()
    {
        Logger.Information($"Checking if twitch pubsub can connect");
        if(!AppState.HasTwitchAccessToken()) { return; }
        if(!AppState.HasTwitchScope(ChatScopes.ReadChat)) { return; }
            
        Logger.Information($"Connecting to twitch pubsub");
        TwitchPubSub.PubSub.Connect();
    }
    
    public async Task AttemptToConnectToEventSub()
    {
        Logger.Information($"Checking if twitch EventSub can connect");
        if(!AppState.HasTwitchAccessToken()) { return; }
        if(!AppState.HasTwitchScope(ChatScopes.ReadChat)) { return; }
            
        Logger.Information($"Connecting to twitch EventSub");
        var connected = await TwitchEventSub.Client.ConnectAsync(new Uri("wss://eventsub.wss.twitch.tv/ws"));

        if (connected)
        {
            Logger.Information("Connected to twitch EventSub");
            
            Logger.Information("Subscribing to Channel Updates");
            var userId = AppState.AppVariables.Get(TwitchVars.Username);
            var subscribed = await TwitchEventSub.SubscribeOnChannel(EventSubTypes.ChannelUpdate, userId, "2");
            if(subscribed)
            { Logger.Information("EventSub Channel Update Subscription Successful"); }
            else
            { Logger.Error($"Unable to subscribe to channel [{userId}] on EventSub"); }
        }
        else
        { Logger.Information("Could not connect to twitch EventSub"); }
    }

    public async Task VerifyAndSetupConnections()
    {
        await VerifyToken();
        AttemptToAuthorizeApi();
        AttemptToConnectToChat();
        await AttemptToConnectToEventSub();
    }

    public async Task DisconnectEverything()
    {
        if(TwitchClient.Client.IsConnected)
        { TwitchClient.Client.Disconnect(); }

        await TwitchEventSub.Client.DisconnectAsync();
        
        TwitchPubSub.PubSub.Disconnect();
    }

    public async Task VerifyToken()
    {
        Logger.Information("Revalidating Twitch Access Token");

        if (AppState.HasTwitchAccessToken())
        { await TwitchOAuthClient.ValidateToken(); }
    }

    public void AttemptToAuthorizeApi()
    {
        if (!AppState.HasTwitchAccessToken()) { return; }
        TwitchApi.Settings.SetCredentials(ApplicationConfig, AppState);
    }
    
    public async Task RefreshChannelDetails()
    {
        Logger.Information("Refreshing Twitch Channel & Stream Information");
        
        if (!AppState.HasTwitchAccessToken()) { return; }
        
        var userId = AppState.AppVariables.Get(TwitchVars.UserId);
        var accessToken = AppState.GetTwitchAccessToken();

        try
        {
            var channelInformation = await TwitchApi.Helix.Channels.GetChannelInformationAsync(userId, accessToken);
            if (channelInformation != null && channelInformation.Data.Length > 0)
            {
                var channelData = channelInformation.Data[0];
                AppState.TransientVariables.Set(TwitchVars.Username, channelData.BroadcasterName);
                AppState.TransientVariables.Set(TwitchVars.UserId, channelData.BroadcasterId);
                AppState.TransientVariables.Set(TwitchVars.ChannelTitle, channelData.Title);
                AppState.TransientVariables.Set(TwitchVars.ChannelGame, channelData.GameName);
            }
        }
        catch (Exception e)
        { Logger.Warning($"Couldnt get channel information: {e.Message}"); }
        
        try
        {
            var streamInformation = await TwitchApi.Helix.Streams.GetStreamsAsync(userIds: [userId]);
            if (streamInformation != null && streamInformation.Streams.Length > 0)
            {
                var streamData = streamInformation.Streams[0];
                AppState.TransientVariables.Set(TwitchVars.StreamViewers, streamData?.ViewerCount.ToString() ?? "0");
                AppState.TransientVariables.Set(TwitchVars.StreamThumbnailUrl, streamData?.ThumbnailUrl ?? "");
            }
            else
            { AppState.TransientVariables.Set(TwitchVars.StreamViewers, "0"); }
        }
        catch (Exception e)
        { Logger.Warning($"Couldnt get stream information: {e.Message}"); }
    }

    public void Dispose()
    { _subs?.Dispose(); }
}