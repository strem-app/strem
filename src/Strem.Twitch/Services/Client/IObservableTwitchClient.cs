using System.Reactive;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Events;

namespace Strem.Twitch.Services.Client;

public interface IObservableTwitchClient
{
    ITwitchClient Client { get; }

    /// <summary>Occurs when [on channel state changed].</summary>
    IObservable<OnChannelStateChangedArgs> OnChannelStateChanged { get; }

    /// <summary>Occurs when [on chat cleared].</summary>
    IObservable<OnChatClearedArgs> OnChatCleared { get; }

    /// <summary>Occurs when [on chat color changed].</summary>
    IObservable<OnChatColorChangedArgs> OnChatColorChanged { get; }

    /// <summary>Occurs when [on chat command received].</summary>
    IObservable<OnChatCommandReceivedArgs> OnChatCommandReceived { get; }

    /// <summary>Occurs when [on connected].</summary>
    IObservable<OnConnectedArgs> OnConnected { get; }

    /// <summary>Occurs when [on connection error].</summary>
    IObservable<OnConnectionErrorArgs> OnConnectionError { get; }

    /// <summary>Occurs when [on disconnected].</summary>
    IObservable<OnDisconnectedEventArgs> OnDisconnected { get; }

    /// <summary>Occurs when [on existing users detected].</summary>
    IObservable<OnExistingUsersDetectedArgs> OnExistingUsersDetected { get; }

    /// <summary>Occurs when [on gifted subscription].</summary>
    IObservable<OnGiftedSubscriptionArgs> OnGiftedSubscription { get; }

    /// <summary>Occurs when [on host left].</summary>
    IObservable<Unit> OnHostLeft { get; }

    /// <summary>Occurs when [on incorrect login].</summary>
    IObservable<OnIncorrectLoginArgs> OnIncorrectLogin { get; }

    /// <summary>Occurs when [on joined channel].</summary>
    IObservable<OnJoinedChannelArgs> OnJoinedChannel { get; }

    /// <summary>Occurs when [on left channel].</summary>
    IObservable<OnLeftChannelArgs> OnLeftChannel { get; }

    /// <summary>Occurs when [on log].</summary>
    IObservable<OnLogArgs> OnLog { get; }

    /// <summary>Occurs when [on message received].</summary>
    IObservable<OnMessageReceivedArgs> OnMessageReceived { get; }

    /// <summary>Occurs when [on message sent].</summary>
    IObservable<OnMessageSentArgs> OnMessageSent { get; }

    /// <summary>Occurs when [on moderator joined].</summary>
    IObservable<OnModeratorJoinedArgs> OnModeratorJoined { get; }

    /// <summary>Occurs when [on moderator left].</summary>
    IObservable<OnModeratorLeftArgs> OnModeratorLeft { get; }

    /// <summary>Occurs when [on moderators received].</summary>
    IObservable<OnModeratorsReceivedArgs> OnModeratorsReceived { get; }

    /// <summary>Occurs when [on new subscriber].</summary>
    IObservable<OnNewSubscriberArgs> OnNewSubscriber { get; }

    /// <summary>Occurs when [on now hosting].</summary>
    IObservable<OnNowHostingArgs> OnNowHosting { get; }

    /// <summary>Occurs when [on raid notification].</summary>
    IObservable<OnRaidNotificationArgs> OnRaidNotification { get; }

    /// <summary>Occurs when [on re subscriber].</summary>
    IObservable<OnReSubscriberArgs> OnReSubscriber { get; }

    /// <summary>Occurs when [on send receive data].</summary>
    IObservable<OnSendReceiveDataArgs> OnSendReceiveData { get; }

    /// <summary>Occurs when [on user banned].</summary>
    IObservable<OnUserBannedArgs> OnUserBanned { get; }

    /// <summary>Occurs when [on user joined].</summary>
    IObservable<OnUserJoinedArgs> OnUserJoined { get; }

    /// <summary>Occurs when [on user left].</summary>
    IObservable<OnUserLeftArgs> OnUserLeft { get; }

    /// <summary>Occurs when [on user state changed].</summary>
    IObservable<OnUserStateChangedArgs> OnUserStateChanged { get; }

    /// <summary>Occurs when [on user timedout].</summary>
    IObservable<OnUserTimedoutArgs> OnUserTimedout { get; }

    /// <summary>Occurs when [on whisper command received].</summary>
    IObservable<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived { get; }

    /// <summary>Occurs when [on whisper received].</summary>
    IObservable<OnWhisperReceivedArgs> OnWhisperReceived { get; }

    /// <summary>Occurs when [on whisper sent].</summary>
    IObservable<OnWhisperSentArgs> OnWhisperSent { get; }

    /// <summary>Occurs when [on message throttled].</summary>
    IObservable<OnMessageThrottledEventArgs> OnMessageThrottled { get; }

    /// <summary>Occurs when [on whisper throttled].</summary>
    IObservable<OnWhisperThrottledEventArgs> OnWhisperThrottled { get; }

    /// <summary>Occurs when [on error].</summary>
    IObservable<OnErrorEventArgs> OnError { get; }

    /// <summary>Occurs when [on reconnected].</summary>
    IObservable<OnReconnectedEventArgs> OnReconnected { get; }

    /// <summary>Occurs when [on vip received].</summary>
    IObservable<OnVIPsReceivedArgs> OnVIPsReceived { get; }

    /// <summary>
    /// Occurs when [on community subscription announcement received].
    /// </summary>
    IObservable<OnCommunitySubscriptionArgs> OnCommunitySubscription { get; }

    /// <summary>Occurs when [on message deleted].</summary>
    IObservable<OnMessageClearedArgs> OnMessageCleared { get; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified email without a verified email attached to the account.
    /// </summary>
    IObservable<OnRequiresVerifiedEmailArgs> OnRequiresVerifiedEmail { get; }

    /// <summary>
    /// Occurs when chatting in a channel that requires a verified phone number without a verified phone number attached to the account.
    /// </summary>
    IObservable<OnRequiresVerifiedPhoneNumberArgs> OnRequiresVerifiedPhoneNumber { get; }

    /// <summary>
    /// Occurs when chatting in a channel that the user is banned in bcs of an already banned alias with the same Email
    /// </summary>
    IObservable<OnBannedEmailAliasArgs> OnBannedEmailAlias { get; }
}