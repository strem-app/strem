using Microsoft.AspNetCore.Mvc;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Twitch.Events.OAuth;
using Strem.Twitch.Models;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Controllers;

[ApiController]
[Route("api/twitch")]
public class TwitchController : Controller
{
    public IAppState AppState { get; }
    public ILogger<TwitchController> Logger { get; }
    
    public TwitchController()
    {
        AppState = this.GetService<IAppState>();
        Logger = this.GetLogger<TwitchController>();
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
            var errorMessage = $"[Twitch OAuth]: Error Approving OAuth: {payload.Error} | {payload.ErrorDescription}";
            Logger.Error(errorMessage);
            return View("OAuthFailed", errorMessage);
        }
        
        Logger.Information("[Twitch OAuth]: Got callback, handling implicit flow");
        return View("OAuthClient");
    }
    
    [HttpPost]
    [Route("oauth")]
    public IActionResult OAuthLocalCallback(TwitchOAuthClientPayload payload)
    {
        if (payload == null || string.IsNullOrEmpty(payload.AccessToken))
        {
            var errorMessage = $"[Twitch Client Callback OAuth]: Error with clientside oauth extraction";
            Logger.Error(errorMessage);
            return BadRequest(errorMessage);
        }

        var existingState = AppState.TransientVariables.Get(CommonVariables.OAuthState, TwitchVars.Context);
        if (payload.State != existingState)
        {
            var errorMessage = $"[Twitch Client Callback OAuth]: OAuth state does not match request state";
            Logger.Error(errorMessage);
            return BadRequest(errorMessage);
        }

        AppState.AppVariables.Set(TwitchVars.OAuthToken, payload.AccessToken);
        Logger.Information("[Twitch OAuth]: Successfully authenticated");
        this.PublishAsyncEvent(new TwitchOAuthSuccessEvent());
        return Ok("Punch It Chewie!");
    }
}