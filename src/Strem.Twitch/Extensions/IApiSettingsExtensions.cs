using Strem.Core.State;
using Strem.Twitch.Variables;
using TwitchLib.Api.Core.Interfaces;

namespace Strem.Twitch.Extensions;

public static class IApiSettingsExtensions
{
    public static void SetCredentials(this IApiSettings settings, IAppState appState)
    {
        settings.AccessToken = appState.GetTwitchOAuthToken();
        settings.ClientId = TwitchVars.TwitchClientId;
    }
}