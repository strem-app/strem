using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Youtube.Events.OAuth;
using Strem.Youtube.Models;
using Strem.Youtube.Services.OAuth;
using Strem.Youtube.Variables;

namespace Strem.Youtube.Controllers;

[ApiController]
[Route("api/youtube")]
public class YoutubeController : Controller
{
    public IAppState AppState { get; }
    public ILogger<YoutubeController> Logger { get; }
    
    public YoutubeController()
    {
        AppState = this.GetService<IAppState>();
        Logger = this.GetLogger<YoutubeController>();
    }

    [HttpGet]
    public IActionResult Index()
    { return Ok("Youtube Works Fine"); }

    [HttpGet]
    [Route("oauth")]
    public IActionResult OAuthYoutubeCallback([FromQuery]YoutubeOAuthQuerystringPayload payload)
    {
        if (payload != null && !string.IsNullOrEmpty(payload.Error))
        {
            var errorMessage = $"[Youtube OAuth]: Error Approving OAuth: {payload.Error} | {payload.ErrorDescription}";
            Logger.Error(errorMessage);
            return View("OAuthFailed", errorMessage);
        }
        
        Logger.Information("[Youtube OAuth]: Got callback, handling implicit flow");
        return View("OAuthClient");
    }
    
    [HttpPost]
    [Route("oauth")]
    public IActionResult OAuthLocalCallback(YoutubeOAuthClientPayload payload)
    {
        if (payload == null || string.IsNullOrEmpty(payload.AccessToken))
        {
            var errorMessage = "[Youtube Client Callback OAuth]: Error with clientside oauth extraction";
            Logger.Error(errorMessage);
            return BadRequest(errorMessage);
        }

        var existingState = AppState.TransientVariables.Get(CommonVariables.OAuthState, YoutubeVars.Context);
        if (payload.State != existingState)
        {
            var errorMessage = "[Youtube Client Callback OAuth]: OAuth state does not match request state";
            Logger.Error(errorMessage);
            return BadRequest(errorMessage);
        }

        AppState.AppVariables.Set(YoutubeVars.OAuthToken, payload.AccessToken);

        int.TryParse(payload.ExpiresIn, out var expiryTime);
        var actualExpiry = DateTime.Now.AddSeconds(expiryTime);
        AppState.AppVariables.Set(YoutubeVars.TokenExpiry, actualExpiry.ToString("u"));

        var scopes = string.Join(",", payload.Scope);
        AppState.AppVariables.Set(YoutubeVars.OAuthScopes, scopes);
        
        Logger.Information("[Youtube OAuth]: Successfully authenticated");
        this.PublishAsyncEvent(new YoutubeOAuthSuccessEvent());
        return Ok("Punch It Chewie!");
    }
}