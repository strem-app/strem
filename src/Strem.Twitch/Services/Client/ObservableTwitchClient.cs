using System.Reactive;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Events;

namespace Strem.Twitch.Services.Client;

public class ObservableTwitchClient : IObservableTwitchClient
{
    public ITwitchClient Client { get; }
    
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

    public ObservableTwitchClient(ITwitchClient client)
    {
        Client = client;
        SetupObservables();
    }
    
    public void SetupObservables()
    {
        OnConnected = Observable.FromEventPattern<OnConnectedArgs>(
            e => Client.OnConnected += e,
            e => Client.OnConnected -= e)
            .Select(x => x.EventArgs);
        
        OnDisconnected = Observable.FromEventPattern<OnDisconnectedEventArgs>(
                e => Client.OnDisconnected += e,
                e => Client.OnDisconnected -= e)
            .Select(x => x.EventArgs);
        
        OnError = Observable.FromEventPattern<OnErrorEventArgs>(
                e => Client.OnError += e,
                e => Client.OnError -= e)
            .Select(x => x.EventArgs);
        
        OnLog = Observable.FromEventPattern<OnLogArgs>(
                e => Client.OnLog += e,
                e => Client.OnLog -= e)
            .Select(x => x.EventArgs);
        
        OnReconnected = Observable.FromEventPattern<OnReconnectedEventArgs>(
                e => Client.OnReconnected += e,
                e => Client.OnReconnected -= e)
            .Select(x => x.EventArgs);
        
        OnChatCleared = Observable.FromEventPattern<OnChatClearedArgs>(
                e => Client.OnChatCleared += e,
                e => Client.OnChatCleared -= e)
            .Select(x => x.EventArgs);
        
        OnCommunitySubscription = Observable.FromEventPattern<OnCommunitySubscriptionArgs>(
                e => Client.OnCommunitySubscription += e,
                e => Client.OnCommunitySubscription -= e)
            .Select(x => x.EventArgs);
        
        OnConnectionError = Observable.FromEventPattern<OnConnectionErrorArgs>(
                e => Client.OnConnectionError += e,
                e => Client.OnConnectionError -= e)
            .Select(x => x.EventArgs);
        
        OnGiftedSubscription = Observable.FromEventPattern<OnGiftedSubscriptionArgs>(
                e => Client.OnGiftedSubscription += e,
                e => Client.OnGiftedSubscription -= e)
            .Select(x => x.EventArgs);

        OnGiftedSubscription = Observable.FromEventPattern<OnGiftedSubscriptionArgs>(
                e => Client.OnGiftedSubscription += e,
                e => Client.OnGiftedSubscription -= e)
            .Select(x => x.EventArgs);
        
        OnIncorrectLogin = Observable.FromEventPattern<OnIncorrectLoginArgs>(
                e => Client.OnIncorrectLogin += e,
                e => Client.OnIncorrectLogin -= e)
            .Select(x => x.EventArgs);
        
        OnJoinedChannel = Observable.FromEventPattern<OnJoinedChannelArgs>(
                e => Client.OnJoinedChannel += e,
                e => Client.OnJoinedChannel -= e)
            .Select(x => x.EventArgs);
        
        OnLeftChannel = Observable.FromEventPattern<OnLeftChannelArgs>(
                e => Client.OnLeftChannel += e,
                e => Client.OnLeftChannel -= e)
            .Select(x => x.EventArgs);
        
        OnMessageCleared = Observable.FromEventPattern<OnMessageClearedArgs>(
                e => Client.OnMessageCleared += e,
                e => Client.OnMessageCleared -= e)
            .Select(x => x.EventArgs);
        
        OnMessageReceived = Observable.FromEventPattern<OnMessageReceivedArgs>(
                e => Client.OnMessageReceived += e,
                e => Client.OnMessageReceived -= e)
            .Select(x => x.EventArgs);
        
        OnMessageSent = Observable.FromEventPattern<OnMessageSentArgs>(
                e => Client.OnMessageSent += e,
                e => Client.OnMessageSent -= e)
            .Select(x => x.EventArgs);
        
        OnMessageThrottled = Observable.FromEventPattern<OnMessageThrottledEventArgs>(
                e => Client.OnMessageThrottled += e,
                e => Client.OnMessageThrottled -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorJoined = Observable.FromEventPattern<OnModeratorJoinedArgs>(
                e => Client.OnModeratorJoined += e,
                e => Client.OnModeratorJoined -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorLeft = Observable.FromEventPattern<OnModeratorLeftArgs>(
                e => Client.OnModeratorLeft += e,
                e => Client.OnModeratorLeft -= e)
            .Select(x => x.EventArgs);
        
        OnModeratorsReceived = Observable.FromEventPattern<OnModeratorsReceivedArgs>(
                e => Client.OnModeratorsReceived += e,
                e => Client.OnModeratorsReceived -= e)
            .Select(x => x.EventArgs);
        
        OnNewSubscriber = Observable.FromEventPattern<OnNewSubscriberArgs>(
                e => Client.OnNewSubscriber += e,
                e => Client.OnNewSubscriber -= e)
            .Select(x => x.EventArgs);

        OnRaidNotification = Observable.FromEventPattern<OnRaidNotificationArgs>(
                e => Client.OnRaidNotification += e,
                e => Client.OnRaidNotification -= e)
            .Select(x => x.EventArgs);
        
        OnReSubscriber = Observable.FromEventPattern<OnReSubscriberArgs>(
                e => Client.OnReSubscriber += e,
                e => Client.OnReSubscriber -= e)
            .Select(x => x.EventArgs);
        
        OnUserBanned = Observable.FromEventPattern<OnUserBannedArgs>(
                e => Client.OnUserBanned += e,
                e => Client.OnUserBanned -= e)
            .Select(x => x.EventArgs);
        
        OnUserJoined = Observable.FromEventPattern<OnUserJoinedArgs>(
                e => Client.OnUserJoined += e,
                e => Client.OnUserJoined -= e)
            .Select(x => x.EventArgs);        
        
        OnUserLeft = Observable.FromEventPattern<OnUserLeftArgs>(
                e => Client.OnUserLeft += e,
                e => Client.OnUserLeft -= e)
            .Select(x => x.EventArgs);
        
        OnUserTimedout = Observable.FromEventPattern<OnUserTimedoutArgs>(
                e => Client.OnUserTimedout += e,
                e => Client.OnUserTimedout -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperReceived = Observable.FromEventPattern<OnWhisperReceivedArgs>(
                e => Client.OnWhisperReceived += e,
                e => Client.OnWhisperReceived -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperSent = Observable.FromEventPattern<OnWhisperSentArgs>(
                e => Client.OnWhisperSent += e,
                e => Client.OnWhisperSent -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperThrottled = Observable.FromEventPattern<OnWhisperThrottledEventArgs>(
                e => Client.OnWhisperThrottled += e,
                e => Client.OnWhisperThrottled -= e)
            .Select(x => x.EventArgs);
        
        OnBannedEmailAlias = Observable.FromEventPattern<OnBannedEmailAliasArgs>(
                e => Client.OnBannedEmailAlias += e,
                e => Client.OnBannedEmailAlias -= e)
            .Select(x => x.EventArgs);
        
        OnChannelStateChanged = Observable.FromEventPattern<OnChannelStateChangedArgs>(
                e => Client.OnChannelStateChanged += e,
                e => Client.OnChannelStateChanged -= e)
            .Select(x => x.EventArgs);
        
        OnChatColorChanged = Observable.FromEventPattern<OnChatColorChangedArgs>(
                e => Client.OnChatColorChanged += e,
                e => Client.OnChatColorChanged -= e)
            .Select(x => x.EventArgs);
        
        OnChatCommandReceived = Observable.FromEventPattern<OnChatCommandReceivedArgs>(
                e => Client.OnChatCommandReceived += e,
                e => Client.OnChatCommandReceived -= e)
            .Select(x => x.EventArgs);
        
        OnExistingUsersDetected = Observable.FromEventPattern<OnExistingUsersDetectedArgs>(
                e => Client.OnExistingUsersDetected += e,
                e => Client.OnExistingUsersDetected -= e)
            .Select(x => x.EventArgs);
        
        OnRequiresVerifiedEmail = Observable.FromEventPattern<OnRequiresVerifiedEmailArgs>(
                e => Client.OnRequiresVerifiedEmail += e,
                e => Client.OnRequiresVerifiedEmail -= e)
            .Select(x => x.EventArgs);
        
        OnSendReceiveData = Observable.FromEventPattern<OnSendReceiveDataArgs>(
                e => Client.OnSendReceiveData += e,
                e => Client.OnSendReceiveData -= e)
            .Select(x => x.EventArgs);
        
        OnUserStateChanged = Observable.FromEventPattern<OnUserStateChangedArgs>(
                e => Client.OnUserStateChanged += e,
                e => Client.OnUserStateChanged -= e)
            .Select(x => x.EventArgs);
        
        OnWhisperCommandReceived = Observable.FromEventPattern<OnWhisperCommandReceivedArgs>(
                e => Client.OnWhisperCommandReceived += e,
                e => Client.OnWhisperCommandReceived -= e)
            .Select(x => x.EventArgs);
        
        OnRequiresVerifiedPhoneNumber = Observable.FromEventPattern<OnRequiresVerifiedPhoneNumberArgs>(
                e => Client.OnRequiresVerifiedPhoneNumber += e,
                e => Client.OnRequiresVerifiedPhoneNumber -= e)
            .Select(x => x.EventArgs);
        
        OnVIPsReceived = Observable.FromEventPattern<OnVIPsReceivedArgs>(
                e => Client.OnVIPsReceived += e,
                e => Client.OnVIPsReceived -= e)
            .Select(x => x.EventArgs);
    }
}