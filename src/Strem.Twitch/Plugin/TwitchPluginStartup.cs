﻿using System.Reactive.Disposables;
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
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IApplicationConfig ApplicationConfig { get; }
    public ILogger<TwitchPluginStartup> Logger { get; }

    public string[] RequiredConfigurationKeys { get; } = new[] { TwitchPluginSettings.TwitchClientIdKey };

    public TwitchPluginStartup(ITwitchOAuthClient twitchOAuthClient, ITwitchAPI twitchApi, IObservableTwitchClient twitchClient, IObservableTwitchPubSub twitchPubSub, IEventBus eventBus, IAppState appState, ILogger<TwitchPluginStartup> logger, IApplicationConfig applicationConfig)
    {
        TwitchOAuthClient = twitchOAuthClient;
        TwitchApi = twitchApi;
        TwitchClient = twitchClient;
        TwitchPubSub = twitchPubSub;
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        ApplicationConfig = applicationConfig;
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
        if(TwitchClient.Client.IsConnected) { return; }
        if(!AppState.HasTwitchScope(ChatScopes.ReadChat)) { return; }
            
        Logger.Information($"Connecting to twitch pubsub");
        TwitchPubSub.PubSub.Connect();
    }

    public async Task VerifyAndSetupConnections()
    {
        await VerifyToken();
        AttemptToConnectToChat();
    }

    public async Task DisconnectEverything()
    {
        if(TwitchClient.Client.IsConnected)
        { TwitchClient.Client.Disconnect(); }

        TwitchPubSub.PubSub.Disconnect();
    }

    public async Task VerifyToken()
    {
        Logger.Information("Revalidating Twitch Access Token");

        if (AppState.HasTwitchAccessToken())
        { await TwitchOAuthClient.ValidateToken(); }
    }

    public async Task RefreshChannelDetails()
    {
        Logger.Information("Refreshing Twitch Channel & Stream Information");
        
        if (!AppState.HasTwitchAccessToken()) { return; }
        TwitchApi.Settings.SetCredentials(ApplicationConfig, AppState);
        
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
            var streamInformation = await TwitchApi.Helix.Streams.GetStreamsAsync(userIds: new List<string> { userId });
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