using Strem.Core.Variables;
using Strem.Youtube.Plugin;

namespace Strem.Youtube.Extensions;

public static class IApplicationConfigExtensions
{
    public static string GetYoutubeClientId(this IApplicationConfig config) => config[YoutubePluginSettings.YoutubeClientIdKey].ToString();
}