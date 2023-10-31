using Newtonsoft.Json;

namespace Strem.Youtube.Models;

public class YoutubeOAuthValidationPayload
{
    [JsonProperty("client_id")]
    public string ClientId { get; set; }
    public string Login { get; set; }
    public string[] Scopes { get; set; }
    [JsonProperty("user_id")]
    public string UserId { get; set; }
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}