using System.Reactive;
using System.Reactive.Linq;
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

    /// <summary>Occurs when [on chat command received].</summary>
    public IObservable<OnChatCommandReceivedArgs> OnChatCommandReceived { get; private set; }

    /// <summary>Occurs when [on connected].</summary>
    public IObservable<TwitchLib.Client.Events.OnConnectedEventArgs> OnConnected { get; private set; }

    /// <summary>Occurs when [on connection error].</summary>
    public IObservable<OnConnectionErrorArgs> OnConnectionError { get; private set; }
    
    /// <summary>Occurs when [on disconnected].</summary>
    public IObservable<OnDisconnectedArgs> OnDisconnected { get; private set; }

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

    /// <summary>Occurs when [on message received].</summary>
    public IObservable<OnMessageReceivedArgs> OnMessageReceived { get; private set; }

    /// <summary>Occurs when [on message sent].</summary>
    public IObservable<OnMessageSentArgs> OnMessageSent { get; private set; }

    /// <summary>Occurs when [on new subscriber].</summary>
    public IObservable<OnNewSubscriberArgs> OnNewSubscriber { get; private set; }

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

    /// <summary>Occurs when [on message throttled].</summary>
    public IObservable<OnMessageThrottledArgs> OnMessageThrottled { get; private set; }

    /// <summary>Occurs when [on error].</summary>
    public IObservable<OnErrorEventArgs> OnError { get; private set; }

    /// <summary>Occurs when [on reconnected].</summary>
    public IObservable<TwitchLib.Client.Events.OnConnectedEventArgs> OnReconnected { get; private set; }

    /// <summary>
    /// Occurs when [on community subscription announcement received].
    /// </summary>
    public IObservable<OnCommunitySubscriptionArgs> OnCommunitySubscription { get; private set; }

    /// <summary>Occurs when [on message deleted].</summary>
    public IObservable<OnMessageClearedArgs> OnMessageCleared { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
    /// </summary>
    public IObservable<NoticeEventArgs> OnRequiresVerifiedEmail { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified phone number without a verified phone number attached to the account.
    /// </summary>
    public IObservable<NoticeEventArgs> OnRequiresVerifiedPhoneNumber { get; private set; }

    /// <summary>
    /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
    /// </summary>
    public IObservable<NoticeEventArgs> OnBannedEmailAlias { get; private set; }

    public bool IsConnected => Client.IsConnected;
    
    public ObservableTwitchClient(ITwitchClient client)
    {
        Client = client;
        SetupObservables();
    }
    
    public void SetupObservables()
    {
        OnConnected = Observable.FromEvent<AsyncEventHandler<TwitchLib.Client.Events.OnConnectedEventArgs>, TwitchLib.Client.Events.OnConnectedEventArgs>(
            h => async (_, args) => h(args),
            h => Client.OnConnected += h,
            h => Client.OnConnected -= h);
        
        OnDisconnected = Observable.FromEvent<AsyncEventHandler<OnDisconnectedArgs>, OnDisconnectedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnDisconnected += h,
            h => Client.OnDisconnected -= h);
        
        OnError = Observable.FromEvent<AsyncEventHandler<OnErrorEventArgs>, OnErrorEventArgs>(
            h => async (_, args) => h(args),
            h => Client.OnError += h,
            h => Client.OnError -= h);
        
        OnReconnected = Observable.FromEvent<AsyncEventHandler<TwitchLib.Client.Events.OnConnectedEventArgs>, TwitchLib.Client.Events.OnConnectedEventArgs>(
            h => async (_, args) => h(args),
            h => Client.OnReconnected += h,
            h => Client.OnReconnected -= h);
        
        OnChatCleared = Observable.FromEvent<AsyncEventHandler<OnChatClearedArgs>, OnChatClearedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnChatCleared += h,
            h => Client.OnChatCleared -= h);
        
        OnCommunitySubscription = Observable.FromEvent<AsyncEventHandler<OnCommunitySubscriptionArgs>, OnCommunitySubscriptionArgs>(
            h => async (_, args) => h(args),
            h => Client.OnCommunitySubscription += h,
            h => Client.OnCommunitySubscription -= h);
        
        OnConnectionError = Observable.FromEvent<AsyncEventHandler<OnConnectionErrorArgs>, OnConnectionErrorArgs>(
            h => async (_, args) => h(args),
            h => Client.OnConnectionError += h,
            h => Client.OnConnectionError -= h);
        
        OnGiftedSubscription = Observable.FromEvent<AsyncEventHandler<OnGiftedSubscriptionArgs>, OnGiftedSubscriptionArgs>(
            h => async (_, args) => h(args),
            h => Client.OnGiftedSubscription += h,
            h => Client.OnGiftedSubscription -= h);
        
        OnIncorrectLogin = Observable.FromEvent<AsyncEventHandler<OnIncorrectLoginArgs>, OnIncorrectLoginArgs>(
            h => async (_, args) => h(args),
            h => Client.OnIncorrectLogin += h,
            h => Client.OnIncorrectLogin -= h);
        
        OnJoinedChannel = Observable.FromEvent<AsyncEventHandler<OnJoinedChannelArgs>, OnJoinedChannelArgs>(
            h => async (_, args) => h(args),
            h => Client.OnJoinedChannel += h,
            h => Client.OnJoinedChannel -= h);
        
        OnLeftChannel = Observable.FromEvent<AsyncEventHandler<OnLeftChannelArgs>, OnLeftChannelArgs>(
            h => async (_, args) => h(args),
            h => Client.OnLeftChannel += h,
            h => Client.OnLeftChannel -= h);
        
        OnMessageCleared = Observable.FromEvent<AsyncEventHandler<OnMessageClearedArgs>, OnMessageClearedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnMessageCleared += h,
            h => Client.OnMessageCleared -= h);
        
        OnMessageReceived = Observable.FromEvent<AsyncEventHandler<OnMessageReceivedArgs>, OnMessageReceivedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnMessageReceived += h,
            h => Client.OnMessageReceived -= h);
        
        OnMessageSent = Observable.FromEvent<AsyncEventHandler<OnMessageSentArgs>, OnMessageSentArgs>(
            h => async (_, args) => h(args),
            h => Client.OnMessageSent += h,
            h => Client.OnMessageSent -= h);
        
        OnMessageThrottled = Observable.FromEvent<AsyncEventHandler<OnMessageThrottledArgs>, OnMessageThrottledArgs>(
            h => async (_, args) => h(args),
            h => Client.OnMessageThrottled += h,
            h => Client.OnMessageThrottled -= h);
        
        OnMessageThrottled = Observable.FromEvent<AsyncEventHandler<OnMessageThrottledArgs>, OnMessageThrottledArgs>(
            h => async (_, args) => h(args),
            h => Client.OnMessageThrottled += h,
            h => Client.OnMessageThrottled -= h);
        
        OnNewSubscriber = Observable.FromEvent<AsyncEventHandler<OnNewSubscriberArgs>, OnNewSubscriberArgs>(
            h => async (_, args) => h(args),
            h => Client.OnNewSubscriber += h,
            h => Client.OnNewSubscriber -= h);
        
        OnRaidNotification = Observable.FromEvent<AsyncEventHandler<OnRaidNotificationArgs>, OnRaidNotificationArgs>(
            h => async (_, args) => h(args),
            h => Client.OnRaidNotification += h,
            h => Client.OnRaidNotification -= h);
        
        OnReSubscriber = Observable.FromEvent<AsyncEventHandler<OnReSubscriberArgs>, OnReSubscriberArgs>(
            h => async (_, args) => h(args),
            h => Client.OnReSubscriber += h,
            h => Client.OnReSubscriber -= h);
        
        OnUserBanned = Observable.FromEvent<AsyncEventHandler<OnUserBannedArgs>, OnUserBannedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnUserBanned += h,
            h => Client.OnUserBanned -= h);
        
        OnUserJoined = Observable.FromEvent<AsyncEventHandler<OnUserJoinedArgs>, OnUserJoinedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnUserJoined += h,
            h => Client.OnUserJoined -= h);
        
        OnUserLeft = Observable.FromEvent<AsyncEventHandler<OnUserLeftArgs>, OnUserLeftArgs>(
            h => async (_, args) => h(args),
            h => Client.OnUserLeft += h,
            h => Client.OnUserLeft -= h);
        
        OnUserTimedout = Observable.FromEvent<AsyncEventHandler<OnUserTimedoutArgs>, OnUserTimedoutArgs>(
            h => async (_, args) => h(args),
            h => Client.OnUserTimedout += h,
            h => Client.OnUserTimedout -= h);
        
        OnWhisperReceived = Observable.FromEvent<AsyncEventHandler<OnWhisperReceivedArgs>, OnWhisperReceivedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnWhisperReceived += h,
            h => Client.OnWhisperReceived -= h);
        
        OnBannedEmailAlias = Observable.FromEvent<AsyncEventHandler<NoticeEventArgs>, NoticeEventArgs>(
            h => async (_, args) => h(args),
            h => Client.OnBannedEmailAlias += h,
            h => Client.OnBannedEmailAlias -= h);
        
        OnChannelStateChanged = Observable.FromEvent<AsyncEventHandler<OnChannelStateChangedArgs>, OnChannelStateChangedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnChannelStateChanged += h,
            h => Client.OnChannelStateChanged -= h);
        
        OnChatCommandReceived = Observable.FromEvent<AsyncEventHandler<OnChatCommandReceivedArgs>, OnChatCommandReceivedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnChatCommandReceived += h,
            h => Client.OnChatCommandReceived -= h);
        
        OnExistingUsersDetected = Observable.FromEvent<AsyncEventHandler<OnExistingUsersDetectedArgs>, OnExistingUsersDetectedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnExistingUsersDetected += h,
            h => Client.OnExistingUsersDetected -= h);
        
        OnRequiresVerifiedEmail = Observable.FromEvent<AsyncEventHandler<NoticeEventArgs>, NoticeEventArgs>(
            h => async (_, args) => h(args),
            h => Client.OnRequiresVerifiedEmail += h,
            h => Client.OnRequiresVerifiedEmail -= h);
        
        OnRequiresVerifiedPhoneNumber = Observable.FromEvent<AsyncEventHandler<NoticeEventArgs>, NoticeEventArgs>(
            h => async (_, args) => h(args),
            h => Client.OnRequiresVerifiedPhoneNumber += h,
            h => Client.OnRequiresVerifiedPhoneNumber -= h);
        
        OnSendReceiveData = Observable.FromEvent<AsyncEventHandler<OnSendReceiveDataArgs>, OnSendReceiveDataArgs>(
            h => async (_, args) => h(args),
            h => Client.OnSendReceiveData += h,
            h => Client.OnSendReceiveData -= h);
        
        OnUserStateChanged = Observable.FromEvent<AsyncEventHandler<OnUserStateChangedArgs>, OnUserStateChangedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnUserStateChanged += h,
            h => Client.OnUserStateChanged -= h);
        
        OnWhisperCommandReceived = Observable.FromEvent<AsyncEventHandler<OnWhisperCommandReceivedArgs>, OnWhisperCommandReceivedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnWhisperCommandReceived += h,
            h => Client.OnWhisperCommandReceived -= h);
        
        OnWhisperCommandReceived = Observable.FromEvent<AsyncEventHandler<OnWhisperCommandReceivedArgs>, OnWhisperCommandReceivedArgs>(
            h => async (_, args) => h(args),
            h => Client.OnWhisperCommandReceived += h,
            h => Client.OnWhisperCommandReceived -= h);
    }
}