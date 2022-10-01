namespace Strem.Twitter.Services.OAuth;

public interface ITwitterOAuthClient
{
    void StartAuthorisationProcess(string[] requiredScopes);
    Task<bool> RevokeToken();
    Task<bool> GetToken(string code);
    Task<bool> RefreshToken();
}