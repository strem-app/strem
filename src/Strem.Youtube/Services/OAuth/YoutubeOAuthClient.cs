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
    public static readonly string TokenEndpoint = "v4/token";
    public static readonly string RevokeEndpoint = "revoke";
    public static readonly string UserInfoEndpoint = "v3/userinfo";
    
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
        Logger.Information("Starting Youtube PKCE OAuth Process");

        var randomState = Randomizer.RandomString();
        var challengeCode = Randomizer.RandomString();
        AppState.TransientVariables.Set(CommonVariables.OAuthState, YoutubeVars.Context, randomState);
        AppState.TransientVariables.Set(CommonVariables.OAuthChallengeCode, YoutubeVars.Context, challengeCode);

        var completeScopes = requiredScopes.ToList();
        if(!completeScopes.Contains(Scopes.Profile))
        { completeScopes.Add(Scopes.Profile); }
        
        var scopeQueryData = Uri.EscapeDataString(string.Join(" ", completeScopes));
        var challengeData = $"code_challenge={challengeCode}&code_challenge_method=plain";
        var queryData = $"client_id={AppConfig.GetYoutubeClientId()}&redirect_uri={OAuthCallbackUrl}&response_type=code&scope={scopeQueryData}&state={randomState}&{challengeData}";
        var completeUrl = $"{ApiUrl}/{AuthorizeEndpoint}?{queryData}";
        WebBrowser.LoadUrl(completeUrl);
    }

    public async Task<bool> GetToken(string code)
    {
        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(TokenEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        var clientContent = $"client_id={AppConfig.GetYoutubeClientId()}&redirect_uri={OAuthCallbackUrl}";
        var challengeCode = AppState.TransientVariables.Get(CommonVariables.OAuthChallengeCode, YoutubeVars.Context);
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

        var payload = JsonConvert.DeserializeObject<YoutubeOAuthTokenPayload>(response.Content);
        if (string.IsNullOrEmpty(payload?.AccessToken))
        {
            Logger.Error($"Payload Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        RefreshTokenState(payload);
        EventBus.PublishAsync(new YoutubeOAuthSuccessEvent());
        return true;
    }
    
    public async Task<bool> RefreshToken()
    {
        Logger.Information("Refreshing Youtube Token");
        
        var refreshToken = AppState.AppVariables.Get(YoutubeVars.OAuthRefreshToken);
        if (string.IsNullOrEmpty(refreshToken))
        { return false; }
        
        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(TokenEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        var clientContent = $"client_id={AppConfig.GetYoutubeClientId()}";
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

        var payload = JsonConvert.DeserializeObject<YoutubeOAuthTokenPayload>(response.Content);
        if (string.IsNullOrEmpty(payload?.AccessToken))
        {
            Logger.Error($"Payload Validation Error: {response.Content ?? "unknown error validating"}");
            ClearTokenState();
            return false;
        }

        RefreshTokenState(payload);
        EventBus.PublishAsync(new YoutubeOAuthSuccessEvent());
        return true;
    }

    public async Task<GoogleUserInfo> GetUserInfo()
    {
        Logger.Information("Getting Youtube UserInfo");

        var accessToken = AttemptGetAccessToken();
        if (accessToken == null) { return null; }

        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(UserInfoEndpoint, Method.Get);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"UserInfo Error: {response.Content ?? "unknown error getting user info"}");
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<GoogleUserInfo>(response.Content);
        }
        catch (Exception ex)
        {
            Logger.Error($"UserInfo Error: Couldnt Deserialize Data {ex.Message}");
            return null;
        }
    }

    public async Task<bool> RevokeToken()
    {
        Logger.Information("Revoking Twitter Token");

        var accessToken = AttemptGetAccessToken();
        if (accessToken == null) { return false; }

        var restClient = new RestClient(ApiUrl);
        var restRequest = new RestRequest(RevokeEndpoint, Method.Post);
        restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        restRequest.AddStringBody($"client_id={AppConfig.GetYoutubeClientId()}&token={accessToken}&token_type_hint=access_token", DataFormat.None);

        var response = await restClient.ExecuteAsync(restRequest);
        if (!response.IsSuccessful)
        {
            Logger.Error($"Revoke Error: {response.Content ?? "unknown error revoking"}");
            return false;
        }
        
        EventBus.PublishAsync(new YoutubeOAuthRevokedEvent());
        ClearTokenState();
        return true;
    }
    
    public string AttemptGetAccessToken()
    {
        if (AppState.HasYoutubeOAuth())
        { return AppState.GetYoutubeOAuthToken(); }
        
        Logger.Error("Cannot find OAuth Token In Vars for request to Youtube OAuth API");
        return null;
    }

    public void RefreshTokenState(YoutubeOAuthTokenPayload payload)
    {
        AppState.AppVariables.Set(YoutubeVars.OAuthToken, payload.AccessToken);
        AppState.AppVariables.Set(YoutubeVars.OAuthRefreshToken, payload.RefreshToken);

        var scopes = payload.Scope.Replace(" ", ",");
        AppState.AppVariables.Set(YoutubeVars.OAuthScopes, scopes);
        
        var actualExpiry = DateTime.Now.AddSeconds(payload.ExpiresIn);
        AppState.AppVariables.Set(YoutubeVars.TokenExpiry, actualExpiry.ToString("u"));
    }
    
    public void ClearTokenState()
    {
        AppState.AppVariables.Delete(YoutubeVars.Username);
        AppState.AppVariables.Delete(YoutubeVars.TokenExpiry);
        AppState.AppVariables.Delete(YoutubeVars.OAuthScopes);
        AppState.AppVariables.Delete(YoutubeVars.OAuthToken);
        AppState.AppVariables.Delete(YoutubeVars.OAuthRefreshToken);
        AppState.AppVariables.Delete(CommonVariables.OAuthChallengeCode, YoutubeVars.Context);
    }
}