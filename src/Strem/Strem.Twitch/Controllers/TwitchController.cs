using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Infrastructure.Extensions;
using Strem.Twitch.Models;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Controllers;

[ApiController]
[Route("api/twitch")]
public class TwitchController : Controller
{
    [HttpGet]
    public IActionResult Index()
    { return Ok("Twitch Works Fine"); }

    [HttpGet]
    [Route("oauth")]
    public IActionResult OAuthTwitchCallback([FromQuery]TwitchOAuthQuerystringPayload payload)
    {
        if (payload != null && !string.IsNullOrEmpty(payload.Error))
        {
            var errorEvent = new ErrorEvent("Twitch OAuth", $"Error Approving OAuth: {payload.Error} | {payload.ErrorDescription}");
            this.PublishAsyncEvent(errorEvent);
            return View("OAuthFailed", errorEvent.Message);
        }
        return View("OAuthClient");
    }
    
    [HttpPost]
    [Route("oauth")]
    public IActionResult OAuthLocalCallback(TwitchOAuthClientPayload payload)
    {
        if (payload == null || string.IsNullOrEmpty(payload.AccessToken))
        {
            var errorEvent = new ErrorEvent("Twitch Client Callback OAuth", $"Error with clientside oauth extraction");
            this.PublishAsyncEvent(errorEvent);
            return BadRequest($"Twitch couldn't complete OAuth: {errorEvent.Message}");
        }

        var existingState = this.GetAppState().TransientVariables.Get(TwitchVariables.OAuthState, TwitchVariables.TwitchContext);
        if (payload.State != existingState)
        {
            var errorEvent = new ErrorEvent("Twitch Client Callback OAuth", $"OAuth state does not match request state");
            this.PublishAsyncEvent(errorEvent);
            return BadRequest($"Twitch couldn't complete OAuth: {errorEvent.Message}");
        }

        WebHostHackExtensions.AppState.AppVariables.Set(TwitchVariables.OAuthToken, TwitchVariables.TwitchContext, payload.AccessToken);
        return Ok("Punch It Chewie!");
    }
}