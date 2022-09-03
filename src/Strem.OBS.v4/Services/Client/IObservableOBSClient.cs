using System.Reactive;
using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;

namespace Strem.OBS.v4.Services.Client;

public interface IObservableOBSClient
{
    /// <summary>
    /// Get basic OBS video information
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<OBSVideoInfo> GetVideoInfo(CancellationToken cancellationToken = default);

    /// <summary>
    /// List existing outputs
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<OBSOutputInfo[]> ListOutputs(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outputName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<OBSOutputInfo> GetOutput(string outputName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outputName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StartOutput(string outputName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outputName"></param>
    /// <param name="force"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StopOutput(string outputName, bool force, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outputName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StopOutput(string outputName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request a screenshot from the specified source. An <paramref name="embedPictureFormat"/> or <paramref name="saveToFilePath"/> must be specified.
    /// Clients can specify width and height parameters to receive scaled pictures. Aspect ratio is preserved if only one of these two parameters is specified.
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="embedPictureFormat">Format of the Data URI encoded picture. Can be "png", "jpg", "jpeg" or "bmp" (or any other value supported by Qt's Image module)</param>
    /// <param name="saveToFilePath">Full file path (file extension included) where the captured image is to be saved. Can be in a format different from pictureFormat. Can be a relative path.</param>
    /// <param name="width">Screenshot width. Defaults to the source's base width.</param>
    /// <param name="height">Screenshot height. Defaults to the source's base height.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<SourceScreenshotResponse> TakeSourceScreenshot(string sourceName, string? embedPictureFormat,
        string? saveToFilePath = null, int width = -1, int height = -1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request a screenshot from the specified source. An <paramref name="embedPictureFormat"/> or <paramref name="saveToFilePath"/> must be specified.
    /// Clients can specify width and height parameters to receive scaled pictures. Aspect ratio is preserved if only one of these two parameters is specified.
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="embedPictureFormat"></param>
    /// <param name="saveToFilePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<SourceScreenshotResponse> TakeSourceScreenshot(string sourceName, string? embedPictureFormat, string? saveToFilePath = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request a screenshot from the specified source embedded in the response.
    /// Clients can specify width and height parameters to receive scaled pictures. Aspect ratio is preserved if only one of these two parameters is specified.
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="embedPictureFormat">Format of the Data URI encoded picture. Can be "png", "jpg", "jpeg" or "bmp" (or any other value supported by Qt's Image module)</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<SourceScreenshotResponse> TakeSourceScreenshot(string sourceName, string embedPictureFormat, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current scene info along with its items
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>An <see cref="OBSScene"/> object describing the current scene</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<OBSScene> GetCurrentScene(CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the current scene to the specified one
    /// </summary>
    /// <param name="sceneName">The desired scene name</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException">Thrown if the requested scene does not exist.</exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetCurrentScene(string sceneName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the filename formatting string
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Current filename formatting string</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<string?> GetFilenameFormatting(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get OBS stats (almost the same info as provided in OBS' stats window)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<OBSStats> GetStats(CancellationToken cancellationToken = default);

    /// <summary>
    /// List every available scene
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="List{OBSScene}" /> of <see cref="OBSScene"/> objects describing each scene</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<OBSScene>> ListScenes(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a list of scenes in the currently active profile
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<GetSceneListInfo> GetSceneList(CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the order of scene items in the requested scene
    /// </summary>
    /// <param name="sceneName">Name of the scene to reorder (defaults to current)</param>
    /// <param name="sceneItems">List of items to reorder, only ID or Name required</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ReorderSceneItems(List<SceneItemStub> sceneItems, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the specified scene's transition override info
    /// </summary>
    /// <param name="sceneName">Name of the scene to return the override info</param>
    /// <param name="cancellationToken"></param>
    /// <returns>TransitionOverrideInfo</returns>
    Task<TransitionOverrideInfo> GetSceneTransitionOverride(string sceneName, CancellationToken cancellationToken);

    /// <summary>
    /// Set specific transition override for a scene
    /// </summary>
    /// <param name="sceneName">Name of the scene to set the transition override</param>
    /// <param name="transitionName">Name of the transition to use</param>
    /// <param name="transitionDuration">Duration in milliseconds of the transition if transition is not fixed. Defaults to the current duration specified in the UI if there is no current override and this value is not given</param>
    /// <param name="cancellationToken"></param>
    Task SetSceneTransitionOverride(string sceneName, string transitionName, int transitionDuration = -1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set specific transition override for a scene with the existing duration.
    /// </summary>
    /// <param name="sceneName">Name of the scene to set the transition override</param>
    /// <param name="transitionName">Name of the transition to use</param>
    /// <param name="cancellationToken"></param>
    Task SetSceneTransitionOverride(string sceneName, string transitionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove any transition override from a specific scene
    /// </summary>
    /// <param name="sceneName">Name of the scene to remove the transition override</param>
    /// <param name="cancellationToken"></param>
    Task RemoveSceneTransitionOverride(string sceneName, CancellationToken cancellationToken = default);

    /// <summary>
    /// List all sources available in the running OBS instance
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<SourceInfo>> GetSourcesList(CancellationToken cancellationToken = default);

    /// <summary>
    /// List all sources available in the running OBS instance
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<SourceType>> GetSourceTypesList(CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the visibility of the specified scene item
    /// </summary>
    /// <param name="itemName">Scene item which visiblity will be changed</param>
    /// <param name="visible">Desired visiblity</param>
    /// <param name="sceneName">Scene name of the specified item</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSourceRender(string itemName, bool visible, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the scene specific properties of the specified source item. Coordinates are relative to the item's parent (the scene or group it belongs to).
    /// </summary>
    /// <param name="itemName">The name of the source</param>
    /// <param name="sceneName">The name of the scene that the source item belongs to. Defaults to the current scene.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<SceneItemProperties> GetSceneItemProperties(string itemName, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the scene specific properties of the specified source item. Coordinates are relative to the item's parent (the scene or group it belongs to).
    /// Response is a JObject
    /// </summary>
    /// <param name="itemName">The name of the source</param>
    /// <param name="sceneName">The name of the scene that the source item belongs to. Defaults to the current scene.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<JObject> GetSceneItemPropertiesJson(string itemName, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current properties of a Text GDI Plus source.
    /// </summary>
    /// <param name="sourceName">The name of the source</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<TextGDIPlusProperties> GetTextGDIPlusProperties(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the current properties of a Text GDI Plus source.
    /// </summary>
    /// <param name="properties">properties for the source</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetTextGDIPlusProperties(TextGDIPlusProperties properties, CancellationToken cancellationToken = default);

    /// <summary>
    /// Move a filter in the chain (relative positioning)
    /// </summary>
    /// <param name="sourceName">Scene Name</param>
    /// <param name="filterName">Filter Name</param>
    /// <param name="movement">Direction to move</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task MoveSourceFilter(string sourceName, string filterName, FilterMovementType movement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Move a filter in the chain (absolute index positioning)
    /// </summary>
    /// <param name="sourceName">Scene Name</param>
    /// <param name="filterName">Filter Name</param>
    /// <param name="newIndex">Desired position of the filter in the chain</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ReorderSourceFilter(string sourceName, string filterName, int newIndex, CancellationToken cancellationToken = default);

    /// <summary>
    /// Apply settings to a source filter
    /// </summary>
    /// <param name="sourceName">Source with filter</param>
    /// <param name="filterName">Filter name</param>
    /// <param name="filterSettings">Filter settings</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSourceFilterSettings(string sourceName, string filterName, JObject filterSettings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modify the Source Filter's visibility
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="filterName">Source filter name</param>
    /// <param name="filterEnabled">New filter state</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSourceFilterVisibility(string sourceName, string filterName, bool filterEnabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return a list of all filters on a source
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<FilterSettings[]> GetSourceFilters(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return a list of all filters on a source
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="filterName">Filter name</param>
    /// <param name="cancellationToken"></param>
    Task<FilterSettings> GetSourceFilterInfo(string sourceName, string filterName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the filter from a source
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="filterName"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> RemoveFilterFromSource(string sourceName, string filterName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a filter to a source
    /// </summary>
    /// <param name="sourceName">Name of the source for the filter</param>
    /// <param name="filterName">Name of the filter</param>
    /// <param name="filterType">Type of filter</param>
    /// <param name="filterSettings">Filter settings object</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task AddFilterToSource(string sourceName, string filterName, string filterType, JObject filterSettings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Start/Stop the streaming output
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ToggleStreaming(CancellationToken cancellationToken = default);

    /// <summary>
    /// Start/Stop the recording output
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ToggleRecording(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current status of the streaming and recording outputs
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>An <see cref="OutputStatus"/> object describing the current outputs states</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<OutputStatus> GetStreamingStatus(CancellationToken cancellationToken = default);

    /// <summary>
    /// List all transitions
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="List{T}"/> of all transition names</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<string>> ListTransitions(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current transition name and duration
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>An <see cref="TransitionSettings"/> object with the current transition name and duration</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<TransitionSettings> GetCurrentTransition(CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the current transition to the specified one
    /// </summary>
    /// <param name="transitionName">Desired transition name</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetCurrentTransition(string transitionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the transition's duration
    /// </summary>
    /// <param name="duration">Desired transition duration (in milliseconds)</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetTransitionDuration(int duration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the volume of the specified source
    /// </summary>
    /// <param name="sourceName">Name of the source which volume will be changed</param>
    /// <param name="volume">Desired volume. Must be between `0.0` and `1.0` for amplitude/mul (useDecibel is false), and under 0.0 for dB (useDecibel is true). Note: OBS will interpret dB values under -100.0 as Inf.</param>
    /// <param name="useDecibel">Interperet `volume` data as decibels instead of amplitude/mul.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetVolume(string sourceName, float volume, bool useDecibel = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the volume of the specified source
    /// </summary>
    /// <param name="sourceName">Name of the source which volume will be changed</param>
    /// <param name="volume">Desired volume. Must be between `0.0` and `1.0` for amplitude/mul</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetVolume(string sourceName, float volume, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the volume of the specified source
    /// Volume is between `0.0` and `1.0` if using amplitude/mul (useDecibel is false), under `0.0` if using dB (useDecibel is true).
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="useDecibel">Output volume in decibels of attenuation instead of amplitude/mul.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An <see cref="VolumeInfo"/>Object containing the volume and mute state of the specified source.</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<VolumeInfo> GetVolume(string sourceName, bool useDecibel = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the volume of the specified source
    /// Volume is between `0.0` and `1.0` using amplitude/mul.
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An <see cref="VolumeInfo"/>Object containing the volume and mute state of the specified source.</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<VolumeInfo> GetVolume(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the mute state of the specified source
    /// </summary>
    /// <param name="sourceName">Name of the source which mute state will be changed</param>
    /// <param name="mute">Desired mute state</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetMute(string sourceName, bool mute, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggle the mute state of the specified source
    /// </summary>
    /// <param name="sourceName">Name of the source which mute state will be toggled</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ToggleMute(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the position of the specified scene item
    /// </summary>
    /// <param name="itemName">Name of the scene item which position will be changed</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="cancellationToken"></param>
    /// <param name="sceneName">(optional) name of the scene the item belongs to</param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemPosition(string itemName, float x, float y, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the position of the specified scene item in the current scene.
    /// </summary>
    /// <param name="itemName">Name of the scene item which position will be changed</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemPosition(string itemName, float x, float y, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the scale and rotation of the specified scene item
    /// </summary>
    /// <param name="itemName">Name of the scene item which transform will be changed</param>
    /// <param name="rotation">Rotation in Degrees</param>
    /// <param name="xScale">Horizontal scale factor</param>
    /// <param name="yScale">Vertical scale factor</param>
    /// <param name="sceneName">(optional) name of the scene the item belongs to</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemTransform(string itemName, float rotation = 0, float xScale = 1, float yScale = 1, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the scale and rotation of the specified scene item in the current scene.
    /// </summary>
    /// <param name="itemName">Name of the scene item which transform will be changed</param>
    /// <param name="rotation">Rotation in Degrees</param>
    /// <param name="xScale">Horizontal scale factor</param>
    /// <param name="yScale">Vertical scale factor</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemTransform(string itemName, float rotation = 0, float xScale = 1, float yScale = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the scene specific properties of a source. Unspecified properties will remain unchanged. Coordinates are relative to the item's parent (the scene or group it belongs to).
    /// </summary>
    /// <param name="props">Object containing changes</param>
    /// <param name="sceneName">Option scene name</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemProperties(SceneItemProperties props, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the scene specific properties of a source in the current scene.
    /// Unspecified properties will remain unchanged. Coordinates are relative to the item's parent (the scene or group it belongs to).
    /// </summary>
    /// <param name="props">Object containing changes</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemProperties(SceneItemProperties props, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="sceneName"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemProperties(JObject obj, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemProperties(JObject obj, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the current scene collection to the specified one
    /// </summary>
    /// <param name="scName">Desired scene collection name</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetCurrentSceneCollection(string scName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the name of the current scene collection
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Name of the current scene collection</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<string> GetCurrentSceneCollection(CancellationToken cancellationToken = default);

    /// <summary>
    /// List all scene collections
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="List{T}"/> of the names of all scene collections</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<string>> ListSceneCollections(CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the current profile to the specified one
    /// </summary>
    /// <param name="profileName">Name of the desired profile</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetCurrentProfile(string profileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the name of the current profile
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Name of the current profile</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<string> GetCurrentProfile(CancellationToken cancellationToken = default);

    /// <summary>
    /// List all profiles
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="List{T}"/> of the names of all profiles</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<List<string>> ListProfiles(CancellationToken cancellationToken = default);

    /// <summary>
    /// Start streaming. Will trigger an error if streaming is already active
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StartStreaming(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop streaming. Will trigger an error if streaming is not active.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StopStreaming(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggle Streaming
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StartStopStreaming(CancellationToken cancellationToken = default);

    /// <summary>
    /// Start recording. Will trigger an error if recording is already active.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StartRecording(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop recording. Will trigger an error if recording is not active.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StopRecording(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggle recording
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StartStopRecording(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pause the current recording. Returns an error if recording is not active or already paused.
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task PauseRecording(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resume/unpause the current recording (if paused). Returns an error if recording is not active or not paused.
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task ResumeRecording(CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the current recording folder
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="recFolder">Recording folder path</param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetRecordingFolder(string recFolder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the path of the current recording folder
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Current recording folder path</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<string> GetRecordingFolder(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get duration of the currently selected transition (if supported)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Current transition duration (in milliseconds)</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<int> GetTransitionDuration(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get duration of the currently selected transition (if supported)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Current transition duration (in milliseconds)</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<GetTransitionListInfo> GetTransitionList(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the position of the current transition. Value will be between 0.0 and 1.0.
    /// Note: Returns 1.0 when not active.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<double> GetTransitionPosition(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get status of Studio Mode
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Studio Mode status (on/off)</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> StudioModeEnabled(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disable Studio Mode
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task DisableStudioMode(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enable Studio Mode
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task EnableStudioMode(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if Studio Mode is enabled, false otherwise.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> GetStudioModeStatus(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enable/disable Studio Mode
    /// </summary>
    /// <param name="enable">Desired Studio Mode status</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetStudioMode(bool enable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggle Studio Mode status (on to off or off to on)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ToggleStudioMode(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the currently selected preview scene. Triggers an error
    /// if Studio Mode is disabled
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Preview scene object</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<OBSScene> GetPreviewScene(CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the currently active preview scene to the one specified.
    /// Triggers an error if Studio Mode is disabled
    /// </summary>
    /// <param name="previewScene">Preview scene name</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetPreviewScene(string previewScene, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the currently active preview scene to the one specified.
    /// Triggers an error if Studio Mode is disabled.
    /// </summary>
    /// <param name="previewScene">Preview scene object</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetPreviewScene(OBSScene previewScene, CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggers a Studio Mode transition (preview scene to program)
    /// </summary>
    /// <param name="transitionDuration">(optional) Transition duration</param>
    /// <param name="transitionName">(optional) Name of transition to use</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task TransitionToProgram(int transitionDuration = -1, string? transitionName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get if the specified source is muted
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Source mute status (on/off)</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> GetMute(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggle the Replay Buffer on/off
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ToggleReplayBuffer(CancellationToken cancellationToken = default);

    /// <summary>
    /// Start recording into the Replay Buffer. Triggers an error
    /// if the Replay Buffer is already active, or if the "Save Replay Buffer"
    /// hotkey is not set in OBS' settings
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StartReplayBuffer(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop recording into the Replay Buffer. Triggers an error if the
    /// Replay Buffer is not active.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StopReplayBuffer(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggle replay buffer
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task StartStopReplayBuffer(CancellationToken cancellationToken = default);

    /// <summary>
    /// Save and flush the contents of the Replay Buffer to disk. Basically
    /// the same as triggering the "Save Replay Buffer" hotkey in OBS.
    /// Triggers an error if Replay Buffer is not active.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SaveReplayBuffer(CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the audio sync offset of the specified source
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="syncOffset">Audio offset (in nanoseconds) for the specified source</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSyncOffset(string sourceName, int syncOffset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the audio sync offset of the specified source
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Audio offset (in nanoseconds) of the specified source</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<int> GetSyncOffset(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a scene item
    /// </summary>
    /// <param name="sceneItem">Scene item, requires name or id of item</param>
    /// /// <param name="sceneName">Scene name to delete item from (optional)</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task DeleteSceneItem(SceneItemStub sceneItem, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a scene item from the current scene.
    /// </summary>
    /// <param name="sceneItem">Scene item, requires name or id of item</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task DeleteSceneItem(SceneItemStub sceneItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a scene item
    /// </summary>
    /// <param name="sceneItemId">Scene item id</param>
    /// <param name="sceneName">Scene name to delete item from (optional)</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task DeleteSceneItem(int sceneItemId, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a scene item from the current scene.
    /// </summary>
    /// <param name="sceneItemId">Scene item id</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task DeleteSceneItem(int sceneItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the relative crop coordinates of the specified source item
    /// </summary>
    /// <param name="sceneItemName">Name of the scene item</param>
    /// <param name="cropInfo">Crop coordinates</param>
    /// <param name="sceneName">(optional) parent scene name of the specified source</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemCrop(string sceneItemName, SceneItemCropInfo cropInfo, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the relative crop coordinates of the specified source item
    /// </summary>
    /// <param name="sceneItemName">Name of the scene item</param>
    /// <param name="cropInfo">Crop coordinates</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemCrop(string sceneItemName, SceneItemCropInfo cropInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the relative crop coordinates of the specified source item
    /// </summary>
    /// <param name="sceneItem">Scene item object</param>
    /// <param name="cropInfo">Crop coordinates</param>
    /// <param name="scene">Parent scene of scene item</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSceneItemCrop(SceneItem sceneItem, SceneItemCropInfo cropInfo, OBSScene scene, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset a scene item
    /// </summary>
    /// <param name="itemName">Name of the source item</param>
    /// <param name="sceneName">Name of the scene the source belongs to. Defaults to the current scene.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ResetSceneItem(string itemName, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset a scene item in the current scene.
    /// </summary>
    /// <param name="itemName">Name of the source item</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task ResetSceneItem(string itemName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send the provided text as embedded CEA-608 caption data. As of OBS Studio 23.1, captions are not yet available on Linux.
    /// </summary>
    /// <param name="text">Captions text</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SendCaptions(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the filename formatting string
    /// </summary>
    /// <param name="filenameFormatting">Filename formatting string to set</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetFilenameFormatting(string filenameFormatting, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the relative crop coordinates of the specified source item
    /// </summary>
    /// <param name="fromSceneName">Source of the scene item</param>
    /// <param name="toSceneName">Destination for the scene item</param>
    /// <param name="sceneItem">Scene item, requires name or id</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task DuplicateSceneItem(string fromSceneName, string toSceneName, SceneItem sceneItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the relative crop coordinates of the specified source item
    /// </summary>
    /// <param name="fromSceneName">Source of the scene item</param>
    /// <param name="toSceneName">Destination for the scene item</param>
    /// <param name="sceneItemID">Scene item id to duplicate</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task DuplicateSceneItem(string fromSceneName, string toSceneName, int sceneItemID, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get names of configured special sources (like Desktop Audio
    /// and Mic sources)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<Dictionary<string, string>> GetSpecialSources(CancellationToken cancellationToken = default);

    /// <summary>
    /// Set current streaming settings
    /// </summary>
    /// <param name="service">Service settings</param>
    /// <param name="save">Save to disk</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetStreamingSettings(StreamingService service, bool save, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get current streaming settings
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<StreamingService> GetStreamSettings(CancellationToken cancellationToken = default);

    /// <summary>
    /// Save current Streaming settings to disk
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SaveStreamSettings(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get settings of the specified BrowserSource
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="sceneName">Optional name of a scene where the specified source can be found</param>
    /// <param name="cancellationToken"></param>
    /// <returns>BrowserSource properties</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<BrowserSourceProperties> GetBrowserSourceProperties(string sourceName, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get settings of the specified BrowserSource in the current scene.
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>BrowserSource properties</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<BrowserSourceProperties> GetBrowserSourceProperties(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set settings of the specified BrowserSource
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="props">BrowserSource properties</param>
    /// <param name="sceneName">Optional name of a scene where the specified source can be found</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetBrowserSourceProperties(string sourceName, BrowserSourceProperties props, string? sceneName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set settings of the specified BrowserSource in the current scene.
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="props">BrowserSource properties</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetBrowserSourceProperties(string sourceName, BrowserSourceProperties props, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enable/disable the heartbeat event
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="cancellationToken"></param>
    Task SetHeartbeat(bool enable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the settings from a source item
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="sourceType">Type of the specified source. Useful for type-checking to avoid settings a set of settings incompatible with the actual source's type.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>settings</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<SourceSettings> GetSourceSettings(string sourceName, string? sourceType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the settings from a source item in the current scene.
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>settings</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<SourceSettings> GetSourceSettings(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set settings of the specified source.
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="settings">Settings for the source</param>
    /// <param name="sourceType">Type of the specified source. Useful for type-checking to avoid settings a set of settings incompatible with the actual source's type.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSourceSettings(string sourceName, JObject settings, string? sourceType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set settings of the specified source.
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="settings">Settings for the source</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetSourceSettings(string sourceName, JObject settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets settings for a media source
    /// </summary>
    /// <param name="sourceName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<MediaSourceSettings> GetMediaSourceSettings(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets settings of a media source
    /// </summary>
    /// <param name="sourceSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task SetMediaSourceSettings(MediaSourceSettings sourceSettings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Open a projector window or create a projector on a monitor. Requires OBS v24.0.4 or newer.
    /// </summary>
    /// <param name="projectorType">Type of projector: "Preview" (default), "Source", "Scene", "StudioProgram", or "Multiview" (case insensitive)</param>
    /// <param name="monitor">Monitor to open the projector on. If -1 or omitted, opens a window</param>
    /// <param name="geometry">Size and position of the projector window (only if monitor is -1). Encoded in Base64 using Qt's geometry encoding. Corresponds to OBS's saved projectors</param>
    /// <param name="name">Name of the source or scene to be displayed (ignored for other projector types)</param>
    /// <param name="cancellationToken"></param>
    Task OpenProjector(string projectorType = "preview", int monitor = -1, string? geometry = null,
        string? name = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renames a source.
    /// Note: If the new name already exists as a source, obs-websocket will return an error.
    /// </summary>
    /// <param name="currentName">Current source name</param>
    /// <param name="newName">New source name</param>
    /// <param name="cancellationToken"></param>
    Task SetSourceName(string currentName, string newName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the audio monitoring type of the specified source.
    /// Valid return values: none, monitorOnly, monitorAndOutput
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The monitor type in use</returns>
    Task<string> GetAudioMonitorType(string sourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the audio monitoring type of the specified source
    /// </summary>
    /// <param name="sourceName">Source name</param>
    /// <param name="monitorType">The monitor type to use. Options: none, monitorOnly, monitorAndOutput</param>
    /// <param name="cancellationToken"></param>
    Task SetAudioMonitorType(string sourceName, string monitorType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcast custom message to all connected WebSocket clients
    /// </summary>
    /// <param name="realm">Identifier to be choosen by the client</param>
    /// <param name="data">User-defined data</param>
    /// <param name="cancellationToken"></param>
    Task BroadcastCustomMessage(string realm, JObject data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Current connection state
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// URL used to connect to the server.
    /// </summary>
    string? ConnectionUrl { get; }

    /// <summary>
    /// Exceptions thrown that are not passed up to the caller will be passed through this event.
    /// </summary>
    IObservable<OBSErrorEventArgs> OnOBSError { get; }

    /// <summary>
    /// Raised when a request is sent.
    /// </summary>
    IObservable<RequestData> OnRequestSent { get; }

    /// <summary>
    /// Raised when any "update-type" event is received.
    /// </summary>
    IObservable<JObject> OnEventReceived { get; }

    /// <summary>
    /// Raised when a request's response is received.
    /// </summary>
    IObservable<JObject> OnResponseReceived { get; }

    /// <summary>
    /// Triggered when switching to another scene
    /// </summary>
    IObservable<SceneChangeEventArgs> OnSceneChanged { get; }

    /// <summary>
    /// Triggered when a scene is created, deleted or renamed
    /// </summary>
    IObservable<Unit> OnSceneListChanged { get; }

    /// <summary>
    /// Scene items within a scene have been reordered.
    /// </summary>
    IObservable<SourceOrderChangedEventArgs> OnSourceOrderChanged { get; }

    /// <summary>
    /// Triggered when a new item is added to the item list of the specified scene
    /// </summary>
    IObservable<SceneItemUpdatedEventArgs> OnSceneItemAdded { get; }

    /// <summary>
    /// Triggered when an item is removed from the item list of the specified scene
    /// </summary>
    IObservable<SceneItemUpdatedEventArgs> OnSceneItemRemoved { get; }

    /// <summary>
    /// Triggered when the visibility of a scene item changes
    /// </summary>
    IObservable<SceneItemVisibilityChangedEventArgs> OnSceneItemVisibilityChanged { get; }

    /// <summary>
    /// Triggered when the lock status of a scene item changes
    /// </summary>
    IObservable<SceneItemLockChangedEventArgs> OnSceneItemLockChanged { get; }

    /// <summary>
    /// Triggered when switching to another scene collection
    /// </summary>
    IObservable<Unit> OnSceneCollectionChanged { get; }

    /// <summary>
    /// Triggered when a scene collection is created, deleted or renamed
    /// </summary>
    IObservable<Unit> OnSceneCollectionListChanged { get; }

    /// <summary>
    /// Triggered when switching to another transition
    /// </summary>
    IObservable<TransitionChangeEventArgs> OnTransitionChanged { get; }

    /// <summary>
    /// Triggered when the current transition duration is changed
    /// </summary>
    IObservable<TransitionDurationChangeEventArgs> OnTransitionDurationChanged { get; }

    /// <summary>
    /// Triggered when a transition is created or removed
    /// </summary>
    IObservable<Unit> OnTransitionListChanged { get; }

    /// <summary>
    /// Triggered when a transition between two scenes starts. Followed by <see cref="SceneChanged"/>
    /// </summary>
    IObservable<TransitionBeginEventArgs> OnTransitionBegin { get; }

    /// <summary>
    /// A transition (other than "cut") has ended. Added in v4.8.0
    /// </summary>
    IObservable<TransitionEndEventArgs> OnTransitionEnd { get; }

    /// <summary>
    /// A stinger transition has finished playing its video. Added in v4.8.0
    /// </summary>
    IObservable<TransitionVideoEndEventArgs> OnTransitionVideoEnd { get; }

    /// <summary>
    /// Triggered when switching to another profile
    /// </summary>
    IObservable<Unit> OnProfileChanged { get; }

    /// <summary>
    /// Triggered when a profile is created, imported, removed or renamed
    /// </summary>
    IObservable<Unit> OnProfileListChanged { get; }

    /// <summary>
    /// Triggered when the streaming output state changes
    /// </summary>
    IObservable<OutputStateChangedEventArgs> OnStreamingStateChanged { get; }

    /// <summary>
    /// Triggered when the recording output state changes
    /// </summary>
    IObservable<OutputStateChangedEventArgs> OnRecordingStateChanged { get; }

    /// <summary>
    /// Triggered when state of the replay buffer changes
    /// </summary>
    IObservable<OutputStateChangedEventArgs> OnReplayBufferStateChanged { get; }

    /// <summary>
    /// Triggered every 2 seconds while streaming is active
    /// </summary>
    IObservable<StreamStatusEventArgs> OnStreamStatus { get; }

    /// <summary>
    /// Triggered when the preview scene selection changes (Studio Mode only)
    /// </summary>
    IObservable<SceneChangeEventArgs> OnPreviewSceneChanged { get; }

    /// <summary>
    /// Triggered when Studio Mode is turned on or off
    /// </summary>
    IObservable<StudioModeChangeEventArgs> OnStudioModeSwitched { get; }

    /// <summary>
    /// Triggered when OBS exits
    /// </summary>
    IObservable<Unit> OnOBSExit { get; }

    /// <summary>
    /// Triggered when connected successfully to an obs-websocket server
    /// </summary>
    IObservable<Unit> OnConnected { get; }

    /// <summary>
    /// Triggered when disconnected from an obs-websocket server
    /// </summary>
    IObservable<Unit> OnDisconnected { get; }

    /// <summary>
    /// Emitted every 2 seconds after enabling it by calling SetHeartbeat
    /// </summary>
    IObservable<HeartBeatEventArgs> OnHeartbeat { get; }

    /// <summary>
    /// A scene item is deselected
    /// </summary>
    IObservable<SceneItemSelectionEventArgs> OnSceneItemDeselected { get; }

    /// <summary>
    /// A scene item is selected
    /// </summary>
    IObservable<SceneItemSelectionEventArgs> OnSceneItemSelected { get; }

    /// <summary>
    /// A scene item transform has changed
    /// </summary>
    IObservable<SceneItemTransformEventArgs> OnSceneItemTransformChanged { get; }

    /// <summary>
    /// Audio mixer routing changed on a source
    /// </summary>
    IObservable<SourceAudioMixersChangedEventArgs> OnSourceAudioMixersChanged { get; }

    /// <summary>
    /// The audio sync offset of a source has changed
    /// </summary>
    IObservable<SourceAudioSyncOffsetEventArgs> OnSourceAudioSyncOffsetChanged { get; }

    /// <summary>
    /// A source has been created. A source can be an input, a scene or a transition.
    /// </summary>
    IObservable<SourceCreatedEventArgs> OnSourceCreated { get; }

    /// <summary>
    /// A source has been destroyed/removed. A source can be an input, a scene or a transition.
    /// </summary>
    IObservable<SourceDestroyedEventArgs> OnSourceDestroyed { get; }

    /// <summary>
    /// A filter was added to a source
    /// </summary>
    IObservable<SourceFilterAddedEventArgs> OnSourceFilterAdded { get; }

    /// <summary>
    /// A filter was removed from a source
    /// </summary>
    IObservable<SourceFilterRemovedEventArgs> OnSourceFilterRemoved { get; }

    /// <summary>
    /// Filters in a source have been reordered
    /// </summary>
    IObservable<SourceFiltersReorderedEventArgs> OnSourceFiltersReordered { get; }

    /// <summary>
    /// Triggered when the visibility of a filter has changed
    /// </summary>
    IObservable<SourceFilterVisibilityChangedEventArgs> OnSourceFilterVisibilityChanged { get; }

    /// <summary>
    /// A source has been muted or unmuted
    /// </summary>
    IObservable<SourceMuteStateChangedEventArgs> OnSourceMuteStateChanged { get; }

    /// <summary>
    /// A source has been renamed
    /// </summary>
    IObservable<SourceRenamedEventArgs> OnSourceRenamed { get; }

    /// <summary>
    /// The volume of a source has changed
    /// </summary>
    IObservable<SourceVolumeChangedEventArgs> OnSourceVolumeChanged { get; }

    /// <summary>
    /// An event was received that obs-websocket-dotnet does not have a defined event handler for.
    /// </summary>
    IObservable<JObject> OnUnhandledEvent { get; }

    /// <summary>
    /// A custom broadcast message was received
    /// </summary>
    IObservable<BroadcastCustomMessageReceivedEventArgs> OnBroadcastCustomMessageReceived { get; }

    /// <summary>
    /// Connect and authenticate (if needed) with the specified password
    /// </summary>
    /// <param name="password">Server password</param>
    Task<bool> Connect(string? password = null);

    /// <summary>
    /// Connect and authenticate (if needed) with the specified password
    /// </summary>
    /// <param name="password">Server password</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="AuthFailureException"></exception>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="SocketErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> Connect(string? password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Connect to the given URL and authenticate (if needed) with the specified password
    /// </summary>
    /// <param name="url"></param>
    /// <param name="password">Server password</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="AuthFailureException"></exception>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="SocketErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> Connect(string url, string? password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnect this instance from the server
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Sends a message to the websocket API with the specified request type.
    /// </summary>
    /// <param name="requestType">obs-websocket request type, must be one specified in the protocol specification</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The server's JSON response as a JObject</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<JObject> SendRequest(string requestType, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message to the websocket API with the specified request type and optional parameters
    /// </summary>
    /// <param name="requestType">obs-websocket request type, must be one specified in the protocol specification</param>
    /// <param name="additionalFields">additional JSON fields if required by the request type</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The server's JSON response as a JObject</returns>
    /// <exception cref="ErrorResponseException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<JObject> SendRequest(string requestType, JObject? additionalFields, CancellationToken cancellationToken);

    /// <summary>
    /// Requests version info regarding obs-websocket, the API and OBS Studio
    /// </summary>
    /// <returns>Version info in an <see cref="OBSVersion"/> object</returns>
    Task<OBSVersion> GetVersion(CancellationToken cancellationToken = default);

    /// <summary>
    /// Request authentication data. You don't have to call this manually.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Authentication data in an <see cref="OBSAuthInfo"/> object</returns>
    /// <exception cref="ErrorResponseException"></exception>
    Task<OBSAuthInfo> GetAuthInfo(CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates to the Websocket server using the challenge and salt given in the passed <see cref="OBSAuthInfo"/> object
    /// </summary>
    /// <param name="password">User password</param>
    /// <param name="authInfo">Authentication data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true if authentication succeeds</returns>
    /// <exception cref="AuthFailureException">Thrown if authentication fails.</exception>
    Task<bool> Authenticate(string password, OBSAuthInfo authInfo, CancellationToken cancellationToken = default);
}