using Microsoft.AspNetCore.Mvc;

namespace Strem.Twitch.Models;

public class TwitchOAuthQuerystringPayload
{
    public string? Code { get; set; }
    public string? Error { get; set; }
    [BindProperty(Name = "error_description")]
    public string? ErrorDescription { get; set; }
    public string? State { get; set; }
}