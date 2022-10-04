namespace Strem.Core.Variables;

public class CommonVariables
{
    public static readonly string VariableNamingPattern = @"\w\s\.,\-_\:\|";
    
    public static readonly string OAuthState = "oauth-state";
    public static readonly string OAuthChallengeCode = "oauth-challenge-code";
    public static readonly string OAuthAccessToken = "oauth-token";
    public static readonly string OAuthRefreshToken = "oauth-refresh-token";

    public static VariableEntry StremVersion = new("version", "strem");
}