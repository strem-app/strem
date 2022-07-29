using Newtonsoft.Json;
using RestSharp;
using Strem.Core.Events;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Utils;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Web;
using Strem.Twitch.Events;
using Strem.Twitch.Extensions;
using Strem.Twitch.Models;
using Strem.Twitch.Variables;
using ILogger = Serilog.ILogger;

namespace Strem.Twitch.Services.OAuth;

public class TwitchOAuthClient : ITwitchOAuthClient
{
    public static readonly string OAuthCallbackUrl = "http://localhost:56721/api/twitch/oauth";

    public static readonly string TwitchApiUrl = "https://id.twitch.tv/oauth2";  
    public static readonly string AuthorizeEndpoint = "authorize";
    public static readonly string ValidateEndpoint = "validate";
    public static readonly string RevokeEndpoint = "revoke";
    
    public static readonly string[] RequiredScopes = {
        "chat:read", "chat:edit"
    };

    public static readonly string SourceName = "TwitchOAuthClient";
    
    public IWebLoader WebLoader { get; }
    public IAppState AppState { get; }
    public IEventBus EventBus { get; }
    public IRandomizer Randomizer { get; }
    public ILogger Logger { get; }

    public TwitchOAuthClient(IWebLoader webLoader, IAppState appState, IRandomizer randomizer, IEventBus eventBus, ILogger logger)
    {
        WebLoader = webLoader;
        AppState = appState;
        Randomizer = randomizer;
        EventBus = eventBus;
        Logger = logger;
    }

    public void StartAuthorisationProcess()
    {
        Logger.Information("Starting Twitch Implicit OAuth Process");
        
        var randomState = Randomizer.RandomString();
        AppState.TransientVariables.Set(CommonVariables.OAuthState, TwitchVars.TwitchContext, randomState);
        
        var scopeQueryData = Uri.EscapeDataString(string.Join(" ", RequiredScopes));
        var queryData = $"client_id={TwitchVars.TwitchClientId}&redirect_uri={OAuthCallbackUrl}&response_type=token&scope={scopeQueryData}&state={randomState}";
        var completeUrl = $"{TwitchApiUrl}/{AuthorizeEndpoint}?{queryData}";
        WebLoader.LoadUrl(completeUrl);
    }

    public string AttemptGetAccessToken()
    {
        if (AppState.HasTwitchOAuth())
        { return AppState.GetTwitchOAuthToken(); }
        
        EventBus.PublishAsync(new ErrorEvent(SourceName, "Cannot find OAuth Token In Vars for request to Twitch OAuth API"));
        return null;
    }

    public void UpdateTokenState(TwitchOAuthValidationPayload payload)
    {
        AppState.SetTwitchVar(TwitchVars.Username, payload.Login);
        AppState.SetTwitchVar(TwitchVars.UserId, payload.UserId);

        var actualExpiry = DateTime.Now.AddSeconds(payload.ExpiresIn);
        AppState.SetTwitchVar(TwitchVars.TokenExpiry, actualExpiry.ToString("u"));

        var scopes = string.Join(",", payload.Scopes);
        AppState.SetTwitchVar(TwitchVars.OAuthScopes, scopes);
    }

    public void ClearTokenState()
    {
        AppState.DeleteTwitchVar(TwitchVars.Username);
        AppState.DeleteTwitchVar(TwitchVars.UserId);
        AppState.DeleteTwitchVar(TwitchVars.TokenExpiry);
        AppState.DeleteTwitchVar(TwitchVars.OAuthScopes);
        AppState.DeleteTwitchVar(CommonVariables.OAuthToken);
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
            EventBus.PublishAsync(new ErrorEvent($"{SourceName}:Validation", response.Content ?? "unknown error validating"));
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
            EventBus.PublishAsync(new ErrorEvent($"{SourceName}:Revoke", response.Content ?? "unknown error revoking"));
            return false;
        }
        
        EventBus.PublishAsync(new TwitchOAuthRevokedEvent());
        ClearTokenState();
        return true;
    }
}