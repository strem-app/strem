using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Strem.Twitch.Models;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TwitchOAuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string ExpriresIn { get; set; }
    public string[] Scope { get; set; }
    public string State { get; set; }
    public string TokenType { get; set; }
}