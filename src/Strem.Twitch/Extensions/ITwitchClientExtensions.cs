using Strem.Core.State;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Extensions;

public static class ITwitchClientExtensions
{
    public static (bool success, string message) ConnectOrRefresh(this ITwitchClient client, IAppState appState)
    {
        if (!appState.HasTwitchOAuth())
        { return (false, "No access token available, cannot initialize chat"); }
        
        if(client.IsConnected)
        { client.Disconnect(); }
        
        var username = appState.GetTwitchUsername();
        var accessToken = appState.GetTwitchOAuthToken();
        var credentials = new ConnectionCredentials(username, accessToken);
        client.Initialize(credentials, username);

        if (client.Connect())
        { return (true, string.Empty); }
        else
        { return (false, "Couldnt connect to twitch chat for unknown reason");}

    }
}