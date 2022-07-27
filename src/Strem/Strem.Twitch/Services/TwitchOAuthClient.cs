using RestSharp;
using Strem.Infrastructure.Services;
using Strem.Infrastructure.Services.Web;

namespace Strem.Twitch.Services;

public class TwitchOAuthClient : ITwitchOAuthClient
{
    public static readonly string ClientId = "yejalwgrfnh5vcup3db5bdxkko2xh1";  
    public static readonly string TwitchApi = "https://id.twitch.tv";  
    public static readonly string OAuthUrl = "oauth2/authorize";
    public static readonly string OAuthCallbackUrl = "http://localhost:56721/api/twitch/oauth";

    public static readonly string[] RequiredScopes = {
        "chat:read", "chat:edit"
    };
    
    public IWebLoader WebLoader { get; }

    public TwitchOAuthClient(IWebLoader webLoader)
    {
        WebLoader = webLoader;
    }

    public void AttemptLogin()
    {
        var scopeQueryData = Uri.EscapeDataString(string.Join(" ", RequiredScopes));
        var queryData = $"client_id={ClientId}&redirect_uri={OAuthCallbackUrl}&response_type=token";
        var completeUrl = $"{TwitchApi}/{OAuthUrl}?{queryData}&scope={scopeQueryData}";
        WebLoader.LoadUrl(completeUrl);
    }
}