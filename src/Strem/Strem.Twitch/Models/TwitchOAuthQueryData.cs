using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Strem.Twitch.Models;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TwitchOAuthQueryData
{
    public string Code { get; set; }
    public string Error { get; set; }
    public string ErrorDescription { get; set; }
    public string State { get; set; }
}