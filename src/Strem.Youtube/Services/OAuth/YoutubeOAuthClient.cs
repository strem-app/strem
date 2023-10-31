using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Services.Browsers.Web;
using Strem.Core.Services.Utils;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Api;
using Strem.Youtube.Events.OAuth;
using Strem.Youtube.Extensions;
using Strem.Youtube.Models;
using Strem.Youtube.Types;
using Strem.Youtube.Variables;

namespace Strem.Youtube.Services.OAuth;

public class YoutubeOAuthClient : IYoutubeOAuthClient
{
    public static readonly string OAuthCallbackUrl = $"http://localhost:{InternalWebHostConfiguration.ApiHostPort}/api/youtube/oauth";

    public static readonly string ApiUrl = "https://accounts.google.com/o/oauth2";
    public static readonly string AuthorizeEndpoint = "v2/auth";
    public static readonly string ValidateEndpoint = "validate";
    public static readonly string RevokeEndpoint = "revoke";
    
    public IWebBrowser WebBrowser { get; }
    public IAppState AppState { get; }
    public IApplicationConfig AppConfig { get; }
    public IEventBus EventBus { get; }
    public IRandomizer Randomizer { get; }
    public ILogger<YoutubeOAuthClient> Logger { get; }

    public YoutubeOAuthClient(IWebBrowser webBrowser, IAppState appState, IRandomizer randomizer, IEventBus eventBus, ILogger<YoutubeOAuthClient> logger, IApplicationConfig appConfig)
    {
        WebBrowser = webBrowser;
        AppState = appState;
        Randomizer = randomizer;
        EventBus = eventBus;
        Logger = logger;
        AppConfig = appConfig;
    }

    public void StartAuthorisationProcess(string[] requiredScopes)
    {
        Logger.Information("Starting Youtube Implicit OAuth Process");

        var randomState = Randomizer.RandomString();
        AppState.TransientVariables.Set(CommonVariables.OAuthState, YoutubeVars.Context, randomState);
        
        var completeScopes = requiredScopes.ToList();
        if(!completeScopes.Contains(Scopes.Profile))
        { completeScopes.Add(Scopes.Profile); }
        
        var scopeQueryData = Uri.EscapeDataString(string.Join(" ", completeScopes));
        var queryData = $"client_id={AppConfig.GetYoutubeClientId()}&redirect_uri={OAuthCallbackUrl}&response_type=token&scope={scopeQueryData}&state={randomState}";
        var completeUrl = $"{ApiUrl}/{AuthorizeEndpoint}?{queryData}";
        WebBrowser.LoadUrl(completeUrl);
    }

    public string AttemptGetAccessToken()
    {
        if (AppState.HasYoutubeAccessToken())
        { return AppState.GetYoutubeAccessToken(); }
        
        Logger.Error("Cannot find OAuth Token In Vars for request to Youtube OAuth API");
        return null;
    }

    public void UpdateTokenState(YoutubeOAuthValidationPayload payload)
    {
        var actualExpiry = DateTime.Now.AddSeconds(payload.ExpiresIn);
        AppState.AppVariables.Set(YoutubeVars.TokenExpiry, actualExpiry.ToString("u"));

        var scopes = string.Join(",", payload.Scopes);
        AppState.AppVariables.Set(YoutubeVars.OAuthScopes, scopes);
    }

    public void ClearTokenState()
    {
        AppState.AppVariables.Delete(YoutubeVars.Username);
        AppState.AppVariables.Delete(YoutubeVars.UserId);
        AppState.AppVariables.Delete(YoutubeVars.TokenExpiry);
        AppState.AppVariables.Delete(YoutubeVars.OAuthScopes);
        AppState.AppVariables.Delete(YoutubeVars.OAuthToken);
    }
    
    public async Task<bool> ValidateToken()
    {
        Logger.Information("Validating Youtube Token");

        var accessToken = AttemptGetAccessToken();
        if (accessToken == null) { return false; }

        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(ValidateEndpoint, Method.Get);
        restRequest.AddHeader("Authorization", $"OAuth {accessToken}");

        var response = await restClient.ExecuteAsync(restRequest);
        
        if (!response.IsSuccessful)
        {
            Logger.Error($"Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        var payload = JsonConvert.DeserializeObject<YoutubeOAuthValidationPayload>(response.Content);
        UpdateTokenState(payload);
        return true;
    }
    
    public async Task<bool> RevokeToken()
    {
        Logger.Information("Revoking Youtube Token");

        var accessToken = AttemptGetAccessToken();
        if (accessToken == null) { return false; }

        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(RevokeEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddStringBody($"client_id={AppConfig.GetYoutubeClientId()}&token={accessToken}", DataFormat.None);

        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"Revoke Error: {response.Content ?? "unknown error revoking"}");
            ClearTokenState();
            return false;
        }
        
        EventBus.PublishAsync(new YoutubeOAuthRevokedEvent());
        ClearTokenState();
        return true;
    }
}