using System.Reactive;
using TwitchLib.Api.Interfaces;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Stream;
using TwitchLib.EventSub.Websockets.Core.EventArgs.User;

namespace Strem.Twitch.Services.Client;

public interface IObservableTwitchEventSub
{
    EventSubWebsocketClient Client { get; }
    ITwitchAPI TwitchApi { get; }

    /// <summary>Occurs when [on channel state changed].</summary>
    IObservable<ChannelHypeTrainEndArgs> OnChannelHypeTrainEnd { get; }

    /// <summary>Occurs when [on chat cleared].</summary>
    IObservable<ChannelUnbanArgs> OnChannelUnban { get; }

    /// <summary>Occurs when [on chat color changed].</summary>
    IObservable<ChannelHypeTrainProgressArgs> OnChannelHypeTrainProgress { get; }

    /// <summary>Occurs when [on chat command received].</summary>
    IObservable<ChannelPointsCustomRewardArgs> OnChannelPointsCustomRewardAdd { get; }

    /// <summary>Occurs when [on connected].</summary>
    IObservable<WebsocketConnectedArgs> OnConnected { get; }

    /// <summary>Occurs when [on connection error].</summary>
    IObservable<ChannelFollowArgs> OnChannelFollow { get; }

    /// <summary>Occurs when [on disconnected].</summary>
    IObservable<Unit> OnDisconnected { get; }

    /// <summary>Occurs when [on existing users detected].</summary>
    IObservable<ChannelPointsCustomRewardArgs> OnChannelPointsCustomRewardRemove { get; }

    /// <summary>Occurs when [on gifted subscription].</summary>
    IObservable<ChannelRaidArgs> OnChannelRaid { get; }

    /// <summary>Occurs when [on gifted subscription].</summary>
    IObservable<ChannelSubscribeArgs> OnChannelSubscribe { get; }

    /// <summary>Occurs when [on host left].</summary>
    IObservable<Unit> OnHostLeft { get; }

    /// <summary>Occurs when [on incorrect login].</summary>
    IObservable<ChannelUpdateArgs> OnChannelUpdate { get; }

    /// <summary>Occurs when [on joined channel].</summary>
    IObservable<StreamOfflineArgs> OnStreamOffline { get; }

    /// <summary>Occurs when [on left channel].</summary>
    IObservable<StreamOnlineArgs> OnStreamOnline { get; }

    /// <summary>Occurs when [on log].</summary>
    IObservable<ChannelBanArgs> OnChannelBan { get; }

    /// <summary>Occurs when [on message received].</summary>
    IObservable<ChannelGoalBeginArgs> OnChannelGoalBegin { get; }

    /// <summary>Occurs when [on message sent].</summary>
    IObservable<ChannelGoalEndArgs> OnChannelGoalEnd { get; }

    /// <summary>Occurs when [on moderator joined].</summary>
    IObservable<ChannelModeratorArgs> OnModeratorAdd { get; }

    /// <summary>Occurs when [on moderator left].</summary>
    IObservable<ChannelModeratorArgs> OnModeratorRemove { get; }

    /// <summary>Occurs when [on moderators received].</summary>
    IObservable<ChannelPollBeginArgs> OnChannelPollBegin { get; }

    /// <summary>Occurs when [on new subscriber].</summary>
    IObservable<ChannelPollEndArgs> OnChannelPollEnd { get; }

    /// <summary>Occurs when [on raid notification].</summary>
    IObservable<ChannelPollProgressArgs> OnChannelPollProgress { get; }

    /// <summary>Occurs when [on re subscriber].</summary>
    IObservable<ChannelPredictionBeginArgs> OnChannelPredictionBegin { get; }

    /// <summary>Occurs when [on send receive data].</summary>
    IObservable<ChannelPointsCustomRewardRedemptionArgs> OnChannelPointsCustomRewardRedemptionAdd { get; }

    /// <summary>Occurs when [on user banned].</summary>
    IObservable<ChannelPredictionEndArgs> OnChannelPredictionEnd { get; }

    /// <summary>Occurs when [on user joined].</summary>
    IObservable<ChannelPredictionLockArgs> OnChannelPredictionLock { get; }

    /// <summary>Occurs when [on user left].</summary>
    IObservable<ChannelPredictionProgressArgs> OnChannelPredictionProgress { get; }

    /// <summary>Occurs when [on user state changed].</summary>
    IObservable<ChannelPointsCustomRewardRedemptionArgs> OnChannelPointsCustomRewardRedemptionUpdate { get; }

    /// <summary>Occurs when [on user timedout].</summary>
    IObservable<ChannelSubscriptionEndArgs> OnChannelSubscriptionEnd { get; }

    /// <summary>Occurs when [on whisper received].</summary>
    IObservable<ChannelSubscriptionGiftArgs> OnChannelSubscriptionGift { get; }

    /// <summary>Occurs when [on whisper sent].</summary>
    IObservable<ChannelSubscriptionMessageArgs> OnChannelSubscriptionMessage { get; }

    /// <summary>Occurs when [on message throttled].</summary>
    IObservable<ChannelGoalProgressArgs> OnChannelGoalProgress { get; }

    /// <summary>Occurs when [on whisper throttled].</summary>
    IObservable<ChannelCharityCampaignDonateArgs> OnChannelCharityCampaignDonate { get; }

    /// <summary>Occurs when [on error].</summary>
    IObservable<ErrorOccuredArgs> OnError { get; }

    /// <summary>Occurs when [on reconnected].</summary>
    IObservable<Unit> OnReconnected { get; }

    /// <summary>
    /// Occurs when [on community subscription announcement received].
    /// </summary>
    IObservable<ChannelCheerArgs> OnChannelCheer { get; }

    /// <summary>Occurs when [on message deleted].</summary>
    IObservable<UserUpdateArgs> OnUserUpdate { get; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
    /// </summary>
    IObservable<ChannelPointsCustomRewardArgs> OnChannelPointsCustomRewardUpdate { get; }

    /// <summary>
    /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
    /// </summary>
    IObservable<ChannelHypeTrainBeginArgs> OnChannelHypeTrainBegin { get; }

    Task<bool> SubscribeOnChannel(string subType, string channelName, string version = "1");
    bool HasSubscribedTo(string subType, string channelName);
}