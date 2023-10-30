using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Youtube.Models;
using Strem.Youtube.Services.OAuth;
using Strem.Youtube.Variables;

namespace Strem.Youtube.Controllers;

[ApiController]
[Route("api/youtube")]
public class YoutubeController : Controller
{
    public IAppState AppState { get; }
    public IYoutubeOAuthClient OAuthClient { get; }
    
    public YoutubeController()
    {
        AppState = this.GetService<IAppState>();
        OAuthClient = this.GetService<IYoutubeOAuthClient>();
    }

    [HttpGet]
    public IActionResult Index()
    { return Ok("Youtube Works Fine"); }

    [HttpGet]
    [Route("oauth")]
    public async Task<IActionResult> OAuthYoutubeAuthorizeCallback([FromQuery]YoutubeOAuthAuthorizePayload? payload)
    {
        if (!string.IsNullOrEmpty(payload?.Error))
        {
            var errorEvent = new ErrorEvent("Youtube OAuth", $"Error Approving OAuth: {payload.Error} | {payload.ErrorDescription}");
            this.PublishAsyncEvent(errorEvent);
            return View("OAuthFailed", errorEvent.Message);
        }
        
        var existingState = AppState.TransientVariables.Get(CommonVariables.OAuthState, YoutubeVars.Context);
        if (payload?.State != existingState)
        {
            var errorEvent = new ErrorEvent("Youtube Client Callback OAuth", $"OAuth state does not match request state");
            this.PublishAsyncEvent(errorEvent);
            return View("OAuthFailed", errorEvent.Message);
        }

        var wasSuccessful = await OAuthClient.GetToken(payload.Code);
        
        return !wasSuccessful 
            ? View("OAuthFailed", "Unable to retrieve token from Youtube api, please try again later") 
            : View("OAuthSuccess");
    }
}