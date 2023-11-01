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
    public IYoutubeOAuthClient OAuthClient { get; }

    public YoutubeController()
    {
        AppState = this.GetService<IAppState>();
        OAuthClient = this.GetService<IYoutubeOAuthClient>();
        Logger = this.GetLogger<YoutubeController>();
    }

    [HttpGet]
    public IActionResult Index()
    { return Ok("Youtube OAuth Works Fine"); }

    [HttpGet]
    [Route("oauth")]
    public async Task<IActionResult> OAuthYoutubeAuthorizeCallback([FromQuery]YoutubeOAuthAuthorizePayload? payload)
    {
        if (!string.IsNullOrEmpty(payload?.Error))
        {
            var errorMessage = $"[Youtube OAuth]: Error Approving OAuth: {payload.Error} | {payload.ErrorDescription}";
            Logger.Error(errorMessage);
            return View("OAuthFailed", errorMessage);
        }
        
        var existingState = AppState.TransientVariables.Get(CommonVariables.OAuthState, YoutubeVars.Context);
        if (payload?.State != existingState)
        {
            var errorMessage = $"[Youtube OAuth]: Client callback issue, OAuth state does not match request state:";
            Logger.Error(errorMessage);
            return View("OAuthFailed", errorMessage);
        }

        Logger.Information("[Youtube OAuth]: Got callback, handling code exchange");
        var wasSuccessful = await OAuthClient.GetToken(payload.Code);

        var logText = wasSuccessful ? "succeeded" : "failed";
        Logger.Information($"[Youtube OAuth]: Code exchange {logText}");
        return !wasSuccessful 
            ? View("OAuthFailed", "Unable to retrieve token from Youtube api, please try again later") 
            : View("OAuthSuccess");
    }
}