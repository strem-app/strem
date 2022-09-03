using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using Strem.Core.Extensions;

namespace Strem.OBS.v4.Services.Client;

public class ObservableOBSClient : OBSWebsocket, IObservableOBSClient
{
    /// <summary>
        /// Exceptions thrown that are not passed up to the caller will be passed through this event.
        /// </summary>
        public IObservable<OBSErrorEventArgs> OnOBSError  { get; private set; }

        /// <summary>
        /// Raised when a request is sent.
        /// </summary>
        public IObservable<RequestData> OnRequestSent  { get; private set; }

        /// <summary>
        /// Raised when any "update-type" event is received.
        /// </summary>
        public IObservable<JObject> OnEventReceived  { get; private set; }

        /// <summary>
        /// Raised when a request's response is received.
        /// </summary>
        public IObservable<JObject> OnResponseReceived  { get; private set; }

        /// <summary>
        /// Triggered when switching to another scene
        /// </summary>
        public IObservable<SceneChangeEventArgs> OnSceneChanged  { get; private set; }

        /// <summary>
        /// Triggered when a scene is created, deleted or renamed
        /// </summary>
        public IObservable<Unit> OnSceneListChanged  { get; private set; }

        /// <summary>
        /// Scene items within a scene have been reordered.
        /// </summary>
        public IObservable<SourceOrderChangedEventArgs> OnSourceOrderChanged  { get; private set; }

        /// <summary>
        /// Triggered when a new item is added to the item list of the specified scene
        /// </summary>
        public IObservable<SceneItemUpdatedEventArgs> OnSceneItemAdded  { get; private set; }

        /// <summary>
        /// Triggered when an item is removed from the item list of the specified scene
        /// </summary>
        public IObservable<SceneItemUpdatedEventArgs> OnSceneItemRemoved  { get; private set; }

        /// <summary>
        /// Triggered when the visibility of a scene item changes
        /// </summary>
        public IObservable<SceneItemVisibilityChangedEventArgs> OnSceneItemVisibilityChanged  { get; private set; }

        /// <summary>
        /// Triggered when the lock status of a scene item changes
        /// </summary>
        public IObservable<SceneItemLockChangedEventArgs> OnSceneItemLockChanged  { get; private set; }

        /// <summary>
        /// Triggered when switching to another scene collection
        /// </summary>
        public IObservable<Unit> OnSceneCollectionChanged  { get; private set; }

        /// <summary>
        /// Triggered when a scene collection is created, deleted or renamed
        /// </summary>
        public IObservable<Unit> OnSceneCollectionListChanged  { get; private set; }

        /// <summary>
        /// Triggered when switching to another transition
        /// </summary>
        public IObservable<TransitionChangeEventArgs> OnTransitionChanged  { get; private set; }

        /// <summary>
        /// Triggered when the current transition duration is changed
        /// </summary>
        public IObservable<TransitionDurationChangeEventArgs> OnTransitionDurationChanged  { get; private set; }

        /// <summary>
        /// Triggered when a transition is created or removed
        /// </summary>
        public IObservable<Unit> OnTransitionListChanged  { get; private set; }

        /// <summary>
        /// Triggered when a transition between two scenes starts. Followed by <see cref="SceneChanged"/>
        /// </summary>
        public IObservable<TransitionBeginEventArgs> OnTransitionBegin  { get; private set; }

        /// <summary>
        /// A transition (other than "cut") has ended. Added in v4.8.0
        /// </summary>
        public IObservable<TransitionEndEventArgs> OnTransitionEnd  { get; private set; }

        /// <summary>
        /// A stinger transition has finished playing its video. Added in v4.8.0
        /// </summary>
        public IObservable<TransitionVideoEndEventArgs> OnTransitionVideoEnd  { get; private set; }

        /// <summary>
        /// Triggered when switching to another profile
        /// </summary>
        public IObservable<Unit> OnProfileChanged  { get; private set; }

        /// <summary>
        /// Triggered when a profile is created, imported, removed or renamed
        /// </summary>
        public IObservable<Unit> OnProfileListChanged  { get; private set; }

        /// <summary>
        /// Triggered when the streaming output state changes
        /// </summary>
        public IObservable<OutputStateChangedEventArgs> OnStreamingStateChanged  { get; private set; }

        /// <summary>
        /// Triggered when the recording output state changes
        /// </summary>
        public IObservable<OutputStateChangedEventArgs> OnRecordingStateChanged  { get; private set; }

        /// <summary>
        /// Triggered when state of the replay buffer changes
        /// </summary>
        public IObservable<OutputStateChangedEventArgs> OnReplayBufferStateChanged  { get; private set; }

        /// <summary>
        /// Triggered every 2 seconds while streaming is active
        /// </summary>
        public IObservable<StreamStatusEventArgs> OnStreamStatus  { get; private set; }

        /// <summary>
        /// Triggered when the preview scene selection changes (Studio Mode only)
        /// </summary>
        public IObservable<SceneChangeEventArgs> OnPreviewSceneChanged  { get; private set; }

        /// <summary>
        /// Triggered when Studio Mode is turned on or off
        /// </summary>
        public IObservable<StudioModeChangeEventArgs> OnStudioModeSwitched  { get; private set; }

        /// <summary>
        /// Triggered when OBS exits
        /// </summary>
        public IObservable<Unit> OnOBSExit  { get; private set; }

        /// <summary>
        /// Triggered when connected successfully to an obs-websocket server
        /// </summary>
        public IObservable<Unit> OnConnected  { get; private set; }

        /// <summary>
        /// Triggered when disconnected from an obs-websocket server
        /// </summary>
        public IObservable<Unit> OnDisconnected  { get; private set; }

        /// <summary>
        /// Emitted every 2 seconds after enabling it by calling SetHeartbeat
        /// </summary>
        public IObservable<HeartBeatEventArgs> OnHeartbeat  { get; private set; }

        /// <summary>
        /// A scene item is deselected
        /// </summary>
        public IObservable<SceneItemSelectionEventArgs> OnSceneItemDeselected  { get; private set; }

        /// <summary>
        /// A scene item is selected
        /// </summary>
        public IObservable<SceneItemSelectionEventArgs> OnSceneItemSelected  { get; private set; }

        /// <summary>
        /// A scene item transform has changed
        /// </summary>
        public IObservable<SceneItemTransformEventArgs> OnSceneItemTransformChanged  { get; private set; }

        /// <summary>
        /// Audio mixer routing changed on a source
        /// </summary>
        public IObservable<SourceAudioMixersChangedEventArgs> OnSourceAudioMixersChanged  { get; private set; }

        /// <summary>
        /// The audio sync offset of a source has changed
        /// </summary>
        public IObservable<SourceAudioSyncOffsetEventArgs> OnSourceAudioSyncOffsetChanged  { get; private set; }

        /// <summary>
        /// A source has been created. A source can be an input, a scene or a transition.
        /// </summary>
        public IObservable<SourceCreatedEventArgs> OnSourceCreated  { get; private set; }

        /// <summary>
        /// A source has been destroyed/removed. A source can be an input, a scene or a transition.
        /// </summary>
        public IObservable<SourceDestroyedEventArgs> OnSourceDestroyed  { get; private set; }

        /// <summary>
        /// A filter was added to a source
        /// </summary>
        public IObservable<SourceFilterAddedEventArgs> OnSourceFilterAdded  { get; private set; }

        /// <summary>
        /// A filter was removed from a source
        /// </summary>
        public IObservable<SourceFilterRemovedEventArgs> OnSourceFilterRemoved  { get; private set; }

        /// <summary>
        /// Filters in a source have been reordered
        /// </summary>
        public IObservable<SourceFiltersReorderedEventArgs> OnSourceFiltersReordered  { get; private set; }

        /// <summary>
        /// Triggered when the visibility of a filter has changed
        /// </summary>
        public IObservable<SourceFilterVisibilityChangedEventArgs> OnSourceFilterVisibilityChanged  { get; private set; }

        /// <summary>
        /// A source has been muted or unmuted
        /// </summary>
        public IObservable<SourceMuteStateChangedEventArgs> OnSourceMuteStateChanged  { get; private set; }

        /// <summary>
        /// A source has been renamed
        /// </summary>
        public IObservable<SourceRenamedEventArgs> OnSourceRenamed  { get; private set; }

        /// <summary>
        /// The volume of a source has changed
        /// </summary>
        public IObservable<SourceVolumeChangedEventArgs> OnSourceVolumeChanged  { get; private set; }

        /// <summary>
        /// An event was received that obs-websocket-dotnet does not have a defined event handler for.
        /// </summary>
        public IObservable<JObject> OnUnhandledEvent  { get; private set; }
        /// <summary>
        /// A custom broadcast message was received
        /// </summary>
        public IObservable<BroadcastCustomMessageReceivedEventArgs> OnBroadcastCustomMessageReceived  { get; private set; }

    public ObservableOBSClient()
    {
        SetupObservables();
    }

    public void SetupObservables()
    {
        OnSceneChanged = Observable.FromEventPattern<SceneChangeEventArgs>(
                x => SceneChanged += x,
                x => SceneChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSceneListChanged = Observable.FromEventPattern(
                x => SceneListChanged += x,
                x => SceneListChanged -= x)
            .ToUnit();
        
        OnOBSError = Observable.FromEventPattern<OBSErrorEventArgs>(
                x => OBSError += x,
                x => OBSError -= x)
            .Select(x => x.EventArgs);
        
        OnRequestSent = Observable.FromEventPattern<RequestData>(
                x => RequestSent += x,
                x => RequestSent -= x)
            .Select(x => x.EventArgs);
        
        OnEventReceived = Observable.FromEventPattern<JObject>(
                x => EventReceived += x,
                x => EventReceived -= x)
            .Select(x => x.EventArgs);
        
        OnResponseReceived = Observable.FromEventPattern<JObject>(
                x => ResponseReceived += x,
                x => ResponseReceived -= x)
            .Select(x => x.EventArgs);
        
        OnSourceOrderChanged = Observable.FromEventPattern<SourceOrderChangedEventArgs>(
                x => SourceOrderChanged += x,
                x => SourceOrderChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSceneItemAdded = Observable.FromEventPattern<SceneItemUpdatedEventArgs>(
                x => SceneItemAdded += x,
                x => SceneItemAdded -= x)
            .Select(x => x.EventArgs);
        
        OnSceneItemRemoved = Observable.FromEventPattern<SceneItemUpdatedEventArgs>(
                x => SceneItemRemoved += x,
                x => SceneItemRemoved -= x)
            .Select(x => x.EventArgs);
        
        OnSceneItemVisibilityChanged = Observable.FromEventPattern<SceneItemVisibilityChangedEventArgs>(
                x => SceneItemVisibilityChanged += x,
                x => SceneItemVisibilityChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSceneItemLockChanged = Observable.FromEventPattern<SceneItemLockChangedEventArgs>(
                x => SceneItemLockChanged += x,
                x => SceneItemLockChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSceneCollectionChanged = Observable.FromEventPattern(
                x => SceneCollectionChanged += x,
                x => SceneCollectionChanged -= x)
            .ToUnit();
        
        OnSceneCollectionListChanged = Observable.FromEventPattern(
                x => SceneCollectionListChanged += x,
                x => SceneCollectionListChanged -= x)
            .ToUnit();
        
        OnTransitionChanged = Observable.FromEventPattern<TransitionChangeEventArgs>(
                x => TransitionChanged += x,
                x => TransitionChanged -= x)
            .Select(x => x.EventArgs);
        
        OnTransitionDurationChanged = Observable.FromEventPattern<TransitionDurationChangeEventArgs>(
                x => TransitionDurationChanged += x,
                x => TransitionDurationChanged -= x)
            .Select(x => x.EventArgs);
        
        OnTransitionListChanged = Observable.FromEventPattern(
                x => TransitionListChanged += x,
                x => TransitionListChanged -= x)
            .ToUnit();
        
        OnTransitionBegin = Observable.FromEventPattern<TransitionBeginEventArgs>(
                x => TransitionBegin += x,
                x => TransitionBegin -= x)
            .Select(x => x.EventArgs);
        
        OnTransitionEnd = Observable.FromEventPattern<TransitionEndEventArgs>(
                x => TransitionEnd += x,
                x => TransitionEnd -= x)
            .Select(x => x.EventArgs);
        
        OnTransitionVideoEnd = Observable.FromEventPattern<TransitionVideoEndEventArgs>(
                x => TransitionVideoEnd += x,
                x => TransitionVideoEnd -= x)
            .Select(x => x.EventArgs);
        
        OnProfileChanged = Observable.FromEventPattern(
                x => ProfileChanged += x,
                x => ProfileChanged -= x)
            .ToUnit();
        
        OnProfileListChanged = Observable.FromEventPattern(
                x => ProfileListChanged += x,
                x => ProfileListChanged -= x)
            .ToUnit();
        
        OnStreamingStateChanged = Observable.FromEventPattern<OutputStateChangedEventArgs>(
                x => StreamingStateChanged += x,
                x => StreamingStateChanged -= x)
            .Select(x => x.EventArgs);
        
        OnRecordingStateChanged = Observable.FromEventPattern<OutputStateChangedEventArgs>(
                x => RecordingStateChanged += x,
                x => RecordingStateChanged -= x)
            .Select(x => x.EventArgs);
        
        OnReplayBufferStateChanged = Observable.FromEventPattern<OutputStateChangedEventArgs>(
                x => ReplayBufferStateChanged += x,
                x => ReplayBufferStateChanged -= x)
            .Select(x => x.EventArgs);
        
        OnStreamStatus = Observable.FromEventPattern<StreamStatusEventArgs>(
                x => StreamStatus += x,
                x => StreamStatus -= x)
            .Select(x => x.EventArgs);
        
        OnPreviewSceneChanged = Observable.FromEventPattern<SceneChangeEventArgs>(
                x => PreviewSceneChanged += x,
                x => PreviewSceneChanged -= x)
            .Select(x => x.EventArgs);
        
        OnStudioModeSwitched = Observable.FromEventPattern<StudioModeChangeEventArgs>(
                x => StudioModeSwitched += x,
                x => StudioModeSwitched -= x)
            .Select(x => x.EventArgs);
        
        OnOBSExit = Observable.FromEventPattern(
                x => OBSExit += x,
                x => OBSExit -= x)
            .ToUnit();
        
        OnConnected = Observable.FromEventPattern(
                x => Connected += x,
                x => Connected -= x)
            .ToUnit();
        
        OnDisconnected = Observable.FromEventPattern(
                x => Disconnected += x,
                x => Disconnected -= x)
            .ToUnit();
        
        OnHeartbeat = Observable.FromEventPattern<HeartBeatEventArgs>(
                x => Heartbeat += x,
                x => Heartbeat -= x)
            .Select(x => x.EventArgs);
        
        OnSceneItemDeselected = Observable.FromEventPattern<SceneItemSelectionEventArgs>(
                x => SceneItemDeselected += x,
                x => SceneItemDeselected -= x)
            .Select(x => x.EventArgs);
        
        OnSceneItemSelected = Observable.FromEventPattern<SceneItemSelectionEventArgs>(
                x => SceneItemSelected += x,
                x => SceneItemSelected -= x)
            .Select(x => x.EventArgs);
        
        OnSceneItemTransformChanged = Observable.FromEventPattern<SceneItemTransformEventArgs>(
                x => SceneItemTransformChanged += x,
                x => SceneItemTransformChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSourceAudioMixersChanged = Observable.FromEventPattern<SourceAudioMixersChangedEventArgs>(
                x => SourceAudioMixersChanged += x,
                x => SourceAudioMixersChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSourceAudioSyncOffsetChanged = Observable.FromEventPattern<SourceAudioSyncOffsetEventArgs>(
                x => SourceAudioSyncOffsetChanged += x,
                x => SourceAudioSyncOffsetChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSourceCreated = Observable.FromEventPattern<SourceCreatedEventArgs>(
                x => SourceCreated += x,
                x => SourceCreated -= x)
            .Select(x => x.EventArgs);
        
        OnSourceDestroyed = Observable.FromEventPattern<SourceDestroyedEventArgs>(
                x => SourceDestroyed += x,
                x => SourceDestroyed -= x)
            .Select(x => x.EventArgs);
        
        OnSourceFilterAdded = Observable.FromEventPattern<SourceFilterAddedEventArgs>(
                x => SourceFilterAdded += x,
                x => SourceFilterAdded -= x)
            .Select(x => x.EventArgs);
        
        OnSourceFilterRemoved = Observable.FromEventPattern<SourceFilterRemovedEventArgs>(
                x => SourceFilterRemoved += x,
                x => SourceFilterRemoved -= x)
            .Select(x => x.EventArgs);
        
        OnSourceFiltersReordered = Observable.FromEventPattern<SourceFiltersReorderedEventArgs>(
                x => SourceFiltersReordered += x,
                x => SourceFiltersReordered -= x)
            .Select(x => x.EventArgs);
        
        OnSourceFilterVisibilityChanged = Observable.FromEventPattern<SourceFilterVisibilityChangedEventArgs>(
                x => SourceFilterVisibilityChanged += x,
                x => SourceFilterVisibilityChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSourceMuteStateChanged = Observable.FromEventPattern<SourceMuteStateChangedEventArgs>(
                x => SourceMuteStateChanged += x,
                x => SourceMuteStateChanged -= x)
            .Select(x => x.EventArgs);
        
        OnSourceRenamed = Observable.FromEventPattern<SourceRenamedEventArgs>(
                x => SourceRenamed += x,
                x => SourceRenamed -= x)
            .Select(x => x.EventArgs);
        
        OnSourceVolumeChanged = Observable.FromEventPattern<SourceVolumeChangedEventArgs>(
                x => SourceVolumeChanged += x,
                x => SourceVolumeChanged -= x)
            .Select(x => x.EventArgs);
        
        OnUnhandledEvent = Observable.FromEventPattern<JObject>(
                x => UnhandledEvent += x,
                x => UnhandledEvent -= x)
            .Select(x => x.EventArgs);
        
        OnBroadcastCustomMessageReceived = Observable.FromEventPattern<BroadcastCustomMessageReceivedEventArgs>(
                x => BroadcastCustomMessageReceived += x,
                x => BroadcastCustomMessageReceived -= x)
            .Select(x => x.EventArgs);
    }
}