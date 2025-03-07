using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Events.Triggers;
using Strem.Flows.Services.Stores;
using Strem.Twitch.Events.OAuth;
using Strem.Twitch.Extensions;
using Strem.Twitch.Flows.Triggers;
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
    public IObservableTwitchEventSub TwitchEventSub { get; }
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IApplicationConfig ApplicationConfig { get; }
    public ILogger<TwitchPluginStartup> Logger { get; }
    public IFlowStore FlowStore { get; }
    
    public string[] RequiredConfigurationKeys { get; } = new[] { TwitchPluginSettings.TwitchClientIdKey };

    public TwitchPluginStartup(ITwitchOAuthClient twitchOAuthClient, ITwitchAPI twitchApi, IObservableTwitchClient twitchClient, IEventBus eventBus, IAppState appState, ILogger<TwitchPluginStartup> logger, IApplicationConfig applicationConfig, IObservableTwitchEventSub twitchEventSub, IFlowStore flowStore)
    {
        TwitchOAuthClient = twitchOAuthClient;
        TwitchApi = twitchApi;
        TwitchClient = twitchClient;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        ApplicationConfig = applicationConfig;
        TwitchEventSub = twitchEventSub;
        FlowStore = flowStore;
    }
    
    public Task SetupPlugin() => Task.CompletedTask;
    
    public async Task StartPlugin()
    {
        Observable.Interval(TimeSpan.FromMinutes(TwitchPluginSettings.RevalidatePeriodInMins))
            .Subscribe(x => VerifyToken())
            .AddTo(_subs);
        
        Observable.Interval(TimeSpan.FromMinutes(TwitchPluginSettings.ChatReconnectInMins))
            .Subscribe(x => AttemptToConnectToChat())
            .AddTo(_subs);
        
        Observable.Interval(TimeSpan.FromMinutes(TwitchPluginSettings.EventSubReconnectInMins))
            .Subscribe(x => AttemptToConnectToEventSub())
            .AddTo(_subs);
        
        Observable.Interval(TimeSpan.FromMinutes(TwitchPluginSettings.RefreshChannelPeriodInMins))
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
        if(TwitchClient.Client.IsConnected) { return; }
        
        Logger.Information($"Checking to see if twitch Chat needs re/connecting");
        if(!AppState.HasTwitchAccessToken()) { return; }
        if(!AppState.HasTwitchScope(ChatScopes.ReadChat)) { return; }
        
        Logger.Information($"Connecting to twitch chat");
        var result = TwitchClient.Client.ConnectOrRefresh(AppState);
        if(result.success)
        { Logger.Information($"Connected to twitch chat"); }
        else
        { Logger.LogError($"Unable to connect to twitch chat due to error: {result.message}"); }
    }
    
    public async Task AttemptToConnectToEventSub()
    {
        if(TwitchEventSub.IsConnected) { return; }
        
        Logger.Information($"Checking to see if twitch EventSub needs re/connecting");
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

            ResetTwitchEventSubTriggers();
        }
        else
        { Logger.Information("Could not connect to twitch EventSub"); }
    }

    public void ResetTwitchEventSubTriggers()
    {
        var flowsWithTwitchEventSubTriggers = FlowStore.Data
            .Where(x => x.TriggerData.Any(trigger => trigger is ITwitchEventSubTriggerData));

        foreach (var flowWithEventSubTrigger in flowsWithTwitchEventSubTriggers)
        {
            // TODO: For now it resets at flow level not trigger level, in future will need to do each individually
            var twitchEventSubTrigger = flowWithEventSubTrigger.TriggerData.First(x => x is ITwitchEventSubTriggerData);
            EventBus.PublishAsync(new FlowTriggerChangedEvent(flowWithEventSubTrigger.Id, twitchEventSubTrigger.Id));
        }
    }

    public async Task VerifyAndSetupConnections()
    {
        await VerifyToken();
        AttemptToConnectToChat();
        await AttemptToConnectToEventSub();
    }

    public async Task DisconnectEverything()
    {
        if(TwitchClient.IsConnected)
        { TwitchClient.Client.Disconnect(); }

        if (TwitchEventSub.IsConnected)
        { await TwitchEventSub.Client.DisconnectAsync(); }
    }

    public async Task VerifyToken()
    {
        Logger.Information("Revalidating Twitch Access Token");

        if (AppState.HasTwitchAccessToken())
        { await TwitchOAuthClient.ValidateToken(); }
        
        AttemptToAuthorizeApi();
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