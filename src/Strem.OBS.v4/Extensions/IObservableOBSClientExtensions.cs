using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using Persistity.Encryption;
using Persistity.Extensions;
using Strem.Core.State;
using Strem.OBS.v4.Services.Client;
using Strem.OBS.v4.Variables;

namespace Strem.OBS.v4.Extensions;

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
            message = !success 
                ? "Unknown Error Connecting To OBS" 
                : $"Connected to OBS server @ {host}:{port}";
        }
        catch (AuthFailureException ex)
        { message = $"Authentication Failed: {ex.Message}"; }
        catch (ErrorResponseException ex)
        { message = $"Connect Failed : {ex.Message}"; }
        catch (Exception ex)
        { message = $"Connect Failed : {ex.Message}"; }

        return (success, message);
    }
    
    public static async Task<(bool success, string message)> AttemptConnect(this IObservableOBSClient client, IAppState appState, IEncryptor encryptor)
    {
        var host = appState.AppVariables.Get(OBSVars.Host);
        var port = appState.AppVariables.Get(OBSVars.Port);
        var password = string.Empty;

        if (appState.AppVariables.Has(OBSVars.Password))
        {
            var encryptedPassword = appState.AppVariables.Get(OBSVars.Password);
            password = encryptor.Decrypt(encryptedPassword);
        }

        return await client.AttemptConnect(host, port, password);
    }
    
    public static async Task PopulateSourceListData(this IObservableOBSClient client, IAppState appState)
    {
        if(!client.IsConnected) { return; }
        var sources = await client.GetSourcesList();
        await UpdateTransientData(appState, sources);
    }

    public static async Task UpdateTransientData(this IAppState appState, IEnumerable<SourceInfo> sourceInfo)
    {
        var sceneItemNames = sourceInfo
            .Select(x => x.Name)
            .Distinct()
            .OrderBy(x => x);
        
        var itemNames = string.Join(",", sceneItemNames);
        appState.TransientVariables.Set(OBSVars.SourceItems, itemNames);
    }
}