using Strem.Core.Variables;

namespace Strem.Youtube.Variables;

public class YoutubeVars
{
    // Generic
    public static readonly string Context = "youtube";
    
    // OAuth (app)
    public static readonly VariableEntry OAuthToken = new(CommonVariables.OAuthAccessToken, Context);
    public static readonly VariableEntry OAuthRefreshToken = new(CommonVariables.OAuthRefreshToken, Context);
    public static readonly VariableEntry TokenExpiry = new("token-expiry", Context);
    public static readonly VariableEntry OAuthScopes = new("oauth-scopes", Context);
    
    // App
    public static readonly VariableEntry ClientId = new("client-id", Context);
    public static readonly VariableEntry ClientSecret = new("client-secret", Context);
    
    // User (app)
    public static readonly VariableEntry ChannelId = new("channelId", Context);
    public static readonly VariableEntry Username = new("username", Context);
}