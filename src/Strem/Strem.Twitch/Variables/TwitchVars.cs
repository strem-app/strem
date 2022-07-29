namespace Strem.Twitch.Variables;

public class TwitchVars
{
    // Generic
    public static readonly string TwitchClientId = "yejalwgrfnh5vcup3db5bdxkko2xh1";
    public static readonly string TwitchContext = "twitch";
 
    // OAuth (app)
    public static readonly string TokenExpiry = "token-expiry";
    public static readonly string OAuthScopes = "oauth-scopes";
    
    // User (app)
    public static readonly string Username = "username";
    public static readonly string UserId = "user-id";

    // Channel (transient)
    public static readonly string ChannelTitle = "channel-title";
    public static readonly string ChannelGame = "channel-game";
    public static readonly string StreamViewers = "stream-viewers";
    public static readonly string StreamThumbnailUrl = "stream-thumbnail-url";
}