using TwitchLib.Api.Interfaces;

namespace Strem.Twitch.Extensions;

public static class ITwitchAPIExtensions
{
    public static async Task<string?> GetUserIdFromName(this ITwitchAPI twitchApi, string username)
    {
        if (string.IsNullOrEmpty(username)) { return null; }

        var response = await twitchApi.Helix.Users.GetUsersAsync(logins: [username]);
        if (response?.Users == null || response.Users.Length == 0) { return null; }

        return response.Users[0].Id;
    }
}