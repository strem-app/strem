using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Strem.Core.Extensions;
using Strem.Twitch.Extensions;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Interfaces;
using TwitchLib.EventSub.Core;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Client;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Stream;
using TwitchLib.EventSub.Websockets.Core.EventArgs.User;
using TwitchLib.PubSub.Models.Responses.Messages.AutomodCaughtMessage;

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
        OnConnected = Observable.FromEvent<AsyncEventHandler<WebsocketConnectedArgs>, WebsocketConnectedArgs>(
            h => async (_, args) => h(args),
            h => Client.WebsocketConnected += h,
            h => Client.WebsocketConnected -= h);
        
        OnDisconnected = Observable.FromEvent<AsyncEventHandler, Unit>(
            h => async (_, args) => h(Unit.Default),
            h => Client.WebsocketDisconnected += h,
            h => Client.WebsocketDisconnected -= h);
        
        OnError = Observable.FromEvent<AsyncEventHandler<ErrorOccuredArgs>, ErrorOccuredArgs>(
            h => async (_, args) => h(args),
            h => Client.ErrorOccurred += h,
            h => Client.ErrorOccurred -= h);
        
        OnReconnected = Observable.FromEvent<AsyncEventHandler, Unit>(
            h => async (_, args) => h(Unit.Default),
            h => Client.WebsocketReconnected += h,
            h => Client.WebsocketReconnected -= h);
        
        OnChannelBan = Observable.FromEvent<AsyncEventHandler<ChannelBanArgs>, ChannelBanArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelBan += h,
            h => Client.ChannelBan -= h);

        OnChannelUnban = Observable.FromEvent<AsyncEventHandler<ChannelUnbanArgs>, ChannelUnbanArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelUnban += h,
            h => Client.ChannelUnban -= h);
        
        OnChannelCheer = Observable.FromEvent<AsyncEventHandler<ChannelCheerArgs>, ChannelCheerArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelCheer += h,
            h => Client.ChannelCheer -= h);
        
        OnChannelFollow = Observable.FromEvent<AsyncEventHandler<ChannelFollowArgs>, ChannelFollowArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelFollow += h,
            h => Client.ChannelFollow -= h);
        
        OnChannelRaid = Observable.FromEvent<AsyncEventHandler<ChannelRaidArgs>, ChannelRaidArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelRaid += h,
            h => Client.ChannelRaid -= h);
        
        OnChannelSubscribe = Observable.FromEvent<AsyncEventHandler<ChannelSubscribeArgs>, ChannelSubscribeArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelSubscribe += h,
            h => Client.ChannelSubscribe -= h);
        
        OnChannelUpdate = Observable.FromEvent<AsyncEventHandler<ChannelUpdateArgs>, ChannelUpdateArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelUpdate += h,
            h => Client.ChannelUpdate -= h);
        
        OnStreamOffline = Observable.FromEvent<AsyncEventHandler<StreamOfflineArgs>, StreamOfflineArgs>(
            h => async (_, args) => h(args),
            h => Client.StreamOffline += h,
            h => Client.StreamOffline -= h);
        
        OnStreamOnline = Observable.FromEvent<AsyncEventHandler<StreamOnlineArgs>, StreamOnlineArgs>(
            h => async (_, args) => h(args),
            h => Client.StreamOnline += h,
            h => Client.StreamOnline -= h);
        
        OnUserUpdate = Observable.FromEvent<AsyncEventHandler<UserUpdateArgs>, UserUpdateArgs>(
            h => async (_, args) => h(args),
            h => Client.UserUpdate += h,
            h => Client.UserUpdate -= h);
        
        OnChannelGoalBegin = Observable.FromEvent<AsyncEventHandler<ChannelGoalBeginArgs>, ChannelGoalBeginArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelGoalBegin += h,
            h => Client.ChannelGoalBegin -= h);
        
        OnChannelGoalEnd = Observable.FromEvent<AsyncEventHandler<ChannelGoalEndArgs>, ChannelGoalEndArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelGoalEnd += h,
            h => Client.ChannelGoalEnd -= h);
        
        OnChannelGoalProgress = Observable.FromEvent<AsyncEventHandler<ChannelGoalProgressArgs>, ChannelGoalProgressArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelGoalProgress += h,
            h => Client.ChannelGoalProgress -= h);
        
        OnModeratorAdd = Observable.FromEvent<AsyncEventHandler<ChannelModeratorArgs>, ChannelModeratorArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelModeratorAdd += h,
            h => Client.ChannelModeratorAdd -= h);
        
        OnModeratorRemove = Observable.FromEvent<AsyncEventHandler<ChannelModeratorArgs>, ChannelModeratorArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelModeratorRemove += h,
            h => Client.ChannelModeratorRemove -= h);

        OnChannelPollBegin = Observable.FromEvent<AsyncEventHandler<ChannelPollBeginArgs>, ChannelPollBeginArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPollBegin += h,
            h => Client.ChannelPollBegin -= h);
        
        OnChannelPollEnd = Observable.FromEvent<AsyncEventHandler<ChannelPollEndArgs>, ChannelPollEndArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPollEnd += h,
            h => Client.ChannelPollEnd -= h);
        
        OnChannelPollProgress = Observable.FromEvent<AsyncEventHandler<ChannelPollProgressArgs>, ChannelPollProgressArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPollProgress += h,
            h => Client.ChannelPollProgress -= h);

        OnChannelPredictionBegin = Observable.FromEvent<AsyncEventHandler<ChannelPredictionBeginArgs>, ChannelPredictionBeginArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPredictionBegin += h,
            h => Client.ChannelPredictionBegin -= h);
        
        OnChannelPredictionEnd = Observable.FromEvent<AsyncEventHandler<ChannelPredictionEndArgs>, ChannelPredictionEndArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPredictionEnd += h,
            h => Client.ChannelPredictionEnd -= h);
        
        OnChannelPredictionLock = Observable.FromEvent<AsyncEventHandler<ChannelPredictionLockArgs>, ChannelPredictionLockArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPredictionLock += h,
            h => Client.ChannelPredictionLock -= h);
        
        OnChannelPredictionProgress = Observable.FromEvent<AsyncEventHandler<ChannelPredictionProgressArgs>, ChannelPredictionProgressArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPredictionProgress += h,
            h => Client.ChannelPredictionProgress -= h);    
        
        OnChannelSubscriptionEnd = Observable.FromEvent<AsyncEventHandler<ChannelSubscriptionEndArgs>, ChannelSubscriptionEndArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelSubscriptionEnd += h,
            h => Client.ChannelSubscriptionEnd -= h);  
        
        OnChannelSubscriptionGift = Observable.FromEvent<AsyncEventHandler<ChannelSubscriptionGiftArgs>, ChannelSubscriptionGiftArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelSubscriptionGift += h,
            h => Client.ChannelSubscriptionGift -= h);  
        
        OnChannelSubscriptionMessage = Observable.FromEvent<AsyncEventHandler<ChannelSubscriptionMessageArgs>, ChannelSubscriptionMessageArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelSubscriptionMessage += h,
            h => Client.ChannelSubscriptionMessage -= h);  
        
        OnChannelCharityCampaignDonate = Observable.FromEvent<AsyncEventHandler<ChannelCharityCampaignDonateArgs>, ChannelCharityCampaignDonateArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelCharityCampaignDonate += h,
            h => Client.ChannelCharityCampaignDonate -= h);  
        
        OnChannelHypeTrainBegin = Observable.FromEvent<AsyncEventHandler<ChannelHypeTrainBeginArgs>, ChannelHypeTrainBeginArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelHypeTrainBegin += h,
            h => Client.ChannelHypeTrainBegin -= h);  
        
        OnChannelHypeTrainEnd = Observable.FromEvent<AsyncEventHandler<ChannelHypeTrainEndArgs>, ChannelHypeTrainEndArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelHypeTrainEnd += h,
            h => Client.ChannelHypeTrainEnd -= h);  
        
        OnChannelHypeTrainProgress = Observable.FromEvent<AsyncEventHandler<ChannelHypeTrainProgressArgs>, ChannelHypeTrainProgressArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelHypeTrainProgress += h,
            h => Client.ChannelHypeTrainProgress -= h);  
        
        OnChannelPointsCustomRewardAdd = Observable.FromEvent<AsyncEventHandler<ChannelPointsCustomRewardArgs>, ChannelPointsCustomRewardArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPointsCustomRewardAdd += h,
            h => Client.ChannelPointsCustomRewardAdd -= h);  
        
        OnChannelPointsCustomRewardRemove = Observable.FromEvent<AsyncEventHandler<ChannelPointsCustomRewardArgs>, ChannelPointsCustomRewardArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPointsCustomRewardRemove += h,
            h => Client.ChannelPointsCustomRewardRemove -= h);  
        
        OnChannelPointsCustomRewardUpdate = Observable.FromEvent<AsyncEventHandler<ChannelPointsCustomRewardArgs>, ChannelPointsCustomRewardArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPointsCustomRewardUpdate += h,
            h => Client.ChannelPointsCustomRewardUpdate -= h);  
        
        OnChannelPointsCustomRewardRedemptionAdd = Observable.FromEvent<AsyncEventHandler<ChannelPointsCustomRewardRedemptionArgs>, ChannelPointsCustomRewardRedemptionArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPointsCustomRewardRedemptionAdd += h,
            h => Client.ChannelPointsCustomRewardRedemptionAdd -= h);  
        
        OnChannelPointsCustomRewardRedemptionUpdate = Observable.FromEvent<AsyncEventHandler<ChannelPointsCustomRewardRedemptionArgs>, ChannelPointsCustomRewardRedemptionArgs>(
            h => async (_, args) => h(args),
            h => Client.ChannelPointsCustomRewardRedemptionUpdate += h,
            h => Client.ChannelPointsCustomRewardRedemptionUpdate -= h);
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