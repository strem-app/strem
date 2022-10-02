using Strem.Core.Variables;

namespace Strem.Twitch.Variables;

public class TwitchVars
{
    // Generic
    public static readonly string TwitchContext = "twitch";
    
    // OAuth (app)
    public static readonly VariableEntry OAuthToken = new(CommonVariables.OAuthAccessToken, TwitchContext);
    public static readonly VariableEntry TokenExpiry = new("token-expiry", TwitchContext);
    public static readonly VariableEntry OAuthScopes = new("oauth-scopes", TwitchContext);
    
    // User (app)
    public static readonly VariableEntry Username = new("username", TwitchContext);
    public static readonly VariableEntry UserId = new("user-id", TwitchContext);

    // Channel (transient)
    public static readonly VariableEntry ChannelTitle = new("channel-title", TwitchContext);
    public static readonly VariableEntry ChannelGame = new("channel-game", TwitchContext);
    public static readonly VariableEntry StreamViewers = new("stream-viewers", TwitchContext);
    public static readonly VariableEntry StreamThumbnailUrl = new("stream-thumbnail-url", TwitchContext);
}