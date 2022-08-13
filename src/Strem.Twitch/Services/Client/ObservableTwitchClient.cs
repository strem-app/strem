using System.Reactive;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Events;

namespace Strem.Twitch.Services.Client;

public class ObservableTwitchClient : IObservableTwitchClient
{
    public ITwitchClient TwitchClient { get; }
    
    /// <summary>Occurs when [on being hosted].</summary>
    public IObservable<OnBeingHostedArgs> OnBeingHosted { get; private set; }

    /// <summary>Occurs when [on channel state changed].</summary>
    public IObservable<OnChannelStateChangedArgs> OnChannelStateChanged { get; private set; }

    /// <summary>Occurs when [on chat cleared].</summary>
    public IObservable<OnChatClearedArgs> OnChatCleared { get; private set; }

    /// <summary>Occurs when [on chat color changed].</summary>
    public IObservable<OnChatColorChangedArgs> OnChatColorChanged { get; private set; }

    /// <summary>Occurs when [on chat command received].</summary>
    public IObservable<OnChatCommandReceivedArgs> OnChatCommandReceived { get; private set; }

    /// <summary>Occurs when [on connected].</summary>
    public IObservable<OnConnectedArgs> OnConnected { get; private set; }

    /// <summary>Occurs when [on connection error].</summary>
    public IObservable<OnConnectionErrorArgs> OnConnectionError { get; private set; }
    
    /// <summary>Occurs when [on disconnected].</summary>
    public IObservable<OnDisconnectedEventArgs> OnDisconnected { get; private set; }

    /// <summary>Occurs when [on existing users detected].</summary>
    public IObservable<OnExistingUsersDetectedArgs> OnExistingUsersDetected { get; private set; }

    /// <summary>Occurs when [on gifted subscription].</summary>
    public IObservable<OnGiftedSubscriptionArgs> OnGiftedSubscription { get; private set; }

    /// <summary>Occurs when [on hosting started].</summary>
    public IObservable<OnHostingStartedArgs> OnHostingStarted { get; private set; }

    /// <summary>Occurs when [on hosting stopped].</summary>
    public IObservable<OnHostingStoppedArgs> OnHostingStopped { get; private set; }

    /// <summary>Occurs when [on host left].</summary>
    public IObservable<Unit> OnHostLeft { get; private set; }

    /// <summary>Occurs when [on incorrect login].</summary>
    public IObservable<OnIncorrectLoginArgs> OnIncorrectLogin { get; private set; }

    /// <summary>Occurs when [on joined channel].</summary>
    public IObservable<OnJoinedChannelArgs> OnJoinedChannel { get; private set; }

    /// <summary>Occurs when [on left channel].</summary>
    public IObservable<OnLeftChannelArgs> OnLeftChannel { get; private set; }

    /// <summary>Occurs when [on log].</summary>
    public IObservable<OnLogArgs> OnLog { get; private set; }

    /// <summary>Occurs when [on message received].</summary>
    public IObservable<OnMessageReceivedArgs> OnMessageReceived { get; private set; }

    /// <summary>Occurs when [on message sent].</summary>
    public IObservable<OnMessageSentArgs> OnMessageSent { get; private set; }

    /// <summary>Occurs when [on moderator joined].</summary>
    public IObservable<OnModeratorJoinedArgs> OnModeratorJoined { get; private set; }

    /// <summary>Occurs when [on moderator left].</summary>
    public IObservable<OnModeratorLeftArgs> OnModeratorLeft { get; private set; }

    /// <summary>Occurs when [on moderators received].</summary>
    public IObservable<OnModeratorsReceivedArgs> OnModeratorsReceived { get; private set; }

    /// <summary>Occurs when [on new subscriber].</summary>
    public IObservable<OnNewSubscriberArgs> OnNewSubscriber { get; private set; }

    /// <summary>Occurs when [on now hosting].</summary>
    public IObservable<OnNowHostingArgs> OnNowHosting { get; private set; }

    /// <summary>Occurs when [on raid notification].</summary>
    public IObservable<OnRaidNotificationArgs> OnRaidNotification { get; private set; }

    /// <summary>Occurs when [on re subscriber].</summary>
    public IObservable<OnReSubscriberArgs> OnReSubscriber { get; private set; }

    /// <summary>Occurs when [on send receive data].</summary>
    public IObservable<OnSendReceiveDataArgs> OnSendReceiveData { get; private set; }

    /// <summary>Occurs when [on user banned].</summary>
    public IObservable<OnUserBannedArgs> OnUserBanned { get; private set; }

    /// <summary>Occurs when [on user joined].</summary>
    public IObservable<OnUserJoinedArgs> OnUserJoined { get; private set; }

    /// <summary>Occurs when [on user left].</summary>
    public IObservable<OnUserLeftArgs> OnUserLeft { get; private set; }

    /// <summary>Occurs when [on user state changed].</summary>
    public IObservable<OnUserStateChangedArgs> OnUserStateChanged { get; private set; }

    /// <summary>Occurs when [on user timedout].</summary>
    public IObservable<OnUserTimedoutArgs> OnUserTimedout { get; private set; }

    /// <summary>Occurs when [on whisper command received].</summary>
    public IObservable<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived { get; private set; }

    /// <summary>Occurs when [on whisper received].</summary>
    public IObservable<OnWhisperReceivedArgs> OnWhisperReceived { get; private set; }

    /// <summary>Occurs when [on whisper sent].</summary>
    public IObservable<OnWhisperSentArgs> OnWhisperSent { get; private set; }

    /// <summary>Occurs when [on message throttled].</summary>
    public IObservable<OnMessageThrottledEventArgs> OnMessageThrottled { get; private set; }

    /// <summary>Occurs when [on whisper throttled].</summary>
    public IObservable<OnWhisperThrottledEventArgs> OnWhisperThrottled { get; private set; }

    /// <summary>Occurs when [on error].</summary>
    public IObservable<OnErrorEventArgs> OnError { get; private set; }

    /// <summary>Occurs when [on reconnected].</summary>
    public IObservable<OnReconnectedEventArgs> OnReconnected { get; private set; }

    /// <summary>Occurs when [on vip received].</summary>
    public IObservable<OnVIPsReceivedArgs> OnVIPsReceived { get; private set; }

    /// <summary>
    /// Occurs when [on community subscription announcement received].
    /// </summary>
    public IObservable<OnCommunitySubscriptionArgs> OnCommunitySubscription { get; private set; }

    /// <summary>Occurs when [on message deleted].</summary>
    public IObservable<OnMessageClearedArgs> OnMessageCleared { get; private set; }

    /// <summary>Occurs when [on ritual for new chatter received].</summary>
    public IObservable<OnRitualNewChatterArgs> OnRitualNewChatter { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
    /// </summary>
    public IObservable<OnRequiresVerifiedEmailArgs> OnRequiresVerifiedEmail { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified phone number without a verified phone number attached to the account.
    /// </summary>
    public IObservable<OnRequiresVerifiedPhoneNumberArgs> OnRequiresVerifiedPhoneNumber { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
    /// </summary>
    public IObservable<OnBannedEmailAliasArgs> OnBannedEmailAlias { get; private set; }

    public ObservableTwitchClient(ITwitchClient twitchClient)
    {
        TwitchClient = twitchClient;
        SetupObservables();
    }
    
    public void SetupObservables()
    {
        OnConnected = Observable.FromEventPattern<OnConnectedArgs>(
            e => TwitchClient.OnConnected += e,
            e => TwitchClient.OnConnected -= e)
            .Select(x => x.EventArgs);
        
        OnDisconnected = Observable.FromEventPattern<OnDisconnectedEventArgs>(
                e => TwitchClient.OnDisconnected += e,
                e => TwitchClient.OnDisconnected -= e)
            .Select(x => x.EventArgs);
        
        OnError = Observable.FromEventPattern<OnErrorEventArgs>(
                e => TwitchClient.OnError += e,
                e => TwitchClient.OnError -= e)
            .Select(x => x.EventArgs);
        
        OnLog = Observable.FromEventPattern<OnLogArgs>(
                e => TwitchClient.OnLog += e,
                e => TwitchClient.OnLog -= e)
            .Select(x => x.EventArgs);
        
        OnReconnected = Observable.FromEventPattern<OnReconnectedEventArgs>(
                e => TwitchClient.OnReconnected += e,
                e => TwitchClient.OnReconnected -= e)
            .Select(x => x.EventArgs);
        
        OnBeingHosted = Observable.FromEventPattern<OnBeingHostedArgs>(
                e => TwitchClient.OnBeingHosted += e,
                e => TwitchClient.OnBeingHosted -= e)
            .Select(x => x.EventArgs);
        
        OnChatCleared = Observable.FromEventPattern<OnChatClearedArgs>(
                e => TwitchClient.OnChatCleared += e,
                e => TwitchClient.OnChatCleared -= e)
            .Select(x => x.EventArgs);
        
        OnCommunitySubscription = Observable.FromEventPattern<OnCommunitySubscriptionArgs>(
                e => TwitchClient.OnCommunitySubscription += e,
                e => TwitchClient.OnCommunitySubscription -= e)
            .Select(x => x.EventArgs);
        
        OnConnectionError = Observable.FromEventPattern<OnConnectionErrorArgs>(
                e => TwitchClient.OnConnectionError += e,
                e => TwitchClient.OnConnectionError -= e)
            .Select(x => x.EventArgs);
        
        OnGiftedSubscription = Observable.FromEventPattern<OnGiftedSubscriptionArgs>(
                e => TwitchClient.OnGiftedSubscription += e,
                e => TwitchClient.OnGiftedSubscription -= e)
            .Select(x => x.EventArgs);
        
        OnHostingStarted = Observable.FromEventPattern<OnHostingStartedArgs>(
                e => TwitchClient.OnHostingStarted += e,
                e => TwitchClient.OnHostingStarted -= e)
            .Select(x => x.EventArgs);
        
        OnHostingStopped = Observable.FromEventPattern<OnHostingStoppedArgs>(
                e => TwitchClient.OnHostingStopped += e,
                e => TwitchClient.OnHostingStopped -= e)
            .Select(x => x.EventArgs);
        
        OnHostLeft = Observable.FromEventPattern(
                e => TwitchClient.OnHostLeft += e,
                e => TwitchClient.OnHostLeft -= e)
            .ToUnit();

        OnGiftedSubscription = Observable.FromEventPattern<OnGiftedSubscriptionArgs>(
                e => TwitchClient.OnGiftedSubscription += e,
                e => TwitchClient.OnGiftedSubscription -= e)
            .Select(x => x.EventArgs);
        
        OnIncorrectLogin = Observable.FromEventPattern<OnIncorrectLoginArgs>(
                e => TwitchClient.OnIncorrectLogin += e,
                e => TwitchClient.OnIncorrectLogin -= e)
            .Select(x => x.EventArgs);
        
        OnJoinedChannel = Observable.FromEventPattern<OnJoinedChannelArgs>(
                e => TwitchClient.OnJoinedChannel += e,
                e => TwitchClient.OnJoinedChannel -= e)
            .Select(x => x.EventArgs);
        
        OnLeftChannel = Observable.FromEventPattern<OnLeftChannelArgs>(
                e => TwitchClient.OnLeftChannel += e,
                e => TwitchClient.OnLeftChannel -= e)
            .Select(x => x.EventArgs);
        
        OnMessageCleared = Observable.FromEventPattern<OnMessageClearedArgs>(
                e => TwitchClient.OnMessageCleared += e,
                e => TwitchClient.OnMessageCleared -= e)
            .Select(x => x.EventArgs);
        
        OnMessageReceived = Observable.FromEventPattern<OnMessageReceivedArgs>(
                e => TwitchClient.OnMessageReceived += e,
                e => TwitchClient.OnMessageReceived -= e)
            .Select(x => x.EventArgs);
        
        OnMessageSent = Observable.FromEventPattern<OnMessageSentArgs>(
                e => TwitchClient.OnMessageSent += e,
                e => TwitchClient.OnMessageSent -= e)
            .Select(x => x.EventArgs);
        
        OnMessageThrottled = Observable.FromEventPattern<OnMessageThrottledEventArgs>(
                e => TwitchClient.OnMessageThrottled += e,
                e => TwitchClient.OnMessageThrottled -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorJoined = Observable.FromEventPattern<OnModeratorJoinedArgs>(
                e => TwitchClient.OnModeratorJoined += e,
                e => TwitchClient.OnModeratorJoined -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorLeft = Observable.FromEventPattern<OnModeratorLeftArgs>(
                e => TwitchClient.OnModeratorLeft += e,
                e => TwitchClient.OnModeratorLeft -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorsReceived = Observable.FromEventPattern<OnModeratorsReceivedArgs>(
                e => TwitchClient.OnModeratorsReceived += e,
                e => TwitchClient.OnModeratorsReceived -= e)
            .Select(x => x.EventArgs);
        
        OnNewSubscriber = Observable.FromEventPattern<OnNewSubscriberArgs>(
                e => TwitchClient.OnNewSubscriber += e,
                e => TwitchClient.OnNewSubscriber -= e)
            .Select(x => x.EventArgs);
        
        OnNowHosting = Observable.FromEventPattern<OnNowHostingArgs>(
                e => TwitchClient.OnNowHosting += e,
                e => TwitchClient.OnNowHosting -= e)
            .Select(x => x.EventArgs);
        
        OnRaidNotification = Observable.FromEventPattern<OnRaidNotificationArgs>(
                e => TwitchClient.OnRaidNotification += e,
                e => TwitchClient.OnRaidNotification -= e)
            .Select(x => x.EventArgs);
        
        OnReSubscriber = Observable.FromEventPattern<OnReSubscriberArgs>(
                e => TwitchClient.OnReSubscriber += e,
                e => TwitchClient.OnReSubscriber -= e)
            .Select(x => x.EventArgs);
        
        OnUserBanned = Observable.FromEventPattern<OnUserBannedArgs>(
                e => TwitchClient.OnUserBanned += e,
                e => TwitchClient.OnUserBanned -= e)
            .Select(x => x.EventArgs);
        
        OnUserJoined = Observable.FromEventPattern<OnUserJoinedArgs>(
                e => TwitchClient.OnUserJoined += e,
                e => TwitchClient.OnUserJoined -= e)
            .Select(x => x.EventArgs);        
        
        OnUserLeft = Observable.FromEventPattern<OnUserLeftArgs>(
                e => TwitchClient.OnUserLeft += e,
                e => TwitchClient.OnUserLeft -= e)
            .Select(x => x.EventArgs);
        
        OnUserTimedout = Observable.FromEventPattern<OnUserTimedoutArgs>(
                e => TwitchClient.OnUserTimedout += e,
                e => TwitchClient.OnUserTimedout -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperReceived = Observable.FromEventPattern<OnWhisperReceivedArgs>(
                e => TwitchClient.OnWhisperReceived += e,
                e => TwitchClient.OnWhisperReceived -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperSent = Observable.FromEventPattern<OnWhisperSentArgs>(
                e => TwitchClient.OnWhisperSent += e,
                e => TwitchClient.OnWhisperSent -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperThrottled = Observable.FromEventPattern<OnWhisperThrottledEventArgs>(
                e => TwitchClient.OnWhisperThrottled += e,
                e => TwitchClient.OnWhisperThrottled -= e)
            .Select(x => x.EventArgs);
        
        OnBannedEmailAlias = Observable.FromEventPattern<OnBannedEmailAliasArgs>(
                e => TwitchClient.OnBannedEmailAlias += e,
                e => TwitchClient.OnBannedEmailAlias -= e)
            .Select(x => x.EventArgs);
        
        OnChannelStateChanged = Observable.FromEventPattern<OnChannelStateChangedArgs>(
                e => TwitchClient.OnChannelStateChanged += e,
                e => TwitchClient.OnChannelStateChanged -= e)
            .Select(x => x.EventArgs);
        
        OnChatColorChanged = Observable.FromEventPattern<OnChatColorChangedArgs>(
                e => TwitchClient.OnChatColorChanged += e,
                e => TwitchClient.OnChatColorChanged -= e)
            .Select(x => x.EventArgs);
        
        OnChatCommandReceived = Observable.FromEventPattern<OnChatCommandReceivedArgs>(
                e => TwitchClient.OnChatCommandReceived += e,
                e => TwitchClient.OnChatCommandReceived -= e)
            .Select(x => x.EventArgs);
        
        OnExistingUsersDetected = Observable.FromEventPattern<OnExistingUsersDetectedArgs>(
                e => TwitchClient.OnExistingUsersDetected += e,
                e => TwitchClient.OnExistingUsersDetected -= e)
            .Select(x => x.EventArgs);
        
        OnRequiresVerifiedEmail = Observable.FromEventPattern<OnRequiresVerifiedEmailArgs>(
                e => TwitchClient.OnRequiresVerifiedEmail += e,
                e => TwitchClient.OnRequiresVerifiedEmail -= e)
            .Select(x => x.EventArgs);
        
        OnRitualNewChatter = Observable.FromEventPattern<OnRitualNewChatterArgs>(
                e => TwitchClient.OnRitualNewChatter += e,
                e => TwitchClient.OnRitualNewChatter -= e)
            .Select(x => x.EventArgs);
        
        OnSendReceiveData = Observable.FromEventPattern<OnSendReceiveDataArgs>(
                e => TwitchClient.OnSendReceiveData += e,
                e => TwitchClient.OnSendReceiveData -= e)
            .Select(x => x.EventArgs);
        
        OnUserStateChanged = Observable.FromEventPattern<OnUserStateChangedArgs>(
                e => TwitchClient.OnUserStateChanged += e,
                e => TwitchClient.OnUserStateChanged -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperCommandReceived = Observable.FromEventPattern<OnWhisperCommandReceivedArgs>(
                e => TwitchClient.OnWhisperCommandReceived += e,
                e => TwitchClient.OnWhisperCommandReceived -= e)
            .Select(x => x.EventArgs);
        
        OnRequiresVerifiedPhoneNumber = Observable.FromEventPattern<OnRequiresVerifiedPhoneNumberArgs>(
                e => TwitchClient.OnRequiresVerifiedPhoneNumber += e,
                e => TwitchClient.OnRequiresVerifiedPhoneNumber -= e)
            .Select(x => x.EventArgs);
        
        OnVIPsReceived = Observable.FromEventPattern<OnVIPsReceivedArgs>(
                e => TwitchClient.OnVIPsReceived += e,
                e => TwitchClient.OnVIPsReceived -= e)
            .Select(x => x.EventArgs);

    }
}