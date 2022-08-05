namespace Strem.Twitch.Services.OAuth;

public interface ITwitchOAuthClient
{
    void StartAuthorisationProcess(string[] requiredScopes);
    Task<bool> ValidateToken();
    Task<bool> RevokeToken();
}