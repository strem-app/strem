using RestSharp;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Utils;
using Strem.Infrastructure.Services;
using Strem.Infrastructure.Services.Web;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Services;

public class TwitchOAuthClient : ITwitchOAuthClient
{
    public static readonly string ClientId = "yejalwgrfnh5vcup3db5bdxkko2xh1";  
    public static readonly string OAuthCallbackUrl = "http://localhost:56721/api/twitch/oauth";

    public static readonly string TwitchApi = "https://id.twitch.tv/oauth2";  
    public static readonly string AuthorizeEndpoint = "authorize";
    
    public static readonly string[] RequiredScopes = {
        "chat:read", "chat:edit"
    };
    
    public IWebLoader WebLoader { get; }
    public IAppState AppState { get; }
    public IRandomizer Randomizer { get; }

    public TwitchOAuthClient(IWebLoader webLoader, IAppState appState, IRandomizer randomizer)
    {
        WebLoader = webLoader;
        AppState = appState;
        Randomizer = randomizer;
    }

    public void RequestOAuthToken()
    {
        var randomState = Randomizer.RandomString();
        AppState.TransientVariables.Set(TwitchVariables.OAuthState, TwitchVariables.TwitchContext, randomState);
        
        var scopeQueryData = Uri.EscapeDataString(string.Join(" ", RequiredScopes));
        var queryData = $"client_id={ClientId}&redirect_uri={OAuthCallbackUrl}&response_type=token&scope={scopeQueryData}&state={randomState}";
        var completeUrl = $"{TwitchApi}/{AuthorizeEndpoint}?{queryData}";
        WebLoader.LoadUrl(completeUrl);
    }
}