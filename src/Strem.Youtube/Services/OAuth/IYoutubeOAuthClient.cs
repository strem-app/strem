using Strem.Youtube.Models;

namespace Strem.Youtube.Services.OAuth;

public interface IYoutubeOAuthClient
{
    void StartAuthorisationProcess(string[] requiredScopes);
    Task<bool> RevokeToken();
    Task<bool> GetToken(string code);
    Task<bool> RefreshToken();
    Task<GoogleUserInfo> GetUserInfo();
}