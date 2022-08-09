using OBSWebsocketDotNet;
using Strem.OBS.Services.Client;

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
}