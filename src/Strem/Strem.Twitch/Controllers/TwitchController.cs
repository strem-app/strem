using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events;
using Strem.Infrastructure.Extensions;
using Strem.Twitch.Events;
using Strem.Twitch.Models;
using ILogger = Serilog.ILogger;

namespace Strem.Twitch.Controllers;

[ApiController]
[Route("api/twitch")]
public class TwitchController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    { return Ok("Twitch Works Fine"); }

    [HttpPost]
    [Route("oauth")]
    public IActionResult OAuthCallback([FromQuery]TwitchOAuthQueryData queryData, [FromBody] TwitchOAuthResponse twitchOAuthResponse)
    {
        if (!string.IsNullOrEmpty(queryData.Error))
        {
            var errorEvent = new ErrorEvent("Twitch OAuth",
                $"Error approving OAuth: {queryData.Error} | {queryData.ErrorDescription}");
            this.PublishAsyncEvent(errorEvent);
            return Problem($"Twitch couldn't complete OAuth: {queryData.Error} - {queryData.ErrorDescription}");
        }

        var authedEvent = new TwitchOAuthedEvent(twitchOAuthResponse);
        this.PublishAsyncEvent(authedEvent);
        return Ok("OAuth completed, you can close this window now!");
    }
}