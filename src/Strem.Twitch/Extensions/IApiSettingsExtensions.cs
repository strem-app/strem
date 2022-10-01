using Strem.Core.State;
using Strem.Core.Variables;
using TwitchLib.Api.Core.Interfaces;

namespace Strem.Twitch.Extensions;

public static class IApiSettingsExtensions
{
    public static void SetCredentials(this IApiSettings settings, IApplicationConfig config, IAppState appState)
    {
        settings.ClientId = config.GetTwitchClientId();
        settings.AccessToken = appState.GetTwitchOAuthToken();
    }
}