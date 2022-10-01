using Newtonsoft.Json;
using RestSharp;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Services.Browsers.Web;
using Strem.Core.Services.Utils;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Api;
using Strem.Twitter.Events.OAuth;
using Strem.Twitter.Extensions;
using Strem.Twitter.Models;
using Strem.Twitter.Types;
using Strem.Twitter.Variables;

namespace Strem.Twitter.Services.OAuth;

public class TwitterOAuthClient : ITwitterOAuthClient
{
    public static readonly string OAuthCallbackUrl = $"http://localhost:{InternalWebHostConfiguration.ApiHostPort}/api/twitter/oauth";

    public static readonly string AuthApiUrl = "https://twitter.com/i/oauth2";
    public static readonly string ApiUrl = "https://api.twitter.com/2/oauth2";
    public static readonly string AuthorizeEndpoint = "authorize";
    public static readonly string TokenEndpoint = "token";
    public static readonly string RevokeEndpoint = "revoke";
    
    public IWebBrowser WebBrowser { get; }
    public IAppState AppState { get; }
    public IApplicationConfig AppConfig { get; }
    public IEventBus EventBus { get; }
    public IRandomizer Randomizer { get; }
    public ILogger<TwitterOAuthClient> Logger { get; }

    public TwitterOAuthClient(IWebBrowser webBrowser, IAppState appState, IRandomizer randomizer, IEventBus eventBus, ILogger<TwitterOAuthClient> logger, IApplicationConfig appConfig)
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
        Logger.Information("Starting Twitter PKCE OAuth Process");

        var randomState = Randomizer.RandomString();
        var challengeCode = Randomizer.RandomString();
        AppState.TransientVariables.Set(CommonVariables.OAuthState, TwitterVars.Context, randomState);
        AppState.TransientVariables.Set(CommonVariables.OAuthChallengeCode, TwitterVars.Context, challengeCode);

        var completeScopes = requiredScopes.ToList();
        if(!completeScopes.Contains(Scopes.OfflineAccess))
        { completeScopes.Add(Scopes.OfflineAccess); }
        
        var scopeQueryData = Uri.EscapeDataString(string.Join(" ", completeScopes));
        var challengeData = $"code_challenge={challengeCode}&code_challenge_method=plain";
        var queryData = $"client_id={AppConfig.GetTwitterClientId()}&redirect_uri={OAuthCallbackUrl}&response_type=code&scope={scopeQueryData}&state={randomState}&{challengeData}";
        var completeUrl = $"{AuthApiUrl}/{AuthorizeEndpoint}?{queryData}";
        WebBrowser.LoadUrl(completeUrl);
    }

    public async Task<bool> GetToken(string code)
    {
        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(TokenEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        var clientContent = $"client_id={AppConfig.GetTwitterClientId()}&redirect_uri={OAuthCallbackUrl}";
        var challengeCode = AppState.TransientVariables.Get(CommonVariables.OAuthChallengeCode, TwitterVars.Context);
        var codeContent = $"grant_type=authorization_code&code={code}&code_verifier={challengeCode}";
        var completeBody = $"{clientContent}&{codeContent}";
        restRequest.AddStringBody(completeBody, DataFormat.None);
        
        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        var payload = JsonConvert.DeserializeObject<TwitterOAuthTokenPayload>(response.Content);
        if (string.IsNullOrEmpty(payload?.AccessToken))
        {
            Logger.Error($"Payload Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        RefreshTokenState(payload);
        EventBus.PublishAsync(new TwitterOAuthSuccessEvent());
        return true;
    }
    
    public async Task<bool> RefreshToken()
    {
        Logger.Information("Refreshing Twitter Token");
        
        var refreshToken = AppState.AppVariables.Get(TwitterVars.OAuthRefreshToken);
        if (string.IsNullOrEmpty(refreshToken))
        { return false; }
        
        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(TokenEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        var clientContent = $"client_id={AppConfig.GetTwitterClientId()}";
        var codeContent = $"grant_type=refresh_token&refresh_token={refreshToken}";
        var totalContent = $"{clientContent}&{codeContent}";
        restRequest.AddStringBody(totalContent, DataFormat.None);
        
        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        var payload = JsonConvert.DeserializeObject<TwitterOAuthTokenPayload>(response.Content);
        if (string.IsNullOrEmpty(payload?.AccessToken))
        {
            Logger.Error($"Payload Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        RefreshTokenState(payload);
        EventBus.PublishAsync(new TwitterOAuthSuccessEvent());
        return true;
    }
    
    public async Task<bool> RevokeToken()
    {
        Logger.Information("Revoking Twitter Token");

        var accessToken = AttemptGetAccessToken();
        if (accessToken == null) { return false; }

        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(RevokeEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddStringBody($"client_id={AppConfig.GetTwitterClientId()}&token={accessToken}&token_type_hint=access_token", DataFormat.None);

        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"Revoke Error: {response.Content ?? "unknown error revoking"}");
            return false;
        }
        
        EventBus.PublishAsync(new TwitterOAuthRevokedEvent());
        ClearTokenState();
        return true;
    }
    
    public string AttemptGetAccessToken()
    {
        if (AppState.HasTwitterOAuth())
        { return AppState.GetTwitterOAuthToken(); }
        
        Logger.Error("Cannot find OAuth Token In Vars for request to Twitter OAuth API");
        return null;
    }

    public void RefreshTokenState(TwitterOAuthTokenPayload payload)
    {
        AppState.AppVariables.Set(TwitterVars.OAuthToken, payload.AccessToken);
        AppState.AppVariables.Set(TwitterVars.OAuthRefreshToken, payload.RefreshToken);

        var scopes = payload.Scope.Replace(" ", ",");
        AppState.AppVariables.Set(TwitterVars.OAuthScopes, scopes);
        
        var actualExpiry = DateTime.Now.AddSeconds(payload.ExpiresIn);
        AppState.AppVariables.Set(TwitterVars.TokenExpiry, actualExpiry.ToString("u"));
    }
    
    public void ClearTokenState()
    {
        AppState.AppVariables.Delete(TwitterVars.Username);
        AppState.AppVariables.Delete(TwitterVars.TokenExpiry);
        AppState.AppVariables.Delete(TwitterVars.OAuthScopes);
        AppState.AppVariables.Delete(TwitterVars.OAuthToken);
        AppState.AppVariables.Delete(TwitterVars.OAuthRefreshToken);
        AppState.AppVariables.Delete(CommonVariables.OAuthChallengeCode, TwitterVars.Context);
    }
}