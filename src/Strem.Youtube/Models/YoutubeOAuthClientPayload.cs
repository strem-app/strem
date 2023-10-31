using System.Text.Json.Serialization;

namespace Strem.Youtube.Models;

public class YoutubeOAuthClientPayload
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; set; }
    public string Scope { get; set; }
    public string State { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
}