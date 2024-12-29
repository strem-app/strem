using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Strem.Core.Extensions;
using Strem.Twitch.Extensions;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Interfaces;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Client;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Stream;
using TwitchLib.EventSub.Websockets.Core.EventArgs.User;

namespace Strem.Twitch.Services.Client;

public class ObservableTwitchEventSub : IObservableTwitchEventSub
{
    public EventSubWebsocketClient Client { get; }
    public ITwitchAPI TwitchApi { get; }
    public ILogger<ObservableTwitchEventSub> Logger { get; }

    private Dictionary<string, List<string>> _subscriptions = new();

    public IReadOnlyDictionary<string, List<string>> Subscriptions => _subscriptions;
    
    /// <summary>Occurs when [on channel state changed].</summary>
    public IObservable<ChannelHypeTrainEndArgs> OnChannelHypeTrainEnd { get; private set; }

    /// <summary>Occurs when [on chat cleared].</summary>
    public IObservable<ChannelUnbanArgs> OnChannelUnban { get; private set; }

    /// <summary>Occurs when [on chat color changed].</summary>
    public IObservable<ChannelHypeTrainProgressArgs> OnChannelHypeTrainProgress { get; private set; }

    /// <summary>Occurs when [on chat command received].</summary>
    public IObservable<ChannelPointsCustomRewardArgs> OnChannelPointsCustomRewardAdd { get; private set; }

    /// <summary>Occurs when [on connected].</summary>
    public IObservable<WebsocketConnectedArgs> OnConnected { get; private set; }

    /// <summary>Occurs when [on connection error].</summary>
    public IObservable<ChannelFollowArgs> OnChannelFollow { get; private set; }
    
    /// <summary>Occurs when [on disconnected].</summary>
    public IObservable<Unit> OnDisconnected { get; private set; }

    /// <summary>Occurs when [on existing users detected].</summary>
    public IObservable<ChannelPointsCustomRewardArgs> OnChannelPointsCustomRewardRemove { get; private set; }

    /// <summary>Occurs when [on gifted subscription].</summary>
    public IObservable<ChannelRaidArgs> OnChannelRaid { get; private set; }
    
    /// <summary>Occurs when [on gifted subscription].</summary>
    public IObservable<ChannelSubscribeArgs> OnChannelSubscribe { get; private set; }

    /// <summary>Occurs when [on host left].</summary>
    public IObservable<Unit> OnHostLeft { get; private set; }

    /// <summary>Occurs when [on incorrect login].</summary>
    public IObservable<ChannelUpdateArgs> OnChannelUpdate { get; private set; }

    /// <summary>Occurs when [on joined channel].</summary>
    public IObservable<StreamOfflineArgs> OnStreamOffline { get; private set; }

    /// <summary>Occurs when [on left channel].</summary>
    public IObservable<StreamOnlineArgs> OnStreamOnline { get; private set; }

    /// <summary>Occurs when [on log].</summary>
    public IObservable<ChannelBanArgs> OnChannelBan { get; private set; }

    /// <summary>Occurs when [on message received].</summary>
    public IObservable<ChannelGoalBeginArgs> OnChannelGoalBegin { get; private set; }

    /// <summary>Occurs when [on message sent].</summary>
    public IObservable<ChannelGoalEndArgs> OnChannelGoalEnd { get; private set; }

    /// <summary>Occurs when [on moderator joined].</summary>
    public IObservable<ChannelModeratorArgs> OnModeratorAdd { get; private set; }

    /// <summary>Occurs when [on moderator left].</summary>
    public IObservable<ChannelModeratorArgs> OnModeratorRemove { get; private set; }

    /// <summary>Occurs when [on moderators received].</summary>
    public IObservable<ChannelPollBeginArgs> OnChannelPollBegin { get; private set; }

    /// <summary>Occurs when [on new subscriber].</summary>
    public IObservable<ChannelPollEndArgs> OnChannelPollEnd { get; private set; }

    /// <summary>Occurs when [on raid notification].</summary>
    public IObservable<ChannelPollProgressArgs> OnChannelPollProgress { get; private set; }

    /// <summary>Occurs when [on re subscriber].</summary>
    public IObservable<ChannelPredictionBeginArgs> OnChannelPredictionBegin { get; private set; }

    /// <summary>Occurs when [on send receive data].</summary>
    public IObservable<ChannelPointsCustomRewardRedemptionArgs> OnChannelPointsCustomRewardRedemptionAdd { get; private set; }

    /// <summary>Occurs when [on user banned].</summary>
    public IObservable<ChannelPredictionEndArgs> OnChannelPredictionEnd { get; private set; }

    /// <summary>Occurs when [on user joined].</summary>
    public IObservable<ChannelPredictionLockArgs> OnChannelPredictionLock { get; private set; }

    /// <summary>Occurs when [on user left].</summary>
    public IObservable<ChannelPredictionProgressArgs> OnChannelPredictionProgress { get; private set; }

    /// <summary>Occurs when [on user state changed].</summary>
    public IObservable<ChannelPointsCustomRewardRedemptionArgs> OnChannelPointsCustomRewardRedemptionUpdate { get; private set; }

    /// <summary>Occurs when [on user timedout].</summary>
    public IObservable<ChannelSubscriptionEndArgs> OnChannelSubscriptionEnd { get; private set; }

    /// <summary>Occurs when [on whisper received].</summary>
    public IObservable<ChannelSubscriptionGiftArgs> OnChannelSubscriptionGift { get; private set; }

    /// <summary>Occurs when [on whisper sent].</summary>
    public IObservable<ChannelSubscriptionMessageArgs> OnChannelSubscriptionMessage { get; private set; }

    /// <summary>Occurs when [on message throttled].</summary>
    public IObservable<ChannelGoalProgressArgs> OnChannelGoalProgress { get; private set; }

    /// <summary>Occurs when [on whisper throttled].</summary>
    public IObservable<ChannelCharityCampaignDonateArgs> OnChannelCharityCampaignDonate { get; private set; }

    /// <summary>Occurs when [on error].</summary>
    public IObservable<ErrorOccuredArgs> OnError { get; private set; }

    /// <summary>Occurs when [on reconnected].</summary>
    public IObservable<Unit> OnReconnected { get; private set; }

    /// <summary>
    /// Occurs when [on community subscription announcement received].
    /// </summary>
    public IObservable<ChannelCheerArgs> OnChannelCheer { get; private set; }

    /// <summary>Occurs when [on message deleted].</summary>
    public IObservable<UserUpdateArgs> OnUserUpdate { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
    /// </summary>
    public IObservable<ChannelPointsCustomRewardArgs> OnChannelPointsCustomRewardUpdate { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
    /// </summary>
    public IObservable<ChannelHypeTrainBeginArgs> OnChannelHypeTrainBegin { get; private set; }

    private FieldInfo _webSocketFieldInfo;

    public ObservableTwitchEventSub(EventSubWebsocketClient client, ITwitchAPI twitchApi, ILogger<ObservableTwitchEventSub> logger)
    {
        Client = client;
        TwitchApi = twitchApi;
        Logger = logger;
        SetupObservables();

        _webSocketFieldInfo = typeof(EventSubWebsocketClient).GetField("_websocketClient", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    public bool IsConnected => ((WebsocketClient)_webSocketFieldInfo.GetValue(Client))?.IsConnected ?? false;
    
    public void SetupObservables()
    {
        OnConnected = Observable.FromEventPattern<WebsocketConnectedArgs>(
                e => Client.WebsocketConnected += e,
                e => Client.WebsocketConnected -= e)
            .Select(x => x.EventArgs);
        
        OnDisconnected = Observable.FromEventPattern(
                e => Client.WebsocketDisconnected += e,
                e => Client.WebsocketDisconnected -= e)
            .ToUnit();
        
        OnError = Observable.FromEventPattern<ErrorOccuredArgs>(
                e => Client.ErrorOccurred += e,
                e => Client.ErrorOccurred -= e)
            .Select(x => x.EventArgs);
        
        OnReconnected = Observable.FromEventPattern(
                e => Client.WebsocketReconnected += e,
                e => Client.WebsocketReconnected -= e)
            .ToUnit();
        
        OnChannelBan = Observable.FromEventPattern<ChannelBanArgs>(
                e => Client.ChannelBan += e,
                e => Client.ChannelBan -= e)
            .Select(x => x.EventArgs);
        
        OnChannelUnban = Observable.FromEventPattern<ChannelUnbanArgs>(
                e => Client.ChannelUnban += e,
                e => Client.ChannelUnban -= e)
            .Select(x => x.EventArgs);
        
        OnChannelCheer = Observable.FromEventPattern<ChannelCheerArgs>(
                e => Client.ChannelCheer += e,
                e => Client.ChannelCheer -= e)
            .Select(x => x.EventArgs);
        
        OnChannelFollow = Observable.FromEventPattern<ChannelFollowArgs>(
                e => Client.ChannelFollow += e,
                e => Client.ChannelFollow -= e)
            .Select(x => x.EventArgs);
        
        OnChannelRaid = Observable.FromEventPattern<ChannelRaidArgs>(
                e => Client.ChannelRaid += e,
                e => Client.ChannelRaid -= e)
            .Select(x => x.EventArgs);

        OnChannelSubscribe = Observable.FromEventPattern<ChannelSubscribeArgs>(
                e => Client.ChannelSubscribe += e,
                e => Client.ChannelSubscribe -= e)
            .Select(x => x.EventArgs);
        
        OnChannelUpdate = Observable.FromEventPattern<ChannelUpdateArgs>(
                e => Client.ChannelUpdate += e,
                e => Client.ChannelUpdate -= e)
            .Select(x => x.EventArgs);
        
        OnStreamOffline = Observable.FromEventPattern<StreamOfflineArgs>(
                e => Client.StreamOffline += e,
                e => Client.StreamOffline -= e)
            .Select(x => x.EventArgs);
        
        OnStreamOnline = Observable.FromEventPattern<StreamOnlineArgs>(
                e => Client.StreamOnline += e,
                e => Client.StreamOnline -= e)
            .Select(x => x.EventArgs);
        
        OnUserUpdate = Observable.FromEventPattern<UserUpdateArgs>(
                e => Client.UserUpdate += e,
                e => Client.UserUpdate -= e)
            .Select(x => x.EventArgs);
        
        OnChannelGoalBegin = Observable.FromEventPattern<ChannelGoalBeginArgs>(
                e => Client.ChannelGoalBegin += e,
                e => Client.ChannelGoalBegin -= e)
            .Select(x => x.EventArgs);
        
        OnChannelGoalEnd = Observable.FromEventPattern<ChannelGoalEndArgs>(
                e => Client.ChannelGoalEnd += e,
                e => Client.ChannelGoalEnd -= e)
            .Select(x => x.EventArgs);
        
        OnChannelGoalProgress = Observable.FromEventPattern<ChannelGoalProgressArgs>(
                e => Client.ChannelGoalProgress += e,
                e => Client.ChannelGoalProgress -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorAdd = Observable.FromEventPattern<ChannelModeratorArgs>(
                e => Client.ChannelModeratorAdd += e,
                e => Client.ChannelModeratorAdd -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorRemove = Observable.FromEventPattern<ChannelModeratorArgs>(
                e => Client.ChannelModeratorRemove += e,
                e => Client.ChannelModeratorRemove -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPollBegin = Observable.FromEventPattern<ChannelPollBeginArgs>(
                e => Client.ChannelPollBegin += e,
                e => Client.ChannelPollBegin -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPollEnd = Observable.FromEventPattern<ChannelPollEndArgs>(
                e => Client.ChannelPollEnd += e,
                e => Client.ChannelPollEnd -= e)
            .Select(x => x.EventArgs);

        OnChannelPollProgress = Observable.FromEventPattern<ChannelPollProgressArgs>(
                e => Client.ChannelPollProgress += e,
                e => Client.ChannelPollProgress -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPredictionBegin = Observable.FromEventPattern<ChannelPredictionBeginArgs>(
                e => Client.ChannelPredictionBegin += e,
                e => Client.ChannelPredictionBegin -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPredictionEnd = Observable.FromEventPattern<ChannelPredictionEndArgs>(
                e => Client.ChannelPredictionEnd += e,
                e => Client.ChannelPredictionEnd -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPredictionLock = Observable.FromEventPattern<ChannelPredictionLockArgs>(
                e => Client.ChannelPredictionLock += e,
                e => Client.ChannelPredictionLock -= e)
            .Select(x => x.EventArgs);        
        
        OnChannelPredictionProgress = Observable.FromEventPattern<ChannelPredictionProgressArgs>(
                e => Client.ChannelPredictionProgress += e,
                e => Client.ChannelPredictionProgress -= e)
            .Select(x => x.EventArgs);
        
        OnChannelSubscriptionEnd = Observable.FromEventPattern<ChannelSubscriptionEndArgs>(
                e => Client.ChannelSubscriptionEnd += e,
                e => Client.ChannelSubscriptionEnd -= e)
            .Select(x => x.EventArgs);
        
        OnChannelSubscriptionGift = Observable.FromEventPattern<ChannelSubscriptionGiftArgs>(
                e => Client.ChannelSubscriptionGift += e,
                e => Client.ChannelSubscriptionGift -= e)
            .Select(x => x.EventArgs);
        
        OnChannelSubscriptionMessage = Observable.FromEventPattern<ChannelSubscriptionMessageArgs>(
                e => Client.ChannelSubscriptionMessage += e,
                e => Client.ChannelSubscriptionMessage -= e)
            .Select(x => x.EventArgs);
        
        OnChannelCharityCampaignDonate = Observable.FromEventPattern<ChannelCharityCampaignDonateArgs>(
                e => Client.ChannelCharityCampaignDonate += e,
                e => Client.ChannelCharityCampaignDonate -= e)
            .Select(x => x.EventArgs);
        
        OnChannelHypeTrainBegin = Observable.FromEventPattern<ChannelHypeTrainBeginArgs>(
                e => Client.ChannelHypeTrainBegin += e,
                e => Client.ChannelHypeTrainBegin -= e)
            .Select(x => x.EventArgs);
        
        OnChannelHypeTrainEnd = Observable.FromEventPattern<ChannelHypeTrainEndArgs>(
                e => Client.ChannelHypeTrainEnd += e,
                e => Client.ChannelHypeTrainEnd -= e)
            .Select(x => x.EventArgs);
        
        OnChannelHypeTrainProgress = Observable.FromEventPattern<ChannelHypeTrainProgressArgs>(
                e => Client.ChannelHypeTrainProgress += e,
                e => Client.ChannelHypeTrainProgress -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPointsCustomRewardAdd = Observable.FromEventPattern<ChannelPointsCustomRewardArgs>(
                e => Client.ChannelPointsCustomRewardAdd += e,
                e => Client.ChannelPointsCustomRewardAdd -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPointsCustomRewardRemove = Observable.FromEventPattern<ChannelPointsCustomRewardArgs>(
                e => Client.ChannelPointsCustomRewardRemove += e,
                e => Client.ChannelPointsCustomRewardRemove -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPointsCustomRewardUpdate = Observable.FromEventPattern<ChannelPointsCustomRewardArgs>(
                e => Client.ChannelPointsCustomRewardUpdate += e,
                e => Client.ChannelPointsCustomRewardUpdate -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPointsCustomRewardRedemptionAdd = Observable.FromEventPattern<ChannelPointsCustomRewardRedemptionArgs>(
                e => Client.ChannelPointsCustomRewardRedemptionAdd += e,
                e => Client.ChannelPointsCustomRewardRedemptionAdd -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPointsCustomRewardRedemptionUpdate = Observable.FromEventPattern<ChannelPointsCustomRewardRedemptionArgs>(
                e => Client.ChannelPointsCustomRewardRedemptionUpdate += e,
                e => Client.ChannelPointsCustomRewardRedemptionUpdate -= e)
            .Select(x => x.EventArgs);
    }

    public async Task<bool> SubscribeOnChannel(string subType, string channelName, string version = "1")
    {
        if (!_subscriptions.ContainsKey(channelName))
        { _subscriptions.Add(channelName, []); }

        if (_subscriptions[channelName].Contains(subType)) { return false; }

        try
        {
            var userId = await TwitchApi.GetUserIdFromName(channelName);
            var conditions = new Dictionary<string, string>()
            {
                { "broadcaster_user_id", userId }
            };

            Logger.Information($"Attempting to Subscribe to [{channelName}] for [{subType}]");
            
            await TwitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(subType, version, conditions,
                EventSubTransportMethod.Websocket, Client.SessionId);

            _subscriptions[channelName].Add(subType);
            
            Logger.Information($"Successfully Subscribed to [{channelName}] for [{subType}]");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Unable to Subscribe to [{channelName}] for [{subType}]");
            return false;
        }

    }

    public bool HasSubscribedTo(string subType, string channelName)
    {
        var hasChannel = _subscriptions.ContainsKey(channelName);
        return hasChannel && _subscriptions[channelName].Contains(subType);
    }
}