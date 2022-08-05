using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Extensions;

public static class IAppSettingsExtensions
{
    public static bool HasTwitchVar(this IAppState state, string name) => state.AppVariables.Has(name, TwitchVars.TwitchContext);
    public static string GetTwitchVar(this IAppState state, string name) => state.AppVariables.Get(name, TwitchVars.TwitchContext);
    public static void SetTwitchVar(this IAppState state, string name, string value) => state.AppVariables.Set(name, TwitchVars.TwitchContext, value);
    public static void DeleteTwitchVar(this IAppState state, string name) => state.AppVariables.Delete(name, TwitchVars.TwitchContext);

    public static bool HasTwitchOAuth(this IAppState state) => state.HasTwitchVar(CommonVariables.OAuthToken);
    public static string GetTwitchOAuthToken(this IAppState state) => state.GetTwitchVar(CommonVariables.OAuthToken);
    public static string GetTwitchUsername(this IAppState state) => state.GetTwitchVar(TwitchVars.Username);
    
    public static bool HasTwitchScope(this IAppState state, string scope) => state.GetTwitchVar(TwitchVars.OAuthScopes).Contains(scope);
    public static string[] GetTwitchScopes(this IAppState state) => state.GetTwitchVar(TwitchVars.OAuthScopes).Split(",");
}