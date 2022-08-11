using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using Strem.Core.State;
using Strem.OBS.Services.Client;
using Strem.OBS.Variables;

namespace Strem.OBS.Extensions;

public static class IObservableOBSClientExtensions
{
    public static async Task<(bool success, string message)> AttemptConnect(this IObservableOBSClient obsClient, string host, string port, string password)
    {
        var websocketAddress = $"ws://{host}:{port}/api/websocket";
        var message = string.Empty;
        var success = false;
        try
        {
            success = await obsClient.Connect(websocketAddress, password);
            if(!success) { message = "Unknown Error Connecting To OBS"; }
        }
        catch (AuthFailureException ex)
        {
            message = $"Authentication Failed: {ex.Message}";
        }
        catch (ErrorResponseException ex)
        {
            message = $"Connect Failed : {ex.Message}";
        }
        catch (Exception ex)
        {
            message = $"Connect Failed : {ex.Message}";
        }

        return (success, message);
    }
    
    public static async Task PopulateActiveSceneTransientData(this IObservableOBSClient client, IAppState appState)
    {
        if(!appState.HasOBSHost()) { return; }
        var scene = await client.GetCurrentScene();
        await UpdateActiveSceneTransientData(appState, scene.Name, scene.Items);
    }

    public static async Task UpdateActiveSceneTransientData(this IAppState appState, string sceneName, IEnumerable<SceneItem> sceneItems)
    {
        appState.TransientVariables.Set(OBSVars.CurrentScene, sceneName);

        var sceneItemNames = sceneItems
            .Select(x => x.SourceName)
            .ToArray();

        if (sceneItemNames.Length == 0) { return; }
        var itemNames = string.Join(",", sceneItemNames);
        appState.TransientVariables.Set(OBSVars.CurrentSceneItems, itemNames);
    }
}