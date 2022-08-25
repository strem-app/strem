using Newtonsoft.Json;
using RestSharp;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Utils;
using Strem.Core.Variables;
using Strem.Core.Web;
using Strem.Infrastructure.Services.Api;
using Strem.Twitch.Events;
using Strem.Twitch.Events.OAuth;
using Strem.Twitch.Extensions;
using Strem.Twitch.Models;
using Strem.Twitch.Variables;

namespace Strem.Twitch.Services.OAuth;

public class TwitchOAuthClient : ITwitchOAuthClient
{
    public static readonly string OAuthCallbackUrl = $"http://localhost:{InternalWebHostConfiguration.ApiHostPort}/api/twitch/oauth";

    public static readonly string TwitchApiUrl = "https://id.twitch.tv/oauth2";  
    public static readonly string AuthorizeEndpoint = "authorize";
    public static readonly string ValidateEndpoint = "validate";
    public static readonly string RevokeEndpoint = "revoke";
    
    public IBrowserLoader BrowserLoader { get; }
    public IAppState AppState { get; }
    public IEventBus EventBus { get; }
    public IRandomizer Randomizer { get; }
    public ILogger<TwitchOAuthClient> Logger { get; }

    public TwitchOAuthClient(IBrowserLoader browserLoader, IAppState appState, IRandomizer randomizer, IEventBus eventBus, ILogger<TwitchOAuthClient> logger)
    {
        BrowserLoader = browserLoader;
        AppState = appState;
        Randomizer = randomizer;
        EventBus = eventBus;
        Logger = logger;
    }

    public void StartAuthorisationProcess(string[] requiredScopes)
    {
        Logger.Information("Starting Twitch Implicit OAuth Process");
        
        var randomState = Randomizer.RandomString();
        AppState.TransientVariables.Set(CommonVariables.OAuthState, TwitchVars.TwitchContext, randomState);
        
        var scopeQueryData = Uri.EscapeDataString(string.Join(" ", requiredScopes));
        var queryData = $"client_id={TwitchVars.TwitchClientId}&redirect_uri={OAuthCallbackUrl}&response_type=token&scope={scopeQueryData}&state={randomState}";
        var completeUrl = $"{TwitchApiUrl}/{AuthorizeEndpoint}?{queryData}";
        BrowserLoader.LoadUrl(completeUrl);
    }

    public string AttemptGetAccessToken()
    {
        if (AppState.HasTwitchOAuth())
        { return AppState.GetTwitchOAuthToken(); }
        
        Logger.Error("Cannot find OAuth Token In Vars for request to Twitch OAuth API");
        return null;
    }

    public void UpdateTokenState(TwitchOAuthValidationPayload payload)
    {
        AppState.AppVariables.Set(TwitchVars.Username, payload.Login);
        AppState.AppVariables.Set(TwitchVars.UserId, payload.UserId);

        var actualExpiry = DateTime.Now.AddSeconds(payload.ExpiresIn);
        AppState.AppVariables.Set(TwitchVars.TokenExpiry, actualExpiry.ToString("u"));

        var scopes = string.Join(",", payload.Scopes);
        AppState.AppVariables.Set(TwitchVars.OAuthScopes, scopes);
    }

    public void ClearTokenState()
    {
        AppState.AppVariables.Delete(TwitchVars.Username);
        AppState.AppVariables.Delete(TwitchVars.UserId);
        AppState.AppVariables.Delete(TwitchVars.TokenExpiry);
        AppState.AppVariables.Delete(TwitchVars.OAuthScopes);
        AppState.AppVariables.Delete(TwitchVars.OAuthToken);
    }
    
    public async Task<bool> ValidateToken()
    {
        Logger.Information("Validating Twitch Token");

        var accessToken = AttemptGetAccessToken();
        if (accessToken == null) { return false; }

        var restClient = new RestClient(TwitchApiUrl);
        var restRequest = new RestRequest(ValidateEndpoint, Method.Get);
        restRequest.AddHeader("Authorization", $"OAuth {accessToken}");

        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        var payload = JsonConvert.DeserializeObject<TwitchOAuthValidationPayload>(response.Content);
        UpdateTokenState(payload);
        return true;
    }
    
    public async Task<bool> RevokeToken()
    {
        Logger.Information("Revoking Twitch Token");

        var accessToken = AttemptGetAccessToken();
        if (accessToken == null) { return false; }

        var restClient = new RestClient(TwitchApiUrl);
        var restRequest = new RestRequest(RevokeEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddStringBody($"client_id={TwitchVars.TwitchClientId}&token={accessToken}", DataFormat.None);

        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"Revoke Error: {response.Content ?? "unknown error revoking"}");
            return false;
        }
        
        EventBus.PublishAsync(new TwitchOAuthRevokedEvent());
        ClearTokenState();
        return true;
    }
}