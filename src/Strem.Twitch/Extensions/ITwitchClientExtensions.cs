using Strem.Core.State;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace Strem.Twitch.Extensions;

public static class ITwitchClientExtensions
{
    public static async Task<(bool success, string message)> ConnectOrRefresh(this ITwitchClient client, IAppState appState)
    {
        if (!appState.HasTwitchAccessToken())
        { return (false, "No access token available, cannot initialize chat"); }
        
        if(client.IsConnected)
        { await client.DisconnectAsync(); }
        
        var username = appState.GetTwitchUsername();
        var accessToken = appState.GetTwitchAccessToken();
        var credentials = new ConnectionCredentials(username, accessToken);
        client.Initialize(credentials, username);

        var hasConnected = await client.ConnectAsync();
        if (hasConnected)
        { return (true, string.Empty); }

        return (false, "Couldnt connect to twitch chat for unknown reason");
    }

    public static bool HasJoinedChannel(this ITwitchClient client, string channel)
    { return client.JoinedChannels.Any(x => x.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase)); }

    public static async Task TemporarilyJoinChannelFor(this ITwitchClient client, string channel, Action action)
    {
        await client.JoinChannelAsync(channel);
        action();
        await client.LeaveChannelAsync(channel);
    }
}