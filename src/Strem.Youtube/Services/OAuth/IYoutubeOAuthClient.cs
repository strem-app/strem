using Strem.Youtube.Models;

namespace Strem.Youtube.Services.OAuth;

public interface IYoutubeOAuthClient
{
    void StartAuthorisationProcess(string[] requiredScopes);
    Task<bool> ValidateToken();
    Task<bool> RevokeToken();
}