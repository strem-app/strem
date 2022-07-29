using System.Text.Json.Serialization;

namespace Strem.Twitch.Models;

public class TwitchOAuthValidationPayload
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }
    public string Login { get; set; }
    public string[] Scopes { get; set; }
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}