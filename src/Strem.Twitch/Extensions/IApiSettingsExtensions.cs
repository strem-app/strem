using Strem.Core.State;
using TwitchLib.Api.Core.Interfaces;

namespace Strem.Twitch.Extensions;

public static class IApiSettingsExtensions
{
    public static void SetCredentials(this IApiSettings settings, IAppState appState)
    {
        settings.AccessToken = appState.GetTwitchOAuthToken();
    }
}