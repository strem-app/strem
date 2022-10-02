using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Twitter.Models;
using Strem.Twitter.Services.OAuth;
using Strem.Twitter.Variables;

namespace Strem.Twitter.Controllers;

[ApiController]
[Route("api/twitter")]
public class TwitterController : Controller
{
    public IAppState AppState { get; }
    public ITwitterOAuthClient OAuthClient { get; }
    
    public TwitterController()
    {
        AppState = this.GetService<IAppState>();
        OAuthClient = this.GetService<ITwitterOAuthClient>();
    }

    [HttpGet]
    public IActionResult Index()
    { return Ok("Twitter Works Fine"); }

    [HttpGet]
    [Route("oauth")]
    public async Task<IActionResult> OAuthTwitterAuthorizeCallback([FromQuery]TwitterOAuthAuthorizePayload? payload)
    {
        if (!string.IsNullOrEmpty(payload?.Error))
        {
            var errorEvent = new ErrorEvent("Twitter OAuth", $"Error Approving OAuth: {payload.Error} | {payload.ErrorDescription}");
            this.PublishAsyncEvent(errorEvent);
            return View("OAuthFailed", errorEvent.Message);
        }
        
        var existingState = AppState.TransientVariables.Get(CommonVariables.OAuthState, TwitterVars.Context);
        if (payload?.State != existingState)
        {
            var errorEvent = new ErrorEvent("Twitter Client Callback OAuth", $"OAuth state does not match request state");
            this.PublishAsyncEvent(errorEvent);
            return View("OAuthFailed", errorEvent.Message);
        }

        var wasSuccessful = await OAuthClient.GetToken(payload.Code);
        
        return !wasSuccessful 
            ? View("OAuthFailed", "Unable to retrieve token from twitter api, please try again later") 
            : View("OAuthSuccess");
    }
}