using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Twitch.Events;
using Strem.Twitch.Extensions;
using Strem.Twitch.Models;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Controllers;

[ApiController]
[Route("api/twitch")]
public class TwitchController : Controller
{
    public IAppState AppState { get; }
    
    public TwitchController()
    {
        AppState = this.GetService<IAppState>();
    }

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

        var existingState = AppState.TransientVariables.Get(CommonVariables.OAuthState, TwitchVariables.TwitchContext);
        if (payload.State != existingState)
        {
            var errorEvent = new ErrorEvent("Twitch Client Callback OAuth", $"OAuth state does not match request state");
            this.PublishAsyncEvent(errorEvent);
            return BadRequest($"Twitch couldn't complete OAuth: {errorEvent.Message}");
        }

        AppState.SetTwitchVar(CommonVariables.OAuthToken, payload.AccessToken);
        this.PublishAsyncEvent(new TwitchOAuthSuccessEvent());
        return Ok("Punch It Chewie!");
    }
}