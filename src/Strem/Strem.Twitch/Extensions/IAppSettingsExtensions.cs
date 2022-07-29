using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Extensions;

public static class IAppSettingsExtensions
{
    public static bool HasTwitchVar(this IAppState state, string name) => state.AppVariables.Has(name, TwitchVariables.TwitchContext);
    public static string GetTwitchVar(this IAppState state, string name) => state.AppVariables.Get(name, TwitchVariables.TwitchContext);
    public static void SetTwitchVar(this IAppState state, string name, string value) => state.AppVariables.Set(name, TwitchVariables.TwitchContext, value);

    public static bool HasTwitchOAuth(this IAppState state) => state.HasTwitchVar(CommonVariables.OAuthToken);
    public static string GetTwitchOAuth(this IAppState state) => state.GetTwitchVar(CommonVariables.OAuthToken);
}