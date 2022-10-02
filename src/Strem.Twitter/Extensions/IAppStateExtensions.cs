using Strem.Core.State;
using Strem.Twitter.Variables;

namespace Strem.Twitter.Extensions;

public static class IAppStateExtensions
{
    public static bool HasTwitterOAuth(this IAppState state) => state.AppVariables.Has(TwitterVars.OAuthToken);
    public static string GetTwitterOAuthToken(this IAppState state) => state.AppVariables.Get(TwitterVars.OAuthToken);
    public static string GetTwitterUsername(this IAppState state) => state.AppVariables.Get(TwitterVars.Username);
    
    public static bool HasTwitterScope(this IAppState state, string scope) => state.AppVariables.Get(TwitterVars.OAuthScopes).Contains(scope, StringComparison.OrdinalIgnoreCase);
    public static bool HasTwitterScopes(this IAppState state, params string[] scopes) => scopes.All(x => state.AppVariables.Get(TwitterVars.OAuthScopes).Contains(x, StringComparison.OrdinalIgnoreCase));
    public static string[] GetTwitterScopes(this IAppState state) => state.AppVariables.Get(TwitterVars.OAuthScopes).Split(",");
}