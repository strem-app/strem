using System.Reactive;
using System.Reactive.Linq;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Interfaces;
using OnEmoteOnlyArgs = TwitchLib.PubSub.Events.OnEmoteOnlyArgs;
using OnLogArgs = TwitchLib.PubSub.Events.OnLogArgs;

namespace Strem.Twitch.Services.Client;

class ObservableTwitchPubSub : IObservableTwitchPubSub
{
    public ITwitchPubSub PubSub { get; }
    
    /// <summary>Occurs when [on ban].</summary>
    public IObservable<OnBanArgs> OnBan { get; private set; }

    /// <summary>Occurs when [on bits received].</summary>
    public IObservable<OnBitsReceivedArgs> OnBitsReceived { get; private set; }

    /// <summary>Occurs when [on channel extension broadcast].</summary>
    public IObservable<OnChannelExtensionBroadcastArgs> OnChannelExtensionBroadcast { get; private set; }

    /// <summary>Occurs when [on channel subscription].</summary>
    public IObservable<OnChannelSubscriptionArgs> OnChannelSubscription { get; private set; }

    /// <summary>Occurs when [on clear].</summary>
    public IObservable<OnClearArgs> OnClear { get; private set; }

    /// <summary>Occurs when [on emote only].</summary>
    public IObservable<OnEmoteOnlyArgs> OnEmoteOnly { get; private set; }

    /// <summary>Occurs when [on emote only off].</summary>
    public IObservable<OnEmoteOnlyOffArgs> OnEmoteOnlyOff { get; private set; }

    /// <summary>Occurs when [on follow].</summary>
    public IObservable<OnFollowArgs> OnFollow { get; private set; }

    /// <summary>Occurs when [on host].</summary>
    public IObservable<OnHostArgs> OnHost { get; private set; }

    /// <summary>Occurs when [on message deleted].</summary>
    public IObservable<OnMessageDeletedArgs> OnMessageDeleted { get; private set; }

    /// <summary>Occurs when [on listen response].</summary>
    public IObservable<OnListenResponseArgs> OnListenResponse { get; private set; }

    /// <summary>Occurs when [on pub sub service closed].</summary>
    public IObservable<Unit> OnPubSubServiceClosed { get; private set; }

    /// <summary>Occurs when [on pub sub service connected].</summary>
    public IObservable<Unit> OnPubSubServiceConnected { get; private set; }

    /// <summary>Occurs when [on pub sub service error].</summary>
    public IObservable<OnPubSubServiceErrorArgs> OnPubSubServiceError { get; private set; }

    /// <summary>Occurs when [on R9K beta].</summary>
    public IObservable<OnR9kBetaArgs> OnR9kBeta { get; private set; }

    /// <summary>Occurs when [on R9K beta off].</summary>
    public IObservable<OnR9kBetaOffArgs> OnR9kBetaOff { get; private set; }

    /// <summary>Occurs when [on stream down].</summary>
    public IObservable<OnStreamDownArgs> OnStreamDown { get; private set; }

    /// <summary>Occurs when [on stream up].</summary>
    public IObservable<OnStreamUpArgs> OnStreamUp { get; private set; }

    /// <summary>Occurs when [on subscribers only].</summary>
    public IObservable<OnSubscribersOnlyArgs> OnSubscribersOnly { get; private set; }

    /// <summary>Occurs when [on subscribers only off].</summary>
    public IObservable<OnSubscribersOnlyOffArgs> OnSubscribersOnlyOff { get; private set; }

    /// <summary>Occurs when [on timeout].</summary>
    public IObservable<OnTimeoutArgs> OnTimeout { get; private set; }

    /// <summary>Occurs when [on unban].</summary>
    public IObservable<OnUnbanArgs> OnUnban { get; private set; }

    /// <summary>Occurs when [on untimeout].</summary>
    public IObservable<OnUntimeoutArgs> OnUntimeout { get; private set; }

    /// <summary>Occurs when [on view count].</summary>
    public IObservable<OnViewCountArgs> OnViewCount { get; private set; }

    /// <summary>Occurs when [on whisper].</summary>
    public IObservable<OnWhisperArgs> OnWhisper { get; private set; }

    /// <summary>Occurs when [on reward created]</summary>
    public IObservable<OnCustomRewardCreatedArgs> OnCustomRewardCreated { get; private set; }

    /// <summary>Occurs when [on reward updated]</summary>
    public IObservable<OnCustomRewardUpdatedArgs> OnCustomRewardUpdated { get; private set; }

    /// <summary>Occurs when [on reward deleted]</summary>
    public IObservable<OnCustomRewardDeletedArgs> OnCustomRewardDeleted { get; private set; }

    /// <summary>Occurs when [on reward redeemed]</summary>
    public IObservable<OnRewardRedeemedArgs> OnRewardRedeemed { get; private set; }
    
    /// <summary>Occurs when [on channel points reward redeemed]</summary>
    public IObservable<OnChannelPointsRewardRedeemedArgs> OnChannelPointsRewardRedeemed { get; private set; }

    /// <summary>Occurs when [on leaderboard subs].</summary>
    public IObservable<OnLeaderboardEventArgs> OnLeaderboardSubs { get; private set; }

    /// <summary>Occurs when [on leaderboard bits].</summary>
    public IObservable<OnLeaderboardEventArgs> OnLeaderboardBits { get; private set; }

    /// <summary>Occurs when [on raid update]</summary>
    public IObservable<OnRaidUpdateArgs> OnRaidUpdate { get; private set; }

    /// <summary>Occurs when [on raid update v2]</summary>
    public IObservable<OnRaidUpdateV2Args> OnRaidUpdateV2 { get; private set; }

    /// <summary>Occurs when [on raid go]</summary>
    public IObservable<OnRaidGoArgs> OnRaidGo { get; private set; }

    /// <summary>Occurs when [on log].</summary>
    public IObservable<OnLogArgs> OnLog { get; private set; }

    /// <summary>Occurs when [on commercial].</summary>
    public IObservable<OnCommercialArgs> OnCommercial { get; private set; }

    /// <summary>Occurs when [on prediction].</summary>
    public IObservable<OnPredictionArgs> OnPrediction { get; private set; }

    public ObservableTwitchPubSub(ITwitchPubSub pubSub)
    {
        PubSub = pubSub;
        SetupObservables();
    }

    private void SetupObservables()
    {
        OnBan = Observable.FromEventPattern<OnBanArgs>(
                e => PubSub.OnBan += e,
                e => PubSub.OnBan -= e)
            .Select(x => x.EventArgs);
        
        OnBitsReceived = Observable.FromEventPattern<OnBitsReceivedArgs>(
                e => PubSub.OnBitsReceived += e,
                e => PubSub.OnBitsReceived -= e)
            .Select(x => x.EventArgs);
        
        OnChannelExtensionBroadcast = Observable.FromEventPattern<OnChannelExtensionBroadcastArgs>(
                e => PubSub.OnChannelExtensionBroadcast += e,
                e => PubSub.OnChannelExtensionBroadcast -= e)
            .Select(x => x.EventArgs);
        
        OnChannelSubscription = Observable.FromEventPattern<OnChannelSubscriptionArgs>(
                e => PubSub.OnChannelSubscription += e,
                e => PubSub.OnChannelSubscription -= e)
            .Select(x => x.EventArgs);
        
        OnEmoteOnly = Observable.FromEventPattern<OnEmoteOnlyArgs>(
                e => PubSub.OnEmoteOnly += e,
                e => PubSub.OnEmoteOnly -= e)
            .Select(x => x.EventArgs);
        
        OnEmoteOnlyOff = Observable.FromEventPattern<OnEmoteOnlyOffArgs>(
                e => PubSub.OnEmoteOnlyOff += e,
                e => PubSub.OnEmoteOnlyOff -= e)
            .Select(x => x.EventArgs);
        
        OnFollow = Observable.FromEventPattern<OnFollowArgs>(
                e => PubSub.OnFollow += e,
                e => PubSub.OnFollow -= e)
            .Select(x => x.EventArgs);
        
        OnSubscribersOnly = Observable.FromEventPattern<OnSubscribersOnlyArgs>(
                e => PubSub.OnSubscribersOnly += e,
                e => PubSub.OnSubscribersOnly -= e)
            .Select(x => x.EventArgs);
        
        OnSubscribersOnlyOff = Observable.FromEventPattern<OnSubscribersOnlyOffArgs>(
                e => PubSub.OnSubscribersOnlyOff += e,
                e => PubSub.OnSubscribersOnlyOff -= e)
            .Select(x => x.EventArgs);
        
        OnChannelPointsRewardRedeemed = Observable.FromEventPattern<OnChannelPointsRewardRedeemedArgs>(
                e => PubSub.OnChannelPointsRewardRedeemed += e,
                e => PubSub.OnChannelPointsRewardRedeemed -= e)
            .Select(x => x.EventArgs);
        
        // Todo: Add More Here
    }
}