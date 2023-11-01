using Strem.Core.State;
using Strem.Youtube.Variables;

namespace Strem.Youtube.Extensions;

public static class IAppStateExtensions
{
    public static bool HasYoutubeAccessToken(this IAppState state) => state.AppVariables.Has(YoutubeVars.OAuthToken);
    public static string GetYoutubeAccessToken(this IAppState state) => state.AppVariables.Get(YoutubeVars.OAuthToken);
    
    public static string GetYoutubeUsername(this IAppState state) => state.AppVariables.Get(YoutubeVars.Username);
    public static string GetYoutubeChannelId(this IAppState state) => state.AppVariables.Get(YoutubeVars.ChannelId);
    
    public static bool HasYoutubeScope(this IAppState state, string scope) => state.AppVariables.Get(YoutubeVars.OAuthScopes).Contains(scope, StringComparison.OrdinalIgnoreCase);
    public static string[] GetYoutubeScopes(this IAppState state) => state.AppVariables.Get(YoutubeVars.OAuthScopes).Split(",");
    
    
}