using Strem.Core.State;
using Strem.Youtube.Variables;

namespace Strem.Youtube.Extensions;

public static class IAppStateExtensions
{
    public static bool HasYoutubeOAuth(this IAppState state) => state.AppVariables.Has(YoutubeVars.OAuthToken);
    public static string GetYoutubeOAuthToken(this IAppState state) => state.AppVariables.Get(YoutubeVars.OAuthToken);
    public static string GetYoutubeUsername(this IAppState state) => state.AppVariables.Get(YoutubeVars.Username);
    public static string GetYoutubeUserId(this IAppState state) => state.AppVariables.Get(YoutubeVars.UserId);
    
    public static bool HasYoutubeScope(this IAppState state, string scope) => state.AppVariables.Get(YoutubeVars.OAuthScopes).Contains(scope, StringComparison.OrdinalIgnoreCase);
    public static string[] GetYoutubeScopes(this IAppState state) => state.AppVariables.Get(YoutubeVars.OAuthScopes).Split(",");
}