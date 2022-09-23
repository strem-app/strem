using Strem.Core.Variables;
using Strem.Twitch.Plugin;

namespace Strem.Twitch.Extensions;

public static class IApplicationConfigExtensions
{
    public static string GetTwitchClientId(this IApplicationConfig config) => config[TwitchPluginSettings.TwitchClientIdKey].ToString();
}