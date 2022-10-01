using Microsoft.AspNetCore.Mvc;

namespace Strem.Twitter.Models;

public class TwitterOAuthAuthorizePayload
{
    public string? Code { get; set; }
    public string? Error { get; set; }
    [BindProperty(Name = "error_description")]
    public string? ErrorDescription { get; set; }
    public string? State { get; set; }
}