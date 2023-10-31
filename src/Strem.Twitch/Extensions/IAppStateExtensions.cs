using Strem.Core.State;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Extensions;

public static class IAppStateExtensions
{
    public static bool HasTwitchAccessToken(this IAppState state) => state.AppVariables.Has(TwitchVars.OAuthToken);
    public static string GetTwitchAccessToken(this IAppState state) => state.AppVariables.Get(TwitchVars.OAuthToken);
    public static string GetTwitchUsername(this IAppState state) => state.AppVariables.Get(TwitchVars.Username);
    public static string GetTwitchUserId(this IAppState state) => state.AppVariables.Get(TwitchVars.UserId);
    
    public static bool HasTwitchScope(this IAppState state, string scope) => state.AppVariables.Get(TwitchVars.OAuthScopes).Contains(scope, StringComparison.OrdinalIgnoreCase);
    public static string[] GetTwitchScopes(this IAppState state) => state.AppVariables.Get(TwitchVars.OAuthScopes).Split(",");
}