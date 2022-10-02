using Strem.Core.Variables;
using Strem.Twitter.Plugin;

namespace Strem.Twitter.Extensions;

public static class IApplicationConfigExtensions
{
    public static string GetTwitterClientId(this IApplicationConfig config) => config[TwitterPluginSettings.TwitterClientIdKey].ToString();
}