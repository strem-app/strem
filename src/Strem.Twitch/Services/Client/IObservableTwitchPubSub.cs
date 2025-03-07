using System.Reactive;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Interfaces;

namespace Strem.Twitch.Services.Client;

public interface IObservableTwitchPubSub
{
    ITwitchPubSub PubSub { get; }

    /// <summary>Occurs when [on ban].</summary>
    IObservable<OnBanArgs> OnBan { get; }

    /// <summary>Occurs when [on bits received].</summary>
    IObservable<OnBitsReceivedArgs> OnBitsReceived { get; }

    /// <summary>Occurs when [on channel extension broadcast].</summary>
    IObservable<OnChannelExtensionBroadcastArgs> OnChannelExtensionBroadcast { get; }

    /// <summary>Occurs when [on channel subscription].</summary>
    IObservable<OnChannelSubscriptionArgs> OnChannelSubscription { get; }

    /// <summary>Occurs when [on clear].</summary>
    IObservable<OnClearArgs> OnClear { get; }

    /// <summary>Occurs when [on emote only].</summary>
    IObservable<OnEmoteOnlyArgs> OnEmoteOnly { get; }

    /// <summary>Occurs when [on emote only off].</summary>
    IObservable<OnEmoteOnlyOffArgs> OnEmoteOnlyOff { get; }

    /// <summary>Occurs when [on follow].</summary>
    IObservable<OnFollowArgs> OnFollow { get; }

    /// <summary>Occurs when [on host].</summary>
    IObservable<OnHostArgs> OnHost { get; }

    /// <summary>Occurs when [on message deleted].</summary>
    IObservable<OnMessageDeletedArgs> OnMessageDeleted { get; }

    /// <summary>Occurs when [on listen response].</summary>
    IObservable<OnListenResponseArgs> OnListenResponse { get; }

    /// <summary>Occurs when [on pub sub service closed].</summary>
    IObservable<Unit> OnPubSubServiceClosed { get; }

    /// <summary>Occurs when [on pub sub service connected].</summary>
    IObservable<Unit> OnPubSubServiceConnected { get; }

    /// <summary>Occurs when [on pub sub service error].</summary>
    IObservable<OnPubSubServiceErrorArgs> OnPubSubServiceError { get; }

    /// <summary>Occurs when [on R9K beta].</summary>
    IObservable<OnR9kBetaArgs> OnR9kBeta { get; }

    /// <summary>Occurs when [on R9K beta off].</summary>
    IObservable<OnR9kBetaOffArgs> OnR9kBetaOff { get; }

    /// <summary>Occurs when [on stream down].</summary>
    IObservable<OnStreamDownArgs> OnStreamDown { get; }

    /// <summary>Occurs when [on stream up].</summary>
    IObservable<OnStreamUpArgs> OnStreamUp { get; }

    /// <summary>Occurs when [on subscribers only].</summary>
    IObservable<OnSubscribersOnlyArgs> OnSubscribersOnly { get; }

    /// <summary>Occurs when [on subscribers only off].</summary>
    IObservable<OnSubscribersOnlyOffArgs> OnSubscribersOnlyOff { get; }

    /// <summary>Occurs when [on timeout].</summary>
    IObservable<OnTimeoutArgs> OnTimeout { get; }

    /// <summary>Occurs when [on unban].</summary>
    IObservable<OnUnbanArgs> OnUnban { get; }

    /// <summary>Occurs when [on untimeout].</summary>
    IObservable<OnUntimeoutArgs> OnUntimeout { get; }

    /// <summary>Occurs when [on view count].</summary>
    IObservable<OnViewCountArgs> OnViewCount { get; }

    /// <summary>Occurs when [on whisper].</summary>
    IObservable<OnWhisperArgs> OnWhisper { get; }

    /// <summary>Occurs when [on reward created]</summary>
    IObservable<OnCustomRewardCreatedArgs> OnCustomRewardCreated { get; }

    /// <summary>Occurs when [on reward updated]</summary>
    IObservable<OnCustomRewardUpdatedArgs> OnCustomRewardUpdated { get; }

    /// <summary>Occurs when [on reward deleted]</summary>
    IObservable<OnCustomRewardDeletedArgs> OnCustomRewardDeleted { get; }

    /// <summary>Occurs when [on leaderboard subs].</summary>
    IObservable<OnLeaderboardEventArgs> OnLeaderboardSubs { get; }

    /// <summary>Occurs when [on leaderboard bits].</summary>
    IObservable<OnLeaderboardEventArgs> OnLeaderboardBits { get; }

    /// <summary>Occurs when [on raid update]</summary>
    IObservable<OnRaidUpdateArgs> OnRaidUpdate { get; }

    /// <summary>Occurs when [on raid update v2]</summary>
    IObservable<OnRaidUpdateV2Args> OnRaidUpdateV2 { get; }

    /// <summary>Occurs when [on raid go]</summary>
    IObservable<OnRaidGoArgs> OnRaidGo { get; }

    /// <summary>Occurs when [on log].</summary>
    IObservable<OnLogArgs> OnLog { get; }

    /// <summary>Occurs when [on commercial].</summary>
    IObservable<OnCommercialArgs> OnCommercial { get; }

    /// <summary>Occurs when [on prediction].</summary>
    IObservable<OnPredictionArgs> OnPrediction { get; }

    /// <summary>Occurs when [on reward redeemed]</summary>
    IObservable<OnRewardRedeemedArgs> OnRewardRedeemed { get; }
}