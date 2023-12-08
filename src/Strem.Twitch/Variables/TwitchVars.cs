using Strem.Core.Variables;

namespace Strem.Twitch.Variables;

public class TwitchVars
{
    // Generic
    public static readonly string Context = "twitch";
    
    // OAuth (app)
    public static readonly VariableEntry OAuthToken = new(CommonVariables.OAuthAccessToken, Context);
    public static readonly VariableEntry TokenExpiry = new("token-expiry", Context);
    public static readonly VariableEntry OAuthScopes = new("oauth-scopes", Context);
    
    // User (app)
    public static readonly VariableEntry Username = new("username", Context);
    public static readonly VariableEntry UserId = new("user-id", Context);

    // Channel (transient)
    public static readonly VariableEntry ChannelTitle = new("channel-title", Context);
    public static readonly VariableEntry ChannelGame = new("channel-game", Context);
    public static readonly VariableEntry StreamViewers = new("stream-viewers", Context);
    public static readonly VariableEntry StreamThumbnailUrl = new("stream-thumbnail-url", Context);
}